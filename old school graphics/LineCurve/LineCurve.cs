using System.Drawing;

namespace LineCurve {
	public class LineCurve {
		int steps;
		int size;
		int movmax;
		int roll;

		Color dark;
		Color light;

		PointF[,] lines;
		PointF[,] points;

		public LineCurve() {
			steps = 10;
			size = 300;
			movmax = 10;
			roll = 0;

			lines = new PointF[steps + 1, 2];
			points = new PointF[steps + 1, steps + 1];

			light = Color.FromArgb(255, 255, 255, 255);
			dark = Color.FromArgb(255, 32, 32, 32);

			CalcPoints();
		}

		public int Roll {
			get {
				return roll;
			}
			set {
				if (value < 0) {
					roll = movmax + (value % movmax);
				} else {
					roll = value % movmax;
				}
			}
		}

		private void CalcPoints() {
			float delta = size / steps;

			for (int i = 0; i <= steps; i++) {
				float idelta = i * delta;

				lines[i, 0] = new PointF(20.0F + idelta, 20.0F + idelta);
				lines[i, 1] = new PointF(20.0F + size + idelta, 20.0F + size - idelta);
			}

			for (int i = 0; i < steps; i++) {
				for (int j = i + 1; j <= steps; j++) {
					points[i, j] = LineCross(lines[i, 0], lines[i, 1], lines[j, 0], lines[j, 1]);
				}

				if (i > 0) {
					var pa = points[i - 1, i];
					var pb = points[i, i + 1];
					points[i, i] = new PointF((pa.X + pb.X) / 2.0F, (pa.Y + pb.Y) / 2.0F);
				}
			}
		}

		public void Draw(Graphics g) {
			PointF[] poly = new PointF[4];

			for (int i = 0; i < (steps - 1); i++) {
				for (int j = i + 1; j < steps; j++) {
					poly[0] = points[i, j];
					poly[1] = points[i, j + 1];
					poly[2] = points[i + 1, j + 1];
					poly[3] = points[i + 1, j];

					int flag = (i + j) & 1;

					RollBox(g, roll, movmax, flag, poly);
				}
			}
		}

		private PointF LineScale(PointF p1, PointF p2, float s) {
			float dx = p1.X - p2.X;
			float dy = p1.Y - p2.Y;
			float sx = s * dx;
			float sy = s * dy;

			return new PointF(p2.X + sx, p2.Y + sy);
		}

		private PointF LineCross(PointF l1a, PointF l1b, PointF l2a, PointF l2b) {

			float d = ((l2b.Y - l2a.Y) * (l1b.X - l1a.X)) -
				((l2b.X - l2a.X) * (l1b.Y - l1a.Y));

			float na = ((l2b.X - l2a.X) * (l1a.Y - l2a.Y)) -
				((l2b.Y - l2a.Y) * (l1a.X - l2a.X));

			float x = 0.0F;
			float y = 0.0F;

			if (System.Math.Abs(d) >= 1e-8) {
				x = l1a.X + ((na * (l1b.X - l1a.X)) / d);
				y = l1a.Y + ((na * (l1b.Y - l1a.Y)) / d);
			}

			return new PointF(x, y);
		}


		private void RollBox(Graphics g, int mov, int movmax, int flag, PointF[] p) {
			float s = (float)mov / (float)movmax;

			var m1 = LineScale(p[0], p[1], s);
			var m2 = LineScale(p[2], p[1], s);
			var m3 = LineScale(p[3], p[2], s);
			var m4 = LineScale(p[3], p[0], s);

			var pc = LineCross(m1, m3, m2, m4);

			using (SolidBrush brushWhite = new SolidBrush(light), brushBlack = new SolidBrush(dark)) {
				if (flag == 0) {
					g.FillPolygon(brushWhite, new PointF[] { m1, p[1], m2, pc });
					g.FillPolygon(brushWhite, new PointF[] { m4, pc, m3, p[3] });
					g.FillPolygon(brushBlack, new PointF[] { p[0], m1, pc, m4 });
					g.FillPolygon(brushBlack, new PointF[] { pc, m2, p[2], m3 });
				} else {
					g.FillPolygon(brushBlack, new PointF[] { m1, p[1], m2, pc });
					g.FillPolygon(brushBlack, new PointF[] { m4, pc, m3, p[3] });
					g.FillPolygon(brushWhite, new PointF[] { m4, p[0], m1, pc });
					g.FillPolygon(brushWhite, new PointF[] { pc, m2, p[2], m3 });
				}
			}
		}
	}
}
