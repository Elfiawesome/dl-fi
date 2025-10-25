using DLFI.Core.Archive.Json;

namespace DLFI.Core.Archive.Domain.Models;

[NodeItem("entry")]
public class Entry : Node
{
	public HashSet<string> Attachments{ get; set; } = [];
}