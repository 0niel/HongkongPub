using System.Runtime.InteropServices;

namespace PubHack {
	public static class KeyStateManager {
		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(System.Int32 vKey);
	}
}