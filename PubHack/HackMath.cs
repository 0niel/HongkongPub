using System;

namespace PubHack {
	internal class HackMath {
		private static readonly float _rad = 180.0f / Convert.ToSingle(Math.PI);
		private static readonly Random _random = new Random();

		public static Vector AngleToTargetWithBonePosition(Vector bone
		                                                   , double horizontalRecoilReduction
		                                                   , double verticalRecoilReduction) {
			return CalculateAngle(bone, (float) horizontalRecoilReduction, (float) verticalRecoilReduction);
		}

		public static Vector CalculateAngle(Vector enpo
		                                    , float horizontalRecoilReduction
		                                    , float verticalRecoilReduction) {
			var delta = new Vector(GameData.LocalPlayer.Position.X - enpo.X, GameData.LocalPlayer.Position.Y - enpo.Y
			                       , GameData.LocalPlayer.Position.Z + GameData.LocalPlayer.VectorView.Z - enpo.Z);
			var returnVector =
				new
					Vector(Convert.ToSingle(Math.Atan(delta.Z / Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y))) *
					       57.295779513082f - GameData.LocalPlayer.AimPunch.X * verticalRecoilReduction
					       ,
					       Convert.ToSingle(Math.Atan(delta.Y / delta.X)) * _rad -
					       GameData.LocalPlayer.AimPunch.Y * horizontalRecoilReduction, 0);

			if (delta.X >= 0.0) returnVector.Y += 180f;

			returnVector.NormalizeAngle();

			return returnVector;
		}

		public static double RandomNumber(double minimum, double maximum) {
			return _random.NextDouble() * (maximum - minimum) + minimum;
		}
	}
}