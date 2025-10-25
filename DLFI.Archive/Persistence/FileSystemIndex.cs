using DLFI.Core.Archive.Domain.Models;

namespace DLFI.Archive.Persistence;

public class FileSystemIndex
{
	public enum Type { Vault, Entry }
	public string Name = "";
	public HashSet<Guid> Relationships = [];
	public Type IndexType;
	public string RelativePath;

	public FileSystemIndex(Node node, string relativePath)
	{
		RelativePath = relativePath;
		Name = node.Name;
		Relationships = node.Relationship;
		if (node is Entry) { IndexType = Type.Entry; } else { IndexType = Type.Vault; }
	}
}