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
	public partial class SliceVisualizer : UserControl
	{
		private List<Image> _slices;
		private Point _offset;
		private int _zoom;
		private bool _moving;
		private Point _lastCursorPosition;
		private CompositingMode _compositingMode = CompositingMode.SourceOver;
		private CompositingQuality _compositingQuality = CompositingQuality.HighSpeed;
		private PixelOffsetMode _pixelOffsetMode = PixelOffsetMode.Half;
		private SmoothingMode _smoothingMode = SmoothingMode.None;
		private InterpolationMode _interpolationMode = InterpolationMode.NearestNeighbor;
		private bool _twoWayString;
		private int _depth;
		private bool _autoDepth;

		public SliceVisualizer()
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			_twoWayString = true;
			_autoDepth = true;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.KeyCode == Keys.Escape)
				Reset();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			var factor = (MaxZoom / 25) * (e.Delta < 0 ? -1 : 1);
			Zoom += factor;
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
			const float EXTENUATED_MOVEMENT_FACTOR = 0.6f;

			e.Graphics.Clear(BackColor);

			e.Graphics.CompositingMode = _compositingMode;
			e.Graphics.CompositingQuality = _compositingQuality;
			e.Graphics.PixelOffsetMode = _pixelOffsetMode;
			e.Graphics.SmoothingMode = _smoothingMode;
			e.Graphics.InterpolationMode = _interpolationMode;

			base.OnPaint(e);

			if (Slices?.Count < 1)
				return;

			var offsetAlpha = Math.Max(Math.Min((Math.Abs(Offset.X) + Math.Abs(Offset.Y)) / 25, 50), 0);
			var zoomAlpha = Zoom == 0 ? 0 : Math.Min(Zoom / 10, 50);
			var depthAlpha = Depth == 0 ? 0 : Math.Min(Depth / 10, 50);
			var sliceAlpha = new[] { offsetAlpha, zoomAlpha, depthAlpha }.Max();

			using (var sliceBackBrush = new SolidBrush(Color.FromArgb(sliceAlpha, Color.Black)))
			using (var sliceBorderPen = new Pen(Color.FromArgb(20, Color.Black)))
			{
				var maxSliceSize = new Size(Slices.Max(l => l.Width), Slices.Max(l => l.Height));
				var originLeft = (Width - maxSliceSize.Width) / 2;
				var originTop = (Height - maxSliceSize.Height) / 2;

				for (int i = 0; i < Slices.Count; i++)
				{
					var slice = Slices[i];

					var offsetAffectFactor = 0.0d;

					if (TwoWaySpring)
						offsetAffectFactor = (((double)i - (double)(Slices.Count / 2)) / (double)Slices.Count * 2);
					else
						offsetAffectFactor = (double)i/ (double)Slices.Count;

					var zoomAffectFactor = (double)(i + 1 /* +1 makes the last slice move with the zoom as well */ ) / (double)Slices.Count;
					if (Zoom < 0)
						zoomAffectFactor = Zoom;

					var depthAffectFactor = ((double)Slices.Count - (double)(i + 1)) * EXTENUATED_MOVEMENT_FACTOR / (double)Slices.Count;

					var left = originLeft + (int)(offsetAffectFactor * Offset.X);
					var top = originTop + (int)(offsetAffectFactor * Offset.Y);

					var rect = new Rectangle(left, top, slice.Width, slice.Height);

					var infateBy = (Zoom * zoomAffectFactor) - (Depth * depthAffectFactor);
					var ratio = ((double)slice.Width / (double)slice.Height);
					rect.Inflate((int)(infateBy * ratio), (int)(infateBy));

					if (rect.Height > Height || rect.Width > Width)
						continue;

					e.Graphics.FillRectangle(sliceBackBrush, rect);

					e.Graphics.DrawImage(slice, rect);

					if (sliceAlpha > 0)
						e.Graphics.DrawRectangle(sliceBorderPen, rect);

				}
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
		}

		private void Reset()
		{
			Depth = 0;
			Zoom = 0;
			Offset = Point.Empty;
		}

		public List<Image> Slices
		{
			get => _slices ?? new List<Image>();
			set
			{
				_slices = value;
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
				_zoom = Math.Max(0, Math.Min(MaxZoom, value));
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

		public int MaxDepth { get; set; } = 1500;

		public int MaxZoom { get; set; } = 2000;
	}
}