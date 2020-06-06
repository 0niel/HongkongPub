using System;
using System.Threading;

namespace PubHack {
	public class GameDataUpdater {
		public static void UpdateAll() {
			if (Program.Settings.UseSounds) Console.Beep(700, 100);
			while (true) {
				try {
					GameData.ClientState.Base = Memory.ReadInt32(Memory.Engine + signatures.dwClientState);
					if (GameData.ClientState.Base != 0) {
						GameData.ClientState.Update();
						if (GameData.ClientState.GameState == 6) {
							GameData.LocalPlayer.Base = Memory.ReadInt32(Memory.Client + signatures.dwLocalPlayer);
							if (GameData.LocalPlayer.Base != 0) {
								GameData.LocalPlayer.Update();
								GameData.GameRulesProxy.Base =
									Memory.ReadInt32(Memory.Client + signatures.dwGameRulesProxy);
								GameData.GameRulesProxy.Update();
								GameData.Enemies.Update();
								Thread.Sleep(1);
							} else {
								GameData.Enemies.Clear();
								Thread.Sleep(1000);
							}
						} else {
							GameData.Enemies.Clear();
							Thread.Sleep(1000);
						}
					} else {
						Memory.Initialize();
					}
				} catch {
					Memory.Initialize();
				}
			}
		}
	}
}