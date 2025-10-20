using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DLFI.Extractor.Nhentai;
using DLFI.Records.BaseModels;

namespace DLFI.Records.Reader;

public class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
	public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);
		Type baseType = typeof(BaseRecordModel);

		if (jsonTypeInfo.Type == baseType)
		{
			jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
			{
				TypeDiscriminatorPropertyName = "type",
				IgnoreUnrecognizedTypeDiscriminators = true,
				UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
				DerivedTypes = {
					new JsonDerivedType(typeof(BaseRecordModel), "base"),
					new JsonDerivedType(typeof(BaseDownloadableRecordModel), "base.downloadable"),
					new JsonDerivedType(typeof(NhentaiGalleryRecordModel), "nhentai.gallery"),
				}
			};
		}
		return jsonTypeInfo;
	}
}