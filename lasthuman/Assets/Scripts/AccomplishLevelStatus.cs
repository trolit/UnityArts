﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccomplishLevelStatus : MonoBehaviour {

    public static int finishedLevel1;

    [SerializeField]
    private Text level1finishedText;

    [SerializeField]
    private Text level2Title;
    [SerializeField]
    private GameObject level2Button;

	// Use this for initialization
	void Start ()
    {
        LoadLevelsDone();
	}


    public void ResetStatsAndSaves()
    {
        PlayerPrefs.SetInt("level1Finished", 0);
        PlayerPrefs.SetInt("warriorKill", 0);
        PlayerPrefs.SetInt("ogreKill", 0);
        PlayerPrefs.SetInt("zombieKill", 0);

        LoadLevelsDone();
    }

    private void LoadLevelsDone()
    {
        int level1 = PlayerPrefs.GetInt("level1Finished");

        if (level1 == 1)
        {
            level1finishedText.enabled = true;
            level2Title.enabled = true;
            level2Button.SetActive(true);
        }
        else if (level1 == 0)
        {
            level1finishedText.enabled = false;
            level2Title.enabled = false;
            level2Button.SetActive(false);
        }
    }
}
