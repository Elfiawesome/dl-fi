using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DLFI.Archive.RecordSystem.Model;

namespace DLFI.Archive.RecordSystem.Serialization;

public class RawRecordResolver : DefaultJsonTypeInfoResolver
{
	public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

		if (jsonTypeInfo.Type == typeof(RawRecord))
		{
			jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
			{
				TypeDiscriminatorPropertyName = "record_type",
				UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
			};

			RawRecordTypeMap.IdToType.ToList().ForEach((x) =>
			{
				jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(x.Value, x.Key));
			});
			Console.WriteLine("Okay!!");
		}

		return jsonTypeInfo;
	}
}