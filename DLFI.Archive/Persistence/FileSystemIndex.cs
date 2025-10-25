using DLFI.Core.Archive.Domain.Models;

namespace DLFI.Archive.Persistence;

public class FileSystemIndex
{
	public string Name = "";
	public HashSet<Guid> Relationships = [];
	public int IndexType = 0;
	public string RelativePath;

	public FileSystemIndex(Node node, string relativePath)
	{
		RelativePath = relativePath;
		Name = node.Name;
		Relationships = node.Relationship;
		if (node is Entry) { IndexType = 1; } else { IndexType = 0; }
	}
}