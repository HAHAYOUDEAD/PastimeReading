using System;

namespace PastimeReading
{
	public static class SoundManager
	{
        private static string stateLast;
        public static bool initDone;

        public static void InitSounds()
		{
			AkSoundEngine.AddBasePath(ReadMain.modsPath);
            AkSoundEngine.LoadFilePackage("pastimeReading/pastimeReadingSounds.pck", out uint num, AkSoundEngine.AK_DEFAULT_POOL_ID);
            SoundManager.initDone = true;
		}

		public static void AnimatorStateDJ(string state, string wwiseEvent)
		{
			if (ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName(state))
			{
				if (SoundManager.stateLast != state)
				{
					AkSoundEngine.PostEvent(wwiseEvent, ReadMain.hands);
				}
				SoundManager.stateLast = state;
				return;
			}
			if (SoundManager.stateLast == state)
			{
				SoundManager.stateLast = null;
			}
		}
	}
}
