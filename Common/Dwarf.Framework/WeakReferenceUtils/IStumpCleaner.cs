namespace Dwarf.Framework.WeakReferenceUtils;

public interface IStumpCleaner
{
	void Start( Action clean );
	void Stop();

	bool IsStarted { get; }
}
