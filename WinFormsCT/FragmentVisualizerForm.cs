using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsCT.Model;

namespace WinFormsCT
{
	public class FragmentVisualizerForm
	{
		public void Show(IEnumerable<Fragment> fragments)
		{
			var formFragment = fragments.FirstOrDefault()?.Control as Form;
			if (formFragment == null)
				throw new ArgumentNullException("First fragment has to be a System.Windows.Form!");

			var displayForm = new Form() { BackColor = Color.FromArgb(30, 30, 30) };
			displayForm.Size = new Size(formFragment.Width + 150, formFragment.Height + 80);
			displayForm.SuspendLayout();

			foreach (var fragment in fragments)
			{
				var pic = new PictureBox();

				pic.Image = fragment.Image;
				pic.Size = fragment.Image.Size;
				pic.Location = fragment.Location;
				pic.Tag = fragment.Layer;

				displayForm.Controls.Add(pic);
				pic.Show();
				pic.BringToFront();
			}

			var track = new TrackBar();
			track.Minimum = fragments.Min(s => s.Layer);
			track.Maximum = fragments.Max(s => s.Layer);
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