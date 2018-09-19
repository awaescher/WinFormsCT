using System.Windows.Forms;
using WinFormsCT;

namespace TestApp
{
	public static class TomographyExtensions
	{
		public static void ScanAndAnalyze(this Form form)
		{
			var tomograph = new Tomograph();
			var visualizer = new SlicesVisualizerForm();

			var slices = tomograph.Scan(form);
			visualizer.Show(slices);
		}
	}
}