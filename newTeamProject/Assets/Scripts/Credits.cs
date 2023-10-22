using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class Credits : MonoBehaviour
{
    public Text titleText;
    public Text subtitleText;
    public Text contributorsText;
    public Text assetsText;
    public Text musicSfxText;
    public Text thanksText;

    public TextAsset creditsData;

    void Start()
    {
        string data = creditsData.text;
        loadCredits(data);
    }

    void loadCredits(string xmlData)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlData);

        XmlNode creditsNode = xmlDoc.SelectSingleNode("credits");
        titleText.text = creditsNode.SelectSingleNode("title").InnerText;
        subtitleText.text = creditsNode.SelectSingleNode("subtitle").InnerText;

        XmlNode sectionNode = creditsNode.SelectSingleNode("section");
        XmlNode contributorsNode = sectionNode.SelectSingleNode("subsection[title='Contributors']");
        contributorsText.text = "";
        XmlNodeList contributorNodes = contributorsNode.SelectNodes("contributor");
        foreach (XmlNode contributorNode in contributorNodes)
        {
            contributorsText.text += contributorNode.InnerText + "\n";
        }

        XmlNode assetsNode = sectionNode.SelectSingleNode("subsection[title='Assets']");
        assetsText.text = "";
        XmlNodeList assetNodes = assetsNode.SelectNodes("asset");
        foreach (XmlNode assetNode in assetNodes)
        {
            string assetName = assetNode.SelectSingleNode("name").InnerText;
            string assetURL = assetNode.SelectSingleNode("url").InnerText;
            assetsText.text += assetName + "\n" + assetURL + "\n\n";
        }

        XmlNode musicSfxNode = sectionNode.SelectSingleNode("subsection[title='Music/SFX']");
        musicSfxText.text = "";
        XmlNodeList audioNodes = musicSfxNode.SelectNodes("audio");
        foreach (XmlNode audioNode in audioNodes)
        {
            string audioName = audioNode.SelectSingleNode("name").InnerText;
            string audioURL = audioNode.SelectSingleNode("url").InnerText;
            musicSfxText.text += audioName + "\n" + audioURL + "\n\n";
        }

        thanksText.text = creditsNode.SelectSingleNode("thanks").InnerText;
    }
}
