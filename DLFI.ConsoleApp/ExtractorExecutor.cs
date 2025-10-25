using DLFI.Archive.Persistence;
using DLFI.Core.Archive.Domain.Models;
using DLFI.Extractors.Extractors.Nhentai.Works;

public class ExtractorExecutor(FileSystemArchiveService archiveService)
{
	public FileSystemArchiveService ArchiveService = archiveService;

	public async Task Start(NhentaiWorkExtractor extractor, Guid? parentId = null)
	{
		Guid? currentTargetId = parentId;
		await foreach (var result in extractor.ExtractAndStoreWorkAsync())
		{
			// I dont like how AddNode is designed bruh...
			if (currentTargetId == null)
			{
				ArchiveService.AddNode(result.Node, result.AttachmentStreams);
			}
			else
			{
				ArchiveService.AddNode(result.Node, (Guid)currentTargetId, result.AttachmentStreams);
			}



			if (result.Node is Entry)
			{

			}
			if (result.Node is Vault)
			{
				currentTargetId = result.Node.Id;
			}
		}
	}
}