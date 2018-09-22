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
			var layers = tomograph.RenderLayers(slices);

			for (int i = 0; i < slices.Count; i++)
				slices[i].Image.Save($@"C:\Temp\WinFormsCT\Slice{i}.png");

			for (int i = 0; i < layers.Count; i++)
				layers[i].Save($@"C:\Temp\WinFormsCT\Layer{i}.png");

			visualizer.Show(slices);

			var f = new VisualizerForm();
			f.Show(layers);
		}
	}
}