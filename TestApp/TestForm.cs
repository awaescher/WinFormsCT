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
			this.ScanAndAnalyze();
		}

		private void biDialog_Click(object sender, EventArgs e)
		{
			using (var dialog = new TestDialog())
				dialog.ShowDialog();
		}
	}
}
