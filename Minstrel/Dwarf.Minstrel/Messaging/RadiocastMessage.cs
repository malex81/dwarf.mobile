﻿namespace Dwarf.Minstrel.Messaging;

public enum RadiocastCommandKind { Refresh, ShallowRefresh }

public record RadiocastMessage(RadiocastCommandKind Command)
{
	public readonly static RadiocastMessage Refresh = new(RadiocastCommandKind.Refresh);
	public readonly static RadiocastMessage ShallowRefresh = new(RadiocastCommandKind.ShallowRefresh);
}