using System;

namespace PastimeReading
{
	// Token: 0x02000007 RID: 7
	internal static class Settings
	{
		// Token: 0x06000014 RID: 20 RVA: 0x0000347E File Offset: 0x0000167E
		public static void OnLoad()
		{
			Settings.options = new ReadSettings();
			Settings.options.AddToModSettings("Pastime Reading Settings");
		}

		// Token: 0x04000039 RID: 57
		public static ReadSettings options;
	}
}
