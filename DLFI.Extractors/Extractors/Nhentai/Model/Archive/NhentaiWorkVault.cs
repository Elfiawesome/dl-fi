using DLFI.Core.Archive.Domain.Models;
using DLFI.Core.Archive.Json;
using DLFI.Extractors.Extractors.Nhentai.Model.Api;

namespace DLFI.Extractors.Extractors.Nhentai.Model.Archive;

[NodeItem("nhentai_work")]
public class NhentaiWorkVault : Vault
{
	public ApiGalleryModel? Api { get; set; }
}