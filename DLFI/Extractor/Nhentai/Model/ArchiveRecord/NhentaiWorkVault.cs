using DLFI.Archive.RecordSystem.Model;
using DLFI.Archive.RecordSystem.Serialization;
using DLFI.Extractor.Nhentai.Model.Api;

namespace DLFI.Extractor.Nhentai.Model.ArchiveRecord;

[RawRecordItem("nhentai_work")]
public class NhentaiWorkVault : Vault
{
	// Used as to store what exactly we requested just in case for archiving-sake
	public ApiGalleryModel? Api { get; set; }
}