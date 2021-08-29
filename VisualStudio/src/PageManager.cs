using System;
using System.IO;
using UnityEngine;
using MelonLoader;
using TMPro;
using System.Collections.Generic;

namespace PastimeReading
{
    public static class PageManager
    {
        public static string currentTurn = null; //current page turn animation: next or prev
        public static int currentPage;
        public static float currentFontSize;
        public static int currentBookTexture = Settings.options.bookTexture;

        public static string bookFileName = "book.txt";

        // idle randomizer values
        private static float a = 0.5f;
        private static float f = 0.0f;
        private static int i = 0;
        private static float o;

        private static int animatorSifter = 0;

        // text split
        public static string bookTitle;
        public static string bookAuthor;
        public static string bookContents;

        public static string[] splitPages;

        private static int firstChar;
        private static int lastChar;

        // misc
        public static Color bookTitleColor = Color.white;
        public static Color bookAuthorColor = Color.white;

        private static string emptyBookPlaceholder = 
            "All work and no play makes Waltz a dull boy.\n" +
            "All work and no play makes Waltz a dull boy.\n " +
            "All work and no play mmakes Waltz a dull boy.\n" +
            "v All work and no PLay ma es Waltz a dull boy.\n" +
            "All work and no play makes Waltz a dull boy.\n" +
            "All work and no play makes Waltz a dull boy.\n" +
            "All work and no ply maKes Waltz a dull boy.\n" +
            "All work and no pllay makes Waltz a dull boy.\n" +
            "All work and no play makes Walt z dyll boy.\n" +
            "All work and no play makes Waltz a dull boy.\n" +
            "All work and no play makes Waltz a dull boy.\n" +
            "All work and no play makes Waltz a dullboy.\n" +
            "All work and no play makes Waltz a dull boy.\n" +
            "All work and NO play makes WALz dull boy.\n" +
            "all work and no play makes Waltz a dull boy.\n" +
            "All work and no plaay makes Waltz a dull boy.\n" +
            "All work and no play makes Waltz a dull bog.\n" +
            "A111 work and no play makes Waltz a dull bot.\n" +
            "All work and noplay makes Watz a dull boy.\n" +
            "All work and no play makes Waltz a dull boy.\n" +
            "All work and no play makes Waltz a dull boy.\n" +
            "All work and no ply maKes Waltz a dyll boy.\n" +
            "All work and no play makes Waltz a dull boy.";


        public static void InitPages(string stage)
		{
            if (stage == "read") // reads from file into variables
            {
                // load book text
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

                if (PageManager.bookContents == null || PageManager.bookContents.Length <= 1)
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "Book content is empty!");
                    PageManager.bookContents = PageManager.emptyBookPlaceholder;
                }

            }

            if (stage == "font") // sets up font and check for odd page, should run before "split"
            {
                ReadMain.p1Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.p2Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.h1Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.h2Text.fontSize = (float)Settings.options.fontSize;

                PageManager.currentFontSize = (float)Settings.options.fontSize;

            }

            if (stage == "split") // calculates pages and splits text from variable
            {
                ReadMain.p1Text.text = PageManager.bookContents; // temporarily set to calculate pages via TMP

                ReadMain.p1Text.ForceMeshUpdate(true); // this is what's taking so long to load texts, without it can't calculate pages correctly

                PageManager.splitPages = new string[ReadMain.p1Text.textInfo.pageCount];

                for (int i = 0; i < ReadMain.p1Text.textInfo.pageCount; i++)
                {
                    PageManager.firstChar = ReadMain.p1Text.textInfo.pageInfo[i].firstCharacterIndex;
                    PageManager.lastChar = ReadMain.p1Text.textInfo.pageInfo[i].lastCharacterIndex;

                    if (lastChar > firstChar)
                    {
                        PageManager.splitPages[i] = PageManager.bookContents.Substring(firstChar, lastChar - firstChar + 1);
                    }
                    else
                    {
                        PageManager.splitPages[i] = PageManager.bookContents.Substring(firstChar);
                    }
                }
            }

            if (stage == "page") // sets current page, should run after "split"
            {
                if (Settings.options.currentPage < PageManager.splitPages.Length)
                {
                    PageManager.currentPage = Settings.options.currentPage;
                }
                else if (PageManager.splitPages.Length % 2 != 0) // currentPage should be odd
                {
                    PageManager.currentPage = PageManager.splitPages.Length;
                }
                else
                {
                    PageManager.currentPage = PageManager.splitPages.Length - 1;
                }
            }

            if (stage == "setup")
            {
                PageManager.TurnpageVisible(false);

                // define text field content
                ReadMain.titleText.text = PageManager.bookTitle;
                ReadMain.authorText.text = PageManager.bookAuthor;

                ReadMain.titleText.color = PageManager.bookTitleColor;
                ReadMain.authorText.color = PageManager.bookAuthorColor;

                PageManager.SetPage("p1", PageManager.currentPage);
                if (PageManager.splitPages.Length > 1)
                {
                    PageManager.SetPage("p2", PageManager.currentPage + 1);
                }
            }
        }


        public static void SetPage(string page, int num)
        {
            if (page == "p1")
            {
                if (num <= PageManager.splitPages.Length)
                {
                    ReadMain.p1Text.text = PageManager.splitPages[num - 1];
                }
                else
                {
                    ReadMain.p1Text.text = null;
                }
                
                ReadMain.p1PageText.text = num.ToString();
            }
            if (page == "p2")
            {
                if (num <= PageManager.splitPages.Length)
                {
                    ReadMain.p2Text.text = PageManager.splitPages[num - 1];
                }
                else
                {
                    ReadMain.p2Text.text = null;
                }
                ReadMain.p2PageText.text = num.ToString();
            }
            if (page == "h1")
            {
                if (num <= PageManager.splitPages.Length)
                {
                    ReadMain.h1Text.text = PageManager.splitPages[num - 1];
                }
                else
                {
                    ReadMain.h1Text.text = null;
                }
                ReadMain.h1PageText.text = num.ToString();
            }
            if (page == "h2")
            {
                if (num <= PageManager.splitPages.Length)
                {
                    ReadMain.h2Text.text = PageManager.splitPages[num - 1];
                }
                else
                {
                    ReadMain.h2Text.text = null;
                }
                ReadMain.h2PageText.text = num.ToString();
            }

        }

        // Idle randomizer
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
				PageManager.a += ((PageManager.i == 0) ? 0.03f : -0.03f); // add if i is 0, otherwise substract
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

			PageManager.i = UnityEngine.Random.Range(0, 2);
			PageManager.f = 0f;

			if ((double)UnityEngine.Random.value <= 0.001) // random idle variation
            {
				ReadMain.handsAnim.SetTrigger("scratch_ear"); 
			}
		}

        // Fire PageFlip() at specified points of animator state
        public static void AnimatorStateSifter(string state, float clipTimeStart, float clipTimeEnd)
		{
			float animTime = ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
			if (ReadMain.handsAnim.GetCurrentAnimatorStateInfo(0).IsName(state))
			{
				if (PageManager.animatorSifter == 0) // state enter
                {
					PageManager.animatorSifter = 1;
					PageManager.PageFlip("firstFrame");
				} 
				if (animTime > clipTimeStart && PageManager.animatorSifter < 2) // state clipTimeStart % in
                {
					PageManager.animatorSifter = 2;
					PageManager.PageFlip("nearFirstFrame");
				}
				if (animTime > clipTimeEnd && PageManager.animatorSifter < 3) // state clipTimeEnd % in
                {
					PageManager.animatorSifter = 3;
					PageManager.PageFlip("nearLastFrame");
					return;
				}
			}
			else if (PageManager.animatorSifter != 0) // state exit
            {
				PageManager.animatorSifter = 0;
				PageManager.PageFlip("lastFrame");
				PageManager.currentTurn = null;
				return;
			}
		}

		private static void TurnpageVisible(bool state)
		{
			ReadMain.turnpage.SetActive(state);
			ReadMain.h1.SetActive(state);
			ReadMain.h2.SetActive(state);
		}

        // Manage text and visibility of pages
        public static void PageFlip(string aEvent)
		{

            if (aEvent == "firstFrame") // start of animation
            {
				PageManager.TurnpageVisible(true);
				if (PageManager.currentTurn == "next")
				{
					ReadMain.p2.SetActive(false);
                    PageManager.SetPage("h1", PageManager.currentPage + 2);
                    PageManager.SetPage("h2", PageManager.currentPage + 1);
                    PageManager.SetPage("p2", PageManager.currentPage + 3);
				}
				if (PageManager.currentTurn == "prev")
				{
					ReadMain.p1.SetActive(false);
                    PageManager.SetPage("h1", PageManager.currentPage);
                    PageManager.SetPage("h2", PageManager.currentPage - 1);
                    PageManager.SetPage("p1", PageManager.currentPage - 2);
				}
			}
			if (aEvent == "nearFirstFrame") // first mid point
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
			if (aEvent == "nearLastFrame") // second mid point
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
			if (aEvent == "lastFrame") // end of animation
            {
				PageManager.TurnpageVisible(false);
				if (PageManager.currentTurn == "next")
				{
					ReadMain.p1.SetActive(true);
                    PageManager.SetPage("p1", PageManager.currentPage + 2);
                    PageManager.currentPage += 2;
				}
				if (PageManager.currentTurn == "prev")
				{
					ReadMain.p2.SetActive(true);
                    PageManager.SetPage("p2", PageManager.currentPage - 1);
                    PageManager.currentPage -= 2;
				}
				Settings.options.currentPage = PageManager.currentPage;
				Settings.options.Save();
			}
		}
	}
}
