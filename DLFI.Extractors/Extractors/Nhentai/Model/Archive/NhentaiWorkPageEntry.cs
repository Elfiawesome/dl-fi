using DLFI.Core.Archive.Json;
using DLFI.Core.Archive.Model;
using DLFI.Extractors.Extractors.Nhentai.Model.Api;

namespace DLFI.Extractors.Extractors.Nhentai.Model.Archive;

[NodeItem("nhentai_page")]
public class NhentaiWorkPageEntry : Entry
{
	public int PageIndex;
}