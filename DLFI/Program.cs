using System.Linq.Expressions;
using DLFI.Archive;
using DLFI.Archive.Model.Base;
using DLFI.Extractor.Nhentai;
using DLFI.Extractor.Nhentai.Model.Archive;

var archiveRoot = @"C:\Users\elfia\OneDrive\Desktop\dlfi-archive";
// if (Directory.Exists(archiveRoot))
// {
// 	Directory.Delete(archiveRoot, true); // Clean up previous runs
// }


var service = new ArchiveService(archiveRoot);


// foreach (var entry in service.FindByType<NhentaiWorksVault>((x) => x.Name == ""))
// {
// 	Console.WriteLine(entry.Id);
// }

// var rootVault = new Vault { Name = "Root", DisplayName = "My Archive" };
// var mangaVault = new Vault { Name = "Manga", DisplayName = "Manga" };
// service.AddVault(rootVault);
// service.AddVault(mangaVault, rootVault.Id);

// int[] download_id = [
// 	604704,
// 	604611,
// 	579312,
// 	417602
// ];
// List<Task> tasks = [];
// foreach (int id in download_id)
// {
// 	var extractor = new NhentaiWorkExtractor(service);
// 	tasks.Add(extractor.ExtractAndStoreWorkAsync(id, mangaVault.Id));
// }
// Task.WaitAll(tasks);


