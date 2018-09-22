using System.Windows.Forms;

namespace WinFormsCT
{
	public class DotNetControlsSliceSelector : ISliceSelector
	{
		public virtual bool ShouldShowSlice(Control control)
		{
			if (control is TabPage p)
			{
				// only show the selected tabpage, hide all others.
				// otherwise we cannot build a linear stack of layers - we then have to build different UI-paths.
				// one for each tabpage and its children.
				var isSelected = (p.Parent as TabControl)?.SelectedTab?.Equals(p) ?? false;
				return isSelected;
			}

			return true;
		}
	}
}