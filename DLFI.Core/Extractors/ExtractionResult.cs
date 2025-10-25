using DLFI.Core.Archive.Model;

namespace DLFI.Core.Extractors;

public class ExtractionResult
{
	public required Node Node;
	public Dictionary<string, Stream> AttachmentStreams = [];
}