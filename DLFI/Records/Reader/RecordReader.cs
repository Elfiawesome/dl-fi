using DLFI.Records.BaseModels;

namespace DLFI.Records.Reader;

public class RecordReader<T> : BaseReader
	where T : BaseRecordModel
{
	public readonly T Record;

	public RecordReader(string nameId, T record, GroupReader groupReader) : base(groupReader, nameId)
	{
		Record = record;
	}

	public IEnumerable<Stream> GetAttachments()
	{
		foreach (var attachmentName in Record.Attachments)
		{
			var stream = GetAttachment(attachmentName);
			if (stream != null) { yield return stream; }
		}
	}

	public Stream? GetAttachment(string attachmentName)
	{
		if (ParentReader == null) { return null; }
		if (ParentReader is GroupReader gr)
		{
			return gr.GetFile(attachmentName);
		}
		return null;
	}

	public void SaveRecord(PendingAttachment[]? pendingAttachments = null)
	{
		if (ParentReader == null) { return; }
		if (ParentReader is GroupReader gr)
		{
			if (pendingAttachments == null)
			{
				gr.StoreRecord(Id, Record, true);
			}
			else
			{
				gr.StoreRecord(Id, Record, pendingAttachments, true);
			}
		}
	}
}