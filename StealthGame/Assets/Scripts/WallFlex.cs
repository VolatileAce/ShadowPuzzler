using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFlex : MonoBehaviour {

    public GameObject top;
    public GameObject mid;
    public GameObject bot;
    public GameObject left;
    public GameObject right;

    public bool topBlock;
    public bool midBlock;
    public bool botBlock;
    public bool leftBlock;
    public bool rightBlock;
    private void Update()
    {
        if (topBlock)
        {
            top.SetActive(true);
        }
        else
        {
            top.SetActive(false);
        }
        if (midBlock)
        {
            mid.SetActive(true);
        }
        else
        {
            mid.SetActive(false);
        }
        if (botBlock)
        {
            bot.SetActive(true);
        }
        else
        {
            bot.SetActive(false);
        }
        if (leftBlock)
        {
            left.SetActive(true);
        }
        else
        {
            left.SetActive(false);
        }
        if (rightBlock)
        {
            right.SetActive(true);
        }
        else
        {
            right.SetActive(false);
        }
    }
}
