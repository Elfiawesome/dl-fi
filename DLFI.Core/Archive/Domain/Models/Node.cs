using DLFI.Core.Archive.Json;

namespace DLFI.Core.Archive.Domain.Models;

[NodeItem("node")]
public class Node
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public required string Name { get; set; }
	public HashSet<Guid> Relationship { get; set; } = [];

	public Node() { }
	public Node(string name)
	{
		Name = name;
	}
}