using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;

namespace PubHack {
	class Program {
		public static bool OverallActive;
		public static Settings Settings;

		static void Main() {
			while (true) {
				Thread thread = new Thread(UpdateKeys);
				try {
					LoadConfig();
					Memory.Initialize();
					Aimbot.Setup();
					thread.Start();
					GameDataUpdater.UpdateAll();
				} catch {
#pragma warning disable 618
					thread.Suspend();
#pragma warning restore 618
					//ignored
				}
			}
		}

		private static void UpdateKeys() {
			while (true) {
				try {
					if ((KeyStateManager.GetAsyncKeyState(Settings.KillKey) & 0x8000) > 0) {
						if (Settings.UseSounds) Console.Beep(200, 100);
						Environment.Exit(0);
					} else if ((KeyStateManager.GetAsyncKeyState(Settings.DeactivateKey) & 0x8000) > 0) {
						OverallActive = false;
						if (Settings.UseSounds) Console.Beep(400, 100);
						Thread.Sleep(300);
					} else if ((KeyStateManager.GetAsyncKeyState(Settings.ReactivateKey) & 0x8000) > 0 &&
					           (KeyStateManager.GetAsyncKeyState(0x11) & 0x8000) > 0) {
						OverallActive = true;
						if (Settings.UseSounds) Console.Beep(1000, 100);
						Thread.Sleep(300);
					} else if ((KeyStateManager.GetAsyncKeyState(Settings.UpperPauseKey) & 0x8000) > 0) {
						Settings.PausesBetweenAiming += 1;
						if (Settings.UseSounds) Console.Beep(1000, 100);
						Thread.Sleep(300);
					} else if ((KeyStateManager.GetAsyncKeyState(Settings.LowerPauseKey) & 0x8000) > 0) {
						Settings.PausesBetweenAiming -= 1;
						if (Settings.UseSounds) Console.Beep(400, 100);
						Thread.Sleep(300);
					} else if ((KeyStateManager.GetAsyncKeyState(Settings.ReloadSettingsKey) & 0x8000) > 0) {
						LoadConfig();
						Thread.Sleep(300);
					}
				} catch {
					// ignored
				}

				Thread.Sleep(20);
			}
		}

		private static void LoadConfig() {
			try {
				if (!File.Exists("config.ini")) {
					File.WriteAllText("config.ini", JsonConvert.SerializeObject(new Settings(), Formatting.Indented));
				}

				Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("config.ini"));
				File.WriteAllText("config.ini", JsonConvert.SerializeObject(Settings, Formatting.Indented));
			} catch {
				// ignored
			}
		}
	}
}