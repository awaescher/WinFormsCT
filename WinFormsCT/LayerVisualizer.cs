using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WinFormsCT
{
	public partial class LayerVisualizer : UserControl
	{
		private List<Image> _layers;
		private Point _offset;
		private int _zoom;
		private bool _moving;
		private Point _lastCursorPosition;
		private CompositingMode _compositingMode = CompositingMode.SourceOver;
		private CompositingQuality _compositingQuality = CompositingQuality.HighSpeed;
		private PixelOffsetMode _pixelOffsetMode = PixelOffsetMode.Half;
		private SmoothingMode _smoothingMode = SmoothingMode.None;
		private InterpolationMode _interpolationMode = InterpolationMode.NearestNeighbor;
		private bool _twoWayString = true;
		private int _depth;
		private bool _autoDepth;

		public LayerVisualizer()
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			trackZoom.Minimum = 0;
			trackZoom.Maximum = 3000;
			trackZoom.Value = Zoom;

			trackDepth.Minimum = 0;
			trackDepth.Maximum = 3000;
			trackDepth.Value = Depth;

			checkAutoDepth.Checked = AutoDepth;
		}

		private void trackZoom_ValueChanged(object sender, EventArgs e)
		{
			Zoom = trackZoom.Value;
		}

		private void trackDepth_ValueChanged(object sender, EventArgs e)
		{
			Depth = trackDepth.Value;
		}

		private void checkAutoDepth_CheckedChanged(object sender, EventArgs e)
		{
			AutoDepth = checkAutoDepth.Checked;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_moving = true;
			_lastCursorPosition = e.Location;

			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (_moving)
			{
				var deltaX = e.Location.X - _lastCursorPosition.X;
				var deltaY = e.Location.Y - _lastCursorPosition.Y;

				_lastCursorPosition = e.Location;

				Offset = new Point(Offset.X + deltaX, Offset.Y + deltaY);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_moving = false;

			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(BackColor);

			e.Graphics.CompositingMode = _compositingMode;
			e.Graphics.CompositingQuality = _compositingQuality;
			e.Graphics.PixelOffsetMode = _pixelOffsetMode;
			e.Graphics.SmoothingMode = _smoothingMode;
			e.Graphics.InterpolationMode = _interpolationMode;

			base.OnPaint(e);

			if (Layers?.Count < 1)
				return;

			var offsetAlpha = Math.Max(Math.Min((Math.Abs(Offset.X) + Math.Abs(Offset.Y)) / 25, 50), 0);
			var zoomAlpha = Zoom == 0 ? 0 : Math.Min(Zoom / 10, 50);
			var depthAlpha = Depth == 0 ? 0 : Math.Min(Depth / 10, 50);
			var layerAlpha = new[] { offsetAlpha, zoomAlpha, depthAlpha }.Max();

			using (var layerBackBrush = new SolidBrush(Color.FromArgb(layerAlpha, Color.Black)))
			using (var layerBorderPen = new Pen(Color.FromArgb(20, Color.Black)))
			{
				var maxLayerSize = new Size(Layers.Max(l => l.Width), Layers.Max(l => l.Height));
				var originLeft = (Width - maxLayerSize.Width) / 2;
				var originTop = (Height - maxLayerSize.Height) / 2;

				for (int i = 0; i < Layers.Count; i++)
				{
					var offsetAffectFactor = 0.0d;

					if (TwoWaySpring)
						offsetAffectFactor = (((double)i - (double)(Layers.Count / 2)) / (double)Layers.Count * 2);
					else
						offsetAffectFactor = (double)i / (double)Layers.Count;

					var zoomAffectFactor = (double)(i + 1 /* +1 makes the last layer move with the zoom as well */ ) / (double)Layers.Count;
					if (Zoom < 0)
						zoomAffectFactor = Zoom;

					var depthAffectFactor = ((double)Layers.Count - (double)(i + 1)) / (double)Layers.Count;

					var layer = Layers[i];

					var left = originLeft + (int)(offsetAffectFactor * Offset.X);
					var top = originTop + (int)(offsetAffectFactor * Offset.Y);

					var rect = new Rectangle(left, top, layer.Width, layer.Height);

					var infateBy = Zoom * zoomAffectFactor - Depth * depthAffectFactor;
					var ratio = ((double)layer.Width / (double)layer.Height);
					rect.Inflate((int)(infateBy * ratio), (int)(infateBy));

					if (rect.Height > Height || rect.Width > Width)
						continue;

					e.Graphics.FillRectangle(layerBackBrush, rect);

					e.Graphics.DrawImage(layer, rect);

					if (layerAlpha > 0)
						e.Graphics.DrawRectangle(layerBorderPen, rect);

				}
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
		}

		public List<Image> Layers
		{
			get => _layers ?? new List<Image>();
			set
			{
				_layers = value;
				Invalidate();
			}
		}

		public Point Offset
		{
			get => _offset;
			set
			{
				_offset = value;
				Invalidate();
			}
		}

		public int Zoom
		{
			get => _zoom;
			set
			{
				_zoom = value;
				Invalidate();
			}
		}

		public int Depth
		{
			get
			{
				if (AutoDepth)
					return (int)(Math.Sqrt((Math.Pow(Offset.X, 2) + Math.Pow(Offset.Y, 2))) / 3);

				return _depth;
			}
			set
			{
				_depth = value;
				Invalidate();
			}
		}

		public CompositingMode CompositingMode
		{
			get => _compositingMode;
			set
			{
				_compositingMode = value;
				Invalidate();
			}
		}

		public CompositingQuality CompositingQuality
		{
			get => _compositingQuality;
			set
			{
				_compositingQuality = value;
				Invalidate();
			}
		}

		public PixelOffsetMode PixelOffsetMode
		{
			get => _pixelOffsetMode;
			set
			{
				_pixelOffsetMode = value;
				Invalidate();
			}
		}

		public SmoothingMode SmoothingMode
		{
			get => _smoothingMode;
			set
			{
				_smoothingMode = value;
				Invalidate();
			}
		}

		public InterpolationMode InterpolationMode
		{
			get => _interpolationMode;
			set
			{
				_interpolationMode = value;
				Invalidate();
			}
		}

		public bool TwoWaySpring
		{
			get => _twoWayString;
			set
			{
				_twoWayString = value;
				Invalidate();
			}
		}

		public bool AutoDepth
		{
			get => _autoDepth;
			set
			{
				_autoDepth = value;
				Invalidate();
			}
		}

		private void buttonReset_Click(object sender, EventArgs e)
		{
			Reset();
		}

		private void Reset()
		{
			AutoDepth = false;
			Depth = 0;
			Zoom = 0;
			Offset = Point.Empty;
		}
	}
}