﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dwarf.Minstrel.Data.Tables;
using Dwarf.Minstrel.Helpers;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadioItem(RadioSource radioSource) : ObservableObject
{
	static readonly byte[] DefaultIcon = ResourceHelper.LoadResource("radio_def_r.png").GetAwaiter().GetResult();

	private readonly RadioSource radioSource = radioSource;

	public int Id => radioSource.Id;
	public string Title => radioSource.Title ?? $"Неизветсное #{Id}";
	public byte[]? Icon => radioSource.Logo ?? DefaultIcon;

	[ObservableProperty]
	public partial bool InFavorites { get; set; }
	[ObservableProperty]
	public partial bool IsPlaying { get; set; }

	[RelayCommand]
	void ToggleFavorites()
	{
		InFavorites = !InFavorites;
	}

	[RelayCommand]
	void TogglePlaying()
	{
		IsPlaying = !IsPlaying;
	}
}