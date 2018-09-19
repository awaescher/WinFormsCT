using System.Drawing;
using System.Windows.Forms;

namespace WinFormsCT.Model
{
	public class Slice
	{
		public Control Control { get; internal set; }

		public Point Location { get; internal set; }

		public int Layer { get; internal set; }

		public bool OriginallyVisible { get; internal set; }

		public Image Image { get; internal set; }
	}
}
