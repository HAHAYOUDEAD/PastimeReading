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
        public static int currentAlignment = Settings.options.textAlignment;

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

        private static readonly int chunkSize = 10000;
        private static int splitCurrentSymbol = 0;
        private static string chunkContents;

        // RTL
        private static PastimeReadingRTL.FastStringBuilder pageContentRTL = new PastimeReadingRTL.FastStringBuilder(PastimeReadingRTL.RTLSupport.DefaultBufferSize);
        public static bool currentlyRTL = Settings.options.enableRTL;

        // misc
        public static Color bookTitleColor = Color.white;
        public static Color bookAuthorColor = Color.white;

        private static readonly string emptyBookPlaceholder = 
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

            if (stage == "rtl") // manage RTL text orientation, actual characters fix is applied in "Read", should be done before "Read"
            {
                currentlyRTL = Settings.options.enableRTL;

                if (currentlyRTL)
                {
                    ReadMain.p1Text.isRightToLeftText = true;
                    ReadMain.p2Text.isRightToLeftText = true;
                    ReadMain.h1Text.isRightToLeftText = true;
                    ReadMain.h2Text.isRightToLeftText = true;
                    ReadMain.titleText.isRightToLeftText = true;
                    ReadMain.authorText.isRightToLeftText = true;
                }
                else
                {
                    ReadMain.p1Text.isRightToLeftText = false;
                    ReadMain.p2Text.isRightToLeftText = false;
                    ReadMain.h1Text.isRightToLeftText = false;
                    ReadMain.h2Text.isRightToLeftText = false;
                    ReadMain.titleText.isRightToLeftText = false;
                    ReadMain.authorText.isRightToLeftText = false;
                }
            }

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
 
                    if (currentlyRTL)
                    {
                        bookContents = RTLconvert(bookContents);
                    }
                }

                if (bookContents == null || bookContents.Length <= 1)
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "Book content is empty!");
                    bookContents = emptyBookPlaceholder;
                }

            }

            if (stage == "font") // sets up font and alignment, should run before "split"
            {
                ReadMain.p1Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.p2Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.h1Text.fontSize = (float)Settings.options.fontSize;
                ReadMain.h2Text.fontSize = (float)Settings.options.fontSize;

                currentFontSize = (float)Settings.options.fontSize;

                switch (Settings.options.textAlignment)
                {
                    case 0:
                        ReadMain.p1Text.alignment = TextAlignmentOptions.TopJustified;
                        ReadMain.p2Text.alignment = TextAlignmentOptions.TopJustified;
                        ReadMain.h1Text.alignment = TextAlignmentOptions.TopJustified;
                        ReadMain.h2Text.alignment = TextAlignmentOptions.TopJustified;
                        break;
                    case 1:
                        ReadMain.p1Text.alignment = TextAlignmentOptions.TopLeft;
                        ReadMain.p2Text.alignment = TextAlignmentOptions.TopLeft;
                        ReadMain.h1Text.alignment = TextAlignmentOptions.TopLeft;
                        ReadMain.h2Text.alignment = TextAlignmentOptions.TopLeft;
                        break;
                    case 2:
                        ReadMain.p1Text.alignment = TextAlignmentOptions.TopRight;
                        ReadMain.p2Text.alignment = TextAlignmentOptions.TopRight;
                        ReadMain.h1Text.alignment = TextAlignmentOptions.TopRight;
                        ReadMain.h2Text.alignment = TextAlignmentOptions.TopRight;
                        break;
                    case 3:
                        ReadMain.p1Text.alignment = TextAlignmentOptions.Top;
                        ReadMain.p2Text.alignment = TextAlignmentOptions.Top;
                        ReadMain.h1Text.alignment = TextAlignmentOptions.Top;
                        ReadMain.h2Text.alignment = TextAlignmentOptions.Top;
                        break;
                }

                currentAlignment = Settings.options.textAlignment;

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

            if (stage == "page") // sets current page, ensures the current page is odd, should run after "split"
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

            if (stage == "setup") // defines page content, flips the book for RTL, should run last
            {
                TurnpageVisible(false);

                if (!currentlyRTL) // LTR text
                {
                    ReadMain.titleText.text = bookTitle;
                    ReadMain.authorText.text = bookAuthor;
                }
                else // RTL text
                {
                    ReadMain.titleText.text = RTLconvert(bookTitle);
                    ReadMain.authorText.text = RTLconvert(bookAuthor);
                }

                ReadMain.titleText.color = bookTitleColor;
                ReadMain.authorText.color = bookAuthorColor;

                if (Settings.options.enableRTL)
                {
                    ReadMain.hands.transform.localScale = new Vector3(-1f, 1f, 1f);
                    SetPage("p2", currentPage);
                    if (splitPages.Length > 1)
                    {
                        SetPage("p1", currentPage + 1);
                    }
                    else
                    {
                        ReadMain.p1Text.text = "";
                        ReadMain.p1PageText.text = "";
                    }
                }
                else
                {
                    ReadMain.hands.transform.localScale = new Vector3(1f, 1f, 1f);
                    SetPage("p1", currentPage);
                    if (splitPages.Length > 1)
                    {
                        SetPage("p2", currentPage + 1);
                    }
                    else
                    {
                        ReadMain.p2Text.text = "";
                        ReadMain.p2PageText.text = "";
                    }
                }
            }
        }

        public static string RTLconvert(string text)
        {
            pageContentRTL.Clear();
            PastimeReadingRTL.RTLSupport.FixRTL(text, pageContentRTL, false, true, true);
            pageContentRTL.Reverse();
            return pageContentRTL.ToString();
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
					PageFlip("firstFrame", Settings.options.enableRTL);
				} 
				if (animTime > clipTimeStart && animatorSifter < 2) // state clipTimeStart % in
                {
					animatorSifter = 2;
					PageFlip("nearFirstFrame", Settings.options.enableRTL);
				}
				if (animTime > clipTimeEnd && animatorSifter < 3) // state clipTimeEnd % in
                {
					animatorSifter = 3;
					PageFlip("nearLastFrame", Settings.options.enableRTL);
					return;
				}
			}
			else if (animatorSifter != 0) // state exit
            {
				animatorSifter = 0;
				PageFlip("lastFrame", Settings.options.enableRTL);
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
        public static void PageFlip(string aEvent, bool RTL)
		{
            string page1 = "p1";
            string page2 = "p2";
            string tech1 = "h1";
            string tech2 = "h2";

            if (RTL)
            {
                page1 = "p2";
                page2 = "p1";
                tech1 = "h2";
                tech2 = "h1";
            }

            if (aEvent == "firstFrame") // start of animation
            {
				TurnpageVisible(true);
				if (currentTurn == "next")
				{
					ReadMain.p2.SetActive(false);
                    SetPage(tech1, currentPage + 2);
                    SetPage(tech2, currentPage + 1);
                    SetPage(page2, currentPage + 3);
				}
				if (currentTurn == "prev")
				{
					ReadMain.p1.SetActive(false);
                    SetPage(tech1, currentPage);
                    SetPage(tech2, currentPage - 1);
                    SetPage(page1, currentPage - 2);
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
                    SetPage(page1, currentPage + 2);
                    currentPage += 2;
				}
				if (currentTurn == "prev")
				{
					ReadMain.p2.SetActive(true);
                    SetPage(page2, currentPage - 1);
                    currentPage -= 2;
				}
				Settings.options.currentPage = currentPage;
				Settings.options.Save();
			}
        }
	}
}
