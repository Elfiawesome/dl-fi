namespace DLFI.Archive;

public enum RecordItemType { Vault, Record }

public class IndexEntry
{
	public Guid Id { get; set; }
	public Guid? ParentId { get; set; }
	public RecordItemType ItemType { get; set; }

	/// <summary>
	/// The full relative path to the item's file or directory from the archive root.
	/// For a Vault: "Manga/Nhentai/Works/a-specific-work-12345"
	/// For a Record: "Manga/Nhentai/Works/a-specific-work-12345/page-01.json"
	/// </summary>
	public required string Path { get; set; }

	public required string Name { get; set; }
	public required string DisplayName { get; set; }

	/// <summary>
	/// The C#/.NET full type name, used for deserialization.
	/// </summary>
	public required string RecordType { get; set; }

	/// <summary>
	/// A copy of the tags for fast searching without needing to open the JSON file.
	/// </summary>
	public HashSet<string> Tags { get; set; } = new HashSet<string>();
}