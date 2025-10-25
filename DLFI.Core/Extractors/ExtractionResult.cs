using DLFI.Core.Archive.Domain.Models;

namespace DLFI.Core.Extractors;

public class ExtractionResult
{
	public required Node Node;
	public Dictionary<string, Stream> AttachmentStreams = [];
}