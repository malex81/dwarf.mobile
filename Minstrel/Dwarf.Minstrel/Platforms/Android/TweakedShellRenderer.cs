using Android.Graphics;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Tabs;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using AndroidApp = Android.App.Application;

namespace Dwarf.Minstrel.Platforms.Android;

/*
 * https://stackoverflow.com/questions/76376374/net-maui-align-shell-tabs
#if ANDROID
				handlers.AddHandler(typeof(Shell),typeof(Dwarf.Minstrel.Platforms.Android.TweakedShellRenderer));
#endif
*/
public class TweakedShellRenderer : ShellRenderer
{
	protected override IShellTabLayoutAppearanceTracker CreateTabLayoutAppearanceTracker(ShellSection shellSection)
	{
		return new TweakedTabLayoutAppearanceTracker(this);
	}

	class TweakedTabLayoutAppearanceTracker : ShellTabLayoutAppearanceTracker
	{
		public TweakedTabLayoutAppearanceTracker(IShellContext shellContext) : base(shellContext)
		{
		}
		public override void SetAppearance(TabLayout tabLayout, ShellAppearance appearance)
		{
			base.SetAppearance(tabLayout, appearance);
			
			tabLayout.TabMode = TabLayout.ModeFixed;
			tabLayout.TabGravity = TabLayout.GravityStart;
			/*
			var displayWidth = (int)DeviceDisplay.MainDisplayInfo.Width;
			for (int i = 0; i < tabLayout.TabCount; i++)
			{
				TabLayout.Tab tab = tabLayout.GetTabAt(i)!;

				if (tab.CustomView == null)
				{
					TextView textview = new(AndroidApp.Context)
					{
						Text = tabLayout.GetTabAt(i)!.Text,
						TextSize = 20,
						Typeface = Typeface.DefaultBold,
						Gravity = GravityFlags.Center
					};
					textview.SetWidth(displayWidth / 5);
					tab.SetCustomView(textview);
				}
			}
			*/
		}
	}
}