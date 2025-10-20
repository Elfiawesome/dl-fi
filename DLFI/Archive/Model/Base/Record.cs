namespace DLFI.Archive.Model.Base;

/// <summary>
/// Represents a single data item in the archive, typically stored as a single JSON file.
/// </summary>
public class Record : RawRecord
{
	public List<string> Attachments { get; set; } = [];
}