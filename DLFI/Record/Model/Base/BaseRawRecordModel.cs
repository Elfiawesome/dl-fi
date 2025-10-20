namespace DLFI.Record.Model.Base;

public class BaseRawRecordModel
{
	public int Version { get; set; } = 0;

	// Same Name as folder/file
	public string Name { get; set; } = "";
	// Display Name
	public string DisplayName { get; set; } = "";
	// string tagging
	public HashSet<string> Tags { get; set; } = [];
}