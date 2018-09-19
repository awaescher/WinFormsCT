using System.Drawing;
using System.Windows.Forms;

namespace WinFormsCT.Model
{
	internal class CapturingContext
	{
		internal CapturingContext(Form form)
		{
			Form = form;

			var screenRectangle = Form.RectangleToScreen(Form.ClientRectangle);

			int titleBoder = Form.Top - screenRectangle.Top;
			int leftBorder = Form.Left - screenRectangle.Left;

			var formLocationOnScreen = form.PointToScreen(Point.Empty);
			FormClientAreaLocationOnScreen = new Point(formLocationOnScreen.X + leftBorder, formLocationOnScreen.Y + titleBoder);
		}

		public Form Form { get; private set; }

		public Point FormClientAreaLocationOnScreen { get; private set; }
	}
}