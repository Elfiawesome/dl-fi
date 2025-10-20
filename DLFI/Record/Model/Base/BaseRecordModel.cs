namespace DLFI.Record.Model.Base;

public class BaseRecordModel : BaseRawRecordModel
{
	public HashSet<string> Attachments { get; set; } = [];
}