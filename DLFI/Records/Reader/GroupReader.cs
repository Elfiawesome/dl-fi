using System.Text.Json;
using DLFI.Records.BaseModels;

namespace DLFI.Records.Reader;

public class GroupReader : BaseReader
{
	public string RootPath;
	private readonly JsonSerializerOptions _jsonSerializerOptions;

	// --- Constructor Methods --- //
	private GroupReader(string path, JsonSerializerOptions jsonSerializerOptions) : base()
	{
		RootPath = path;
		_jsonSerializerOptions = jsonSerializerOptions;
	}

	public static GroupReader? Open(string path, JsonSerializerOptions jsonSerializerOptions)
	{
		if (!Directory.Exists(path)) { return null; }
		return new GroupReader(path, jsonSerializerOptions);
	}

	// --- Groups --- //
	public GroupReader? GetGroup(string groupName, bool makeDir = true)
	{
		string path = Path.Join(RootPath, groupName);
		if (!Directory.Exists(path))
		{
			if (makeDir) { Directory.CreateDirectory(path); }
		}
		var gr = Open(path, _jsonSerializerOptions);
		if (gr != null) { gr.ParentReader = this; gr.Id = Path.GetFileNameWithoutExtension(path) ?? ""; }
		return gr;
	}

	public GroupReader? GetGroupRecursive(string[] groupNames, bool makeDir = true)
	{
		if (groupNames.Length == 1)
		{
			return GetGroup(groupNames[0], makeDir);
		}
		else
		{
			return GetGroup(groupNames[0], makeDir)?.GetGroupRecursive(groupNames[1..]);
		}
	}

	public IEnumerable<GroupReader> GetGroups()
	{
		foreach (var groupName in GetGroupNames())
		{
			var rr = GetGroup(groupName);
			if (rr != null) { yield return rr; }
		}
	}

	public string[] GetGroupNames()
	{
		if (!Directory.Exists(RootPath)) { return []; }
		return [.. Directory.GetDirectories(RootPath).Select((s) => Path.GetFileNameWithoutExtension(s) ?? "").Where(x => x != "")];
	}


	// --- Record --- //
	public RecordReader<T>? GetRecord<T>(string recordName) where T : BaseRecordModel
	{
		string path = Path.Join(RootPath, recordName + ".json");
		if (!File.Exists(path)) { return null; }
		var record = JsonSerializer.Deserialize<T>(File.ReadAllBytes(path), _jsonSerializerOptions);
		if (record == null) { return null; }
		return new(recordName, record, this);
	}

	public IEnumerable<RecordReader<BaseRecordModel>> GetRecords()
	{
		foreach (var recordName in GetRecordNames())
		{
			var rr = GetRecord<BaseRecordModel>(recordName);
			if (rr != null) { yield return rr; }
		}
	}

	public string[] GetRecordNames()
	{
		if (!Directory.Exists(RootPath)) { return []; }
		return [.. Directory.GetFiles(RootPath).Select((s) =>
		{
			if (Path.GetExtension(s) == ".json"){ return Path.GetFileNameWithoutExtension(s); }
			return "";
		}).Where(x => x != "")];
	}

	public void StoreRecord(string recordName, BaseRecordModel record, bool overwrite = false)
	{
		string path = Path.Join(RootPath, recordName + ".json");
		if (!Directory.Exists(RootPath)) { return; }
		if (File.Exists(path) && !overwrite) { return; }
		File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(record, _jsonSerializerOptions));
	}

	public void StoreRecord(string recordName, BaseRecordModel record, PendingAttachment[] pendingAttachments, bool overwrite = false)
	{
		string path = Path.Join(RootPath, recordName + ".json");
		if (!Directory.Exists(RootPath)) { return; }
		if (File.Exists(path) && !overwrite) { return; }

		HashSet<string> attachmentNames = [];
		for (var attachmentIndex = 0; attachmentIndex < pendingAttachments.Length; attachmentIndex++)
		{
			var pendingAttachment = pendingAttachments[attachmentIndex];
			string attachmentName = recordName + $".a{attachmentIndex}.{pendingAttachment.Ext}";
			string attachmentPath = Path.Join(RootPath, attachmentName);
			using var fs = new FileStream(attachmentPath, FileMode.Create, FileAccess.Write);
			pendingAttachment.Stream.CopyTo(fs);
			attachmentNames.Add(attachmentName);
		}

		record.Attachments = attachmentNames;
		StoreRecord(recordName, record, overwrite);
	}


	// --- Query --- //
	public IEnumerable<RecordReader<BaseRecordModel>> QueryRecords(QueryParam param)
	{
		if ((param.Scope != null && param.Scope.Contains(FullId)) || param.Scope == null)
		{
			foreach (var rr in GetRecords())
			{
				if (param.Tags == null)
				{
					yield return rr;
				}
				else
				{
					bool allTagsInScope = true;
					foreach (string s in param.Tags)
					{
						if (!rr.Record.Tags.Contains(s))
						{
							allTagsInScope = false;
							break;
						}
					}
					if (allTagsInScope) { yield return rr; }
				}
			}
		}

		foreach (var gr in GetGroups())
		{
			foreach (var rr in gr.QueryRecords(param))
			{
				yield return rr;
			}
		}
	}

	public IEnumerable<GroupReader> QueryGroups(QueryParam param)
	{
		foreach (var rr in GetGroups())
		{
			yield return rr;
			foreach (var _rr in rr.QueryGroups(param))
			{
				yield return _rr;
			}
		}
	}

	// --- Misc --- //
	public FileStream? GetFile(string fileName)
	{
		string path = Path.Join(RootPath, fileName);
		if (!File.Exists(path)) { return null; }
		return new FileStream(path, FileMode.Open, FileAccess.Read);
	}
}