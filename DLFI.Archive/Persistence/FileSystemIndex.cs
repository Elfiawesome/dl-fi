using DLFI.Core.Archive.Domain.Models;

namespace DLFI.Archive.Persistence;

public class FileSystemIndex
{
	public enum Type { Vault, Entry }

	public string Name { get; set; } = "";
	public HashSet<Guid> Relationships { get; set; } = [];
	public Type IndexType { get; set; }
	public string RelativePath { get; set; }

	public FileSystemIndex(Node node, string relativePath)
	{
		RelativePath = relativePath;
		Name = node.Name;
		Relationships = node.Relationship;
		if (node is Entry) { IndexType = Type.Entry; } else { IndexType = Type.Vault; }
	}
}