namespace Dwarf.Minstrel.Data;

public interface IBatchGrabber
{
	string Title { get; }
	LoadResult Load();
}

public record LoadResult();