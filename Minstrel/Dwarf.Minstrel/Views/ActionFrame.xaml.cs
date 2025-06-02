using CommunityToolkit.Mvvm.Input;
using Dwarf.Toolkit.Base.SystemExtension;
using Maui.BindableProperty.Generator.Core;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace Dwarf.Minstrel.Views;

public partial class ActionFrame : ContentView
{
#pragma warning disable CS0169
	//https://github.com/rrmanzano/maui-bindableproperty-generator
	[AutoBindable]
	private readonly string? _title;
	[AutoBindable]
	private readonly string? _description;
	[AutoBindable]
	private readonly ImageSource? _icon;
	[AutoBindable]
	private readonly ImageSource? _secondaryIcon;
	[AutoBindable]
	private readonly ICommand? _command;
	[AutoBindable]
	private readonly ICommand? _commandParameter;
#pragma warning restore CS0169

	public ActionFrame()
	{
		InitializeComponent();
		Icons = new IconsList(this);
	}

	public IList<ImageSource?> Icons { get; }
	public bool IsExecuted => Command is IAsyncRelayCommand asyncCmd && asyncCmd.IsRunning;

	partial void OnIconChanged(ImageSource? value)
	{
		if (value is FontImageSource fis)
			fis.Size = 64;
	}

	// https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.visualelement.unloaded?view=net-maui-8.0
	private readonly DisposableList commandDisp = [];
	partial void OnCommandChanged(ICommand? value)
	{
		if (value is IAsyncRelayCommand asyncCmd)
		{
			asyncCmd.PropertyChanged += AsyncCmd_PropertyChanged;
			commandDisp.AddAction(() =>
			{
				asyncCmd.PropertyChanged -= AsyncCmd_PropertyChanged;
			});
		}
	}

	private void AsyncCmd_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(IAsyncRelayCommand.IsRunning))
			OnPropertyChanged(nameof(IsExecuted));
	}

	private async void OnFrameTapped(object sender, TappedEventArgs e)
	{
		if (IsExecuted)
			return;
		if (Command is IAsyncRelayCommand asyncCmd)
		{
			await asyncCmd.ExecuteAsync(CommandParameter);
		}
		else Command?.Execute(CommandParameter);
	}

	#region IconsList
	partial class IconsList : IList<ImageSource?>
	{
		private readonly ActionFrame parent;

		public IconsList(ActionFrame parent)
		{
			this.parent = parent;
		}

		public ImageSource? this[int index]
		{
			get => index switch
			{
				0 => parent.Icon,
				1 => parent.SecondaryIcon,
				_ => throw new IndexOutOfRangeException()
			};
			set
			{
				if (index == 0) parent.Icon = value;
				else if (index == 1) parent.SecondaryIcon = value;
				else throw new IndexOutOfRangeException();
			}
		}
		public int Count => 2;
		public bool IsReadOnly => false;
		public void Add(ImageSource? item)
		{
			if (item == null) return;
			if (parent.Icon == null) parent.Icon = item;
			else parent.SecondaryIcon ??= item;
		}
		public void Clear()
		{
			parent.Icon = null;
			parent.SecondaryIcon = null;
		}
		public bool Contains(ImageSource? item) => parent.Icon == item || parent.SecondaryIcon == item;
		public void CopyTo(ImageSource?[] array, int arrayIndex)
		{
			array[arrayIndex] = parent.Icon;
			array[arrayIndex + 1] = parent.SecondaryIcon;
		}
		public IEnumerator<ImageSource?> GetEnumerator()
		{
			yield return parent.Icon;
			yield return parent.SecondaryIcon;
		}
		public int IndexOf(ImageSource? item) => parent.Icon == item ? 0 : parent.SecondaryIcon == item ? 1 : -1;
		public void Insert(int index, ImageSource? item) => this[index] = item;
		public bool Remove(ImageSource? item)
		{
			if (item == null) return false;
			if (item == parent.Icon)
			{
				parent.Icon = null;
				return true;
			}
			if (item == parent.SecondaryIcon)
			{
				parent.SecondaryIcon = null;
				return true;
			}
			return false;
		}
		public void RemoveAt(int index)
		{
			if (index == 0) parent.Icon = null;
			else if (index == 1) parent.SecondaryIcon = null;
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
	#endregion
}