using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour {
    private Slider[] sliders;

    void Start() {
        GameObject.FindWithTag("Button (PadPosition)").GetComponentInChildren<Text>().text = (PlayerPrefs.GetString("PadPosition") == "Right") ? "Right" : "Left";  // si rien, on met Left par défaut
        sliders = GameObject.FindWithTag("Canvas").GetComponentsInChildren<Slider>();
        sliders[0].value = PlayerPrefs.GetFloat("SoundVolume");
        sliders[1].value = PlayerPrefs.GetInt("PadTransparency");
    }

    public void ChangePad()
    {
        string padPosition = (PlayerPrefs.GetString("PadPosition") == "Right") ? "Right" : "Left";  // si rien, on met Left par défaut
        if (padPosition == "Right")
        {
            PlayerPrefs.SetString("PadPosition", "Left");
        }
        else { PlayerPrefs.SetString("PadPosition", "Right"); }
        GameObject.FindWithTag("Button (PadPosition)").GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PadPosition");
    }

    public void SetSoundVolume()
    {
        PlayerPrefs.SetFloat("SoundVolume", sliders[0].value);
    }

    public void SetPadTransparency()
    {
        PlayerPrefs.SetInt("PadTransparency", (int) sliders[1].value);
    }
}
