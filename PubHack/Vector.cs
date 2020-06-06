using System;

namespace PubHack {
	public class Vector {
		public float X, Y, Z;

		public Vector(float x, float y, float z) {
			X = x;
			Y = y;
			Z = z;
		}

		public Vector(Structures.VectorStructure vector) {
			X = vector.X;
			Y = vector.Y;
			Z = vector.Z;
		}

		public Vector Clone() { return new Vector(X, Y, Z); }

		public Vector Subtract(Vector vector) {
			X -= vector.X;
			Y -= vector.Y;
			Z -= vector.Z;
			return this;
		}

		public void NormalizeAngle() {
			while (X > 89.0f) X -= 180f;

			while (X < -89.0f) X += 180f;

			while (Y > 180f) Y -= 360f;

			while (Y < -180f) Y += 360f;
		}

		public void SetMaxMousePixels(int step) {
			if (X > step)
				X = step;
			else if (X < -step) X = -step;

			if (Y > step)
				Y = step;
			else if (Y < -step) Y = -step;
		}

		public void VectorToMouseinches() {
			X /= (float) (Program.Settings.Sensitivity * 0.022);
			Y /= (float) (Program.Settings.Sensitivity * 0.022);
		}

		public float Distance(Vector targetVector) {
			return (float) Math.Sqrt(Math.Pow(targetVector.X - X, 2f) + Math.Pow(targetVector.Y - Y, 2f) + Math.Pow(targetVector.Z - Z, 2f));
		}

        #region Operator overloading

        public static Vector operator +(Vector ve1, Vector ve2) {
			return new Vector(ve1.X + ve2.X, ve1.Y + ve2.Y, ve1.Z + ve2.Z);
		}

		public static Vector operator -(Vector ve1, Vector ve2) {
			return new Vector(ve1.X - ve2.X, ve1.Y - ve2.Y, ve1.Z - ve2.Z);
		}

		public static Vector operator *(Vector ve1, Vector ve2) {
			return new Vector(ve1.X * ve2.X, ve1.Y * ve2.Y, ve1.Z * ve2.Z);
		}

		public static Vector operator /(Vector ve1, Vector ve2) {
			return new Vector(ve1.X / ve2.X, ve1.Y / ve2.Y, ve1.Z / ve2.Z);
		}

		public static Vector operator +(Vector ve1, float f) { return new Vector(ve1.X + f, ve1.Y + f, ve1.Z + f); }

		public static Vector operator -(Vector ve1, float f) { return new Vector(ve1.X - f, ve1.Y - f, ve1.Z - f); }

		public static Vector operator *(Vector ve1, float f) { return new Vector(ve1.X * f, ve1.Y * f, ve1.Z * f); }

		public static Vector operator /(Vector ve1, float f) { return new Vector(ve1.X / f, ve1.Y / f, ve1.Z / f); }

		#endregion
	}
}