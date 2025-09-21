using System.Text.Json;
using DLFI.Extractor.Nhentai;
using DLFI.Records.Reader;
using DLFI.Serializer;

var options = new JsonSerializerOptions
{
	PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
	TypeInfoResolver = new PolymorphicTypeResolver()
};
const string path = @"C:\Users\elfia\OneDrive\Desktop\DL-FI Project\Serpent";
var main = GroupReader.Open(path, options);

// List<Task> tasks = [];
// foreach (var nhId in new int[] {
// 	598028, 597835, 597688
// })
// {
// 	var rr = main?.GetGroupRecursive(["Nhentai", "Galleries", $"{nhId}"]);
// 	if (rr == null) { return; }
// 	NhentaiWorkExtracter extracter = new(rr, nhId);
// 	tasks.Add(extracter.ExtractAsync());
// }
// foreach (var t in tasks) { await t; }