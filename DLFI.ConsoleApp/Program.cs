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


var extr = new NhentaiWorkExtractor(604225);
var exec = new ExtractorExecutor(fsas);

await exec.Start(extr, fsas.CommonPoint?.Manga);