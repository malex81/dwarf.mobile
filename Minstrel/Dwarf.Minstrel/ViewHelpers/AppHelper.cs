namespace Dwarf.Minstrel.ViewHelpers;

public static class AppHelper
{
	//Page SomePage => Application.Current?.Windows is [{ Page: Page activePage }, ..]
	//				? activePage
	//				: throw new InvalidOperationException("Active window or page not initialized");

	public static Page? MainPage => App.MainWindow?.Page;

	public static IEnumerable<T> FindVisualTreeElements<T>() where T : class, IVisualTreeElement
	{
		var app = Application.Current;
		if (app != null)
			foreach (var wnd in app.Windows)
			{
				var page = wnd.Page;
				if (page != null)
				{
					var res = page.GetVisualTreeDescendants().OfType<T>();
					foreach (var el in res)
						yield return el;
				}
			}
	}

	public static T? FindFirstVisualTreeElement<T>() where T : class, IVisualTreeElement => FindVisualTreeElements<T>().FirstOrDefault();
}