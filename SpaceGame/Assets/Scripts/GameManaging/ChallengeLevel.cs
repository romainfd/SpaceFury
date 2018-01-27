using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Doit avoir un script MESSAGE sur son gameObject

public class ChallengeLevel : MonoBehaviour {
    public static string challengeLevel;
    private static string debugStart = "Debug[Display]:", localTestStart = "LocalTest:", cheatCodeStart = "CheatCode[Developer]:";


    public void LoadChallengeLevel(string challengeLevelName)
    {
        // ON GERE TOUS LES CAS DE DEBUG, TEST LOCAL ET CHEAT CODE AVANT DE PASSER AU NORMAL

        // DEBUG : de la forme "debugStart"+les debugInfos de la forme "type:clé" séparés par des '|'
        if (challengeLevelName.Contains(debugStart))
        {
            string debugInput = challengeLevelName.Substring(debugStart.Length);
            string debugOutput = "";
            string[] debugInfos;
            if (debugInput.Contains("|")) { debugInfos = debugInput.Split('|'); }
            else { debugInfos = new string[] { debugInput }; }   // juste une info

            foreach (string debugInfo in debugInfos)
            {
                if (debugInfo.Contains(":"))
                {
                    string type = debugInfo.Split(':')[0], key = debugInfo.Split(':')[1];
                    string output = key + ":";
                    switch (type)
                    {
                        case "int":
                            output += PlayerPrefs.GetInt(key);
                            break;
                        case "float":
                            output += PlayerPrefs.GetFloat(key);
                            break;
                        case "string":
                            output += PlayerPrefs.GetString(key);
                            break;
                        default:
                            output += "Error";
                            break;
                    }
                    debugOutput += output + "\n";
                }                
                StartCoroutine(gameObject.GetComponent<Message>().DisplayMessage("Debug data: \n" + debugOutput, 5));
            }
        }

        // TEST LOCAL D'UN NIVEAU : "localTestStart"+"levelName"
        else if (challengeLevelName.Contains(localTestStart))
        {
            challengeLevelName = PlayerPrefs.GetString("PlayerPseudo") + ":" + challengeLevelName.Substring(localTestStart.Length);
            PlayerPrefs.SetString("ChallengeLevel", challengeLevelName);
            if (!PlayerPrefs.GetString(challengeLevelName).Contains("|")) { // le niveau a juste été créé et n'a pas encore d'Id
                PlayerPrefs.SetString(challengeLevelName, "0|"+PlayerPrefs.GetString(challengeLevelName)); // on doit lui donner une Id car il est pas encore passé par la base de données. On lui donne 0
            }
            GameManager.LoadOnClick("ChallengeLevel");
        }

        // CHEAT CODE : "cheatCodeStart"+"key:type:value"
        else if (challengeLevelName.Contains(cheatCodeStart))
        {
            string cheatCode = challengeLevelName.Substring(cheatCodeStart.Length);
            if (cheatCode.Contains(":"))
            {
                string key = cheatCode.Split(':')[0], type = cheatCode.Split(':')[1], value = cheatCode.Split(':')[2];
                switch (type)
                {
                    case "int":
                        PlayerPrefs.SetInt(key, int.Parse(value));
                        break;
                    case "float":
                        PlayerPrefs.SetFloat(key, float.Parse(value));
                        break;
                    case "string":
                        PlayerPrefs.SetString(key, value);
                        break;
                }
                GameManager.LoadOnClick("PlayerCreation"); // On relance le jeu pour actualiser le cheat
            }
        }

        // ON CHARGE NORMALEMENT UN NIVEAU
        else if (challengeLevelName.Contains(":")) //de la bonne forme
        {
            if (PlayerPrefs.GetString("DownloadedLevels").Contains(challengeLevelName))
            // déjà téléchargé
            {
                PlayerPrefs.SetString("ChallengeLevel", challengeLevelName);
                GameManager.LoadOnClick("ChallengeLevel");
            }
            else if (PlayerPrefs.HasKey(challengeLevelName)) // pas encore uploadé
            {
                StartCoroutine(gameObject.GetComponent<Message>().DisplayMessage("You should first upload your level using 1 Ruby to be able to play !", 6));
            }
            else
            {
                StartCoroutine(gameObject.GetComponent<Message>().DisplayMessage("You should first download the level using 1 Ruby", 5));
            }
        }
        else
        {
            StartCoroutine(gameObject.GetComponent<Message>().DisplayMessage("The correct form is Pseudo:LevelName", 4));
        }
    }

    // Surcharge pour lancer depuis un inputField
    public void LoadChallengeLevel()
    {
        string challengeLevelName = GameObject.FindWithTag("ChallengeLevelName").GetComponent<InputField>().text; 
        LoadChallengeLevel(challengeLevelName);
    }
}
