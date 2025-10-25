namespace DLFI.Core.Archive.Json;

[AttributeUsage(AttributeTargets.Class)]
public class NodeItemAttribute(string id) : Attribute
{
	public string Id { get; set; } = id;
}