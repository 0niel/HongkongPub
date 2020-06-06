using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace PubHack {
	class Memory {
		private static IntPtr _processHandle;
		public static int Client;
		public static int Engine;

		public static void Initialize() {
			Client = 0;
			Engine = 0;
			if (_processHandle != IntPtr.Zero) CloseHandle(_processHandle);

			Process process;
			do {
				Process[] list;
				do {
					Thread.Sleep(1000);
					list = Process.GetProcessesByName("csgo");
				} while (list.Length == 0);

				process = list[0];
				ProcessModuleCollection modules = process.Modules;
				Thread.Sleep(1000);
				foreach (ProcessModule module in modules) {
					if (module.ModuleName.Equals("client.dll")) {
						Client = (int) module.BaseAddress;
					} else if (module.ModuleName.Equals("engine.dll")) {
						Engine = (int) module.BaseAddress;
					}
				}
			} while (Client == 0 || Engine == 0);

			_processHandle = OpenProcess(ProcessAccessFlags.VirtualMemoryRead, false, process.Id);


			OffsetUpdater.UpdateOffsets();
		}

		public static int ReadInt32(int adress) { return BitConverter.ToInt32(ReadProcMem(adress, sizeof(int)), 0); }

		public static short ReadShort(int adress) {
			return BitConverter.ToInt16(ReadProcMem(adress, sizeof(Int16)), 0);
		}

		// ReSharper disable once UnusedMember.Global
		public static float ReadFloat(int adress) { return BitConverter.ToSingle(ReadProcMem(adress, sizeof(int)), 0); }
		public static bool ReadBool(int adress) { return BitConverter.ToBoolean(ReadProcMem(adress, sizeof(int)), 0); }

		public static Structures.VectorStructure ReadVector(int adress) {
			int size = Marshal.SizeOf(typeof(Structures.VectorStructure));
			byte[] bytes = new byte[size];
			ReadProcessMemory(_processHandle, adress, bytes, size, out _);
			GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			Structures.VectorStructure theStructure =
				(Structures.VectorStructure) Marshal.PtrToStructure(handle.AddrOfPinnedObject()
				                                                    , typeof(Structures.VectorStructure));
			handle.Free();
			return theStructure;
		}
		
		public static Structures.PlayerBones ReadBones(int adress) {
			int size = Marshal.SizeOf(typeof(Structures.PlayerBones));
			byte[] bytes = new byte[size];
			ReadProcessMemory(_processHandle, adress, bytes, size, out _);
			GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			Structures.PlayerBones theStructure =
				(Structures.PlayerBones) Marshal.PtrToStructure(handle.AddrOfPinnedObject()
				                                                , typeof(Structures.PlayerBones));
			handle.Free();
			return theStructure;
		}

		private static byte[] ReadProcMem(int adress, int size) {
			var array = new byte[size];
			ReadProcessMemory(_processHandle, adress, array, size, out _);
			return array;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

		[DllImport("kernel32.dll", SetLastError = true)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(IntPtr hObject);

		[Flags]
		public enum ProcessAccessFlags : uint {
			VirtualMemoryRead = 0x00000010
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr process
		                                            , int baseAddress
		                                            , [Out] byte[] buffer
		                                            , int dwSize
		                                            , out int numberOfBytesRead);
	}
}