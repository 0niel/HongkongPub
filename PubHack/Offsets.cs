using System;

// ReSharper disable InconsistentNaming

namespace PubHack {
	public static class netvars {
		public static Int32 m_aimPunchAngle = 0x302C;
		public static Int32 m_bFreezePeriod = 0x20;
		public static Int32 m_bSpottedByMask = 0x980;
		public static Int32 m_dwBoneMatrix = 0x26A8;
		public static Int32 m_hActiveWeapon = 0x2EF8;
		public static Int32 m_iHealth = 0x100;
		public static Int32 m_iItemDefinitionIndex = 0x2FAA;
		public static Int32 m_iShotsFired = 0xA380;
		public static Int32 m_iTeamNum = 0xF4;
		public static Int32 m_vecOrigin = 0x138;
		public static Int32 m_vecVelocity = 0x114;
		public static Int32 m_vecViewOffset = 0x108;
	}

	public static class signatures {
		public static Int32 dwClientState = 0x589DD4;
		public static Int32 dwClientState_State = 0x108;
		public static Int32 dwClientState_ViewAngles = 0x4D88;
		public static Int32 dwEntityList = 0x4D4B104;
		public static Int32 dwGameRulesProxy = 0x526807C;
		public static Int32 dwLocalPlayer = 0xD36B94;
		public static Int32 m_bDormant = 0xED;
	}
}