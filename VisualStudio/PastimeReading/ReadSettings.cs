using System;
using System.IO;
using MelonLoader;
using ModSettings;
using UnityEngine;

namespace PastimeReading
{
	// Token: 0x02000006 RID: 6
	internal class ReadSettings : JsonModSettings
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00003150 File Offset: 0x00001350
		protected override void OnConfirm()
		{
			if (Settings.options.reloadBook)
			{
				StreamReader streamReader = new StreamReader(ReadMain.modsPath + "/pastimeReading/" + PageManager.bookFileName);
				while (!streamReader.EndOfStream)
				{
					for (int i = 0; i < 2; i++)
					{
						if (i == 0)
						{
							PageManager.bookTitle = streamReader.ReadLine();
						}
						if (i == 1)
						{
							PageManager.bookAuthor = streamReader.ReadLine();
						}
					}
					PageManager.bookContents = streamReader.ReadToEnd();
				}
				ReadMain.titleText.text = PageManager.bookTitle;
				ReadMain.authorText.text = PageManager.bookAuthor;
				ReadMain.p1Text.text = PageManager.bookContents;
				ReadMain.p2Text.text = PageManager.bookContents;
				ReadMain.h1Text.text = PageManager.bookContents;
				ReadMain.h2Text.text = PageManager.bookContents;
				Settings.options.reloadBook = false;
			}
			if (Settings.options.handsVariant == 0)
			{
				ReadMain.handsFMesh.SetActive(true);
				ReadMain.handsMMesh.SetActive(false);
			}
			else if (Settings.options.handsVariant == 1)
			{
				ReadMain.handsFMesh.SetActive(false);
				ReadMain.handsMMesh.SetActive(true);
			}
			string text = null;
			switch (Settings.options.bookTexture)
			{
			case 0:
				text = "Mods/pastimeReading/textures/variant_A.png";
				break;
			case 1:
				text = "Mods/pastimeReading/textures/variant_B.png";
				break;
			case 2:
				text = "Mods/pastimeReading/textures/variant_C.png";
				break;
			case 3:
				text = "Mods/pastimeReading/textures/variant_D.png";
				break;
			}
			if (text != null)
			{
				Texture2D texture2D = new Texture2D(2, 2);
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				ReadMain.hands.transform.Find("readingBook").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture = texture2D;
			}
			else
			{
				MelonLogger.Log(ConsoleColor.Red, "Book texture is missing");
			}
			ReadMain.p1Text.fontSize = (float)Settings.options.fontSize;
			ReadMain.p2Text.fontSize = (float)Settings.options.fontSize;
			ReadMain.h1Text.fontSize = (float)Settings.options.fontSize;
			ReadMain.h2Text.fontSize = (float)Settings.options.fontSize;
			if (Settings.options.currentPage < ReadMain.p1Text.textInfo.pageCount)
			{
				PageManager.currentPage = Settings.options.currentPage;
			}
			else if (ReadMain.p1Text.textInfo.pageCount % 2 != 0)
			{
				PageManager.currentPage = ReadMain.p1Text.textInfo.pageCount;
			}
			else
			{
				PageManager.currentPage = ReadMain.p1Text.textInfo.pageCount - 1;
			}
			ReadMain.p1Text.pageToDisplay = PageManager.currentPage;
			ReadMain.p2Text.pageToDisplay = PageManager.currentPage + 1;
			ReadMain.p1PageText.text = PageManager.currentPage.ToString();
			ReadMain.p2PageText.text = (PageManager.currentPage + 1).ToString();
			if (Settings.options.disableCameras)
			{
				PageManager.BonkCameras();
			}
			else
			{
				ReadMain.cCam.GetComponent<Camera>().enabled = true;
				ReadMain.pCam.GetComponent<Camera>().enabled = true;
				ReadMain.hCam.GetComponent<Camera>().enabled = true;
			}
			base.OnConfirm();
		}

		// Token: 0x04000032 RID: 50
		[Section("General settings")]
		[Name("Current page")]
		[Description("Current page. Use to quickly navigate book.")]
		[Slider(1f, 899f, 450)]
		public int currentPage = 1;

		// Token: 0x04000033 RID: 51
		[Name("Font size")]
		[Description("Font size of pages. Default: 14")]
		[Slider(14f, 24f)]
		public int fontSize = 14;

		// Token: 0x04000034 RID: 52
		[Name("Keybinding")]
		[Description("The key you press to open the book.")]
		public KeyCode openKeyCode = 53;

		// Token: 0x04000035 RID: 53
		[Section("Visuals")]
		[Name("Hands variant")]
		[Description("Idk how to check which character you are playing, so set it here manually")]
		[Choice(new string[]
		{
			"Astrid",
			"Will"
		})]
		public int handsVariant;

		// Token: 0x04000036 RID: 54
		[Name("Book texture")]
		[Description("Book appearance (you can modify textures yourself in ...Mods/pastimeReading/textures)")]
		[Choice(new string[]
		{
			"variant_A",
			"variant_B",
			"variant_C",
			"variant_D"
		})]
		public int bookTexture;

		// Token: 0x04000037 RID: 55
		[Section("Performance?")]
		[Name("Only enable utility cameras when turning pages")]
		[Description("Enable this if you have some performance issues (you shouldn't), it will update text on pages only when needed, but it might spike a little when turning pages, idk.")]
		public bool disableCameras;

		// Token: 0x04000038 RID: 56
		[Section("Reload")]
		[Name("Reload book from text file?")]
		[Description("Check this to reload book from file when pressing CONFIRM")]
		public bool reloadBook;
	}
}
