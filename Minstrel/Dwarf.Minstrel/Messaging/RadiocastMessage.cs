namespace Dwarf.Minstrel.Messaging;

public enum RadiocastCommandKind { Refresh }

public record RadiocastMessage(RadiocastCommandKind Command)
{
	public readonly static RadiocastMessage Refresh = new(RadiocastCommandKind.Refresh);
}