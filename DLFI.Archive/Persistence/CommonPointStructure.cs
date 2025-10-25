namespace DLFI.Archive.Persistence;

public class CommonPointStructure
{
	public Guid Definition { get; set; } = Guid.Empty;
	public Guid Author { get; set; } = Guid.Empty;
	public Guid Tag { get; set; } = Guid.Empty;
	public Guid Medium { get; set; } = Guid.Empty;
	public Guid Manga { get; set; } = Guid.Empty;
	public Guid Image { get; set; } = Guid.Empty;
}