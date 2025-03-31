using Dwarf.Framework.SystemExtension;

namespace Dwarf.Minstrel.ViewHelpers;

public interface IAnimationMode
{
	IDisposable BeginAnimation(View view);
}

public class PulseAnimation : IAnimationMode
{
	public IDisposable BeginAnimation(View view)
	{
		var running = true;
		var animation = new Animation(v => view.Scale = v, 0.9, 1.3);
		animation.Commit(view, "PulseAnimation", 25, 500, Easing.SpringIn, (v, c) => view.Scale = 1, () => running);
		return DisposableHelper.FromAction(() => { running = false; });
	}
}

public class RotateAnimation : IAnimationMode
{
	public IDisposable BeginAnimation(View view)
	{
		var running = true;
		var animation = new Animation(v => view.Rotation = v, 0, 360);
		animation.Commit(view, "RotateAnimation", 25, 2000, Easing.Linear, (v, c) => view.Rotation = 0, () => running);
		return DisposableHelper.FromAction(() => { running = false; });
	}
}