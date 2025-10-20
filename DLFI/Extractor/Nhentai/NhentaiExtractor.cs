using System.Text.Json;
using System.Text.RegularExpressions;
using DLFI.Records.BaseModels;
using DLFI.Records.Reader;

namespace DLFI.Extractor.Nhentai;

public class NhentaiWorkExtracter(GroupReader groupReader, int targetId) : BaseExtracter(groupReader)
{
	public const string GalleryApiLink = "https://nhentai.net/api/gallery/";

	public string GalleriesApiLink
	{
		get
		{
			_galleryApiIndex++;
			if (_galleryApiIndex > 3) { _galleryApiIndex = 1; }
			return $"https://i{_galleryApiIndex}.nhentai.net/galleries/";
		}
	}
	private int _galleryApiIndex = 1;

	public HttpClient client = new();
	public int TargetId = targetId; //597688;

	public override async Task ExtractAsync()
	{
		var link = $"{GalleryApiLink}{TargetId}";
		var req = await client.GetAsync(link);
		var htmlContent = await req.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<ApiGalleryModel>(htmlContent);
		groupReader.StoreRecord("work", new NhentaiGalleryRecordModel() { Gallery = data ?? new() });
		if (data == null) { return; }


		for (var pageIndex = 0; pageIndex < data.Images.Pages.Count; pageIndex++)
		{
			var page = data.Images.Pages[pageIndex];
			var fileType = page.Type switch
			{
				"w" => "webp",
				_ => "jpg"
			};

			string pageLink;
			HttpResponseMessage pageReq;
			while (true)
			{
				try
				{
					pageLink = $"{GalleriesApiLink}{data.MediaId}/{pageIndex + 1}.{fileType}";
					pageReq = await client.GetAsync(pageLink);
					if (pageReq.IsSuccessStatusCode) { break; }
				}
				catch
				{

				}
			}
			var dataStream = pageReq.Content.ReadAsStream();
			var record = new BaseDownloadableRecordModel() { DownloadLink = pageLink };
			var pendingAttachment = new PendingAttachment(fileType, dataStream);
			groupReader.StoreRecord($"{pageIndex + 1}", record, [pendingAttachment]);
		}
	}
}