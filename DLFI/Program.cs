using DLFI.Archive;
using DLFI.Archive.RecordSystem.Model;
using DLFI.Extractor.Nhentai;

var archiveRoot = @"C:\Users\elfia\OneDrive\Desktop\dlfi-archive";
if (Directory.Exists(archiveRoot))
{
	Directory.Delete(archiveRoot, true); // Clean up previous runs
}

var arSv = new ArchiveService(archiveRoot);

// Root vault to store our archive
var serpentArchive = new Vault() { Name = "serpent-archive" };
arSv.AddItem(serpentArchive);

// A top-level vault which we will use relational-tags to relate works -> people/author
var people = new Vault() { Name = "people" };
arSv.AddItem(people, serpentArchive);

// A top-level vault to store manga related works
var manga = new Vault() { Name = "manga" };
arSv.AddItem(manga, serpentArchive);

// Manga specific vaults
var nhentai = new Vault() { Name = "nhentai" };
arSv.AddItem(nhentai, manga);
var myreadingmanga = new Vault() { Name = "myreadingmanga" };
arSv.AddItem(myreadingmanga, manga);
var bato = new Vault() { Name = "bato" };
arSv.AddItem(bato, manga);

var nhex = new NhentaiWorkExtractor(arSv);
await nhex.ExtractAndStoreWorkAsync(605454, nhentai.Id);
await nhex.ExtractAndStoreWorkAsync(604611, nhentai.Id);
await nhex.ExtractAndStoreWorkAsync(604252, nhentai.Id);
await nhex.ExtractAndStoreWorkAsync(604245, nhentai.Id);