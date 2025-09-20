using DLFI.Records;

namespace DLFI.Extractor;

public abstract class BaseExtracter
{
	protected readonly RecordsReader _recordsReader;
	public virtual string[] BaseGroup { get; protected set; } = [];

	public BaseExtracter(RecordsReader recordsReader)
	{
		_recordsReader = recordsReader;
	}

	public abstract Task ExtractAsync();
}

public class ExtractedItem
{
	public required BaseRecordModel RecordModel;
}
