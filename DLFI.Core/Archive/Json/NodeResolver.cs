using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DLFI.Core.Archive.Domain.Models;

namespace DLFI.Core.Archive.Json;

public class NodeResolver : DefaultJsonTypeInfoResolver
{
	public NodeTypeMap Mapping;

	public NodeResolver(NodeTypeMap mapping)
	{
		Mapping = mapping;
	}

	public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

		if (jsonTypeInfo.Type == typeof(Node))
		{
			jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
			{
				TypeDiscriminatorPropertyName = "node_type",
				UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
			};

			Mapping.IdToType.ToList().ForEach((x) =>
			{
				jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(x.Value, x.Key));
			});
		}

		return jsonTypeInfo;
	}
}