using System.Runtime.InteropServices;

namespace PubHack {
	public class Structures {
		[StructLayout(LayoutKind.Explicit)]
		public struct VectorStructure {
			[FieldOffset(0x0)]
			public float X;

			[FieldOffset(0x4)]
			public float Y;

			[FieldOffset(0x8)]
			public float Z;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Matx3X4 {
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public Float4[] First;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Float4 {
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public float[] Second;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct PlayerBones {
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public Matx3X4[] Bones;
		}
    }
}