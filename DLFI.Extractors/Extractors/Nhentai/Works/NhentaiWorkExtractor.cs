using System.Text.Json;
using DLFI.Core.Extractors;
using DLFI.Extractors.Extractors.Nhentai.Model.Api;
using DLFI.Extractors.Extractors.Nhentai.Model.Archive;

namespace DLFI.Extractors.Extractors.Nhentai.Works;

public class NhentaiWorkExtractor
{
	public const string GalleryApiLink = "https://nhentai.net/api/gallery/";


	private readonly HttpClient _client = new();
	private int _mediaIndex = 1;


	public async IAsyncEnumerable<ExtractionResult> ExtractAndStoreWorkAsync(int workId)
	{
		var workInfoRequest = await _client.GetAsync($"{GalleryApiLink}{workId}");
		if (!workInfoRequest.IsSuccessStatusCode)
		{
			yield break;
		}

		var jsonContent = await workInfoRequest.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<ApiGalleryModel>(jsonContent);
		if (data == null) yield break;

		// Save Work Data
		yield return new()
		{
			Node = new NhentaiWorkVault()
			{
				Name = data.Id.ToString()
			}
		};

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

						// Save Image here...

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
		yield break;
	}
}