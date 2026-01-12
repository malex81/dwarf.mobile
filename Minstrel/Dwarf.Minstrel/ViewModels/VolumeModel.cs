using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dwarf.Minstrel.Base;
using Dwarf.Minstrel.MediaEngine;
using Dwarf.Toolkit.Basic.AsyncHelpers;
using Dwarf.Toolkit.Basic.LinqBinder;
using Dwarf.Minstrel.ViewHelpers;

namespace Dwarf.Minstrel.ViewModels;

public sealed partial class VolumeModel : ObservableObject
{
#if WINDOWS
	static double DefaultVolume => Preferences.Get(PreferenceNames.Volume, 0.5);
#else
	static double DefaultVolume => 1;
#endif

	private readonly Func<Action, Task> volumeCallback = ActionFlow.Debounce(TimeSpan.FromSeconds(0.5));

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(VolumeButtonIcon))]
	public partial double Volume { get; set; } = DefaultVolume;

	public FontIconDescriptor VolumeButtonIcon => new(Volume > 0.6 ? FASolidGlyphs.VolumnHigh : Volume == 0 ? FASolidGlyphs.VolumnXMark : FASolidGlyphs.VolumnLow);

	public VolumeModel(MediaBox mediaBox)
	{
		var mediaBinding = mediaBox.CreateBindingContent();
		mediaBinding.Bind(b => b.Volume).To(this, s => s.Volume).Direction(BindingWay.TwoWay).ReadTarget();
	}

#if WINDOWS
	partial void OnVolumeChanged(double value)
	{
		volumeCallback(() =>
		{
			if (Volume > 0)
				Preferences.Set(PreferenceNames.Volume, Volume);
		});
	}
#endif

	[RelayCommand]
	void ToggleMute()
	{
		Volume = Volume == 0 ? DefaultVolume : 0;
	}
}