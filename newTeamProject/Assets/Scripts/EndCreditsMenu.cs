using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;

public class EndCreditsMenu : MonoBehaviour
{
    public bool play;
    public int playDelay;
    public RectTransform textTrans;
    public Text text;
    public TextAsset creditsCSV;

    public float lineHeight;
    public float yDistance;
    public float scrollSpeed;
    public int maxLines;

    private WaitForSeconds delayWords;
    private float y;
    private Vector2 startPoint;
    private int linesDisplayed;

    private string[][] credits;
    private StringBuilder sb;

    public const string Column = " - ";
    public const string Row = "\n"; 

    public void Start()
    {
        delayWords = new WaitForSeconds(playDelay);
        startPoint = textTrans.anchoredPosition; 

        sb = new StringBuilder();
        text.text = "";

        textTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxLines * lineHeight);

        string csvContent = creditsCSV.text.Replace("<br>", "\n");

        string[] creditLines = creditsCSV.text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        credits = new string[creditLines.Length][];
        
        for(int n = 0; n < creditLines.Length; n++)
        {
            credits[n] = creditLines[n].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); 

        }

        StartCoroutine(playCredits());
    }
    public void Update()
    {
        if(!play)
        {
            return;
        }
        y += Time.deltaTime * scrollSpeed; 

        while(y >= yDistance)
        {
            if(linesDisplayed > maxLines + 2 && text.alignment != TextAnchor.UpperCenter)
            {
                text.alignment = TextAnchor.UpperCenter;
            }

            linesInText();

            y -= yDistance;

            linesDisplayed++; 

            if(linesDisplayed > credits.Length)
            {
                play = false;   
            }
        }
        textTrans.anchoredPosition = startPoint + new Vector2(0, y);
    }
    public void linesInText()
    {
        sb.Length = 0;

        int rowIndex = Mathf.Max(0, linesDisplayed - maxLines);

        int rowCount = Mathf.Min(linesDisplayed, maxLines, credits.Length - linesDisplayed - 1);

        for(int n =0; n < rowCount; n++)
        {

            for(int i = 0; i < credits[rowIndex].Length; i++)
            {
                if(i > 0)
                {
                    sb.Append(Column);
                }
                sb.Append(credits[rowIndex][i]);
            }
            rowIndex++;

            if(n < rowCount - 1)
            {
                sb.Append(Row);
            }
        }
        text.text = sb.ToString();
    }
    private IEnumerator playCredits()
    {
        yield return new WaitForSeconds(playDelay);

        play = true;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("MainCamera"))
            StartCoroutine(playCredits());
    }
}
