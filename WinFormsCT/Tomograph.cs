using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsCT.Model;

namespace WinFormsCT
{
	public class Tomograph
	{
		public Tomograph()
			: this(new DotNetControlsSliceSelector())
		{
		}

		public Tomograph(ISliceSelector sliceSelector)
		{
			SliceSelector = sliceSelector ?? throw new ArgumentNullException(nameof(sliceSelector));
		}

		public List<Slice> Scan(Form target)
		{
			var slices = CaptureSlices(target);

			Develop(slices);

			return slices;
		}

		public List<Image> RenderLayers(IEnumerable<Slice> slices)
		{
			var slicedForm = slices.FirstOrDefault()?.Control as Form;
			if (slicedForm == null)
				throw new ArgumentNullException("First slice has to be a System.Windows.Form!");

			var layers = new List<Image>();
			var layerSize = slicedForm.Size;

			var slicesInLayers = slices
				.OrderBy(s => s.Layer)
				.GroupBy(s => s.Layer);

			foreach (var slicesInLayer in slicesInLayers)
			{
				var bmp = new Bitmap(layerSize.Width, layerSize.Height);
				using (var gfx = Graphics.FromImage(bmp))
				{
					var orderedSlices = slicesInLayer.OrderBy(s => s.Stamp);
					foreach (var slice in orderedSlices)
						gfx.DrawImage(slice.Image, slice.Location);
				}

				layers.Add(bmp);
			}

			return layers;

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
			var slice = CaptureSlice(context, current, layer);

			// slice should not be shown (could be a hidden tabpage) -> skip it with all of its children
			if (slice == null)
				return;

			slices.Add(slice);

			foreach (Control control in current.Controls)
				CaptureSlices(context, control, slices, layer + 1);
		}

		private Slice CaptureSlice(CapturingContext context, Control control, int layer)
		{
			if (!SliceSelector.ShouldShowSlice(control))
				return null;

			var location = control is Form ? Point.Empty : GetRelativeLocation(context, control);

			return new Slice()
			{
				Control = control,
				OriginallyVisible = control.Visible,
				Location = location,
				Layer = layer,
				Stamp = DateTime.UtcNow.Ticks
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

				if (!slice.OriginallyVisible)
				{
					using (var gfx = Graphics.FromImage(bmp))
					{
						gfx.DrawRectangle(Pens.Red, new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));
					}
				}

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

		public ISliceSelector SliceSelector { get; }
	}
}