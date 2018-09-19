using System;
using System.Windows.Forms;
using WinFormsCT;

namespace TestApp
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();
		}

		private void biCT_Click(object sender, EventArgs e)
		{
			var tomograph = new Tomograph();
			var visualizer = new SlicesVisualizerForm();

			var slices = tomograph.Scan(this);
			visualizer.Show(slices);
		}
	}
}
