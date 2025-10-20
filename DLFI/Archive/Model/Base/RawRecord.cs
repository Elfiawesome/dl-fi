namespace DLFI.Archive.Model.Base;

/// <summary>
/// The absolute base class for any item in the archive, whether it's a folder (Vault) or item (Record).
/// </summary>
public abstract class RawRecord
{
	public Guid Id { get; set; } = Guid.NewGuid();

	/// <summary>
	/// The programmatic/file-safe name. Used for directory or file names.
	/// Example: "page_1"
	/// </summary>
	public string Name { get; set; } = "";

	/// <summary>
	/// The user-friendly name for display purposes.
	/// Example: "Page 1"
	/// </summary>
	public string DisplayName { get; set; } = "";

	/// <summary>
	/// A set of tags applied directly to this item.
	/// </summary>
	public HashSet<string> Tags { get; set; } = [];

	/// <summary>
    /// A "type discriminator" used for polymorphic deserialization.
    /// This tells our storage manager which specific class this JSON object represents.
    /// </summary>
    public string? RecordType => GetType().FullName;
}