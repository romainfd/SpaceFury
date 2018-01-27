using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ruby_use : MonoBehaviour
{
    private static string insertLevelURL = "spacefury.000webhostapp.com/InsertLevel.php";
    private static string downloadLevelURL = "spacefury.000webhostapp.com/DisplayLevelDownload.php";
    public static int unlockEditorPrice = 10;
    public GameObject messageZone;
    public InputField[] inputFields;
    public Text[] texts;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        { // Pour les autres aussi je dois passer par la classe message plutôt !
            messageZone = GameObject.FindWithTag("Message");
            messageZone.SetActive(false);
        }
        if (inputFields != null)
        {
            foreach (InputField inputField in inputFields)
            {
                inputField.gameObject.SetActive(false);
            }
        }
    }

    public void LoadEditor()
    {
        if (PlayerPrefs.GetString("EditorAccess") == "True")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Editor");
        }
        else
        {
            StartCoroutine(gameObject.GetComponent<Message>().DisplayMessage("You need to unlock the Editor with " + unlockEditorPrice + " Rubis", 2));
        }
    }

    public IEnumerator DisplayMessage(string message, float deltaTime)
    {
        messageZone.SetActive(true);
        messageZone.GetComponentInChildren<Text>().text = message;
        yield return new WaitForSeconds(deltaTime);
        messageZone.SetActive(false);
    }

    public void UnlockEditor()
    {
        if (PlayerPrefs.GetString("EditorAccess") == "True")
        {
            StartCoroutine(DisplayMessage("You have already unlocked the Editor mode", 1.5f));
        }
        else if (ChangeRubyNumber(-unlockEditorPrice))
        {
            PlayerPrefs.SetString("EditorAccess", "True");
            StartCoroutine(DisplayMessage("You have unlocked the Editor mode. Congratulations !", 2.2f));
        }
        else
        {
            StartCoroutine(DisplayMessage("You don't have enough Rubis", 1.5f));
        }
    }

    public void DisplayInputField(int i)
    {
        texts[i].enabled = false;
        inputFields[i].gameObject.SetActive(true);
    }

    public void UploadLevel()
    {
        string name = inputFields[0].text;
        string pseudo = PlayerPrefs.GetString("PlayerPseudo");
        if (canPay(1))
        {
            if (PlayerPrefs.HasKey(pseudo + ":" + name))
            {
                string data;
                if (PlayerPrefs.GetString(pseudo + ":" + name).Contains("0|")) // on l'a testé en local et on a donc mis l'Id 0
                {
                    data = PlayerPrefs.GetString(pseudo + ":" + name).Substring(2); // On enlève le "0|"
                }
                StartCoroutine(insertLevel(pseudo, name, PlayerPrefs.GetString(pseudo + ":" + name)));
            }
            else
            {
                StartCoroutine(DisplayMessage("There isn't a level with this name", 2));
            }
        }
        else
        {
            StartCoroutine(DisplayMessage("You don't have enough Rubis", 2));
        }
    }

    public IEnumerator insertLevel(string pseudo, string levelName, string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("pseudoPost", pseudo);
        form.AddField("levelNamePost", levelName);
        form.AddField("dataPost", data);
        Debug.print(data);
        WWW www = new WWW( "https://" + 
            insertLevelURL, form);
        yield return www;         // on attend que le niveau soit inséré
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.print(www.error);
            StartCoroutine(DisplayMessage("Unable to access server", 3));
        }
        else
        {
            // et on le retélécharge pour avoir l'Id devant et donc pouvoir faire des scores
            WWWForm formDownload = new WWWForm();
            formDownload.AddField("pseudoPost", pseudo);
            formDownload.AddField("levelNamePost", levelName);

            WWW wwwDownload = new WWW("https://" + 
                downloadLevelURL, formDownload);
            yield return wwwDownload;         // on attend que le niveau soit téléchargé
            if (!string.IsNullOrEmpty(wwwDownload.error))
            {
                Debug.print(wwwDownload.error);
                StartCoroutine(DisplayMessage("Unable to access server", 3));
            }
            else
            {
                if (wwwDownload.text == "No data available;")
                {
                    StartCoroutine(DisplayMessage("There is no such level. Are you sure you typed the name correctly ? The right form is Pseudo:LevelName", 7));
                }
                else
                {
                    ChangeRubyNumber(-1);
                    PlayerPrefs.SetString(pseudo + ':' + levelName, wwwDownload.text);
                    AddChallengeLevel(pseudo + ':' + levelName);
                    StartCoroutine(DisplayMessage("Your level has been uploaded ! You can now challenge your friends, they can download it with the ID : " + PlayerPrefs.GetString("PlayerPseudo") + ":" + levelName, 7));
                }
            }
            Debug.print(wwwDownload.text);
        }
        Debug.print(PlayerPrefs.GetString(pseudo + ':' + levelName));
    }

    public void DownloadLevel()
    {
        string completeName = inputFields[1].text;
        if (!completeName.Contains(":")) {
            StartCoroutine(DisplayMessage("There is no such level. Are you sure you typed the name correctly ? The right form is Pseudo:LevelName", 7));
        }
        else
        {
            string pseudo = completeName.Split(':')[0];
            string levelName = completeName.Split(':')[1];
            if (PlayerPrefs.HasKey(pseudo + ":" + levelName))
            {
                StartCoroutine(DisplayMessage("You already have downloaded this level. You can load it using " + pseudo + ":" + levelName, 4));
            }
            else if (canPay(-1))
            {
                StartCoroutine(downloadLevelFromDB(pseudo, levelName));
            }
            else
            {
                StartCoroutine(DisplayMessage("You don't have enough Rubis", 2));
            }
        }
    }

    public IEnumerator downloadLevelFromDB(string pseudo, string levelName)
    {
        WWWForm form = new WWWForm();
        form.AddField("pseudoPost", pseudo);
        form.AddField("levelNamePost", levelName);

        WWW www = new WWW("https://" + 
            downloadLevelURL, form);
        yield return www;         // on attend que le niveau soit téléchargé
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.print(www.error);
            StartCoroutine(DisplayMessage("Unable to access server", 3));
        }
        else
        {
            if (www.text == "No data available;")
            {
                StartCoroutine(DisplayMessage("There is no such level. Are you sure you typed the name correctly ? The right form is Pseudo:LevelName", 7));
            }
            else
            {
                ChangeRubyNumber(-1); 
                PlayerPrefs.SetString(pseudo + ':' + levelName, www.text);
                AddChallengeLevel(pseudo + ':' + levelName);
                StartCoroutine(DisplayMessage("The level " + levelName + " from " + pseudo + " has been successfully downloaded. You can now load it using " + pseudo + ":" + levelName + " !", 8));
            }
        }
    }

    // Renvoie true si on peut payer ET FAIT PAYER en parallèle !! 
    public static bool ChangeRubyNumber(int changement)
    {
        int nbRuby = PlayerPrefs.GetInt("Rubis");
        if (nbRuby + changement < 0)  // on fait rien si pas assez : juste false
        {
            return false;
        }
        else
        {
            PlayerPrefs.SetInt("Rubis", PlayerPrefs.GetInt("Rubis") + changement);
            return true;
        }
    }

    public static bool canPay(int price)
    {
        return price <= PlayerPrefs.GetInt("Rubis");
    }


    public static void AddChallengeLevel(string challengeLevelCompleteName)
    {
        PlayerPrefs.SetString("DownloadedLevels", PlayerPrefs.GetString("DownloadedLevels") + "|" + challengeLevelCompleteName);
    }
}
