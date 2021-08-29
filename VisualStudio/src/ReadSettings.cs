using System;
using System.IO;
using MelonLoader;
using ModSettings;
using UnityEngine;

namespace PastimeReading
{
    internal class ReadSettings : JsonModSettings
    {
        [Section("General settings")]

        [Name("Current page")]
        [Description("Current page. Use to quickly navigate the book.")]
        [Slider(1f, 199f, 100)]
        public int currentPage = 1;

        [Name("Font size")]
        [Description("Font size of pages. Default: 16")]
        [Slider(14f, 24f)]
        public int fontSize = 16;

        [Name("Keybinding")]
        [Description("The key you press to take out the book. Press again to open it. Holster to close.")]
        public KeyCode openKeyCode = KeyCode.Alpha5;

        [Name("Book texture")]
        [Description("Book appearance. \n\n You can modify textures yourself. They should be here ...Mods/pastimeReading/textures/")]
        [Choice(new string[]
        {
            "variant_A",
            "variant_B",
            "variant_C",
            "variant_D"
        })]
        public int bookTexture;

        [Section("Reload")]

        [Name("Reload book from text file?")]
        [Description("Check this to reload book from file when pressing CONFIRM \n\n WARNING! Avoid loading massive books, the game will hang and crash when it runs out of memory. The plugin that does that allows me to accurately split text into pages, without it I wouldn't be able to make this mod, so it's a compromise I guess. \n\n For example, 300kb text file (about 100 pages) takes about 10 seconds to load on my machine(not a beefy one), keep that in mind.")]
        public bool reloadBook;

        public static bool settingsChanged;

        protected override void OnConfirm()
		{
            ReadSettings.settingsChanged = true;
            // book reload
            if (Settings.options.reloadBook)
			{
                PageManager.InitPages("read");
                PageManager.InitPages("split");
                Settings.options.currentPage = 1;
                PageManager.InitPages("setup");
                Settings.options.reloadBook = false;
            }

            // book texture

            if (PageManager.currentBookTexture != Settings.options.bookTexture)
            {
                ReadInstance.UpdateBookTexture();
                PageManager.InitPages("setup");
            }

            // change page

            if (PageManager.currentPage != Settings.options.currentPage)
            {
                PageManager.currentPage = Settings.options.currentPage;
                PageManager.InitPages("page");
                PageManager.InitPages("setup");
            }


            // font change
            if (PageManager.currentFontSize != (float)Settings.options.fontSize)
            {
                PageManager.InitPages("font");
                PageManager.InitPages("split");
                PageManager.InitPages("page");
                PageManager.InitPages("setup");
            }

            base.OnConfirm();
		}
	}
}
