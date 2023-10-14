using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Victorymsg : MonoBehaviour
{
    public Text victoryText;
    public float victoryduration = 5.0f;
    public void Start()
    {
        victoryText.enabled = false;
    }
public void ShowVictoryMessage()
    {

    }
    IEnumerator Displaymsg()
    {
        victoryText.enabled=true;
        yield return new WaitForSeconds(victoryduration);
        victoryText.enabled=false;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
