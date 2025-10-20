using DLFI.Extractor.Nhentai.VaultModel;
using DLFI.Record;
using DLFI.Record.Accessor;
using DLFI.Record.Model.Base;

string archiveFolderPath = @"C:\Users\elfia\OneDrive\Desktop\dlfi-archive\";

var v = VaultAccessor<BaseVaultModel>.Open<BaseVaultModel>(archiveFolderPath);
var v2 = v?.ChangeModel<NhentaiWorkModel>();