using DLFI.Archive.RecordSystem.Serialization;

namespace DLFI.Archive.RecordSystem.Model;

[RawRecordItem("base")]
public abstract class RawRecord
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public required string Name { get; set; }
	public string DisplayName { get; set; } = string.Empty;
	public HashSet<string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
	public HashSet<Guid> RelationalTags { get; set; } = [];
}