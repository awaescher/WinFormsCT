using System.Windows.Forms;
using WinFormsCT;

namespace TestApp
{
	public static class TomographyExtensions
	{
		public static void ScanAndAnalyze(this Form form)
		{
			var tomograph = new Tomograph();
			var visualizer = new FragmentVisualizerForm();

			var fragments = tomograph.Scan(form);
			var layers = tomograph.RenderLayers(fragments);

			for (int i = 0; i < fragments.Count; i++)
				fragments[i].Image.Save($@"C:\Temp\WinFormsCT\Fragment{i}.png");

			for (int i = 0; i < layers.Count; i++)
				layers[i].Save($@"C:\Temp\WinFormsCT\Layer{i}.png");

			visualizer.Show(fragments);

			var f = new VisualizerForm();
			f.Show(layers);
		}
	}
}