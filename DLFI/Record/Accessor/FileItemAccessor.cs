using System.Text.Json;

namespace DLFI.Record.Accessor;

public abstract class FileItemAccessor(string path)
{
	public const string RecordsExtension = ".json";
	private readonly string _rawDirPath = Path.TrimEndingDirectorySeparator(path);

	// Full File Path (C:/path/to/record-file.json) or (C:/path/to/vault)
	public string DirPath => _rawDirPath;

	// Name including ext (record-file.json) or (vault)
	public string DirName => Path.GetFileName(DirPath);

	// Name without ext (record-file) or (vault)
	public string DirNameWithoutExtension => Path.GetFileNameWithoutExtension(DirPath);

	internal JsonSerializerOptions jsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
	};
}