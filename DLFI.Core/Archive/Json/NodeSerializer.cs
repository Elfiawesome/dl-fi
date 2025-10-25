using System.Text.Json;
using DLFI.Core.Archive.Model;

namespace DLFI.Core.Archive.Json;

public class NodeSerializer(NodeTypeMap mapping)
{
	private JsonSerializerOptions _option = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		TypeInfoResolver = new NodeResolver(mapping),
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
	};

	public string Serialize<T>(T node)
		where T : Node
	{
		return JsonSerializer.Serialize<Node>(node, _option);
	}

	public T? Deserialize<T>(string json)
			where T : Node
	{
		return JsonSerializer.Deserialize<T>(json, _option);
	}
}