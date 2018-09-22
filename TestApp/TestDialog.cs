using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
	public partial class TestDialog : Form
	{
		public TestDialog()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void picSlice_Click(object sender, EventArgs e)
		{
			this.ScanAndAnalyze();
		}
	}
}
