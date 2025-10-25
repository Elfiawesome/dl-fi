using System.Text.Json;
using DLFI.Archive;
using DLFI.Extractor.Nhentai.Model.Api;
using DLFI.Extractor.Nhentai.Model.ArchiveRecord;

namespace DLFI.Extractor.Nhentai;

public class NhentaiWorkExtractor
{
	private readonly ArchiveService _archiveService;
	private readonly HttpClient _client = new();
	private const string GalleryApiLink = "https://nhentai.net/api/gallery/";

	private int _mediaIndex = 1;

	public NhentaiWorkExtractor(ArchiveService archiveService)
	{
		_archiveService = archiveService;
	}

	public async Task ExtractAndStoreWorkAsync(int workId, Guid? parentId = null)
	{
		Console.WriteLine($"Extracting work ID: {workId}");
		var req = await _client.GetAsync($"{GalleryApiLink}{workId}");
		Console.WriteLine($"Received work ID: {workId}");
		if (!req.IsSuccessStatusCode)
		{
			Console.WriteLine($"Failed to get API data for {workId}.");
			return;
		}

		var jsonContent = await req.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<ApiGalleryModel>(jsonContent);
		if (data == null) return;

		// Add Vault
		var workVault = new NhentaiWorkVault()
		{
			Name = $"{workId}",
			DisplayName = data.Title.English,
			Api = data
		};
		if (parentId == null) { _archiveService.AddItem(workVault); } else { _archiveService.AddItem(workVault, (Guid)parentId); }


		for (var i = 0; i < data.Images.Pages.Count; i++)
		{
			var pageNum = i + 1;
			var pageInfo = data.Images.Pages[i];
			string extension = pageInfo.Type.ToLower() switch { "j" => "jpg", "p" => "png", "w" => "webp", _ => "gif" };

			HttpResponseMessage imageResponse;
			while (true)
			{
				string imageUrl = $"https://i{_mediaIndex}.nhentai.net/galleries/{data.MediaId}/{pageNum}.{extension}";
				try
				{
					Console.WriteLine($"  - Downloading page {pageNum}... ");
					imageResponse = await _client.GetAsync(imageUrl);
					Console.WriteLine($"  Received response ... ");
					if (imageResponse.IsSuccessStatusCode)
					{
						using var imageStream = new MemoryStream();
						await imageResponse.Content.CopyToAsync(imageStream);
						imageStream.Position = 0; // Reset stream for reading? (TODO: wat?)

						// Add record
						_archiveService.AddItem(new NhentaiWorkPageRecord()
						{
							Name = $"{pageNum}",
							DisplayName = $"Page {pageNum}",
							AttachmentStreams = { { $"{pageNum}.{extension}", imageStream } }
						}, workVault.Id);


						break;
					}
				}
				catch
				{
					_mediaIndex++;
					if (_mediaIndex > 3) { _mediaIndex = 1; }
				}
			}
		}

	}
}