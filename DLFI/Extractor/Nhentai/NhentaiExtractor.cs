using System.Text.Json;
using DLFI.Records;

namespace DLFI.Extractor.Nhentai;

public class NhentaiWorkExtracter(RecordsReader recordsReader, int targetId) : BaseExtracter(recordsReader)
{
	public const string GalleryApiLink = "https://nhentai.net/api/gallery/";
	public const string GalleriesApiLink = "https://i3.nhentai.net/galleries/";
	public HttpClient client = new();
	public int TargetId = targetId; //597688;

	public override async Task ExtractAsync()
	{
		var link = $"{GalleryApiLink}{TargetId}";
		var req = await client.GetAsync(link);
		var htmlContent = await req.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<ApiGalleryModel>(htmlContent);
		_recordsReader.StoreRecord(TargetId.ToString(), new NhentaiGalleryRecordModel() { Gallery = data ?? new() });
		if (data == null) { return; }


		for (var pageIndex = 0; pageIndex < data.Images.Pages.Count; pageIndex++)
		{
			var page = data.Images.Pages[pageIndex];
			var fileType = page.Type switch
			{
				"w" => "webp",
				_ => "jpg"
			};
			var pageLink = $"{GalleriesApiLink}{data.MediaId}/{pageIndex + 1}.{fileType}";
			var pageReq = await client.GetAsync(pageLink);
			var dataStream = pageReq.Content.ReadAsStream();

			var record = new BaseDownloadableRecordModel() { DownloadLink = pageLink };
			var pendingAttachment = new PendingAttachment(fileType, dataStream);
			_recordsReader.StoreRecord($"{TargetId}-{pageIndex + 1}", record, [pendingAttachment]);
		}
	}
}