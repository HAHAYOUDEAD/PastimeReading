using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class testing_release : MonoBehaviour
{
    Animator anim;
    float a = 0.5f;
    float f = 0.0f;
    int i = 0;

    int page = 1;
    GameObject pcam;
    TMP_Text p1Text;
    TMP_Text p2Text;


    //private IEnumerator coroutineTest;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Debug.Log(TMPro.TMP_Settings.version);
    }



    void IdleFluc()
    {
        if (f < 1.0f)
        {
            a += i == 0 ? 0.03f : -0.03f; // add if i is 0, otherwise substract

            if (a > 0.9f) i = 1;
            if (a < 0.1f) i = 0;

            anim.SetFloat("idle_random", a);

            f += 0.1f;
        }
        else
        {
            i = Random.Range(0, 2);
            f = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            pcam = GameObject.Find("p1-2Cam");
            p1Text = pcam.transform.GetChild(0).GetComponent<TMP_Text>();
            p2Text = pcam.transform.GetChild(1).GetComponent<TMP_Text>();

            p1Text.pageToDisplay = page;
            page += 1;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("bring_book");
            InvokeRepeating("IdleFluc", 0.0f, 0.1f); //should be called when entering bring_book or open_book state, and called off when any other
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            anim.SetTrigger("open_book"); 
            //CancelInvoke(); //should be called on next animator state (which is open_book)
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anim.SetTrigger("next_page");

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anim.SetTrigger("prev_page");

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anim.SetTrigger("close_book");

        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anim.SetTrigger("scratch_ear");

        }



    }
}
