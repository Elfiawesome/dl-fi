using DLFI.Core.Extractors;

namespace DLFI.Extractors.Extractors;

public interface IExtractor
{
	string Name { get; }
	IAsyncEnumerable<ExtractionResult> ExtractAndStoreWorkAsync();
}