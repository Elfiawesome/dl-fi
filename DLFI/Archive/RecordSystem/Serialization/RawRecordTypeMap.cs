using System.Reflection;
using DLFI.Archive.RecordSystem.Model;

namespace DLFI.Archive.RecordSystem.Serialization;

public static class RawRecordTypeMap
{
	public static IReadOnlyDictionary<string, Type> IdToType { get; }
	public static IReadOnlyDictionary<Type, string> TypeToId { get; }

	static RawRecordTypeMap()
	{
		var idToType = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

		var recordTypes = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(RawRecord)));

		foreach (var type in recordTypes)
		{
			var attr = type.GetCustomAttribute<RawRecordItemAttribute>();
			if (attr != null)
			{
				if (idToType.ContainsKey(attr.Id))
				{
					// Handle error: two classes cannot have the same ID
					throw new InvalidOperationException(
						$"Duplicate RawRecordItem ID '{attr.Id}' found on types " +
						$"{type.FullName} and {idToType[attr.Id].FullName}");
				}
				idToType[attr.Id] = type;
			}
		}

		IdToType = idToType;
		// Create the reverse mapping for efficient lookup during serialization
		TypeToId = IdToType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
	}
}