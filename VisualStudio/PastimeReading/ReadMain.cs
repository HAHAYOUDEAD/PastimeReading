using System;
using System.IO;
using MelonLoader;
using TMPro;
using UnityEngine;

namespace PastimeReading
{
	// Token: 0x02000005 RID: 5
	public class ReadMain : MelonMod
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00002B24 File Offset: 0x00000D24
		public override void OnApplicationStart()
		{
			ReadMain.loadBundle = AssetBundle.LoadFromFile("Mods/pastimeReading/pastimeReadingAssets.ass");
			if (ReadMain.loadBundle == null)
			{
				MelonLogger.Log("Failed to load AssetBundle");
				return;
			}
			ReadMain.modsPath = Path.GetFullPath(typeof(MelonMod).Assembly.Location + "\\..\\..\\Mods");
			Settings.OnLoad();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002B88 File Offset: 0x00000D88
		public override void OnLevelWasInitialized(int level)
		{
			if (level >= 6 && ReadMain.hands == null)
			{
				if (!SoundManager.initDone)
				{
					SoundManager.InitSounds();
				}
				ReadInstance.ReadInstanceLoad();
				PageManager.InitPages();
				ReadMain.loadVanillaHands = true;
				if (Settings.options.handsVariant == 0)
				{
					ReadMain.handsMMesh.SetActive(false);
				}
				else if (Settings.options.handsVariant == 1)
				{
					ReadMain.handsFMesh.SetActive(false);
				}
				if (Settings.options.disableCameras)
				{
					ReadMain.cCam.GetComponent<Camera>().enabled = false;
					ReadMain.pCam.GetComponent<Camera>().enabled = false;
					ReadMain.hCam.GetComponent<Camera>().enabled = false;
				}
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002C38 File Offset: 0x00000E38
		private static void StopOnBookClose()
		{
			if (ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName("close_book"))
			{
				ReadMain.bookClosing = true;
				return;
			}
			if (ReadMain.bookClosing)
			{
				ReadMain.usingBook = false;
				ReadMain.bookClosing = false;
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002C7C File Offset: 0x00000E7C
		public override void OnUpdate()
		{
			if (ReadMain.loadVanillaHands)
			{
				if (ReadMain.vanillaHandsF == null || ReadMain.vanillaHandsM == null)
				{
					GameObject gameObject = GameObject.Find("CHARACTER_FPSPlayer/NEW_FPHand_Rig/GAME_DATA/Meshes");
					ReadMain.vanillaHandsF = gameObject.transform.FindChild("Astrid_Arms_NoRing").gameObject;
					ReadMain.vanillaHandsM = gameObject.transform.FindChild("Will_Hands").gameObject;
				}
				else
				{
					ReadMain.handsFMesh.GetComponent<SkinnedMeshRenderer>().material.mainTexture = ReadMain.vanillaHandsF.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture;
					ReadMain.handsMMesh.GetComponent<SkinnedMeshRenderer>().material.mainTexture = ReadMain.vanillaHandsM.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture;
					ReadMain.loadVanillaHands = false;
				}
			}
			if (ReadMain.bonkCameras)
			{
				ReadMain.cCam.GetComponent<Camera>().enabled = false;
				ReadMain.pCam.GetComponent<Camera>().enabled = false;
				ReadMain.hCam.GetComponent<Camera>().enabled = false;
				ReadMain.bonkCameras = false;
			}
			bool keyDown = InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.openKeyCode);
			bool flag = string.IsNullOrEmpty(GameManager.m_ActiveScene) || GameManager.GetPlayerManagerComponent() == null || InterfaceManager.IsOverlayActiveCached() || GameManager.GetPlayerManagerComponent().IsInPlacementMode() || GameManager.GetPlayerManagerComponent().m_ItemInHands != null;
			if (keyDown && !flag)
			{
				if (ReadMain.currentState == "title")
				{
					ReadMain.handsAnim.SetTrigger("open_book");
					ReadMain.currentState = "open";
				}
				if (ReadMain.currentState == "pocket")
				{
					if (Settings.options.disableCameras)
					{
						PageManager.BonkCameras();
					}
					ReadMain.usingBook = true;
					ReadMain.handsAnim.SetTrigger("bring_book");
					ReadMain.currentState = "title";
				}
			}
			if (ReadMain.usingBook)
			{
				float num = GameObject.Find("/CHARACTER_FPSPlayer/WeaponView/WeaponCamera").transform.rotation.eulerAngles.x;
				if (num > 90f)
				{
					num = 0f;
				}
				float num2 = (10f / Mathf.Pow(1f + num / 55f, 1.7f) - 23f) / 100f;
				num = 40f / Mathf.Pow(1f + num / 45f, 2f) - 40f;
				ReadMain.hands.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
				ReadMain.hands.transform.localPosition = new Vector3(0f, num2, 0.05f);
				if (PageManager.currentTurn == "next")
				{
					PageManager.AnimatorStateSifter("next_page", 0.6f, 0.75f);
				}
				if (PageManager.currentTurn == "prev")
				{
					PageManager.AnimatorStateSifter("prev_page", 0.35f, 0.65f);
				}
				if (ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName("book_open_idle") || ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName("book_title_idle"))
				{
					PageManager.IdleFluc();
				}
				SoundManager.AnimatorStateDJ("book_open_idle_4", "playSound_scratch");
				SoundManager.AnimatorStateDJ("open_book", "playSound_bookOpen");
				SoundManager.AnimatorStateDJ("close_book", "playSound_bookClose");
				SoundManager.AnimatorStateDJ("bring_book", "playSound_bookBring");
				SoundManager.AnimatorStateDJ("next_page", "playSound_pageNext");
				SoundManager.AnimatorStateDJ("prev_page", "playSound_pagePrev");
				ReadMain.StopOnBookClose();
				if (InputManager.GetRotateClockwiseHeld(InputManager.m_CurrentContext) && !flag)
				{
					if (ReadMain.currentState != "open")
					{
						return;
					}
					if (PageManager.currentPage + 1 >= ReadMain.p1Text.textInfo.pageCount)
					{
						return;
					}
					if (PageManager.currentTurn != null)
					{
						return;
					}
					PageManager.currentTurn = "next";
					ReadMain.handsAnim.SetTrigger("next_page");
				}
				if (InputManager.GetRotateCounterClockwiseHeld(InputManager.m_CurrentContext) && !flag)
				{
					if (ReadMain.currentState != "open")
					{
						return;
					}
					if (PageManager.currentPage - 1 < 1)
					{
						return;
					}
					if (PageManager.currentTurn != null)
					{
						return;
					}
					PageManager.currentTurn = "prev";
					ReadMain.handsAnim.SetTrigger("prev_page");
				}
				if (InputManager.GetHolsterPressed(InputManager.m_CurrentContext) || flag)
				{
					if (ReadMain.currentState == "title")
					{
						ReadMain.handsAnim.SetTrigger("remove_book");
						ReadMain.currentState = "pocket";
					}
					if (ReadMain.currentState == "open")
					{
						ReadMain.handsAnim.SetTrigger("close_book");
						ReadMain.currentState = "pocket";
					}
				}
			}
		}

		// Token: 0x0400000F RID: 15
		public static AssetBundle loadBundle;

		// Token: 0x04000010 RID: 16
		public static GameObject handsAsset;

		// Token: 0x04000011 RID: 17
		public static GameObject hands = null;

		// Token: 0x04000012 RID: 18
		public static Animator handsAnim;

		// Token: 0x04000013 RID: 19
		public static GameObject vanillaHandsF = null;

		// Token: 0x04000014 RID: 20
		public static GameObject vanillaHandsM = null;

		// Token: 0x04000015 RID: 21
		public static GameObject handsFMesh;

		// Token: 0x04000016 RID: 22
		public static GameObject handsMMesh;

		// Token: 0x04000017 RID: 23
		private static bool loadVanillaHands = false;

		// Token: 0x04000018 RID: 24
		public static GameObject pCamAsset;

		// Token: 0x04000019 RID: 25
		public static GameObject hCamAsset;

		// Token: 0x0400001A RID: 26
		public static GameObject cCamAsset;

		// Token: 0x0400001B RID: 27
		public static GameObject pCam = null;

		// Token: 0x0400001C RID: 28
		public static GameObject hCam = null;

		// Token: 0x0400001D RID: 29
		public static GameObject cCam = null;

		// Token: 0x0400001E RID: 30
		public static GameObject turnpage;

		// Token: 0x0400001F RID: 31
		public static GameObject p1;

		// Token: 0x04000020 RID: 32
		public static GameObject p2;

		// Token: 0x04000021 RID: 33
		public static GameObject h1;

		// Token: 0x04000022 RID: 34
		public static GameObject h2;

		// Token: 0x04000023 RID: 35
		public static TMP_Text titleText;

		// Token: 0x04000024 RID: 36
		public static TMP_Text authorText;

		// Token: 0x04000025 RID: 37
		public static TMP_Text p1Text;

		// Token: 0x04000026 RID: 38
		public static TMP_Text p2Text;

		// Token: 0x04000027 RID: 39
		public static TMP_Text h1Text;

		// Token: 0x04000028 RID: 40
		public static TMP_Text h2Text;

		// Token: 0x04000029 RID: 41
		public static TMP_Text p1PageText;

		// Token: 0x0400002A RID: 42
		public static TMP_Text p2PageText;

		// Token: 0x0400002B RID: 43
		public static TMP_Text h1PageText;

		// Token: 0x0400002C RID: 44
		public static TMP_Text h2PageText;

		// Token: 0x0400002D RID: 45
		public static string modsPath;

		// Token: 0x0400002E RID: 46
		private static bool usingBook = false;

		// Token: 0x0400002F RID: 47
		private static bool bookClosing = false;

		// Token: 0x04000030 RID: 48
		private static string currentState = "pocket";

		// Token: 0x04000031 RID: 49
		public static bool bonkCameras = false;
	}
}
