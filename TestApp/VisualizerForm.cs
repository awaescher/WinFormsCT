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
	public partial class VisualizerForm : Form
	{
		public VisualizerForm()
		{
			InitializeComponent();
		}

		public void Show(List<Image> slices)
		{
			sliceVisualizer.Slices = slices;
			Show();
		}
	}
}
