using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreatePlayer : MonoBehaviour {
    public void VersionUpdates() {
        LocalHighScores.CleanLocalScores();
    }

    public void SavePlayerPseudo()
    {
        PlayerPrefs.SetString("PlayerPseudo", GameObject.FindWithTag("InputField").GetComponent<InputField>().text);
        PlayerPrefs.SetInt("Stage", 1);
        PlayerPrefs.SetInt("Level", 0);
        string scoresEXP = "";
        for (int i = 0; i < LocalHighScores.nbLevels - 1; i++) { scoresEXP += "0;0;0;0#"; }
        PlayerPrefs.SetString("PlayerScores", scoresEXP + "0;0;0;0");
        PlayerPrefs.Save();
	}

    void Awake()
    {
        if (GameManager.gameVersionNumber != PlayerPrefs.GetFloat("GameVersion")) {
            PlayerPrefs.SetFloat("GameVersion", GameManager.gameVersionNumber);
            VersionUpdates();
        }
        // INITIALISATIONS 
        if (!PlayerPrefs.HasKey("Rubis")) { PlayerPrefs.SetInt("Rubis", 0); }
        if (!PlayerPrefs.HasKey("EditorAccess")) { PlayerPrefs.SetString("EditorAccess", "False"); }
        if (!PlayerPrefs.HasKey("PadPosition")) { PlayerPrefs.SetString("PadPosition", "Left"); }
        if (!PlayerPrefs.HasKey("PadTransparency")) { PlayerPrefs.SetInt("PadTransparency", 0); }
        if (!PlayerPrefs.HasKey("SoundVolume")) { PlayerPrefs.SetFloat("SoundVolume", 0.5f); }
        if (!PlayerPrefs.HasKey("DownloadedLevels")) { PlayerPrefs.SetString("DownloadedLevels", ""); }
        if (!PlayerPrefs.HasKey("ChallengeLevel")) { PlayerPrefs.SetString("ChallengeLevel", ""); }

        PlayerPrefs.Save();

        // On lance direct si l'utilisateur n'a pas à remplir PlayerPseudo
        if (PlayerPrefs.HasKey("PlayerPseudo")) { SceneManager.LoadScene("MainMenu"); }
    }

    public void LoadMenu()
    {
        if (GameObject.FindWithTag("InputField").GetComponent<InputField>().text != "")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
