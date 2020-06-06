using System;
using System.Collections.Generic;
using System.Linq;

namespace PubHack {
	public static class GameData {
		public static class LocalPlayer {
			public static int Base;
			public static int Health;
			public static int TeamNumber;
			public static int ShotsFired;
			public static Vector Position;
			public static Vector AimPunch;
			public static Vector VectorView;
			public static short WeaponId;
			public static Vector Velocity;
			public static int Index;

			public static void Update() {
				if (Base != 0) {
					try {
						Health = Memory.ReadInt32(Base + netvars.m_iHealth);
						TeamNumber = Memory.ReadInt32(Base + netvars.m_iTeamNum);
						ShotsFired = Memory.ReadInt32(Base + netvars.m_iShotsFired);
						Position = new Vector(Memory.ReadVector(Base + netvars.m_vecOrigin));
						AimPunch = new Vector(Memory.ReadVector(Base + netvars.m_aimPunchAngle));
						VectorView = new Vector(Memory.ReadVector(Base + netvars.m_vecViewOffset));

						var weaponIndex = Memory.ReadInt32(Base + netvars.m_hActiveWeapon);
						weaponIndex &= 0xFFF;
						var weaponEntity =
							Memory.ReadInt32(Memory.Client + signatures.dwEntityList + (weaponIndex - 1) * 0x10);
						WeaponId = Memory.ReadShort(weaponEntity + netvars.m_iItemDefinitionIndex);
						Velocity = new Vector(Memory.ReadVector(Base + netvars.m_vecVelocity));
					} catch {
						// ignored
					}
				}
			}

			public static bool NotUseable() {
				switch (WeaponId) {
					case (short) WeaponIds.WEAPON_DECOY:
					case (short) WeaponIds.WEAPON_FLASHBANG:
					case (short) WeaponIds.WEAPON_HEGRENADE:
					case (short) WeaponIds.WEAPON_INCGRENADE:
					case (short) WeaponIds.WEAPON_SMOKEGRENADE:
					case (short) WeaponIds.WEAPON_MOLOTOV:
					case (short) WeaponIds.WEAPON_C4:
					case (short) WeaponIds.WEAPON_KNIFE:
					case (short) WeaponIds.WEAPON_KNIFE_BAYONET:
					case (short) WeaponIds.WEAPON_KNIFE_BUTTERFLY:
					case (short) WeaponIds.WEAPON_KNIFE_FALCHION:
					case (short) WeaponIds.WEAPON_KNIFE_FLIP:
					case (short) WeaponIds.WEAPON_KNIFE_GUT:
					case (short) WeaponIds.WEAPON_KNIFE_KARAMBIT:
					case (short) WeaponIds.WEAPON_KNIFE_M9_BAYONET:
					case (short) WeaponIds.WEAPON_KNIFE_PUSH:
					case (short) WeaponIds.WEAPON_KNIFE_SURVIVAL_BOWIE:
					case (short) WeaponIds.WEAPON_KNIFE_TACTICAL:
					case (short) WeaponIds.WEAPON_KNIFE_T:
						return true;
				}

				return false;
			}

			public static bool IsPistol() {
				switch (WeaponId) {
					case (short) WeaponIds.WEAPON_CZ75A:
					case (short) WeaponIds.WEAPON_DEAGLE:
					case (short) WeaponIds.WEAPON_FIVESEVEN:
					case (short) WeaponIds.WEAPON_ELITE:
					case (short) WeaponIds.WEAPON_GLOCK:
					case (short) WeaponIds.WEAPON_HKP2000:
					case (short) WeaponIds.WEAPON_REVOLVER:
					case (short) WeaponIds.WEAPON_TEC9:
					case (short) WeaponIds.WEAPON_USP_SILENCER:
					case (short) WeaponIds.WEAPON_TASER:
					case (short) WeaponIds.WEAPON_P250:
						return true;
				}

				return false;
			}

			public static bool IsSniper() {
				switch (WeaponId) {
					case (short) WeaponIds.WEAPON_AWP:
					case (short) WeaponIds.WEAPON_SSG08:
					case (short) WeaponIds.WEAPON_G3SG1:
					case (short) WeaponIds.WEAPON_SCAR20:
						return true;
				}

				return false;
			}

			// ReSharper disable once UnusedMember.Global
			public static bool IsMp() {
				switch (WeaponId) {
					case (short) WeaponIds.WEAPON_BIZON:
					case (short) WeaponIds.WEAPON_MAC10:
					case (short) WeaponIds.WEAPON_MP7:
					case (short) WeaponIds.WEAPON_MP9:
					case (short) WeaponIds.WEAPON_P90:
					case (short) WeaponIds.WEAPON_UMP45:
					case (short) WeaponIds.WEAPON_MP5SD:
						return true;
				}

				return false;
			}

			// ReSharper disable once UnusedMember.Global
			public static bool IsPumpgun() {
				switch (WeaponId) {
					case (short) WeaponIds.WEAPON_MAG7:
					case (short) WeaponIds.WEAPON_SAWEDOFF:
					case (short) WeaponIds.WEAPON_NOVA:
					case (short) WeaponIds.WEAPON_XM1014:
						return true;
				}

				return false;
			}

			// ReSharper disable once UnusedMember.Global
			public static bool IsRifle() {
				switch (WeaponId) {
					case (short) WeaponIds.WEAPON_M4A1:
					case (short) WeaponIds.WEAPON_AUG:
					case (short) WeaponIds.WEAPON_FAMAS:
					case (short) WeaponIds.WEAPON_GALILAR:
					case (short) WeaponIds.WEAPON_M249:
					case (short) WeaponIds.WEAPON_M4A1_SILENCER:
					case (short) WeaponIds.WEAPON_NEGEV:
					case (short) WeaponIds.WEAPON_SG556:
					case (short) WeaponIds.WEAPON_AK47:
						return true;
				}

				return false;
			}
		}

		public static class Enemies {
			private static readonly Dictionary<int, Enemy> _dictionary = new Dictionary<int, Enemy>();
			public static List<Enemy> GetList() { return _dictionary.Select(x => x.Value).ToList(); }

			public static void Clear() { _dictionary.Clear(); }

			public static void Update() {
				for (int i = 1; i <= 64; i++) {
					var entityHandle = Memory.ReadInt32(Memory.Client + signatures.dwEntityList + (i - 1) * 0x10);
					if (entityHandle != 0) {
						if (entityHandle != LocalPlayer.Base) {
							var teamNumber = Memory.ReadInt32(entityHandle + netvars.m_iTeamNum);
							if (teamNumber != LocalPlayer.TeamNumber && (teamNumber == 2 || teamNumber == 3)) {
								Enemy enemy;
								if (_dictionary.ContainsKey(entityHandle)) {
									enemy = _dictionary[entityHandle];
								} else {
									enemy = new Enemy();
									_dictionary.Add(entityHandle, enemy);
								}

								//could be read in junks (structs) but only boosts performance if you would read way more variables
								enemy.Base = entityHandle;
								enemy.Dormant = Memory.ReadBool(enemy.Base + signatures.m_bDormant);
								enemy.TeamNumber = teamNumber;
								enemy.Index = i;
								enemy.Health = Memory.ReadInt32(enemy.Base + netvars.m_iHealth);
								enemy.Bonematrix = Memory.ReadInt32(enemy.Base + netvars.m_dwBoneMatrix);
								enemy.Position = new Vector(Memory.ReadVector(enemy.Base + netvars.m_vecOrigin));

								var bones = Memory.ReadBones(enemy.Bonematrix);
								for (int boneId = 0; boneId < bones.Bones.Length; boneId++) {
									if (!enemy.Bones.ContainsKey(boneId)) {
										enemy.Bones.Add(boneId, bones.Bones[boneId]);
									} else {
										enemy.Bones[boneId] = bones.Bones[boneId];
									}
								}

								enemy.Velocity = new Vector(Memory.ReadVector(enemy.Base + netvars.m_vecVelocity));

								var spottedByMask = Memory.ReadInt32(enemy.Base + netvars.m_bSpottedByMask);
								enemy.SpottedByLocalPlayer =
									(spottedByMask & (1 << LocalPlayer.Index - 1)) > 0;
								enemy.LastUpdated = DateTime.Now;
							}
						} else {
							LocalPlayer.Index = i;
						}
					}
				}

				var list = GetList();
				foreach (var enemy in list) {
					if ((DateTime.Now - enemy.LastUpdated).TotalSeconds > 20) {
						_dictionary.Remove(enemy.Index);
					}
				}
			}
		}

		public class Enemy {
			public int Base;
			public int Bonematrix;
			public bool Dormant;
			public int Health;
			public int TeamNumber;
			public int Index;
			public Vector Position;
			public Vector Velocity;
			public bool SpottedByLocalPlayer;
			public DateTime LastUpdated;
			public Dictionary<int, Structures.Matx3X4> Bones { get; set; } = new Dictionary<int, Structures.Matx3X4>();

			public Vector GetBone(int bone) {
				var boneIndex = bone != 130 ? bone : 8;

				if (!Bones.ContainsKey(boneIndex)) {
					return new Vector(0, 0, 0);
				}

				var boneToReturn = Bones[boneIndex];
				var vector = new Vector(boneToReturn.First[0].Second[3], boneToReturn.First[1].Second[3]
				                        , boneToReturn.First[2].Second[3]);
				if (bone == 130) vector.Z += 3f;

				return vector;
			}
		}

		public static class ClientState {
			public static int Base;
			public static int GameState;
			public static Vector ViewAngles;

			public static void Update() {
				if (Base != 0) {
					try {
						GameState = Memory.ReadInt32(Base + signatures.dwClientState_State);
						ViewAngles = new Vector(Memory.ReadVector(Base + signatures.dwClientState_ViewAngles));
					} catch {
						// ignored
					}
				}
			}
		}

		public static class GameRulesProxy {
			public static int Base;
			public static int TotalRoundsPlayed;
			public static bool FreezeTime;

			public static void Update() {
				try {
					if (Base != 0) {
						TotalRoundsPlayed = Memory.ReadInt32(Base + 0x64);
						FreezeTime = Memory.ReadBool(Base + netvars.m_bFreezePeriod);
					}
				} catch {
					// ignored
				}
			}
		}
	}
}