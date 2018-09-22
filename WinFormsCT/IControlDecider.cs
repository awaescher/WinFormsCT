using System.Windows.Forms;

namespace WinFormsCT
{
	public interface ISliceSelector
	{
		bool ShouldShowSlice(Control control);
	}
}