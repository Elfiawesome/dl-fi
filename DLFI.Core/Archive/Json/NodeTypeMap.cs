using System.Reflection;
using DLFI.Core.Archive.Model;

namespace DLFI.Core.Archive.Json;

public class NodeTypeMap
{
	public Dictionary<string, Type> IdToType = [];


	public void RegisterAssembly(Assembly assembly)
	{
		var nodeTypes = assembly.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Node)));

		foreach (var type in nodeTypes)
		{
			var attr = type.GetCustomAttribute<NodeItemAttribute>();
			if (attr != null)
			{
				if (IdToType.TryGetValue(attr.Id, out Type? value))
				{
					throw new InvalidOperationException(
						$"Duplicate RawRecordItem ID '{attr.Id}' found on types " +
						$"{type.FullName} and {value.FullName}");
				}
				IdToType[attr.Id] = type;
			}
		}
	}
}