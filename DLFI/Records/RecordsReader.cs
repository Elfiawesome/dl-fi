using System.Text.Json;

namespace DLFI.Records;

public class RecordsReader
{
	private readonly JsonSerializerOptions _jsonSerializerOptions;
	private string _rootPath = "";
	private RecordsReader(string path, JsonSerializerOptions jsonSerializerOptions)
	{
		_rootPath = path;
		_jsonSerializerOptions = jsonSerializerOptions;
	}

	public static RecordsReader? Open(string path, JsonSerializerOptions jsonSerializerOptions)
	{
		if (!Directory.Exists(path)) { return null; }
		return new RecordsReader(path, jsonSerializerOptions);
	}

	public RecordsReader? GetGroup(string groupName, bool makeDir = true)
	{
		string path = Path.Join(_rootPath, groupName);
		if (!Directory.Exists(path))
		{
			if (makeDir) { Directory.CreateDirectory(path); }
		}
		return Open(path, _jsonSerializerOptions);
	}

	public RecordsReader? GetGroupRecursive(string[] groupNames, bool makeDir = true)
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

	public T? GetRecord<T>(string recordName) where T : BaseRecordModel
	{
		string path = Path.Join(_rootPath, recordName + ".json");
		Console.WriteLine(path);
		if (!File.Exists(path)) { return null; }
		return JsonSerializer.Deserialize<T>(
			File.ReadAllBytes(path), _jsonSerializerOptions
		);
	}

	public Stream[] GetRecordAttachments(string recordName)
	{
		var record = GetRecord<BaseRecordModel>(recordName);
		if (record == null) { return []; }
		return GetRecordAttachments(record);
	}

	public Stream[] GetRecordAttachments(BaseRecordModel record)
	{
		return [.. record?.Attachments?.ToList().Select((s) => new FileStream(s, FileMode.Open, FileAccess.Read)) ?? []];
	}

	public void StoreRecord(string recordName, BaseRecordModel record)
	{
		string path = Path.Join(_rootPath, recordName + ".json");
		if (!Directory.Exists(_rootPath)) { return; }
		if (File.Exists(path)) { return; }
		File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(record, _jsonSerializerOptions));
	}

	public void StoreRecord(string recordName, BaseRecordModel record, PendingAttachment[] pendingAttachments)
	{
		string path = Path.Join(_rootPath, recordName + ".json");
		if (!Directory.Exists(_rootPath)) { return; }
		if (File.Exists(path)) { return; }

		List<string> attachmentNames = [];
		for (var attachmentIndex = 0; attachmentIndex < pendingAttachments.Length; attachmentIndex++)
		{
			var pendingAttachment = pendingAttachments[attachmentIndex];
			string attachmentName = recordName + $".a{attachmentIndex}.{pendingAttachment.Ext}";
			string attachmentPath = Path.Join(_rootPath, attachmentName);
			using var fs = new FileStream(attachmentPath, FileMode.Create, FileAccess.Write);
			pendingAttachment.Stream.CopyTo(fs);
			attachmentNames.Add(attachmentName);
		}

		record.Attachments = attachmentNames;
		StoreRecord(recordName, record);
	}

	public string[] GetGroups()
	{
		if (!Directory.Exists(_rootPath)) { return []; }
		return [.. Directory.GetDirectories(_rootPath).Select((s) => Path.GetFileName(s) ?? "").Where(x => x != "")];
	}

	public string[] GetRecords()
	{
		if (!Directory.Exists(_rootPath)) { return []; }
		return [.. Directory.GetFiles(_rootPath).Select((s) =>
		{
			if (Path.GetExtension(s) == ".json"){ return Path.GetFileNameWithoutExtension(s); }
			return "";
		}).Where(x => x != "")];
	}

	public IEnumerable<string> Query(string[] Tags)
	{
		foreach (var recordName in GetRecords())
		{
			var record = GetRecord<BaseRecordModel>(recordName);
			if (record == null) { continue; }
			yield return recordName;
		}


		foreach (var groupName in GetGroups())
		{
			var rr = GetGroup(groupName);
			if (rr == null) { continue; }
			foreach (var recordName in rr.Query(Tags))
			{
				yield return groupName + "/" + recordName;
			}
		}
	}
}

public record struct PendingAttachment(string Ext, Stream Stream);