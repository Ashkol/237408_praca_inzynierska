using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesPanel : MonoBehaviour
{
    public static ResourcesPanel instance;
    public List<Image> icons = new List<Image>();
    public List<Text> countersText = new List<Text>();
    int[] counters;
    public GameObject winText;

    void Awake()
    {
        instance = this;
        counters = new int[countersText.Count];
        winText.SetActive(false);
    }

    public void AddResource(int index, int amount)
    {
        if (index < counters.Length)
        {
            counters[index] += amount;
            countersText[index].text = counters[index].ToString() + "/100";
        }

        bool showWinText = true;
        foreach(int count in counters)
        {
            if (count < 100)
            {
                showWinText = false;
                break;
            }
        }
        winText.SetActive(showWinText);
    }
}
