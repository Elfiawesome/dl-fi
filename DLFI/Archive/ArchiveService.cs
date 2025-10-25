using System.Text.Json;
using DLFI.Archive.RecordSystem.Model;
using DLFI.Archive.RecordSystem.Serialization;

namespace DLFI.Archive;

public class ArchiveService
{
	private readonly string _rootPath;
	private const string _indexFileName = "_index.json";
	private const string _vaultFileName = "_vault.json";
	private Dictionary<Guid, RecordIndex> _index = [];

	public ArchiveService(string rootPath)
	{
		_rootPath = rootPath;
		Directory.CreateDirectory(_rootPath);
		if (!TryLoadIndexFromLocalFile())
		{

		}
	}

	private bool TryLoadIndexFromLocalFile()
	{
		if (File.Exists(Path.Combine(_rootPath, _indexFileName)))
		{
			string dataJson = File.ReadAllText(Path.Combine(_rootPath, _indexFileName));
			_index = JsonSerializer.Deserialize<Dictionary<Guid, RecordIndex>>(dataJson) ?? [];
			return true;
		}
		return false;
	}

	private void SaveIndex()
	{
		string dataJson = JsonSerializer.Serialize(_index);
		File.WriteAllText(Path.Combine(_rootPath, _indexFileName), dataJson);
	}

	public void AddItem<T>(T rawRecord, Vault parentRecord)
		where T : RawRecord
	{
		AddItem(rawRecord, parentRecord.Id);
	}

	public void AddItem<T>(T rawRecord, Guid parentVaultId)
		where T : RawRecord
	{
		if (!_index.TryGetValue(parentVaultId, out var parentEntry) || parentEntry.BaseType != RecordIndex.IndexType.Vault)
		{
			throw new ArgumentException("Parent vault not found or is not a record.", nameof(parentVaultId));
		}
		AddItem(rawRecord, parentEntry.RelativePath);
	}

	public void AddItem<T>(T rawRecord, string[] parentRelativePath)
		where T : RawRecord
	{
		AddItem(rawRecord, Path.Combine(parentRelativePath));
	}

	public void AddItem<T>(T rawRecord, string parentRelativePath = "")
		where T : RawRecord
	{
		string parentAbsolutePath = Path.Combine(_rootPath, parentRelativePath);
		Directory.CreateDirectory(parentAbsolutePath);


		Type rawRecordType = rawRecord.GetType();
		if (rawRecordType == typeof(Record) || rawRecordType.IsSubclassOf(typeof(Record)))
		{
			string fileName = $"{rawRecord.Name}.json";
			string relativeFilePath = Path.Combine(parentRelativePath, fileName);
			string absoluteFilePath = Path.Combine(parentAbsolutePath, fileName);

			string dataJson = RawRecordSerializer.Serialize(rawRecord);
			if (File.Exists(absoluteFilePath)) { }
			File.WriteAllText(absoluteFilePath, dataJson);
			_index.Add(rawRecord.Id, new RecordIndex() { BaseType = RecordIndex.IndexType.Record, RelativePath = relativeFilePath, Tags = rawRecord.Tags });

			if (rawRecord is Record record)
			{
				foreach (var item in record.AttachmentStreams)
				{
					var attachmentFilename = item.Key;
					var attachmentStream = item.Value;
					string relativeAttachmentPath = Path.Combine(parentRelativePath, attachmentFilename);
					string absoluteAttachmentPath = Path.Combine(parentAbsolutePath, attachmentFilename);
					using var fs = new FileStream(absoluteAttachmentPath, FileMode.CreateNew);
					attachmentStream.CopyTo(fs);
					record.Attachments.Add(item.Key);
				}
				record.AttachmentStreams.Clear();
			}
		}
		if (rawRecordType == typeof(Vault) || rawRecordType.IsSubclassOf(typeof(Vault)))
		{
			string vaultName = $"{rawRecord.Name}";
			string relativeVaultPath = Path.Combine(parentRelativePath, vaultName);
			string absoluteVaultPath = Path.Combine(parentAbsolutePath, vaultName);
			Directory.CreateDirectory(absoluteVaultPath);

			string fileName = _vaultFileName;
			string relativeFilePath = Path.Combine(relativeVaultPath, fileName);
			string absoluteFilePath = Path.Combine(absoluteVaultPath, fileName);

			string dataJson = RawRecordSerializer.Serialize(rawRecord);
			if (File.Exists(absoluteFilePath)) { }
			File.WriteAllText(absoluteFilePath, dataJson);
			_index.Add(rawRecord.Id, new RecordIndex() { BaseType = RecordIndex.IndexType.Vault, RelativePath = relativeVaultPath, Tags = rawRecord.Tags });
		}
		SaveIndex();
	}

	private class RecordIndex
	{
		public enum IndexType { Vault, Record }

		public required IndexType BaseType { get; set; }
		public required string RelativePath { get; set; }
		public HashSet<string> Tags { get; set; } = [];
	}
}