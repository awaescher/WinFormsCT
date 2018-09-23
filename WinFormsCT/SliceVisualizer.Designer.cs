namespace WinFormsCT
{
	partial class SliceVisualizer
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.trackZoom = new System.Windows.Forms.TrackBar();
			this.checkAutoDepth = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.trackDepth = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonReset = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.trackZoom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackDepth)).BeginInit();
			this.SuspendLayout();
			// 
			// trackZoom
			// 
			this.trackZoom.Location = new System.Drawing.Point(84, 16);
			this.trackZoom.Name = "trackZoom";
			this.trackZoom.Size = new System.Drawing.Size(484, 56);
			this.trackZoom.TabIndex = 0;
			this.trackZoom.ValueChanged += new System.EventHandler(this.trackZoom_ValueChanged);
			// 
			// checkAutoDepth
			// 
			this.checkAutoDepth.AutoSize = true;
			this.checkAutoDepth.Location = new System.Drawing.Point(95, 114);
			this.checkAutoDepth.Name = "checkAutoDepth";
			this.checkAutoDepth.Size = new System.Drawing.Size(101, 21);
			this.checkAutoDepth.TabIndex = 2;
			this.checkAutoDepth.Text = "Auto Depth";
			this.checkAutoDepth.UseVisualStyleBackColor = true;
			this.checkAutoDepth.CheckedChanged += new System.EventHandler(this.checkAutoDepth_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "Zoom";
			// 
			// trackDepth
			// 
			this.trackDepth.Location = new System.Drawing.Point(84, 66);
			this.trackDepth.Name = "trackDepth";
			this.trackDepth.Size = new System.Drawing.Size(484, 56);
			this.trackDepth.TabIndex = 0;
			this.trackDepth.ValueChanged += new System.EventHandler(this.trackDepth_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 70);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(46, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Depth";
			// 
			// buttonReset
			// 
			this.buttonReset.Location = new System.Drawing.Point(95, 151);
			this.buttonReset.Name = "buttonReset";
			this.buttonReset.Size = new System.Drawing.Size(89, 26);
			this.buttonReset.TabIndex = 5;
			this.buttonReset.Text = "Reset";
			this.buttonReset.UseVisualStyleBackColor = true;
			this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
			// 
			// SliceVisualizer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonReset);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkAutoDepth);
			this.Controls.Add(this.trackDepth);
			this.Controls.Add(this.trackZoom);
			this.Name = "SliceVisualizer";
			this.Size = new System.Drawing.Size(1779, 604);
			((System.ComponentModel.ISupportInitialize)(this.trackZoom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackDepth)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar trackZoom;
		private System.Windows.Forms.CheckBox checkAutoDepth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trackDepth;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonReset;
	}
}
