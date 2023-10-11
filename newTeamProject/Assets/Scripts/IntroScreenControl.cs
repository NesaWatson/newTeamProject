using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroScreenControl : MonoBehaviour
{
    public GameObject introScreen;

    //void Start()
    //{
    //    introScreen.SetActive(true);
    //}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            introScreen.SetActive(false);
        }
    }
}
