using DLFI.Archive.Persistence;
using DLFI.Core.Archive.Domain.Models;
using DLFI.Extractors.Extractors;

public class ExtractorExecutor(FileSystemArchiveService archiveService)
{
	public FileSystemArchiveService ArchiveService = archiveService;

	public async Task Start(IExtractor extractor, Guid? parentId = null)
	{
		Guid? currentTargetId = parentId;
		await foreach (var result in extractor.ExtractAndStoreWorkAsync())
		{
			ArchiveService.AddNode(result.Node, currentTargetId, result.AttachmentStreams);

			if (result.Node is Vault)
			{
				currentTargetId = result.Node.Id;
			}
		}
	}
}