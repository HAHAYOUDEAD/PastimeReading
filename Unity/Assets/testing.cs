using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class testing : MonoBehaviour
{
    Animator anim;

    private static GameObject turnpage;
    private static GameObject p1;
    private static GameObject p2;
    private static GameObject h1;
    private static GameObject h2;

    private static GameObject pCam;
    private static GameObject hCam;

    private static TMP_Text p1Text;
    private static TMP_Text p2Text;
    private static TMP_Text h1Text;
    private static TMP_Text h2Text;

    private static TMP_Text p1PageText;
    private static TMP_Text p2PageText;
    private static TMP_Text h1PageText;
    private static TMP_Text h2PageText;

    private static int currentPage;
    private static string currentTurn = null;

    // Start is called before the first frame update
    void Start()
    {
        turnpage = transform.Find("readingBook_turnpage").gameObject;
        p1 = transform.Find("readingBook_textField_p1").gameObject;
        p2 = transform.Find("readingBook_textField_p2").gameObject;
        h1 = transform.Find("readingBook_textField_h1").gameObject;
        h2 = transform.Find("readingBook_textField_h2").gameObject;

        pCam = GameObject.Find("p1-2Cam");
        hCam = GameObject.Find("h1-2Cam");

        p1Text = pCam.transform.GetChild(0).GetComponentInChildren<TMP_Text>();
        p2Text = pCam.transform.GetChild(1).GetComponentInChildren<TMP_Text>();
        h1Text = hCam.transform.GetChild(0).GetComponentInChildren<TMP_Text>();
        h2Text = hCam.transform.GetChild(1).GetComponentInChildren<TMP_Text>();

        p1PageText = pCam.transform.GetChild(2).GetComponentInChildren<TMP_Text>();
        p2PageText = pCam.transform.GetChild(3).GetComponentInChildren<TMP_Text>();
        h1PageText = hCam.transform.GetChild(2).GetComponentInChildren<TMP_Text>();
        h2PageText = hCam.transform.GetChild(3).GetComponentInChildren<TMP_Text>();

        currentPage = 1;

        p1Text.pageToDisplay = currentPage;
        p2Text.pageToDisplay = currentPage + 1;

        p1PageText.text = currentPage.ToString();
        p2PageText.text = (currentPage + 1).ToString();

        anim = GetComponent<Animator>();

        //turnpage.SetActive(false);
        //h1.SetActive(false);
       // h2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //EventManager.StartListening("PageFlip", PageFlip());

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            anim.SetBool("open", true);
            //print("opening");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (currentTurn == null)
            {
                //currentTurn = "next";
                anim.SetBool("next_page", true);
                //print("next page");
            }
            else print("currentTurn is currently = " + currentTurn);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            //if (currentPage - 1 < 1) return;

            if (currentTurn == null)
            {
                //currentTurn = "prev";
                anim.SetBool("prev_page", true);
                //print("prev page");
            }
            else print("currentTurn is currently = " + currentTurn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            anim.SetBool("open", false);
           // print("closing");
        }
    }
    

    public void PageFlip(AnimationEvent aEvent)
    {
        if (aEvent.stringParameter == "lastFrame") 
        {
            if (currentTurn == "next") //next page - FIRST frame
            {
                h1Text.pageToDisplay = currentPage + 2;
                h2Text.pageToDisplay = currentPage + 1;
                h1PageText.text = (currentPage + 2).ToString();
                h2PageText.text = (currentPage + 1).ToString();

                turnpage.SetActive(true);
                h1.SetActive(true);
                h2.SetActive(true);

                p2.SetActive(false);
                p2Text.pageToDisplay = currentPage + 3;
                p2PageText.text = (currentPage + 3).ToString();
            }
            if (currentTurn == "prev") //prev page - LAST frame
            {
                turnpage.SetActive(false);
                h1.SetActive(false);
                h2.SetActive(false);

                p2Text.pageToDisplay = currentPage - 1;
                p2PageText.text = (currentPage - 1).ToString();
                p2.SetActive(true);

                currentPage -= 2;
                currentTurn = null;
            }

        }

        if (aEvent.stringParameter == "3rdLastFrame")
        {
            if (currentTurn == "next") //next page - 3rd frame from start
            {
                p2.SetActive(true);
            }
            if (currentTurn == "prev") //prev page - 3rd frame from end
            {
                p2.SetActive(false);
            }
        }


        if (aEvent.stringParameter == "firstFrame") 
        {
            if (currentTurn == "next") //next page - LAST frame
            {
                turnpage.SetActive(false);
                h1.SetActive(false);
                h2.SetActive(false);

                p1Text.pageToDisplay = currentPage + 2;
                p1PageText.text = (currentPage + 2).ToString();
                p1.SetActive(true);

                currentPage += 2;
                currentTurn = null;
            }
            if (currentTurn == "prev") //prev page - FIRST frame
            {
                h1Text.pageToDisplay = currentPage;
                h2Text.pageToDisplay = currentPage - 1;
                h1PageText.text = (currentPage).ToString();
                h2PageText.text = (currentPage - 1).ToString();

                turnpage.SetActive(true);
                h1.SetActive(true);
                h2.SetActive(true);

                p1.SetActive(false);
                p1Text.pageToDisplay = currentPage - 2;
                p1PageText.text = (currentPage - 2).ToString();
            }
        }

        if (aEvent.stringParameter == "3rdFirstFrame")
        {
            if (currentTurn == "next") //next page - 3rd frame from end
            {
                p1.SetActive(false);
            }
            if (currentTurn == "prev") //prev page - 3rd frame from start
            {
                p1.SetActive(true);
            }
        }


    }

}


