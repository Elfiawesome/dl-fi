using DLFI.Archive.Model.Base;

namespace DLFI.Extractor.Nhentai.Model.Archive;

/// <summary>
/// A specialized Vault to represent a specific manga work, containing its own unique metadata.
/// </summary>
public class NhentaiWorksVault : Vault
{
	public string SourceId { get; set; } = "";
	public DateTime UploadDate { get; set; }
	public ApiGalleryModel RawApi { get; set; } = new();
	public List<string> Artists { get; set; } = [];
}