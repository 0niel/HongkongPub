using System;
using System.Diagnostics;
using System.Text;
using PubHack;

namespace Hongkong {
	public static class SignatureScan {
		private static byte[] _dumbedArray;
		private const int MAX_DUMP_SIZE = 204800;
		
		public static void ResetRegion() { _dumbedArray = null; }

		public static Process Process { get; set; }

		public static IntPtr Address { get; set; }

		public static int Size { get; set; }

        private static bool DumpMemory() {
			try {
				if (Process == null) return false;
				if (Process.HasExited) return false;
				if (Address == IntPtr.Zero) return false;
				if (Size == 0) return false;

				_dumbedArray = new byte[Size];

				var bReturn =
					Memory.ReadProcessMemory(Process.Handle, (int) Address, _dumbedArray, Size, out var nBytesRead);
				if (bReturn == false || nBytesRead != Size) return false;
				return true;
			} catch {
				return false;
			}
		}

		public static int ReadInt(long address) {
			byte[] buffer = new byte[4];

			ReadMemory(Process.Handle, address, ref buffer);

			return BitConverter.ToInt32(buffer, 0);
		}

		public static int ReadByte(long address) {
			byte[] buffer = new byte[1];

			ReadMemory(Process.Handle, address, ref buffer);

			return buffer[0];
		}

		public static void ReadMemory(IntPtr handle, long address, ref Byte[] buffer) {
			Memory.ReadProcessMemory(handle, (int) address, buffer, buffer.Length, out _);
		}

		private static bool MaskCheck(int nOffset, byte[] btPattern, string strMask, bool wNonZero = false) {
			for (int x = 0; x < btPattern.Length; x++) {
				if (strMask[x] == '?')
					if (wNonZero && _dumbedArray[nOffset + x] == 0x00)
						return false;
					else
						continue;
				if (strMask[x] == 'x' && btPattern[x] != _dumbedArray[nOffset + x]) return false;
			}

			return true;
		}

		public static IntPtr FindPattern(byte[] btPattern, string strMask, int nOffset, bool wNonZero = false) {
			try {
				if (_dumbedArray == null || _dumbedArray.Length == 0) {
					if (!DumpMemory()) return IntPtr.Zero;
				}

				if (strMask.Length != btPattern.Length) return IntPtr.Zero;
				if (_dumbedArray != null) {
					for (int x = 0; x < _dumbedArray.Length; x++) {
						if (MaskCheck(x, btPattern, strMask, wNonZero)) {
							return new IntPtr((int) Address + x + nOffset);
						}
					}
				}

				return IntPtr.Zero;
			} catch {
				return IntPtr.Zero;
			}
		}

		public static string MaskFromPattern(byte[] pattern) {
			StringBuilder builder = new StringBuilder();

			foreach (byte data in pattern) builder.Append(data == 0x00 ? '?' : 'x');

			return builder.ToString();
		}

		public static int FindAddress(byte[] pattern
		                              , int offset
		                              , string mask
		                              , int dllAddress
		                              , long dllSize
		                              , bool wNonZero = false) {
			int address = 0;

			for (int i = 0; i < dllSize && address == 0; i += MAX_DUMP_SIZE) {
				Address = new IntPtr(dllAddress + i);

				address = FindPattern(pattern, mask, offset, wNonZero).ToInt32();

				ResetRegion();
			}


			return address;
		}

		private static ProcessModule GetModuleByName(Process process, string name) {
			try {
				foreach (ProcessModule module in process.Modules) {
					if (module.FileName.EndsWith(name)) return module;
				}
			} catch {
				// ignored
			}

			return null;
		}

		public static long GetModuleSize(Process process, string name) {
			ProcessModule module = GetModuleByName(process, name);

			if (module != null) return module.ModuleMemorySize;

			return 0L;
		}

		public static IntPtr GetModuleBaseAddressByName(Process process, string name) {
			ProcessModule module = GetModuleByName(process, name);

			if (module != null) return module.BaseAddress;

			return IntPtr.Zero;
		}
	}
}