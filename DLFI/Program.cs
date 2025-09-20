using System.Text.Json;
using DLFI.Extractor.Nhentai;
using DLFI.Records;
using DLFI.Serializer;

var options = new JsonSerializerOptions
{
	PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
	TypeInfoResolver = new PolymorphicTypeResolver()
};
string path = @"C:\Users\elfia\OneDrive\Desktop\dl-fi-2\Serpent";
var main = RecordsReader.Open(path, options);

main.Query([]).ToList().ForEach((e) =>
{
	Console.WriteLine(e);
});

// int tgt = 597863;
// var rr = main?.GetGroupRecursive(["Nhentai", "Galleries", $"{tgt}"]);
// if (rr == null) { return; }
// NhentaiWorkExtracter extracter = new(rr, tgt);
// await extracter.ExtractAsync();
