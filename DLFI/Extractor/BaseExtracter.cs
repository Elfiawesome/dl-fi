using DLFI.Records.BaseModels;
using DLFI.Records.Reader;

namespace DLFI.Extractor;

public abstract class BaseExtracter
{
	protected readonly GroupReader _groupReader;
	public virtual string[] BaseGroup { get; protected set; } = [];

	public BaseExtracter(GroupReader groupReader)
	{
		_groupReader = groupReader;
	}

	public abstract Task ExtractAsync();
}

public class ExtractedItem
{
	public required BaseRecordModel RecordModel;
}
