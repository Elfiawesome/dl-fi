using System.Text.Json;
using DLFI.Record.Model.Base;

namespace DLFI.Record.Accessor;

public class RecordAccessor<T> : FileItemAccessor
	where T : BaseRecordModel
{
	public bool IsValid => true;
	public T? RecordModel;

	public RecordAccessor(string path) : base(path)
	{
		if (!Path.Exists(DirPath)) { return; }
		RecordModel = JsonSerializer.Deserialize<T>(File.ReadAllBytes(DirPath));
	}

	public static RecordAccessor<TModel> CreateRecord<TModel>(string parentPath, TModel model)
		where TModel : BaseRecordModel
	{
		var record = new RecordAccessor<TModel>(Path.Join(parentPath, Path.ChangeExtension(model.Name, "json")));
		record.SaveModel();
		return record;
	}


	private void SaveModel()
	{
		if (RecordModel == null) { return; }
		var data = JsonSerializer.SerializeToUtf8Bytes(RecordModel, jsonSerializerOptions);
		File.WriteAllBytes(DirPath, data);
	}
}