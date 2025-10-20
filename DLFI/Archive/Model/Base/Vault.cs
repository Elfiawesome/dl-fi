using System.Text.Json.Serialization;

namespace DLFI.Archive.Model.Base;

/// <summary>
/// Represents a container (like a folder) in the archive. It can hold other Vaults and Records.
/// </summary>
public class Vault : RawRecord
{
}