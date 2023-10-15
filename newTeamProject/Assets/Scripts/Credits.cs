using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

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
        loadCredits();
    }
    private void loadCredits()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(creditsData.text);

        XmlNode titleNode = xmlDoc.SelectSingleNode("credits/title");
        titleText.text = titleNode.InnerText;

        XmlNode subtitleNode = xmlDoc.SelectSingleNode("credits/subtitle");
        subtitleText.text = subtitleNode.InnerText;

        XmlNode contributorsNode = xmlDoc.SelectSingleNode("credits/section/subsection[title='Contributors']");
        contributorsText.text = contributorsNode.InnerXml;

        XmlNode assetsNode = xmlDoc.SelectSingleNode("credits/section/subsection[title='Assets']");
        assetsText.text = assetsNode.InnerXml;

        XmlNode musicSfxNode = xmlDoc.SelectSingleNode("credits/section/subsection[title='Music/SFX']");
        musicSfxText.text = musicSfxNode.InnerXml;

        XmlNode thanksNode = xmlDoc.SelectSingleNode("credits/thanks");
        thanksText.text = thanksNode.InnerText;
    }
}
