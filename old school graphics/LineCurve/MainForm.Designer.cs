namespace LineCurve {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.pnl = new DoubleBufferedPanel();

			this.SuspendLayout();
			// 
			// pnl
			// 
			this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl.Location = new System.Drawing.Point(0, 0);
			this.pnl.Name = "pnl";
			this.pnl.Size = new System.Drawing.Size(642, 406);
			this.pnl.TabIndex = 0;
			this.pnl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pnl_MouseWheel);
			this.pnl.MouseEnter += new System.EventHandler(this.pnl_MouseEnter);
			this.pnl.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_Paint);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(642, 406);
			this.Controls.Add(this.pnl);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private DoubleBufferedPanel pnl;
	}

	public class DoubleBufferedPanel : System.Windows.Forms.Panel {
		public DoubleBufferedPanel() {
			base.DoubleBuffered = true;
		}
	}
}

