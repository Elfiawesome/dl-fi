using System.Text.Json;
using DLFI.Archive.RecordSystem.Model;

namespace DLFI.Archive.RecordSystem.Serialization;

public class RawRecordSerializer
{

	private static JsonSerializerOptions _option = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		TypeInfoResolver = new RawRecordResolver(),
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
	};

	public static string Serialize<T>(T record)
		where T : RawRecord
	{
		return JsonSerializer.Serialize<RawRecord>(record, _option);
	}

	public static T? Deserialize<T>(string json)
			where T : RawRecord
	{
		return JsonSerializer.Deserialize<T>(json, _option);
	}
}