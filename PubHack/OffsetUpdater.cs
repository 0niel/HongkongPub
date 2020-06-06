using System;
using System.Diagnostics;
using Hongkong;

namespace PubHack {
	public class OffsetUpdater {
		private static DateTime _lastUpdatedOffset = DateTime.MinValue;

		private const int MAX_DUMP_SIZE = 204800;

		public static void UpdateOffsets() {
			if ((DateTime.Now - _lastUpdatedOffset).TotalMinutes > 1) {
				try {
					Process process = Process.GetProcessesByName("csgo")[0];
					SignatureScan.Process = process;
					SignatureScan.Address = IntPtr.Zero;
					SignatureScan.Size = MAX_DUMP_SIZE;

					int clientDll = SignatureScan.GetModuleBaseAddressByName(process, "client.dll").ToInt32();
					long clientDllSize = SignatureScan.GetModuleSize(process, "client.dll");
					int engineDll = SignatureScan.GetModuleBaseAddressByName(process, "engine.dll").ToInt32();
					long engineDllSize = SignatureScan.GetModuleSize(process, "engine.dll");

					FindWeaponOffset(clientDll, clientDllSize);
					FindTeamnumOffset(clientDll, clientDllSize);
					FindHealthOffset(clientDll, clientDllSize);
					FindVecOriginOffset(clientDll, clientDllSize);
					FindBonematrixOffset(clientDll, clientDllSize);
					FindDormantOffset(clientDll, clientDllSize);
					FindGameRulesProxyOffset(clientDll, clientDllSize);
					FindViewAnglesOffset(engineDll, engineDllSize);
					FindClientstateOffset(engineDll, engineDllSize);
					FindEntitylistOffset(clientDll, clientDllSize);
					FindLocalPlayerOffset(clientDll, clientDllSize);
					_lastUpdatedOffset = DateTime.Now;
				} catch {
					if (Program.Settings.UseSounds) Console.Beep(400, 100);
				}
			}
		}

		private static void FindWeaponOffset(int clientDll, long clientDllSize) {
			var pattern = new byte[] {
				                         0x0F, 0x45, 0xF7, 0x5F, 0x8B, 0x8E, 0x00, 0x00, 0x00, 0x00, 0x5E, 0x83, 0xF9
				                         , 0xFF
			                         };
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 0, mask, clientDll, clientDllSize);
			netvars.m_hActiveWeapon = SignatureScan.ReadInt(adress + 6);
		}

		private static void FindTeamnumOffset(int clientDll, long clientDllSize) {
			var pattern = new byte[] {
				                         0xCC, 0xCC, 0xCC, 0x8B, 0x89, 0x00, 0x00, 0x00, 0x00, 0xE9, 0x00, 0x00, 0x00
				                         , 0x00, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0x8B, 0x81, 0x00, 0x00, 0x00, 0x00, 0xC3
				                         , 0xCC, 0xCC
			                         };
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 0, mask, clientDll, clientDllSize);
			netvars.m_iTeamNum = SignatureScan.ReadInt(adress + 5);
		}

		private static void FindHealthOffset(int clientDll, long clientDllSize) {
			var pattern = new byte[] {
				                         0x8B, 0x41, 0x00, 0x89, 0x41, 0x00, 0x8B, 0x41, 0x00, 0x89, 0x41, 0x00, 0x8B
				                         , 0x41, 0x00, 0x89, 0x41, 0x00, 0x8B, 0x4F, 0x00, 0x83, 0xB9, 0x00, 0x00, 0x00
				                         , 0x00, 0x00, 0x7F, 0x2E
			                         };
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 0, mask, clientDll, clientDllSize);
			netvars.m_iHealth = SignatureScan.ReadInt(adress + 23);
		}

		private static void FindVecOriginOffset(int clientDll, long clientDllSize) {
			var pattern = new byte[] {
				                         0x8A, 0x0E, 0x80, 0xE1, 0xFC, 0x0A, 0xC8, 0x88, 0x0E, 0xF3, 0x00, 0x00, 0x87
				                         , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x87, 0x00, 0x00, 0x00, 0x00, 0x9F
			                         };
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 0, mask, clientDll, clientDllSize);
			netvars.m_vecOrigin = SignatureScan.ReadInt(adress + 13);
		}

		private static void FindBonematrixOffset(int clientDll, long clientDllSize) {
			var pattern = new byte[] {
				                         0x75, 0x15, 0x8B, 0x87, 0x00, 0x00, 0x00, 0x00, 0x8B, 0xCF, 0x8B, 0x17, 0x03
				                         , 0x44, 0x24, 0x14, 0x50
			                         };
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 4, mask, clientDll, clientDllSize);
			netvars.m_dwBoneMatrix = SignatureScan.ReadInt(adress);
		}

		private static void FindDormantOffset(int clientDll, long clientDllSize) {
			var pattern = new byte[] {0x8A, 0x81, 0x00, 0x00, 0x00, 0x00, 0xC3, 0x32, 0xC0};
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 2, mask, clientDll, clientDllSize);
			signatures.m_bDormant = SignatureScan.ReadInt(adress) + 8;
		}

		private static void FindGameRulesProxyOffset(int clientDll, long clientDllSize) {
			var pattern = new byte[] {
				                         0xA1, 0x00, 0x00, 0x00, 0x00, 0x85, 0xC0, 0x0F, 0x84, 0x00, 0x00, 0x00, 0x00
				                         , 0x80, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x84, 0x00, 0x00, 0x00, 0x00
				                         , 0x0F, 0x10, 0x05
			                         };
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 1, mask, clientDll, clientDllSize);
			var val1 = SignatureScan.ReadInt(adress);
			signatures.dwGameRulesProxy = val1 - clientDll;
		}

		private static void FindViewAnglesOffset(int engineDll, long engineDllSize) {
			var pattern = new byte[] {0xF3, 0x0F, 0x11, 0x80, 0x00, 0x00, 0x00, 0x00, 0xD9, 0x46, 0x04, 0xD9, 0x05};
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 4, mask, engineDll, engineDllSize);
			signatures.dwClientState_ViewAngles = SignatureScan.ReadInt(adress);
		}

		private static void FindClientstateOffset(int engineDll, long engineDllSize) {
			var pattern = new byte[] {
				                         0xA1, 0x00, 0x00, 0x00, 0x00, 0x33, 0xD2, 0x6A, 0x00, 0x6A, 0x00, 0x33, 0xC9
				                         , 0x89, 0xB0
			                         };
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 1, mask, engineDll, engineDllSize);
			var val1 = SignatureScan.ReadInt(adress);
			signatures.dwClientState = val1 - engineDll;
		}

		private static void FindLocalPlayerOffset(int clientDll, long clientDllSize) {
			byte[] pattern = {
				                 0x8D, 0x34, 0x85, 0x00, 0x00, 0x00, 0x00, 0x89, 0x15, 0x00, 0x00, 0x00, 0x00, 0x8B
				                 , 0x41, 0x08, 0x8B, 0x48, 0x04, 0x83, 0xF9, 0xFF
			                 };
			string mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 3, mask, clientDll, clientDllSize);

			var localplayer = SignatureScan.ReadInt(adress);
			adress = SignatureScan.FindAddress(pattern, 18, mask, clientDll, clientDllSize);
			var val2 = SignatureScan.ReadByte(adress);
			localplayer += val2;
			localplayer -= clientDll;
			signatures.dwLocalPlayer = localplayer;
		}

		private static void FindEntitylistOffset(int clientDll, long clientDllSize) {
			var pattern = new byte[] {
				                         0xBB, 0x00, 0x00, 0x00, 0x00, 0x83, 0xFF, 0x01, 0x0F, 0x8C, 0x00, 0x00, 0x00
				                         , 0x00, 0x3B, 0xF8
			                         };
			var mask = SignatureScan.MaskFromPattern(pattern);
			var adress = SignatureScan.FindAddress(pattern, 1, mask, clientDll, clientDllSize);
			var val1 = SignatureScan.ReadInt(adress);
			signatures.dwEntityList = val1 - clientDll;
		}
	}
}