using System.Text.Json;
using DLFI.Record.Model.Base;

namespace DLFI.Record.Accessor;

public class VaultAccessor<T> : FileItemAccessor
	where T : BaseVaultModel
{
	public T? VaultModel { get; private set; }

	public const string VaultFileName = ".vault.json";
	public string VaultFile => Path.Join(DirPath, VaultFileName);
	public string[] Vaults => Directory.GetDirectories(DirPath);
	public string[] Records => [.. Directory.GetFiles(DirPath).
										Where((x) => Path.GetFileName(x) != Path.ChangeExtension(DirName, "json")).
										Where((x) => Path.GetExtension(x) == RecordsExtension)
								];


	private VaultAccessor(string path) : base(path)
	{
		if (!Directory.Exists(DirPath)) { return; }
		if (Path.Exists(VaultFile))
		{
			Console.WriteLine($"Open Vault with a '{VaultFileName}' at {VaultFile}");
			VaultModel = JsonSerializer.Deserialize<T>(File.ReadAllBytes(VaultFile));
		}
		else
		{
			Console.WriteLine($"Open Vault without a '{VaultFileName}'");
		}
	}

	// Open a root vault from external
	public static VaultAccessor<TModel>? Open<TModel>(string fullPath)
		where TModel : BaseVaultModel
	{
		if (Directory.Exists(fullPath))
		{
			return new VaultAccessor<TModel>(fullPath);
		}
		return null;
	}

	// Open a child vault from internal
	public VaultAccessor<TModel>? OpenVault<TModel>(string name)
		where TModel : BaseVaultModel
	{
		string newGroupPath = Path.Join(DirPath, name);
		return Open<TModel>(newGroupPath);
	}

	public IEnumerable<VaultAccessor<BaseVaultModel>> ListGroups()
	{
		foreach (var dirPath in Vaults)
		{
			var group = Open<BaseVaultModel>(dirPath);
			if (group == null) { continue; }
			yield return group;
		}
	}

	public IEnumerable<RecordAccessor<BaseRecordModel>> ListRecords()
	{
		foreach (var recordPath in Records)
		{
			var record = new RecordAccessor<BaseRecordModel>(recordPath);
			if (record == null) { continue; }
			yield return record;
		}
	}


	// Set's and overides this folder's vault model
	public void SetModel(T model)
	{
		VaultModel = model;
		var data = JsonSerializer.SerializeToUtf8Bytes(VaultModel, jsonSerializerOptions);
		File.WriteAllBytes(VaultFile, data);
	}

	public VaultAccessor<TModel>? ChangeModel<TModel>()
		where TModel : BaseVaultModel, new()
	{
		var newVault = Open<TModel>(DirPath);
		newVault?.SetModel(new TModel());
		return newVault;
	}

	public RecordAccessor<TRecordAccessorModel> StoreRecord<TRecordAccessorModel>(TRecordAccessorModel recordAccessorModel)
		where TRecordAccessorModel : BaseRecordModel
	{
		return RecordAccessor<TRecordAccessorModel>.CreateRecord(DirPath, recordAccessorModel);
	}
}

