using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayScores : MonoBehaviour {
    public Text persoBest;
    public Text[] displayLines;
    public static Image[] rubies;
     
    // Awake pour avoir lieu avant de désactiver les Scores
    void Awake()
    {
        rubies = GameObject.FindWithTag("Rubies").GetComponentsInChildren<Image>();
        foreach (Image ruby in rubies) { ruby.enabled = false; }
        for (int i = 0; i < displayLines.Length; i++)
        {
            displayLines[i].text = "Fetching...";
        }
    }

    public void displayScores(Highscore[] levelScores)
    {
        if (levelScores.Length >= displayLines.Length)
        {
            for (int i = 0; i < displayLines.Length; i++)
            {
                displayLines[i].text = levelScores[i].ToString();
            }
        }
    }

    public void ScoreBeatten(int stars) {
        PlayerPrefs.SetInt("Rubis", PlayerPrefs.GetInt("Rubis") + 1);
        displayLines[stars].GetComponent<Text>().color = new Color(186f/255, 24f/255, 14f/255);
        rubies[stars].enabled = true;
    }


    /*
        HighScores highscoreManager;

        void Start()
        {
            for (int i = 0; i < displayLines.Length; i++)
            {
                displayLines[i].text = (i + 1) + ". Fetching...";
            }

            highscoreManager = GetComponent<HighScores>();

            StartCoroutine("RefreshHighscores");
        }

        IEnumerator RefreshHighscores()
        {
            while (true)
            {
                highscoreManager.DownloadHighscores();
                highscoreManager.DownloadYourHighscore();
                yield return new WaitForSeconds(30);
            }
        }

        public void OnDownloadedHighscores(Highscore[] highscoresList)
        {
            for (int i = 0; i < displayLines.Length; i++)
            {
                displayLines[i].text = (i + 1) + ". ";
                if (highscoresList.Length > i && highscoresList[i].GetTime() != 999999999)   // il reste des scores à afficher et ce ne sont pas juste des updates (pour le gameover)
                {
                    displayLines[i].text += highscoresList[i].username + " : " + highscoresList[i].GetTime() + "s";
                }
            }

        }

        public void OnDownloadedYourHighscore(Highscore yourHighscore)
        {
            persoBest.text = "Personal best : " + yourHighscore.GetTime() + "s";
        } */
}
