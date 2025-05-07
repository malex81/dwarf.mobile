using System.ComponentModel;

namespace Dwarf.Minstrel.ViewHelpers;

[ContentProperty("Mode")]
public partial class AnimationBehavior : Behavior<View>
{
	//public static readonly BindableProperty ModeProperty =
	//   BindableProperty.CreateAttached("Mode", typeof(IAnimationMode), typeof(AnimationBehavior), default(IAnimationMode), propertyChanged: OnModeChanged);

	//public static IAnimationMode GetMode(AnimationBehavior bh) => (IAnimationMode)bh.GetValue(ModeProperty);
	//public static void SetMode(AnimationBehavior bh, IAnimationMode value) => bh.SetValue(ModeProperty, value);
	public static readonly BindableProperty ModeProperty =
		BindableProperty.Create(nameof(Mode), typeof(IAnimationMode), typeof(AnimationBehavior), propertyChanged: OnModeChanged);
	public IAnimationMode? Mode
	{
		get => (IAnimationMode)GetValue(ModeProperty);
		set => SetValue(ModeProperty, value);
	}
	static void OnModeChanged(BindableObject bh, object oldValue, object newValue)
	{
		if (bh is not AnimationBehavior ab) return;
		ab.UpdateAnimation();
	}

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