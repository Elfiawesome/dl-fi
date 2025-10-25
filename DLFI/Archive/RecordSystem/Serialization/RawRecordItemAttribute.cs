namespace DLFI.Archive.RecordSystem.Serialization;

[AttributeUsage(AttributeTargets.Class)]
public class RawRecordItemAttribute(string id) : Attribute
{
	public string Id { get; set; } = id;
}