using DLFI.Core.Archive.Json;

namespace DLFI.Core.Archive.Domain.Models;

[NodeItem("vault")]
public class Vault : Node
{
	public string ThumbnailViewType{ get; set; } = ""; // [First Item only] / [All Items]
}