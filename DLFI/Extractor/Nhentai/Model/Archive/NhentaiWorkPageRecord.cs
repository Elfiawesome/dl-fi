using DLFI.Archive.Model.Common;

namespace DLFI.Extractor.Nhentai.Model.Archive;

/// <summary>
/// A specific page from a Nhentai work.
/// </summary>
public class NhentaiWorkPageRecord : MangaPageRecord
{
	public string OriginalSourceUrl { get; set; } = "";
}