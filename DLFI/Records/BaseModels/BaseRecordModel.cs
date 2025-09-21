namespace DLFI.Records.BaseModels;

public class BaseRecordModel
{
	public BaseRecordModel()
	{
		DateCreated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		DateRecorded = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
	public int Version { get; set; } = 0;
	// Given name to be used by us
	public string Name { get; set; } = "";
	// Date Created (can be copied from the individual sources itself)
	public long DateCreated { get; set; }
	// Date recorded into the system (must)
	public long DateRecorded { get; set; }
	public HashSet<string> Tags { get; set; } = [];

	public HashSet<string> Attachments { get; set; } = [];
}