using System.Text.Json;
using DLFI.Archive;
using DLFI.Extractor.Nhentai.Model.Archive;

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

	public async Task ExtractAndStoreWorkAsync(int workId, Guid parentVaultId)
	{
		Console.WriteLine($"Extracting work ID: {workId}");
		var req = await _client.GetAsync($"{GalleryApiLink}{workId}");
		if (!req.IsSuccessStatusCode)
		{
			Console.WriteLine($"Failed to get API data for {workId}.");
			return;
		}

		var jsonContent = await req.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<ApiGalleryModel>(jsonContent);
		if (data == null) return;

		// 1. Create and Save the Vault for the work
		var workVault = new NhentaiWorksVault()
		{
			Name = $"{data.Id}",
			DisplayName = data.Title.English,
			SourceId = data.Id.ToString(),
			UploadDate = DateTimeOffset.FromUnixTimeSeconds(data.UploadDate).UtcDateTime,
			Tags = new HashSet<string>(data.Tags.Select(t => t.Name), StringComparer.OrdinalIgnoreCase)
		};
		_archiveService.AddVault(workVault, parentVaultId);
		Console.WriteLine($"Created vault for '{workVault.DisplayName}'");

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
					Console.Write($"  - Downloading page {pageNum}... ");
					imageResponse = await _client.GetAsync(imageUrl);
					Console.Write($"  Received response ... ");
					if (imageResponse.IsSuccessStatusCode)
					{
						using var imageStream = new MemoryStream();
						await imageResponse.Content.CopyToAsync(imageStream);
						imageStream.Position = 0; // Reset stream for reading? (TODO: wat?)

						var pageRecord = new NhentaiWorkPageRecord
						{
							Name = $"{pageNum}",
							DisplayName = $"Page {pageNum}",
							PageNumber = pageNum,
							OriginalSourceUrl = imageUrl
						};

						var attachments = new Dictionary<string, Stream>
						{
							{ $"{pageRecord.Name}.{extension}", imageStream }
						};

						_archiveService.AddRecord(pageRecord, workVault.Id, attachments);
						Console.WriteLine("Done.");
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