using DLFI.Archive.Model.Base;

namespace DLFI.Archive.Model.Common;

/// <summary>
/// A more specific record for a generic manga/work page.
/// </summary>
public class MangaPageRecord : Record
{
	public int PageNumber { get; set; }
}