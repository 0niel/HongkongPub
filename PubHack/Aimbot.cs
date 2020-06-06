using System;
using System.Collections.Generic;

namespace PubHack {
	public static class Aimbot {
		private static DateTime _lastTimeAimed;
		private static Vector _boneToAimFor;

		private static MultimediaTimer _timer;
		private static float _activeDistance;
		private static GameData.Enemy _lastTarget;

		public static void Setup() {
			_timer = new MultimediaTimer();
			_timer.Elapsed += Do;
			_timer.Start();
		}

		private static void Do(object sender, EventArgs f) {
			_timer.Stop();
			try {
				if (Program.OverallActive && 0 < (KeyStateManager.GetAsyncKeyState(Program.Settings.Aimkey) & 0x8000) &&
				    GameData.LocalPlayer.Health > 0 &&
				    !GameData.LocalPlayer.NotUseable()) {
					var fov = Program.Settings.FOV + GameData.LocalPlayer.ShotsFired * 3;
					if (fov > 30) fov = 30;

					var target = GetNearestTargetByFov(fov);

					if (target != null && target != _lastTarget &&
					    (DateTime.Now - _lastTimeAimed).TotalMilliseconds < 500) {
						target = null;
					}

					if (target != null) {
						if (DoAim()) {
							_lastTarget = target;
							_lastTimeAimed = DateTime.Now;
							_timer.Interval = (int) HackMath.RandomNumber(Program.Settings.PausesBetweenAiming
							                                              , Program.Settings.PausesBetweenAiming + 2);
						} else {
							_timer.Interval = 1;
						}
					} else {
						_timer.Interval = 1;
					}
				} else if (GameData.LocalPlayer.IsSniper() || GameData.LocalPlayer.IsPistol()) {
					_timer.Interval = 1;
				} else {
					_timer.Interval = 20;
				}
			} catch {
				_timer.Interval = 1;
			}

			_timer.Start();
		}

		private static bool DoAim() {
			var aimAngles = GetAimAngle();
			aimAngles.VectorToMouseinches();
			aimAngles.SetMaxMousePixels(1);
			aimAngles.NormalizeAngle();

			if (aimAngles.Y != 0 || aimAngles.X != 0) {
				Mouse.MouseMove((int) aimAngles.Y, (int) aimAngles.X);
				return true;
			}

			return false;
		}

		private static Vector GetAimAngle() {
			var verticalRecoilPercent =
				HackMath.RandomNumber(90, 100) /
				100;
			var horizontalRecoilPercent =
				HackMath.RandomNumber(90, 100) /
				100;

			bool rcsActive = !GameData.LocalPlayer.IsPistol() && !GameData.LocalPlayer.IsSniper() &&
			                 GameData.LocalPlayer.ShotsFired > 1;

			var angleToAimFor = HackMath.AngleToTargetWithBonePosition(_boneToAimFor, rcsActive
				                                                                          ? 2 *
				                                                                            horizontalRecoilPercent
				                                                                          : 0, rcsActive
					                                                                               ? 2 *
					                                                                                 verticalRecoilPercent
					                                                                               : 0);

			var aimAngles = GameData.ClientState.ViewAngles.Clone()
			                        .Subtract(angleToAimFor);
			aimAngles.X *= -1;
			aimAngles.NormalizeAngle();
			if (!rcsActive && (aimAngles.X < (_activeDistance < 500 ? 0.35f : 0.2f) && aimAngles.X > 0 ||
			                   aimAngles.X > -(_activeDistance < 500 ? 0.35f : 0.2f) && aimAngles.X < 0)) {
				aimAngles.X = 0;
			}

			return aimAngles;
		}

		public static GameData.Enemy GetNearestTargetByFov(float fov) {
			GameData.Enemy nearestEnemy = null;
			float actualFov = 10000f;
			foreach (var player in GameData.Enemies.GetList())
				if (!player.Dormant && player.Health > 0 &&
				    (!Program.Settings.VisibleCheck || player.SpottedByLocalPlayer)) {
					var realDistance = GameData.LocalPlayer.Position.Distance(player.GetBone(8));
					List<Vector> bones = new List<Vector>();
					var lowestBone = player.GetBone(5);
					var highestBone = player.GetBone(130);
					var zVerschiebung = (highestBone.Z - lowestBone.Z) / 20;
					var xVerschiebung = (highestBone.X - lowestBone.X) / 20;
					var yVerschiebung = (highestBone.Y - lowestBone.Y) / 20;
					for (var i = 0; i < 20; i++) {
						var vector = lowestBone.Clone();
						vector.Z += zVerschiebung * i;
						vector.X += xVerschiebung * i;
						vector.Y += yVerschiebung * i;
						bones.Add(vector);
					}

					for (var i = bones.Count - 1; i >= 0; i--) {
						var angle = HackMath.AngleToTargetWithBonePosition(bones[i], 0, 0);
						var fovToBone = angle.Distance(GameData.ClientState.ViewAngles) *
						                (realDistance / 500);
						if (fovToBone > 0.1f && fovToBone < fov && fovToBone < actualFov) {
							actualFov = fovToBone;

							nearestEnemy = player;
							_boneToAimFor = bones[i];
							_activeDistance = realDistance;
						} else if (fov > 40) {
							break;
						}
					}
				}

			return nearestEnemy;
		}
	}
}