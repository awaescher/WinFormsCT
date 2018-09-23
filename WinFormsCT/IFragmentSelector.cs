using System.Windows.Forms;

namespace WinFormsCT
{
	public interface IFragmentSelector
	{
		bool ShouldShowFragment(Control control);
	}
}