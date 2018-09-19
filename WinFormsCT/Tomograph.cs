using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WinFormsCT.Model;

namespace WinFormsCT
{
	public class Tomograph
	{
		public List<Slice> Scan(Form target)
		{
			var slices = CaptureSlices(target);
			Develop(slices);

			return slices;
		}

		private List<Slice> CaptureSlices(Form form)
		{
			var slices = new List<Slice>();
			var context = new CapturingContext(form);

			CaptureSlices(context, form, slices, layer: 0);

			return slices;
		}

		private void CaptureSlices(CapturingContext context, Control current, List<Slice> slices, int layer)
		{
			slices.Add(CaptureSlice(context, current, layer));

			foreach (Control control in current.Controls)
				CaptureSlices(context, control, slices, layer + 1);
		}

		private Slice CaptureSlice(CapturingContext context, Control control, int layer)
		{
			var location = control is Form ? Point.Empty : GetRelativeLocation(context, control);

			return new Slice()
			{
				Control = control,
				OriginallyVisible = control.Visible,
				Location = location,
				Layer = layer
			};
		}

		private void Develop(List<Slice> slices) => slices.ForEach(Develop);

		private void Develop(Slice slice)
		{
			try
			{
				slice.Control.Visible = true;

				foreach (Control child in slice.Control.Controls)
					child.Visible = false;

				var bmp = new Bitmap(slice.Control.Width, slice.Control.Height);
				slice.Control.DrawToBitmap(bmp, new Rectangle(Point.Empty, slice.Control.Size));
				slice.Image = bmp;
			}
			finally
			{
				slice.Control.Visible = slice.OriginallyVisible;
			}
		}

		private static Point GetRelativeLocation(CapturingContext context, Control control)
		{
			var controlLocationOnScreen = control.PointToScreen(Point.Empty);

			var result = new Point(
				controlLocationOnScreen.X - context.FormClientAreaLocationOnScreen.X,
				controlLocationOnScreen.Y - context.FormClientAreaLocationOnScreen.Y);

			System.Diagnostics.Debug.WriteLine($"{control.Name}: Relative location is {result}");

			return result;
		}
	}
}