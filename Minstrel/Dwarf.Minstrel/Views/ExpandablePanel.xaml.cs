using Dwarf.Toolkit.Maui;

namespace Dwarf.Minstrel.Views;

public partial class ExpandablePanel : ContentView
{
	record HeightRange(double Collapsed, double Expanded);

	private HeightRange? heightRange;

	[BindableProperty]
	public partial View? StaticHeader { get; set; }
	[BindableProperty]
	public partial View? CollapsableContent { get; set; }
	[BindableProperty]
	public partial bool IsExpanded { get; set; }
	[BindableProperty]
	public partial bool IsIntermediateState { get; set; }
	[BindableProperty(DefaultValue = 1.0)]
	public partial double ContentOpacity { get; set; }

	public ExpandablePanel()
	{
		InitializeComponent();
	}

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);
		if (heightRange == null && height > 0 && CollapsableContent != null)
		{
			SizeRequest measurement = CollapsableContent.Measure(self.Width, double.PositiveInfinity);
			heightRange = IsExpanded ? new(height - measurement.Request.Height, height) : new(height, height + measurement.Request.Height);
		}
	}

	void ResetPanState()
	{
		self.HeightRequest = -1;
		IsIntermediateState = false;
		ContentOpacity = 1;
	}

	private void ExpandButtonTapped(object sender, TappedEventArgs e)
	{
		if (IsIntermediateState)
			ResetPanState();
		else
			IsExpanded = !IsExpanded;
	}

	double startHeight = 100;
	private void ExpandButtonPanUpdated(object sender, PanUpdatedEventArgs e)
	{
		if (CollapsableContent == null || heightRange == null)
			return;
		switch (e.StatusType)
		{
			case GestureStatus.Started:
				startHeight = self.Height;
				IsIntermediateState = true;
				break;
			case GestureStatus.Running:
				self.HeightRequest = Math.Clamp(startHeight + e.TotalY, heightRange.Collapsed - 5, heightRange.Expanded + 40);
				IsExpanded = Math.Abs(self.Height - heightRange.Expanded) < Math.Abs(self.Height - heightRange.Collapsed);
				ContentOpacity = Math.Min(1.0, (2 * self.Height - heightRange.Collapsed - heightRange.Expanded) / (heightRange.Expanded - heightRange.Collapsed));
				break;
			case GestureStatus.Completed:
				ResetPanState();
				break;
		}
	}
}