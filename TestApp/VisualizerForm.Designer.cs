using WinFormsCT;

namespace TestApp
{
	partial class VisualizerForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualizerForm));
			this.layerVisualizer = new LayerVisualizer();
			this.SuspendLayout();
			// 
			// layerVisualizer
			// 
			this.layerVisualizer.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
			this.layerVisualizer.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
			this.layerVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layerVisualizer.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.layerVisualizer.Layers = ((System.Collections.Generic.List<System.Drawing.Image>)(resources.GetObject("layerVisualizer.Layers")));
			this.layerVisualizer.Location = new System.Drawing.Point(0, 0);
			this.layerVisualizer.Name = "layerVisualizer";
			this.layerVisualizer.Offset = new System.Drawing.Point(0, 0);
			this.layerVisualizer.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
			this.layerVisualizer.Size = new System.Drawing.Size(1241, 657);
			this.layerVisualizer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			this.layerVisualizer.TabIndex = 0;
			this.layerVisualizer.Zoom = 0;
			// 
			// VisualizerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1241, 657);
			this.Controls.Add(this.layerVisualizer);
			this.Name = "VisualizerForm";
			this.Text = "VisualizerForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.ResumeLayout(false);

		}

		#endregion

		private LayerVisualizer layerVisualizer;
	}
}