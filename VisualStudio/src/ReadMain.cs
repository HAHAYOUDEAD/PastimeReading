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

        public static bool usingBook = false;
        public static bool bookIsOpened = false;
        private static bool bookClosing = false;
        private static string currentState = "pocket";
        private static string currentCharacter;

        private static float slowdownLerp;
        private static bool interrupted;

        private static bool timescaleReset;

        public static bool lockInteraction;

        public static bool gameStarted;

        public static readonly float iteractionAllowanceAngle = 27f;


        public override void OnApplicationStart()
		{
            // Load main asset
            loadBundle = AssetBundle.LoadFromFile("Mods/pastimeReading/pastimeReadingAssets.ass");
			if (loadBundle == null)
			{
				MelonLogger.Msg(System.ConsoleColor.Yellow, "Failed to load AssetBundle");
				return;
			}

            // Get Mods folder path
            modsPath = Path.GetFullPath(typeof(MelonMod).Assembly.Location + "\\..\\..\\..\\Mods");

            // Load settings
            Settings.OnLoad();
		}

		public override void OnSceneWasInitialized(int level, string name)
		{
            if (Utility.IsScenePlayable() && hands == null)
            {
                if (!SoundManager.initDone)
                {
                    SoundManager.InitSounds();
                    Utility.Log(CC.Gray, "OnSceneWasInitialized - Sounds done");
                }
                ReadInstance.ReadInstanceLoad();
                Utility.Log(CC.Gray, "OnSceneWasInitialized - Instance done");

                PageManager.InitPages("font");
                Utility.Log(CC.Gray, "OnSceneWasInitialized - Font done");
                PageManager.InitPages("rtl");
                Utility.Log(CC.Gray, "OnSceneWasInitialized - RTL done");


                if (PageManager.bookContents == null || ReadSettings.settingsChanged == true)  // if text is not yet loaded or settings were changed in main menu
                {
                    PageManager.InitPages("read");
                    Utility.Log(CC.Gray, "OnSceneWasInitialized - Read done");
                    PageManager.InitPages("split");
                    Utility.Log(CC.Gray, "OnSceneWasInitialized - Split done");
                    PageManager.InitPages("page");
                    Utility.Log(CC.Gray, "OnSceneWasInitialized - Page done");
                    ReadSettings.settingsChanged = false;
                    Settings.options.reloadBook = false;
                }

                PageManager.InitPages("setup");
                Utility.Log(CC.Gray, "OnSceneWasInitialized - Setup done");

                // Allow search for vanilla textures since game is now loaded
                loadVanillaHands = true;
                Utility.Log(CC.Gray, "OnSceneWasInitialized - Done");

                gameStarted = true;
            }
        }

        public override void OnSceneWasUnloaded(int level, string name) // reset the book on scene unload
        {
            currentCharacter = null;
            usingBook = false;
            bookIsOpened = false;
            currentState = "pocket";
            bookClosing = false;
            timescaleReset = true;
        }




        private static void StopOnBookClose()
		{
			if (handsAnim.GetCurrentAnimatorStateInfo(0).IsName("close_book") || handsAnim.GetCurrentAnimatorStateInfo(0).IsName("remove_book") || handsAnim.GetCurrentAnimatorStateInfo(0).IsName("remove_book_from_opened"))
			{
                bookIsOpened = false;
                bookClosing = true;
				return;
			}
			if (bookClosing) //fire when closing animation ended
            {
				usingBook = false;
				bookClosing = false;

            }
		}

		public override void OnUpdate()
		{
            // applying vanilla texture to imported hands
            if (loadVanillaHands)
			{
                if (vanillaHandsF == null || vanillaHandsM == null)
				{
					GameObject meshes = GameObject.Find("CHARACTER_FPSPlayer/NEW_FPHand_Rig/GAME_DATA/Meshes");
					vanillaHandsF = meshes.transform.FindChild("Astrid_Arms_NoRing").gameObject;
					vanillaHandsM = meshes.transform.FindChild("Will_Hands").gameObject;
				}
				else
				{
                    //handsFMesh.GetComponent<SkinnedMeshRenderer>().material = vanillaHandsF.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                    //handsFMesh.GetComponent<SkinnedMeshRenderer>().material = vanillaHandsF.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                    handsFMesh.GetComponent<SkinnedMeshRenderer>().material.shader = ReadInstance.vanillaSkinShader;
                    handsMMesh.GetComponent<SkinnedMeshRenderer>().material.shader = ReadInstance.vanillaSkinShader;


                    handsFMesh.GetComponent<SkinnedMeshRenderer>().material.mainTexture = vanillaHandsF.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture;
                    handsMMesh.GetComponent<SkinnedMeshRenderer>().material.mainTexture = vanillaHandsM.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture;

                    loadVanillaHands = false;
				}
			}

            if (!gameStarted) return;

            // defining flags to hide book when needed
            bool keyDown = InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.openKeyCode);
            bool flagItem = GameManager.GetPlayerManagerComponent().m_ItemInHands != null;
            bool flagGeneral = string.IsNullOrEmpty(GameManager.m_ActiveScene) || GameManager.GetPlayerManagerComponent() == null || GameManager.GetPlayerManagerComponent().IsInPlacementMode();

            if (keyDown && !flagGeneral && !InterfaceManager.IsOverlayActiveCached() && !bookClosing)
			{
                //GameManager.GetVpFPSCamera().UnWieldCurrentWeapon();

                if (currentState == "title")
				{
                    handsAnim.SetTrigger("open_book");
					currentState = "open";
                    bookIsOpened = true;
                    interrupted = false;

                }
				if (currentState == "pocket")
				{
                    usingBook = true;
					handsAnim.SetTrigger("bring_book");
					currentState = "title";

                    // fixing FOV
                    weaponCamera.GetComponent<Camera>().fieldOfView = 37.5f;
                }

            }


            if (usingBook)
            {
                // switch hands according to current character
                if (PlayerManager.m_VoicePersona == VoicePersona.Female && currentCharacter != "Astrid")
                {
                    handsMMesh.SetActive(false);
                    handsFMesh.SetActive(true);
                    currentCharacter = "Astrid";
                }
                else if (PlayerManager.m_VoicePersona == VoicePersona.Male && currentCharacter != "Will")
                {
                    handsFMesh.SetActive(false);
                    handsMMesh.SetActive(true);
                    currentCharacter = "Will";
                }

                // tilt book with camera
                float x = weaponCamera.transform.rotation.eulerAngles.x;
                if (x > 90f) // 90 is looking straight down, 0 is straight ahead
                {
                    x = 0f;
                }

                if (x < iteractionAllowanceAngle) lockInteraction = false;
                else if (bookIsOpened) lockInteraction = true;


                float y = (10f / Mathf.Pow(1f + x / 55f, 1.7f) - 23f) / 100f;

                if (Settings.options.bookTilt == 0)
                {
                    x = 50f / Mathf.Pow(1f + (x - 8f) / 50f, 1.2f) - 50f; // better
                }
                else
                {
                    x = 40f / Mathf.Pow(1f + x / 45f, 2f) - 40f; // classic
                }

                hands.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
                hands.transform.localPosition = new Vector3(0f, y, 0.05f);

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
                if (handsAnim.GetCurrentAnimatorStateInfo(0).IsName("book_open_idle") || handsAnim.GetCurrentAnimatorStateInfo(0).IsName("book_title_idle"))
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
                StopOnBookClose();

                // smart listener for key presses
                if (InputManager.GetRotateClockwiseHeld(InputManager.m_CurrentContext) && !flagItem && !flagGeneral)
                {
                    if (currentState != "open")
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
                    handsAnim.SetTrigger("next_page");
                }
                if (InputManager.GetRotateCounterClockwiseHeld(InputManager.m_CurrentContext) && !flagItem && !flagGeneral)
                {
                    if (currentState != "open")
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
                    handsAnim.SetTrigger("prev_page");
                }

                // interrupt if pulling up any other tool, interacting or in struggle
                if ((GameManager.GetPlayerManagerComponent().m_ItemInHands != null || InputManager.HasInteractedThisFrame() || GameManager.GetPlayerStruggleComponent().InStruggle()) && currentState != "pocket") //  || ReadSettings.settingsChanged
                {
                    handsAnim.SetTrigger("remove_book");
                    currentState = "pocket";
                    interrupted = true;
                }

                if (InputManager.GetHolsterPressed(InputManager.m_CurrentContext) || flagItem || flagGeneral)
                {
                    if (InterfaceManager.IsOverlayActiveCached()) // ignore if menu is opened
                    {
                        return;
                    }

                    if (currentState == "title")
                    {
                        handsAnim.SetTrigger("remove_book");
                        currentState = "pocket";

                    }
                    if (currentState == "open")
                    {
                        handsAnim.SetTrigger("close_book");
                        currentState = "pocket";
                    }
                }
            }
            else // if not using book
            {
                lockInteraction = false;
            }

            if (bookIsOpened)
            {
                if (Settings.options.timeScale != 1f && Time.timeScale != Settings.options.timeScale) 
                {
                    if (Mathf.Approximately(1f, Time.timeScale))
                    {
                        slowdownLerp = 0f;
                    }

                    Time.timeScale = GameManager.m_GlobalTimeScale = Mathf.Lerp(1f, Settings.options.timeScale, slowdownLerp += Time.unscaledDeltaTime / 2f);

                    if (handsAnim.updateMode != AnimatorUpdateMode.UnscaledTime)
                    {
                        handsAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
                        timescaleReset = false;
                    }

                    //MelonLogger.Msg("opened " + Time.timeScale);
                }
            }
            else
            {
                if (timescaleReset) return;

                if (!Mathf.Approximately(1f, Time.timeScale)) 
                {
                    if (Time.timeScale == Settings.options.timeScale)
                    {
                        slowdownLerp = 0f;
                    }

                    float coeff = 2f;

                    if (interrupted) // go faster if interrupted
                    {
                        coeff = 0.3f;
                    }

                    Time.timeScale = GameManager.m_GlobalTimeScale = Mathf.Lerp(Settings.options.timeScale, 1f, slowdownLerp += Time.unscaledDeltaTime / coeff);

                    MelonLogger.Msg("closed " + Time.timeScale); 

                }
                else if (handsAnim.updateMode != AnimatorUpdateMode.Normal) // reset animator to normal state when timescale = 1
                {
                    handsAnim.updateMode = AnimatorUpdateMode.Normal;
                    timescaleReset = true;
                }
            }
		}
	}
}
