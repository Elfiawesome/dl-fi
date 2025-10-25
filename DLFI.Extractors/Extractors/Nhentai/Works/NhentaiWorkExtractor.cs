using System.Text.Json;
using DLFI.Core.Extractors;
using DLFI.Extractors.Extractors.Nhentai.Model.Api;
using DLFI.Extractors.Extractors.Nhentai.Model.Archive;

namespace DLFI.Extractors.Extractors.Nhentai.Works;


public class NhentaiWorkExtractor : IExtractor
{
	public string Name => "Nhentai";
	public const string GalleryApiLink = "https://nhentai.net/api/gallery/";


	private readonly HttpClient _client = new();
	private int _mediaIndex = 4;
	private int _targetId;


	public NhentaiWorkExtractor(int targetId)
	{
		_targetId = targetId;
	}


	public async IAsyncEnumerable<ExtractionResult> ExtractAndStoreWorkAsync()
	{
		var workInfoRequest = await _client.GetAsync($"{GalleryApiLink}{_targetId}");
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
				Name = data.Id.ToString(),
				Api = data
			}
		};

		for (var i = 0; i < data.Images.Pages.Count; i++)
		{
			var pageNum = i + 1;
			var pageInfo = data.Images.Pages[i];
			string extension = pageInfo.Type.ToLower() switch { "j" => "jpg", "p" => "png", "w" => "webp", _ => "gif" };

			HttpResponseMessage? imageResponse = null;
			for (int mediaServerIndex = 3; mediaServerIndex <= 7; mediaServerIndex++) //Idk why is i1 server so slow...
			{
				string imageUrl = $"https://i{mediaServerIndex}.nhentai.net/galleries/{data.MediaId}/{pageNum}.{extension}";
				try
				{
					imageResponse = await _client.GetAsync(imageUrl);
					if (imageResponse.IsSuccessStatusCode) break;

					imageResponse.Dispose(); // Dispose failed response before retrying
					imageResponse = null;
				}
				catch { /* Ignore and try next server */ }
			}

			if (imageResponse?.IsSuccessStatusCode == true)
			{
				var imageStream = new MemoryStream();
				await imageResponse.Content.CopyToAsync(imageStream);
				imageStream.Position = 0;

				var pageEntry = new NhentaiWorkPageEntry
				{
					Name = $"{pageNum}",
					PageIndex = pageNum
				};

				yield return new ExtractionResult
				{
					Node = pageEntry,
					AttachmentStreams = new Dictionary<string, Stream>
					{
						{ $"{pageEntry.Name}.{extension}", imageStream }
					}
				};
				imageResponse.Dispose();
			}
			else
			{
			}
		}
	}
}