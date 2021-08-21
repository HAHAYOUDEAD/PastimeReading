using System;
using System.IO;
using UnityEngine;

namespace PastimeReading
{
	// Token: 0x02000002 RID: 2
	public static class PageManager
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static void InitPages()
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
			ReadMain.p1Text.fontSize = (float)Settings.options.fontSize;
			ReadMain.p2Text.fontSize = (float)Settings.options.fontSize;
			ReadMain.h1Text.fontSize = (float)Settings.options.fontSize;
			ReadMain.h2Text.fontSize = (float)Settings.options.fontSize;
			PageManager.currentPage = Settings.options.currentPage;
			ReadMain.p1Text.pageToDisplay = PageManager.currentPage;
			ReadMain.p2Text.pageToDisplay = PageManager.currentPage + 1;
			ReadMain.p1PageText.text = PageManager.currentPage.ToString();
			ReadMain.p2PageText.text = (PageManager.currentPage + 1).ToString();
			PageManager.TurnpageVisible(false);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000021C8 File Offset: 0x000003C8
		public static void IdleFluc()
		{
			PageManager.o += Time.deltaTime;
			if (PageManager.o <= 0.1f)
			{
				return;
			}
			PageManager.o = 0f;
			if (PageManager.f < 1f)
			{
				PageManager.a += ((PageManager.i == 0) ? 0.03f : -0.03f);
				if (PageManager.a > 0.9f)
				{
					PageManager.i = 1;
				}
				if (PageManager.a < 0.1f)
				{
					PageManager.i = 0;
				}
				ReadMain.handsAnim.SetFloat("idle_random", PageManager.a);
				PageManager.f += 0.1f;
				return;
			}
			PageManager.i = Random.Range(0, 2);
			PageManager.f = 0f;
			if ((double)Random.value <= 0.001)
			{
				ReadMain.handsAnim.SetTrigger("scratch_ear");
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000022A8 File Offset: 0x000004A8
		public static void AnimatorStateSifter(string state, float clipTimeStart, float clipTimeEnd)
		{
			float num = ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
			if (ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName(state))
			{
				if (PageManager.animatorSifter == 0)
				{
					PageManager.animatorSifter = 1;
					PageManager.PageFlip("firstFrame");
				}
				if (num > clipTimeStart && PageManager.animatorSifter < 2)
				{
					PageManager.animatorSifter = 2;
					PageManager.PageFlip("nearFirstFrame");
				}
				if (num > clipTimeEnd && PageManager.animatorSifter < 3)
				{
					PageManager.animatorSifter = 3;
					PageManager.PageFlip("nearLastFrame");
					return;
				}
			}
			else if (PageManager.animatorSifter != 0)
			{
				PageManager.animatorSifter = 0;
				PageManager.PageFlip("lastFrame");
				PageManager.currentTurn = null;
				return;
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002353 File Offset: 0x00000553
		public static void BonkCameras()
		{
			ReadMain.cCam.GetComponent<Camera>().enabled = true;
			ReadMain.pCam.GetComponent<Camera>().enabled = true;
			ReadMain.hCam.GetComponent<Camera>().enabled = true;
			ReadMain.bonkCameras = true;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000238B File Offset: 0x0000058B
		private static void TurnpageVisible(bool state)
		{
			ReadMain.turnpage.SetActive(state);
			ReadMain.h1.SetActive(state);
			ReadMain.h2.SetActive(state);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000023B0 File Offset: 0x000005B0
		public static void PageFlip(string aEvent)
		{
			if (aEvent == "firstFrame")
			{
				PageManager.TurnpageVisible(true);
				if (PageManager.currentTurn == "next")
				{
					ReadMain.p2.SetActive(false);
					ReadMain.h1Text.pageToDisplay = PageManager.currentPage + 2;
					ReadMain.h2Text.pageToDisplay = PageManager.currentPage + 1;
					ReadMain.h1PageText.text = (PageManager.currentPage + 2).ToString();
					ReadMain.h2PageText.text = (PageManager.currentPage + 1).ToString();
					ReadMain.p2Text.pageToDisplay = PageManager.currentPage + 3;
					ReadMain.p2PageText.text = (PageManager.currentPage + 3).ToString();
				}
				if (PageManager.currentTurn == "prev")
				{
					ReadMain.p1.SetActive(false);
					ReadMain.h1Text.pageToDisplay = PageManager.currentPage;
					ReadMain.h2Text.pageToDisplay = PageManager.currentPage - 1;
					ReadMain.h1PageText.text = PageManager.currentPage.ToString();
					ReadMain.h2PageText.text = (PageManager.currentPage - 1).ToString();
					ReadMain.p1Text.pageToDisplay = PageManager.currentPage - 2;
					ReadMain.p1PageText.text = (PageManager.currentPage - 2).ToString();
				}
				if (Settings.options.disableCameras)
				{
					PageManager.BonkCameras();
				}
			}
			if (aEvent == "nearFirstFrame")
			{
				if (PageManager.currentTurn == "next")
				{
					ReadMain.p2.SetActive(true);
				}
				if (PageManager.currentTurn == "prev")
				{
					ReadMain.p1.SetActive(true);
				}
			}
			if (aEvent == "nearLastFrame")
			{
				if (PageManager.currentTurn == "next")
				{
					ReadMain.p1.SetActive(false);
				}
				if (PageManager.currentTurn == "prev")
				{
					ReadMain.p2.SetActive(false);
				}
			}
			if (aEvent == "lastFrame")
			{
				PageManager.TurnpageVisible(false);
				if (PageManager.currentTurn == "next")
				{
					ReadMain.p1.SetActive(true);
					ReadMain.p1Text.pageToDisplay = PageManager.currentPage + 2;
					ReadMain.p1PageText.text = (PageManager.currentPage + 2).ToString();
					PageManager.currentPage += 2;
				}
				if (PageManager.currentTurn == "prev")
				{
					ReadMain.p2.SetActive(true);
					ReadMain.p2Text.pageToDisplay = PageManager.currentPage - 1;
					ReadMain.p2PageText.text = (PageManager.currentPage - 1).ToString();
					PageManager.currentPage -= 2;
				}
				Settings.options.currentPage = PageManager.currentPage;
				if (Settings.options.disableCameras)
				{
					PageManager.BonkCameras();
				}
				Settings.options.Save();
			}
		}

		// Token: 0x04000001 RID: 1
		public static string currentTurn = null;

		// Token: 0x04000002 RID: 2
		public static int currentPage;

		// Token: 0x04000003 RID: 3
		public static string bookFileName = "book.txt";

		// Token: 0x04000004 RID: 4
		private static float a = 0.5f;

		// Token: 0x04000005 RID: 5
		private static float f = 0f;

		// Token: 0x04000006 RID: 6
		private static int i = 0;

		// Token: 0x04000007 RID: 7
		private static float o;

		// Token: 0x04000008 RID: 8
		private static int animatorSifter = 0;

		// Token: 0x04000009 RID: 9
		public static string bookTitle;

		// Token: 0x0400000A RID: 10
		public static string bookAuthor;

		// Token: 0x0400000B RID: 11
		public static string bookContents;
	}
}
