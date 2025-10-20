using System.Text.Json.Serialization;

namespace DLFI.Extractor.Nhentai;

public class ApiGalleryModel
{
	[JsonPropertyName("id")] public int Id { get; set; }
	[JsonPropertyName("media_id")] public string MediaId { get; set; } = "";
	[JsonPropertyName("title")] public ApiGalleryTitleModel Title { get; set; } = new();
	[JsonPropertyName("images")] public ApiImagesModel Images { get; set; } = new();
	[JsonPropertyName("scanlator")] public string Scanlator { get; set; } = "";
	[JsonPropertyName("upload_date")] public int UploadDate { get; set; }
	[JsonPropertyName("tags")] public List<ApiTagsModel> Tags { get; set; } = [];
	[JsonPropertyName("num_pages")] public int NumPages { get; set; }
	[JsonPropertyName("num_favorites")] public int NumFavorites { get; set; }
}

public class ApiGalleryTitleModel
{
	[JsonPropertyName("english")] public string English { get; set; } = "";
	[JsonPropertyName("japanese")] public string Japanese { get; set; } = "";
	[JsonPropertyName("pretty")] public string Pretty { get; set; } = "";
}

public class ApiImagesModel
{
	[JsonPropertyName("pages")] public List<ApiImagePagesModel> Pages { get; set; } = [];
}

public class ApiImagePagesModel
{
	[JsonPropertyName("t")] public string Type { get; set; } = "";
	[JsonPropertyName("w")] public int Width { get; set; }
	[JsonPropertyName("h")] public int Height { get; set; }
}

public class ApiTagsModel
{
	[JsonPropertyName("id")] public int Id { get; set; }
	[JsonPropertyName("type")] public string Type { get; set; } = "";
	[JsonPropertyName("name")] public string Name { get; set; } = "";
	[JsonPropertyName("url")] public string Url { get; set; } = "";
	[JsonPropertyName("count")] public int Count { get; set; }
}
