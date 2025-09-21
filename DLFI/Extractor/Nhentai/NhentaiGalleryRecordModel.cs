using DLFI.Records.BaseModels;

namespace DLFI.Extractor.Nhentai;

public class NhentaiGalleryRecordModel : BaseRecordModel
{
	public ApiGalleryModel Gallery { get; set; } = new();
}