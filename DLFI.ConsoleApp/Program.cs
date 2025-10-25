using DLFI.Archive.Persistence;
using DLFI.Core.Archive.Domain.Models;
using DLFI.Core.Archive.Json;
using DLFI.Extractors.Extractors.Nhentai.Works;

var archiveRoot = @"C:\Users\elfia\OneDrive\Desktop\dlfi-archive";
if (Directory.Exists(archiveRoot))
{
	Directory.Delete(archiveRoot, true); // Clean up previous runs
}


Console.WriteLine("Setting up services...");

// Need to improve on instantiating the FileSystemArchiveService
NodeTypeMap nodeTypeMap = new();
nodeTypeMap.RegisterAssembly(typeof(Node).Assembly);
nodeTypeMap.RegisterAssembly(typeof(NhentaiWorkExtractor).Assembly);
NodeSerializer serializer = new(nodeTypeMap);
var fsas = new FileSystemArchiveService(archiveRoot, serializer);


var extr1 = new NhentaiWorkExtractor(604225);
var extr2 = new NhentaiWorkExtractor(604225);
var extr3 = new NhentaiWorkExtractor(604225);
var exec = new ExtractorExecutor(fsas);

var task1= exec.Start(extr1, fsas.CommonPoint?.Manga);
var task2= exec.Start(extr2, fsas.CommonPoint?.Manga);
var task3 = exec.Start(extr3, fsas.CommonPoint?.Manga);

await task1;
await task2;
await task3;

Console.WriteLine("Done!");