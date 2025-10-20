using System.Text.Json;
using DLFI.Archive.Model.Base;

namespace DLFI.Archive;

public class ArchiveService
{
	private readonly string _rootPath;
	private readonly string _indexPath;
	private List<IndexEntry> _index = [];
	private Dictionary<Guid, IndexEntry> _lookup = [];
	private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
	private static readonly Lock _indexLock = new();

	public ArchiveService(string rootPath)
	{
		_rootPath = rootPath;
		Directory.CreateDirectory(_rootPath);
		_indexPath = Path.Combine(_rootPath, "_index.json");
		LoadIndex();
	}

	private void LoadIndex()
	{
		lock (_indexLock)
		{
			if (File.Exists(_indexPath))
			{
				var json = File.ReadAllText(_indexPath);
				_index = JsonSerializer.Deserialize<List<IndexEntry>>(json, _jsonOptions) ?? new List<IndexEntry>();
			}
			else
			{
				_index = new List<IndexEntry>();
			}
			_lookup = _index.ToDictionary(e => e.Id);
		}
	}

	private void SaveIndex()
	{
		lock (_indexLock)
		{
			var json = JsonSerializer.Serialize(_index, _jsonOptions);
			File.WriteAllText(_indexPath, json);
		}
	}

	// --- Write Operations ---
	public void AddVault(Vault vault, Guid? parentId = null)
	{
		string parentPath = "";
		if (parentId.HasValue && _lookup.TryGetValue(parentId.Value, out var parentEntry))
		{
			parentPath = parentEntry.Path;
		}

		string vaultRelativePath = Path.Combine(parentPath, vault.Name);
		string vaultAbsolutePath = Path.Combine(_rootPath, vaultRelativePath);
		Directory.CreateDirectory(vaultAbsolutePath);

		string metadataPath = Path.Combine(vaultAbsolutePath, "_vault.json");
		string vaultJson = JsonSerializer.Serialize(vault, vault.GetType(), _jsonOptions);
		File.WriteAllText(metadataPath, vaultJson);

		var entry = new IndexEntry
		{
			Id = vault.Id,
			ParentId = parentId,
			ItemType = RecordItemType.Vault,
			Path = vaultRelativePath,
			Name = vault.Name,
			DisplayName = vault.DisplayName,
			RecordType = vault.RecordType ?? "",
			Tags = vault.Tags
		};

		lock (_indexLock)
		{
			_index.Add(entry);
			_lookup.Add(entry.Id, entry);
		}
		SaveIndex();
	}

	public void AddRecord(Record record, Guid parentVaultId, Dictionary<string, Stream>? attachments = null)
	{
		if (!_lookup.TryGetValue(parentVaultId, out var parentEntry) || parentEntry.ItemType != RecordItemType.Vault)
		{
			throw new ArgumentException("Parent vault not found or invalid ID.", nameof(parentVaultId));
		}

		string vaultAbsolutePath = Path.Combine(_rootPath, parentEntry.Path);
		string recordRelativePath = Path.Combine(parentEntry.Path, $"{record.Name}.json");
		string recordAbsolutePath = Path.Combine(_rootPath, recordRelativePath);

		string recordJson = JsonSerializer.Serialize(record, record.GetType(), _jsonOptions);
		File.WriteAllText(recordAbsolutePath, recordJson);

		// Save attachments
		if (attachments != null)
		{
			foreach (var attachment in attachments)
			{
				string attachmentPath = Path.Combine(vaultAbsolutePath, attachment.Key);
				using (var fileStream = new FileStream(attachmentPath, FileMode.Create, FileAccess.Write))
				{
					attachment.Value.CopyTo(fileStream);
				}
			}
		}

		var entry = new IndexEntry
		{
			Id = record.Id,
			ParentId = parentVaultId,
			ItemType = RecordItemType.Record,
			Path = recordRelativePath,
			Name = record.Name,
			DisplayName = record.DisplayName,
			RecordType = record.RecordType ?? "",
			Tags = record.Tags
		};

		lock (_indexLock)
		{
			_index.Add(entry);
			_lookup.Add(entry.Id, entry);
		}
		SaveIndex();
	}


	// --- Read Operations ---
	public T? LoadItem<T>(Guid id) where T : RawRecord
	{
		if (!_lookup.TryGetValue(id, out var entry))
		{
			return null;
		}

		string absolutePath;
		if (entry.ItemType == RecordItemType.Vault)
		{
			absolutePath = Path.Combine(_rootPath, entry.Path, "_vault.json");
		}
		else
		{
			absolutePath = Path.Combine(_rootPath, entry.Path);
		}

		string jsonContent = File.ReadAllText(absolutePath);
		Type? targetType = Type.GetType(entry.RecordType);

		if (targetType == null) throw new InvalidOperationException($"Type '{entry.RecordType}' not found.");

		return (T?)JsonSerializer.Deserialize(jsonContent, targetType, _jsonOptions);
	}

	public IEnumerable<IndexEntry> Find(Func<IndexEntry, bool> predicate)
	{
		return _index.Where(predicate);
	}

	public IEnumerable<IndexEntry> FindByTag(string tag)
	{
		return _index.Where(e => e.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
	}

	// public IEnumerable<IndexEntry> FindByType<TRawRecord>(Func<TRawRecord, bool> predicate)
	// 	where TRawRecord : RawRecord
	// {
	// 	return _index.Select((x) => LoadItem<TRawRecord>(x.Id)).Where((x) => x != null).Where(predicate).Select((x) => _lookup[x.Id]);
	// }

	public IEnumerable<string> GetAggregatedTags(Guid vaultId)
	{
		var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var queue = new Queue<Guid>();
		queue.Enqueue(vaultId);

		while (queue.Count > 0)
		{
			var currentId = queue.Dequeue();
			if (!_lookup.TryGetValue(currentId, out var currentEntry)) continue;

			allTags.UnionWith(currentEntry.Tags);

			// Find all direct children (vaults and records)
			var children = _index.Where(e => e.ParentId == currentId);
			foreach (var child in children)
			{
				if (child.ItemType == RecordItemType.Vault)
				{
					queue.Enqueue(child.Id); // This vault will be processed to get its children
				}
				else // It's a record, just add its tags
				{
					allTags.UnionWith(child.Tags);
				}
			}
		}
		return allTags;
	}
}