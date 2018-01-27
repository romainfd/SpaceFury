using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;


public class Ruby : MonoBehaviour {
    private Text rubyDisplay;

	void Start () {
        rubyDisplay = GameObject.FindWithTag("Rubies").GetComponent<Text>();
        UpdateRubyDisplay();
	}
	
	// Update is called once per frame
	public void UpdateRubyDisplay() {
        rubyDisplay.text = ""+PlayerPrefs.GetInt("Rubis");
	}

    public void AdsRuby()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                PlayerPrefs.SetInt("Rubis", PlayerPrefs.GetInt("Rubis") + 1);
                UpdateRubyDisplay();
                StartCoroutine(gameObject.GetComponent<Ruby_use>().DisplayMessage("You've earned one Ruby !", 1));
                break;
            case ShowResult.Skipped:
                StartCoroutine(gameObject.GetComponent<Ruby_use>().DisplayMessage("You need to watch the entire advertisement to get your ruby", 1));
                break;
        }
    }
}
