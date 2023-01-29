namespace PastimeReading
{
	public static class ReadInstance
	{
        private static readonly int renderLayer = LayerMask.NameToLayer("Weapon");

        public static readonly Shader vanillaSkinShader = Shader.Find("Shader Forge/TLD_StandardSkinned");
        public static readonly Shader vanillaDefaultShader = Shader.Find("Shader Forge/TLD_StandardDiffuse");

        public static void ReadInstanceLoad()
		{
            // Instantiate assets in scene
            ReadMain.handsAsset = ReadMain.loadBundle.LoadAsset<GameObject>("hands_with_book");
			ReadMain.pCamAsset = ReadMain.loadBundle.LoadAsset<GameObject>("p1-2Cam");
			ReadMain.hCamAsset = ReadMain.loadBundle.LoadAsset<GameObject>("h1-2Cam");
			ReadMain.cCamAsset = ReadMain.loadBundle.LoadAsset<GameObject>("coverCam");

			ReadMain.hands = UnityEngine.Object.Instantiate(ReadMain.handsAsset);
			ReadMain.pCam = UnityEngine.Object.Instantiate(ReadMain.pCamAsset);
			ReadMain.hCam = UnityEngine.Object.Instantiate(ReadMain.hCamAsset);
			ReadMain.cCam = UnityEngine.Object.Instantiate(ReadMain.cCamAsset);

			ReadMain.handsFMesh = ReadMain.hands.transform.Find("readingArmsF").gameObject;
			ReadMain.handsMMesh = ReadMain.hands.transform.Find("readingArmsM").gameObject;

            Utility.Log(CC.Gray, "ReadInstanceLoad - Instantiated");

            // Get vanilla camera
            ReadMain.weaponCamera = GameObject.Find("/CHARACTER_FPSPlayer/WeaponView/WeaponCamera");
            Utility.Log(CC.Gray, "ReadInstanceLoad - Found camera");

            // Assign book texture
            ReadInstance.UpdateBookTexture();
            Utility.Log(CC.Gray, "ReadInstanceLoad - Book texture update");

            // Get Animator
            ReadMain.handsAnim = ReadMain.hands.GetComponent<Animator>();
            Utility.Log(CC.Gray, "ReadInstanceLoad - Found animator");

            // Set object layer ("Weapon" snaps object to player, sets zTest to always)
            foreach (Transform child in ReadMain.hands.GetComponentsInChildren<Transform>())
			{
				child.gameObject.layer = ReadInstance.renderLayer;
			}
            Utility.Log(CC.Gray, "ReadInstanceLoad - Layer set");

            // Set parents
            ReadMain.hands.transform.SetParent((ReadMain.weaponCamera != null) ? ReadMain.weaponCamera.transform : null);

			ReadMain.cCam.transform.SetParent(ReadMain.hands.transform);
			ReadMain.pCam.transform.SetParent(ReadMain.hands.transform);
			ReadMain.hCam.transform.SetParent(ReadMain.hands.transform);
            Utility.Log(CC.Gray, "ReadInstanceLoad - Parented");

            // Get Pages
            ReadMain.turnpage = ReadMain.hands.transform.Find("readingBook_turnpage").gameObject;
			ReadMain.p1 = ReadMain.hands.transform.Find("readingBook_textField_p1").gameObject;
			ReadMain.p2 = ReadMain.hands.transform.Find("readingBook_textField_p2").gameObject;
			ReadMain.h1 = ReadMain.hands.transform.Find("readingBook_textField_h1").gameObject;
			ReadMain.h2 = ReadMain.hands.transform.Find("readingBook_textField_h2").gameObject;

            // Get text fields
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
            Utility.Log(CC.Gray, "ReadInstanceLoad - Book loaded");
        }

        public static void UpdateBookTexture()
        {
            string texPath = null;
            switch (Settings.options.bookTexture)
            {
                case 0:
                    texPath = "Mods/pastimeReading/textures/simpleRed.png";
                    break;
                case 1:
                    texPath = "Mods/pastimeReading/textures/simpleBlue.png";
                    break;
                case 2:
                    texPath = "Mods/pastimeReading/textures/simpleWhite.png";
                    break;
                case 3:
                    texPath = "Mods/pastimeReading/textures/simpleYellow.png";
                    break;
                case 4:
                    texPath = "Mods/pastimeReading/textures/detailGray.png";
                    break;
                case 5:
                    texPath = "Mods/pastimeReading/textures/detailYellow.png";
                    break;
                case 6:
                    texPath = "Mods/pastimeReading/textures/derailBlack.png";
                    break;
            }
            if (File.Exists(texPath))
            {
                Texture2D bookTex = new Texture2D(2, 2);
                GameObject bookMesh = ReadMain.hands.transform.Find("readingBook").gameObject;
                ImageConversion.LoadImage(bookTex, File.ReadAllBytes(texPath));
                bookMesh.GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture = bookTex;
                bookMesh.GetComponent<SkinnedMeshRenderer>().sharedMaterial.shader = vanillaSkinShader;

                PageManager.bookTitleColor = bookTex.GetPixel(1, 0);
                PageManager.bookAuthorColor = bookTex.GetPixel(5, 0);

                PageManager.currentBookTexture = Settings.options.bookTexture;
            }
            else
            {
                MelonLogger.Msg(ConsoleColor.Red, "Book texture is missing");
                Settings.options.bookTexture = PageManager.currentBookTexture;

            }

        }
    }
}
