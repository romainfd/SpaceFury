using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        AddDebug("Chaîne vide", (PlayerPrefs.GetString("PlayerPseudo") == "").ToString());
        AddDebug("Pseudo", PlayerPrefs.GetString("PlayerPseudo"));
    }

    // Update is called once per frame
    void AddDebug(string newParam, string newDebugLog)
    {
        gameObject.GetComponent<Text>().text += newParam + " : "+newDebugLog +"\r\n";
    }
}
