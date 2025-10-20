using DLFI.Record.Model.Base;

namespace DLFI.Extractor.Nhentai.VaultModel;

public class NhentaiWorkModel : BaseVaultModel
{
	public Dictionary<string, string> TranslatedWorkNames { get; set; } = new()
	{
		{"1", "2"}
	};
}