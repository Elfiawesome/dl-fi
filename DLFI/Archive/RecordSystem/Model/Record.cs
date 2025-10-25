using System.Text.Json.Serialization;
using DLFI.Archive.RecordSystem.Serialization;

namespace DLFI.Archive.RecordSystem.Model;

[RawRecordItem("record")]
public class Record : RawRecord
{
	public List<string> Attachments{ get; set; } = [];

	[JsonIgnore]
	public Dictionary<string, Stream> AttachmentStreams = [];
}