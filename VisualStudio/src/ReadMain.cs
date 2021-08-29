using System;
using System.IO;
using MelonLoader;
using TMPro;
using UnityEngine;

namespace PastimeReading
{
	public class ReadMain : MelonMod
	{
        // Main assets
        public static AssetBundle loadBundle;
        public static GameObject handsAsset;
        public static GameObject hands = null;
        public static Animator handsAnim;

        // Getting vanilla hands texture
        public static GameObject vanillaHandsF = null;
        public static GameObject vanillaHandsM = null;
        public static GameObject handsFMesh;
        public static GameObject handsMMesh;
        private static bool loadVanillaHands = false;

        // Pages and text
        public static GameObject pCamAsset; //main page camera (cameras needed to transfer text to texture)
        public static GameObject hCamAsset; //turnpage camera
        public static GameObject cCamAsset; //title camera
        public static GameObject pCam = null;
        public static GameObject hCam = null;
        public static GameObject cCam = null;

        public static GameObject turnpage;
        public static GameObject p1; //main page left (object that holds text)
        public static GameObject p2; //main page right
        public static GameObject h1; //turnpage right side
        public static GameObject h2; //turnpage left side

        public static TMP_Text titleText; //text fields
        public static TMP_Text authorText;
        public static TMP_Text p1Text;
        public static TMP_Text p2Text;
        public static TMP_Text h1Text;
        public static TMP_Text h2Text;
        public static TMP_Text p1PageText; //page numbers
        public static TMP_Text p2PageText;
        public static TMP_Text h1PageText;
        public static TMP_Text h2PageText;

        // Misc parameters
        public static GameObject weaponCamera; //vanilla camera

        public static string modsPath;

        private static bool usingBook = false;
        private static bool bookClosing = false;
        private static string currentState = "pocket";
        private static string currentCharacter;

        public override void OnApplicationStart()
		{
            // Load main asset
            ReadMain.loadBundle = AssetBundle.LoadFromFile("Mods/pastimeReading/pastimeReadingAssets.ass");
			if (ReadMain.loadBundle == null)
			{
				MelonLogger.Msg("Failed to load AssetBundle");
				return;
			}


            // Get Mods folder path
            ReadMain.modsPath = Path.GetFullPath(typeof(MelonMod).Assembly.Location + "\\..\\..\\Mods");

            // Load settings
            Settings.OnLoad();
		}

		public override void OnSceneWasInitialized(int level, string name)
		{

            if (level >= 6 && ReadMain.hands == null)
			{
                if (!SoundManager.initDone)
				{
					SoundManager.InitSounds();
				}

				ReadInstance.ReadInstanceLoad();

                if (PageManager.bookContents == null)  // if text is not yet loaded
                {
                    PageManager.InitPages("read");
                    PageManager.InitPages("font");
                    PageManager.InitPages("split");
                    PageManager.InitPages("page");
                }


                PageManager.InitPages("setup"); 

                // Allow search for vanilla textures since game is now loaded
                ReadMain.loadVanillaHands = true;
            }
            ReadMain.currentCharacter = null;
            ReadMain.usingBook = false;
        }


        private static void StopOnBookClose()
		{
			if (ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName("close_book") || ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName("remove_book"))
			{
                ReadMain.bookClosing = true;
				return;
			}
			if (ReadMain.bookClosing) //fire when closing animation ended
            {
				ReadMain.usingBook = false;
				ReadMain.bookClosing = false;
            }
		}

		public override void OnUpdate()
		{
            // applying vanilla texture to imported hands
            if (ReadMain.loadVanillaHands)
			{
				if (ReadMain.vanillaHandsF == null || ReadMain.vanillaHandsM == null)
				{
					GameObject meshes = GameObject.Find("CHARACTER_FPSPlayer/NEW_FPHand_Rig/GAME_DATA/Meshes");
					ReadMain.vanillaHandsF = meshes.transform.FindChild("Astrid_Arms_NoRing").gameObject;
					ReadMain.vanillaHandsM = meshes.transform.FindChild("Will_Hands").gameObject;
				}
				else
				{
					ReadMain.handsFMesh.GetComponent<SkinnedMeshRenderer>().material.mainTexture = ReadMain.vanillaHandsF.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture;
					ReadMain.handsMMesh.GetComponent<SkinnedMeshRenderer>().material.mainTexture = ReadMain.vanillaHandsM.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture;
					ReadMain.loadVanillaHands = false;
				}
			}

            

            // defining flags to hide book when needed
            bool keyDown = InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.openKeyCode);
			bool flag = string.IsNullOrEmpty(GameManager.m_ActiveScene) || GameManager.GetPlayerManagerComponent() == null || GameManager.GetPlayerManagerComponent().IsInPlacementMode() || GameManager.GetPlayerManagerComponent().m_ItemInHands != null;

            if (keyDown && !flag && !InterfaceManager.IsOverlayActiveCached() && !ReadMain.bookClosing)
			{
                if (ReadMain.currentState == "title")
				{
                    ReadMain.handsAnim.SetTrigger("open_book");
					ReadMain.currentState = "open";
                }
				if (ReadMain.currentState == "pocket")
				{
                    ReadMain.usingBook = true;
					ReadMain.handsAnim.SetTrigger("bring_book");
					ReadMain.currentState = "title";

                    // fixing FOV
                    ReadMain.weaponCamera.GetComponent<Camera>().fieldOfView = 37.5f;

                    // fixing incorrect text when first time opening
                    //ReadMain.pCam.SetActive(false);
                    //ReadMain.pCam.SetActive(true);
                }
			}

			if (ReadMain.usingBook)
			{
                // switch hands according to current character
                if (GameManager.GetPlayerManagerComponent().m_VoicePersona == VoicePersona.Female && ReadMain.currentCharacter != "Astrid")
                {
                    ReadMain.handsMMesh.SetActive(false);
                    ReadMain.handsFMesh.SetActive(true);
                    ReadMain.currentCharacter = "Astrid";
                }
                else if (GameManager.GetPlayerManagerComponent().m_VoicePersona == VoicePersona.Male && ReadMain.currentCharacter != "Will")
                {
                    ReadMain.handsFMesh.SetActive(false);
                    ReadMain.handsMMesh.SetActive(true);
                    ReadMain.currentCharacter = "Will";
                }

                // tilt book with camera
                float x = ReadMain.weaponCamera.transform.rotation.eulerAngles.x;
                if (x > 90f) // 90 is looking straight down, 0 is straight ahead
                {
                    x = 0f;
                }
                float y = (10f / Mathf.Pow(1f + x / 55f, 1.7f) - 23f) / 100f;
				x = 40f / Mathf.Pow(1f + x / 45f, 2f) - 40f;

				ReadMain.hands.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
				ReadMain.hands.transform.localPosition = new Vector3(0f, y, 0.05f);

                // change page content
                if (PageManager.currentTurn == "next")
				{
					PageManager.AnimatorStateSifter("next_page", 0.6f, 0.75f);
				}
				if (PageManager.currentTurn == "prev")
				{
					PageManager.AnimatorStateSifter("prev_page", 0.35f, 0.65f);
				}

                // make idle animation a bit random
                if (ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName("book_open_idle") || ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName("book_title_idle"))
				{
					PageManager.IdleFluc();
				}

                // disgusting sound management
                SoundManager.AnimatorStateDJ("book_open_idle_4", "playSound_scratch");
				SoundManager.AnimatorStateDJ("open_book", "playSound_bookOpen");
				SoundManager.AnimatorStateDJ("close_book", "playSound_bookClose");
				SoundManager.AnimatorStateDJ("bring_book", "playSound_bookBring");
				SoundManager.AnimatorStateDJ("next_page", "playSound_pageNext");
				SoundManager.AnimatorStateDJ("prev_page", "playSound_pagePrev");

                // disable this section when closing book
                ReadMain.StopOnBookClose();

                // smart listener for key presses
                
                /*
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.L))
                {
                    MelonLogger.Msg("3 " + ReadMain.p1Text.textInfo.pageCount);
                    
                    PageManager.currentPage += 1;
                    ReadMain.p1Text.pageToDisplay = PageManager.currentPage;
                    ReadMain.p2Text.pageToDisplay = PageManager.currentPage + 1;
                }
                */
                if (InputManager.GetRotateClockwiseHeld(InputManager.m_CurrentContext) && !flag)
				{
                    if (ReadMain.currentState != "open")
					{
						return;
					}
                    if (PageManager.currentPage + 1 >= PageManager.splitPages.Length)
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

                

                // interrupt if pulling up any other tool
                if ((GameManager.GetPlayerManagerComponent().m_ItemInHands != null || InputManager.HasInteractedThisFrame() || ReadSettings.settingsChanged) && ReadMain.currentState != "pocket")
                {
                    ReadMain.handsAnim.SetTrigger("remove_book");
                    ReadMain.currentState = "pocket";
                    ReadSettings.settingsChanged = false;
                }

                if (InputManager.GetHolsterPressed(InputManager.m_CurrentContext) || flag)
				{
                    // ignore if menu is opened
                    if (InterfaceManager.IsOverlayActiveCached())
                    {
                        return;
                    }
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
	}
}
