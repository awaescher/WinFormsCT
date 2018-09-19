using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsCT.Model;

namespace WinFormsCT
{
	public class SlicesVisualizerForm
	{
		public void Show(IEnumerable<Slice> slices)
		{
			var slicedForm = slices.FirstOrDefault()?.Control as Form;
			if (slicedForm == null)
				throw new ArgumentNullException("First slice has to be a System.Windows.Form!");

			var displayForm = new Form() { BackColor = Color.FromArgb(30, 30, 30) };
			displayForm.Size = new Size(slicedForm.Width + 150, slicedForm.Height + 80);
			displayForm.SuspendLayout();

			foreach (var slice in slices)
			{
				var pic = new PictureBox();

				pic.Image = slice.Image;
				pic.Size = slice.Image.Size;
				pic.Location = slice.Location;
				pic.Tag = slice.Layer;

				displayForm.Controls.Add(pic);
				pic.Show();
				pic.BringToFront();
			}

			var track = new TrackBar();
			track.Minimum = slices.Min(s => s.Layer);
			track.Maximum = slices.Max(s => s.Layer);
			track.Orientation = Orientation.Vertical;
			track.Dock = DockStyle.Right;
			track.Value = track.Maximum;
			track.ValueChanged += Track_ValueChanged;
			displayForm.Controls.Add(track);
			track.Show();

			displayForm.ResumeLayout();

			displayForm.Show();
		}

		private void Track_ValueChanged(object sender, EventArgs e)
		{
			var track = sender as TrackBar;

			foreach (Control control in track.Parent.Controls)
			{
				if (int.TryParse(control.Tag?.ToString() ?? "", out int layer))
					control.Visible = layer <= track.Value;
			}
		}
	}
}