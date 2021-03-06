﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using WinFormsCT.Model;

namespace WinFormsCT
{
	public class Tomograph
	{
		public Tomograph()
			: this(new DotNetControlsFragmentSelector())
		{
		}

		public Tomograph(IFragmentSelector fragmentSelector)
		{
			FragmentSelector = fragmentSelector ?? throw new ArgumentNullException(nameof(fragmentSelector));
		}

		public List<Fragment> Scan(Form target)
		{
			var fragments = CaptureFragments(target);

			Develop(fragments);

			return fragments;
		}

		public List<Image> RenderSlices(IEnumerable<Fragment> fragments)
		{
			var formFragment = fragments.FirstOrDefault()?.Control as Form;
			if (formFragment == null)
				throw new ArgumentNullException("First fragment has to be a System.Windows.Form!");

			var slices = new List<Image>();
			var sliceSize = formFragment.Size;

			var fragmentsInSlice = fragments
				.OrderBy(s => s.Slice)
				.GroupBy(s => s.Slice);

			foreach (var fragmentInSlice in fragmentsInSlice)
			{
				var bmp = new Bitmap(sliceSize.Width, sliceSize.Height);
				using (var gfx = Graphics.FromImage(bmp))
				{
					var orderedFragments = fragmentInSlice.OrderBy(s => s.Stamp);
					foreach (var fragment in orderedFragments)
						gfx.DrawImage(fragment.Image, fragment.Location);
				}

				slices.Add(bmp);
			}

			return slices;

		}

		private List<Fragment> CaptureFragments(Form form)
		{
			var fragments = new List<Fragment>();
			var context = new CapturingContext(form);

			CaptureFragments(context, form, fragments, slice: 0);

			return fragments;
		}

		private void CaptureFragments(CapturingContext context, Control current, List<Fragment> fragments, int slice)
		{
			var fragment = CaptureFragment(context, current, slice);

			// fragment should not be shown (could be a hidden tabpage) -> skip it with all of its children
			if (fragment == null)
				return;

			fragments.Add(fragment);

			foreach (Control control in current.Controls)
				CaptureFragments(context, control, fragments, slice + 1);
		}

		private Fragment CaptureFragment(CapturingContext context, Control control, int slice)
		{
			if (!FragmentSelector.ShouldShowFragment(control))
				return null;

			var location = control is Form ? Point.Empty : GetRelativeLocation(context, control);

			return new Fragment()
			{
				Control = control,
				OriginallyVisible = control.Visible,
				Location = location,
				Slice = slice,
				Stamp = DateTime.UtcNow.Ticks
			};
		}

		private void Develop(List<Fragment> fragments) => fragments.ForEach(Develop);

		private void Develop(Fragment fragment)
		{
			var invisibleControlColor = Color.FromArgb(125, Color.Red);

			using (var invisibleControlBrush = new HatchBrush(HatchStyle.Percent20, invisibleControlColor, Color.Transparent))
			using (var invisibleControlPen = new Pen(invisibleControlColor, 1))
			{
				try
				{
					fragment.Control.Visible = true;

					foreach (Control child in fragment.Control.Controls)
						child.Visible = false;

					var bmp = new Bitmap(fragment.Control.Width, fragment.Control.Height);

					fragment.Control.DrawToBitmap(bmp, new Rectangle(Point.Empty, fragment.Control.Size));

					if (!fragment.OriginallyVisible)
					{
						using (var gfx = Graphics.FromImage(bmp))
						{
							gfx.FillRectangle(invisibleControlBrush, new Rectangle(Point.Empty, bmp.Size));
							gfx.DrawRectangle(invisibleControlPen, new Rectangle(
								(int)(invisibleControlPen.Width / 2),
								(int)(invisibleControlPen.Width / 2),
								(int)(bmp.Width - invisibleControlPen.Width),
								(int)(bmp.Height - invisibleControlPen.Width)));
							
						}
					}

					fragment.Image = bmp;
				}
				finally
				{
					fragment.Control.Visible = fragment.OriginallyVisible;
				}
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

		public IFragmentSelector FragmentSelector { get; }
	}
}