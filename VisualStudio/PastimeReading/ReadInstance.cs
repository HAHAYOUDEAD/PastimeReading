using System;
using System.IO;
using MelonLoader;
using TMPro;
using UnityEngine;

namespace PastimeReading
{
	// Token: 0x02000004 RID: 4
	public static class ReadInstance
	{
		// Token: 0x0600000A RID: 10 RVA: 0x00002740 File Offset: 0x00000940
		public static void ReadInstanceLoad()
		{
			ReadMain.handsAsset = ReadMain.loadBundle.LoadAsset<GameObject>("hands_with_book");
			ReadMain.pCamAsset = ReadMain.loadBundle.LoadAsset<GameObject>("p1-2Cam");
			ReadMain.hCamAsset = ReadMain.loadBundle.LoadAsset<GameObject>("h1-2Cam");
			ReadMain.cCamAsset = ReadMain.loadBundle.LoadAsset<GameObject>("coverCam");
			ReadMain.hands = Object.Instantiate<GameObject>(ReadMain.handsAsset);
			ReadMain.pCam = Object.Instantiate<GameObject>(ReadMain.pCamAsset);
			ReadMain.hCam = Object.Instantiate<GameObject>(ReadMain.hCamAsset);
			ReadMain.cCam = Object.Instantiate<GameObject>(ReadMain.cCamAsset);
			ReadMain.handsFMesh = ReadMain.hands.transform.Find("readingArmsF").gameObject;
			ReadMain.handsMMesh = ReadMain.hands.transform.Find("readingArmsM").gameObject;
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
			ReadMain.handsAnim = ReadMain.hands.GetComponent<Animator>();
			foreach (Transform transform in ReadMain.hands.GetComponentsInChildren<Transform>())
			{
				transform.gameObject.layer = ReadInstance.renderLayer;
			}
			Transform transform2 = ReadMain.hands.transform;
			GameObject gameObject = GameObject.Find("/CHARACTER_FPSPlayer/WeaponView/WeaponCamera");
			transform2.SetParent((gameObject != null) ? gameObject.transform : null);
			ReadMain.cCam.transform.SetParent(ReadMain.hands.transform);
			ReadMain.pCam.transform.SetParent(ReadMain.hands.transform);
			ReadMain.hCam.transform.SetParent(ReadMain.hands.transform);
			ReadMain.turnpage = ReadMain.hands.transform.Find("readingBook_turnpage").gameObject;
			ReadMain.p1 = ReadMain.hands.transform.Find("readingBook_textField_p1").gameObject;
			ReadMain.p2 = ReadMain.hands.transform.Find("readingBook_textField_p2").gameObject;
			ReadMain.h1 = ReadMain.hands.transform.Find("readingBook_textField_h1").gameObject;
			ReadMain.h2 = ReadMain.hands.transform.Find("readingBook_textField_h2").gameObject;
			ReadMain.titleText = ReadMain.cCam.transform.GetChild(0).GetComponent<TMP_Text>();
			ReadMain.authorText = ReadMain.cCam.transform.GetChild(1).GetComponent<TMP_Text>();
			ReadMain.p1Text = ReadMain.pCam.transform.GetChild(0).GetComponent<TMP_Text>();
			ReadMain.p2Text = ReadMain.pCam.transform.GetChild(1).GetComponent<TMP_Text>();
			ReadMain.p1PageText = ReadMain.pCam.transform.GetChild(2).GetComponent<TMP_Text>();
			ReadMain.p2PageText = ReadMain.pCam.transform.GetChild(3).GetComponent<TMP_Text>();
			ReadMain.h1Text = ReadMain.hCam.transform.GetChild(0).GetComponent<TMP_Text>();
			ReadMain.h2Text = ReadMain.hCam.transform.GetChild(1).GetComponent<TMP_Text>();
			ReadMain.h1PageText = ReadMain.hCam.transform.GetChild(2).GetComponent<TMP_Text>();
			ReadMain.h2PageText = ReadMain.hCam.transform.GetChild(3).GetComponent<TMP_Text>();
		}

		// Token: 0x0400000E RID: 14
		private static readonly int renderLayer = LayerMask.NameToLayer("Weapon");
	}
}
