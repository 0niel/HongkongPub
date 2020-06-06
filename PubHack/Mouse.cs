using System.Runtime.InteropServices;

namespace PubHack {
	public static class Mouse {
		const uint MOUSEEVENTF_MOVE = 0x0001;

		[DllImport("user32.dll")]
		private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

		public static void MouseMove(int x, int y) { mouse_event(MOUSEEVENTF_MOVE, (uint) x, (uint) y, 0, 0); }
	}
}