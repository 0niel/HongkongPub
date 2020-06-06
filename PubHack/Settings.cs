namespace PubHack {
	public class Settings {
		private float _fov = 2.5f;
		private int _pausesBetweenAiming = 5;

		public int KillKey { get; set; } = 0x77;
		public int DeactivateKey { get; set; } = 0x70;
		public int ReactivateKey { get; set; } = 0x71;
		public int ReloadSettingsKey { get; set; } = 0x73;
        public bool UseSounds { get; set; } = true;

        // ReSharper disable once InconsistentNaming
        public float FOV {
			get => _fov;
			set {
				if (value > 10)
					value = 10;
				else if (value < 0.1f) value = 0.1f;
				_fov = value;
			}
		}

		public float Sensitivity { get; set; } = 1.5f;
		public int Aimkey { get; set; } = 0x05;

		public int UpperPauseKey { get; set; } = 0x26;
		public int LowerPauseKey { get; set; } = 0x28;

		public int PausesBetweenAiming {
			get => _pausesBetweenAiming;
			set {
				if (value > 30)
					value = 30;
				else if (value < 1) value = 1;
				_pausesBetweenAiming = value;
			}
		}

		public bool VisibleCheck { get; set; } = false;
	}
}