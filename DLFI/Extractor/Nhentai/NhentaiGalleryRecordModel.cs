using DLFI.Records;

namespace DLFI.Extractor.Nhentai;

public class NhentaiGalleryRecordModel : BaseRecordModel
{
	public ApiGalleryModel Gallery { get; set; } = new();
}