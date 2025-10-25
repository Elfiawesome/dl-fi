using System.Text.Json;
using DLFI.Core.Archive.Domain.Models;
using DLFI.Core.Archive.Json;

namespace DLFI.Archive.Persistence;

public class FileSystemArchiveService
{
	public const string VaultFileName = "_vault.json";
	public const string IndexFileName = "_index.json";

	public CommonPointStructure? CommonPoint { get; private set; }

	private string _rootPath;
	private Dictionary<Guid, FileSystemIndex> _index = [];
	public IReadOnlyDictionary<Guid, FileSystemIndex> Index => _index;
	private NodeSerializer _serializer;

	public FileSystemArchiveService(string rootPath, NodeSerializer serializer)
	{
		_rootPath = rootPath;
		_serializer = serializer;
		Directory.CreateDirectory(_rootPath);
		TempSetupSystem();
	}

	public void TempSetupSystem()
	{
		var definitionVault = new Vault() { Name = "definition" };
		var authorVault = new Vault() { Name = "author" };
		var tagVault = new Vault() { Name = "tag" };
		AddNode(definitionVault);
		AddNode(authorVault, definitionVault);
		AddNode(tagVault, definitionVault);

		var mediumVault = new Vault() { Name = "medium" };
		var mangaVault = new Vault() { Name = "manga" };
		var imageVault = new Vault() { Name = "image" };
		AddNode(mediumVault);
		AddNode(mangaVault, mediumVault);
		AddNode(imageVault, mediumVault);

		CommonPoint = new()
		{
			Definition = definitionVault.Id,
			Author = authorVault.Id,
			Tag = tagVault.Id,
			Medium = mediumVault.Id,
			Manga = mangaVault.Id,
			Image = imageVault.Id,
		};
	}


	// --- Writing Functions ---
	public void AddNode(Node node, Vault? parentVault, Dictionary<string, Stream>? attachmentStreams = null) { AddNode(node, parentVault?.Id ?? null, attachmentStreams); }
	public void AddNode(Node node, Guid? parentId = null, Dictionary<string, Stream>? attachmentStreams = null)
	{
		string parentRelativePath = "";
		if (parentId.HasValue)
		{
			if (!_index.TryGetValue(parentId.Value, out var parentVaultIndex) || parentVaultIndex.IndexType != FileSystemIndex.Type.Vault)
			{
				throw new ArgumentException("Parent vault not found or is not a entry.", nameof(parentId));
			}
			parentRelativePath = parentVaultIndex.RelativePath;
		}
		AddNodeByPath(node, parentRelativePath, attachmentStreams);
	}
	private void AddNodeByPath(Node node, string parentRelativePath = "", Dictionary<string, Stream>? attachmentStreams = null)
	{
		string parentAbsolutePath = Path.Combine(_rootPath, parentRelativePath);
		Directory.CreateDirectory(parentAbsolutePath);

		Type nodeType = node.GetType();
		if (nodeType == typeof(Entry) || nodeType.IsSubclassOf(typeof(Entry)))
		{
			string fileName = $"{node.Name}.json";
			string relativeFilePath = Path.Combine(parentRelativePath, fileName);
			string absoluteFilePath = Path.Combine(parentAbsolutePath, fileName);

			if (attachmentStreams != null && node is Entry entry)
			{
				foreach (var item in attachmentStreams)
				{
					var attachmentFilename = item.Key;
					var attachmentStream = item.Value;
					string relativeAttachmentPath = Path.Combine(parentRelativePath, attachmentFilename);
					string absoluteAttachmentPath = Path.Combine(parentAbsolutePath, attachmentFilename);
					using var fs = new FileStream(absoluteAttachmentPath, FileMode.CreateNew);
					attachmentStream.CopyTo(fs);
					entry.Attachments.Add(item.Key);
					fs.Close();
					attachmentStream.Close();
				}
			}

			string data = _serializer.Serialize(node);
			if (File.Exists(absoluteFilePath)) { /* uh oh... */ }
			File.WriteAllText(absoluteFilePath, data);
			_index.Add(node.Id, new FileSystemIndex(node, relativeFilePath));
			Console.WriteLine("Writing Entry to " + absoluteFilePath);
		}
		if (nodeType == typeof(Vault) || nodeType.IsSubclassOf(typeof(Vault)))
		{
			string vaultName = $"{node.Name}";
			string relativeVaultPath = Path.Combine(parentRelativePath, vaultName);
			string absoluteVaultPath = Path.Combine(parentAbsolutePath, vaultName);
			Directory.CreateDirectory(absoluteVaultPath);

			string fileName = VaultFileName;
			string relativeFilePath = Path.Combine(relativeVaultPath, fileName);
			string absoluteFilePath = Path.Combine(absoluteVaultPath, fileName);

			string dataJson = _serializer.Serialize(node);
			if (File.Exists(absoluteFilePath)) { /* uh oh... */ }
			File.WriteAllText(absoluteFilePath, dataJson);
			_index.Add(node.Id, new FileSystemIndex(node, relativeVaultPath));
			Console.WriteLine("Writing Vault to " + absoluteFilePath);
		}
	}


	// --- Reading Functions (TODO) ---


	// 
	public void SaveIndexFile()
	{
		var data = JsonSerializer.Serialize(_index);
		string absoluteFilePath = Path.Combine(_rootPath, IndexFileName);
		File.WriteAllText(absoluteFilePath, data);
	}
}

// Planned Schema
// .
// ├── definition (used mostly for tagging purposes)
// │   ├── author
// │   └── tag
// └── medium
//     ├── manga
//     │   ├── nhentai
//     │   │   └── work
//     │   │       ├── XXXXXX
//     │   │       │   ├── page-XX
//     │   │       │   └── ...
//     │   │       └── ...
//     │   ├── mangago
//     │   │   ├── manga-XXX
//     │   │   │   ├── chapter-XX
//     │   │   │   │   └── page-XX
//     │   │   │   └── ...
//     │   │   └── ...
//     │   ├── bato
//     │   └── myreadingmanga
//     └── image
//         ├── pixiv
//         ├── kemono
//         ├── gelbooru
//         ├── twitter
//         │   └── profiles
//         │       └── username-XXX
//         │           ├── tweets
//         │           │   └── tweet-XX
//         │           ├── likes
//         │           └── bookmarks
//         └── poipiku
//             └── XXXXXX
//                 └── XXXXXX
//                     └── image-XX