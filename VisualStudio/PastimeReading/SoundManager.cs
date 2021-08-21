using System;

namespace PastimeReading
{
	// Token: 0x02000003 RID: 3
	public static class SoundManager
	{
		// Token: 0x06000008 RID: 8 RVA: 0x000026B8 File Offset: 0x000008B8
		public static void InitSounds()
		{
			AkSoundEngine.AddBasePath(ReadMain.modsPath);
			uint num;
			AkSoundEngine.LoadFilePackage("pastimeReading/pastimeReadingSounds.pck", ref num, AkSoundEngine.AK_DEFAULT_POOL_ID);
			SoundManager.initDone = true;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000026E8 File Offset: 0x000008E8
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

		// Token: 0x0400000C RID: 12
		private static string stateLast;

		// Token: 0x0400000D RID: 13
		public static bool initDone;
	}
}
