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

        private static readonly int chunkSize = 100000;
        private static int splitCurrentSymbol = 0;
        private static string chunkContents;

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
            splitCurrentSymbol = 0;

            if (stage == "read") // reads from file into variables
            {
                // load book text
                StreamReader streamReader = new StreamReader(ReadMain.modsPath + "/pastimeReading/" + bookFileName);
                while (!streamReader.EndOfStream)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            bookTitle = streamReader.ReadLine();
                        }
                        if (i == 1)
                        {
                            bookAuthor = streamReader.ReadLine();
                        }
                    }
                    bookContents = streamReader.ReadToEnd();
                    
                }

                if (bookContents == null || bookContents.Length <= 1)
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "Book content is empty!");
                    bookContents = emptyBookPlaceholder;
                }

            }

            if (stage == "font") // sets up font and check for odd page, should run before "split"
            {
                ReadMain.p1Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.p2Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.h1Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.h2Text.fontSize = (float)Settings.options.fontSize;

                currentFontSize = (float)Settings.options.fontSize;

            }

            if (stage == "split") // calculates pages and splits text from variable
            {

                splitPages = new string[0];
                
                while (bookContents.Length > splitCurrentSymbol)
                {
                    bool last = false;

                    if (bookContents.Length < splitCurrentSymbol + chunkSize)
                    {
                        last = true;
                        chunkContents = bookContents.Substring(splitCurrentSymbol);
                    }
                    else
                    {
                        chunkContents = bookContents.Substring(splitCurrentSymbol, chunkSize);
                    }

                    ReadMain.p1Text.text = chunkContents; // temporarily set to calculate pages via TMP
                    ReadMain.p1Text.ForceMeshUpdate(true); 

                    string[] additionChunk = new string[ReadMain.p1Text.textInfo.pageCount];

                    for (int i = 0; i < ReadMain.p1Text.textInfo.pageCount; i++)
                    {
                        firstChar = ReadMain.p1Text.textInfo.pageInfo[i].firstCharacterIndex;
                        lastChar = ReadMain.p1Text.textInfo.pageInfo[i].lastCharacterIndex;
                        if (lastChar > firstChar)
                        {
                            additionChunk[i] = chunkContents.Substring(firstChar, lastChar - firstChar + 1);
                        }
                        else
                        {
                            additionChunk[i] = chunkContents.Substring(firstChar);
                        }
                    }

                    if (!last)
                    {
                        splitCurrentSymbol = splitCurrentSymbol + chunkSize - additionChunk[additionChunk.Length - 1].Length; // remove last page(incomplete) from current iteration
                        Array.Resize(ref additionChunk, additionChunk.Length - 1);
                    }
                    else
                    {
                        splitCurrentSymbol = splitCurrentSymbol + chunkSize + 1;
                    }

                    int position = splitPages.Length; // hold array size before resizing
                    Array.Resize(ref splitPages, position + additionChunk.Length); // resize main array to fit new pages
                    additionChunk.CopyTo(splitPages, position); // add current chunk to main array
                }

                
            }

            if (stage == "page") // sets current page, should run after "split"
            {
                if (Settings.options.currentPage < splitPages.Length)
                {
                    currentPage = Settings.options.currentPage;
                }
                else if (splitPages.Length % 2 != 0) // currentPage should be odd
                {
                    currentPage = splitPages.Length;
                }
                else
                {
                    currentPage = splitPages.Length - 1;
                }
            }

            if (stage == "setup")
            {
                TurnpageVisible(false);

                // define text field content
                ReadMain.titleText.text = bookTitle;
                ReadMain.authorText.text = bookAuthor;

                ReadMain.titleText.color = bookTitleColor;
                ReadMain.authorText.color = bookAuthorColor;

                SetPage("p1", currentPage);
                if (splitPages.Length > 1)
                {
                    SetPage("p2", currentPage + 1);
                }
            }
        }


        public static void SetPage(string page, int num)
        {
            if (page == "p1")
            {
                if (num <= splitPages.Length)
                {
                    ReadMain.p1Text.text = splitPages[num - 1];
                }
                else
                {
                    ReadMain.p1Text.text = null;
                }
                
                ReadMain.p1PageText.text = num.ToString();
            }
            if (page == "p2")
            {
                if (num <= splitPages.Length)
                {
                    ReadMain.p2Text.text = splitPages[num - 1];
                }
                else
                {
                    ReadMain.p2Text.text = null;
                }
                ReadMain.p2PageText.text = num.ToString();
            }
            if (page == "h1")
            {
                if (num <= splitPages.Length)
                {
                    ReadMain.h1Text.text = splitPages[num - 1];
                }
                else
                {
                    ReadMain.h1Text.text = null;
                }
                ReadMain.h1PageText.text = num.ToString();
            }
            if (page == "h2")
            {
                if (num <= splitPages.Length)
                {
                    ReadMain.h2Text.text = splitPages[num - 1];
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
			o += Time.deltaTime;
			if (o <= 0.1f)
			{
				return;
			}
			o = 0f;

			if (f < 1f)
			{
				a += ((i == 0) ? 0.03f : -0.03f); // add if i is 0, otherwise substract
                if (a > 0.9f)
				{
					i = 1;
				}
				if (a < 0.1f)
				{
					i = 0;
				}
				ReadMain.handsAnim.SetFloat("idle_random", a);
				f += 0.1f;
				return;
			}

			i = UnityEngine.Random.Range(0, 2);
			f = 0f;

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
				if (animatorSifter == 0) // state enter
                {
					animatorSifter = 1;
					PageFlip("firstFrame");
				} 
				if (animTime > clipTimeStart && animatorSifter < 2) // state clipTimeStart % in
                {
					animatorSifter = 2;
					PageFlip("nearFirstFrame");
				}
				if (animTime > clipTimeEnd && animatorSifter < 3) // state clipTimeEnd % in
                {
					animatorSifter = 3;
					PageFlip("nearLastFrame");
					return;
				}
			}
			else if (animatorSifter != 0) // state exit
            {
				animatorSifter = 0;
				PageFlip("lastFrame");
				currentTurn = null;
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
				TurnpageVisible(true);
				if (currentTurn == "next")
				{
					ReadMain.p2.SetActive(false);
                    SetPage("h1", currentPage + 2);
                    SetPage("h2", currentPage + 1);
                    SetPage("p2", currentPage + 3);
				}
				if (currentTurn == "prev")
				{
					ReadMain.p1.SetActive(false);
                    SetPage("h1", currentPage);
                    SetPage("h2", currentPage - 1);
                    SetPage("p1", currentPage - 2);
				}
			}
			if (aEvent == "nearFirstFrame") // first mid point
            {
				if (currentTurn == "next")
				{
					ReadMain.p2.SetActive(true);
				}
				if (currentTurn == "prev")
				{
					ReadMain.p1.SetActive(true);
				}
			}
			if (aEvent == "nearLastFrame") // second mid point
            {
				if (currentTurn == "next")
				{
					ReadMain.p1.SetActive(false);
				}
				if (currentTurn == "prev")
				{
					ReadMain.p2.SetActive(false);
				}
			}
			if (aEvent == "lastFrame") // end of animation
            {
				TurnpageVisible(false);
				if (currentTurn == "next")
				{
					ReadMain.p1.SetActive(true);
                    SetPage("p1", currentPage + 2);
                    currentPage += 2;
				}
				if (currentTurn == "prev")
				{
					ReadMain.p2.SetActive(true);
                    SetPage("p2", currentPage - 1);
                    currentPage -= 2;
				}
				Settings.options.currentPage = currentPage;
				Settings.options.Save();
			}
		}
	}
}
