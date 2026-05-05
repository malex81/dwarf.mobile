namespace Dwarf.Minstrel.Messaging;

public enum RadiocastCommandKind { Refresh, ShallowRefresh, Invalidate }

public record RadiocastMessage(RadiocastCommandKind Command)
{
	public readonly static RadiocastMessage Refresh = new(RadiocastCommandKind.Refresh);
	public readonly static RadiocastMessage ShallowRefresh = new(RadiocastCommandKind.ShallowRefresh);
	public readonly static RadiocastMessage Invalidate = new(RadiocastCommandKind.Invalidate);
}