namespace DLFI.Records.Reader;

public abstract class BaseReader
{
	public string Id; // essentially its filename
	public BaseReader? ParentReader;
	public string FullId => GetFullId();
	protected BaseReader() { Id = ""; }

	protected BaseReader(BaseReader parentReader, string id) { Id = id; ParentReader = parentReader; }

	protected string GetFullId()
	{
		if (ParentReader == null) { return "~"; }
		return (ParentReader?.FullId ?? "") + "/" + Id;
	}
}