using Dwarf.Toolkit.Maui;
using System.ComponentModel;

namespace Dwarf.Minstrel.ViewHelpers;

[ContentProperty("Mode")]
public partial class AnimationBehavior : Behavior<View>
{
	[BindableProperty]
	public partial IAnimationMode? Mode { get; set; }
	partial void OnModeChanged(IAnimationMode? value) => UpdateAnimation();

	private View? view;
	private IDisposable? currentRun;

	protected override void OnAttachedTo(View view)
	{
		base.OnAttachedTo(view);
		view.PropertyChanged += View_PropertyChanged;
		this.view = view;
		UpdateAnimation();
	}

	protected override void OnDetachingFrom(View view)
	{
		base.OnDetachingFrom(view);
		view.PropertyChanged -= View_PropertyChanged;
		this.view = null;
		UpdateAnimation();
	}

	private void View_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(view.IsVisible) || e.PropertyName == nameof(view.Width))
			UpdateAnimation();
	}

	void UpdateAnimation()
	{
		currentRun?.Dispose();
		currentRun = null;
		if (view != null && view.IsVisible && view.Width > 0)
		{
			var mode = Mode;
			if (mode != null)
				currentRun = mode.BeginAnimation(view);
		}
	}
}