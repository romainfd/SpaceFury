using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static float gameVersionNumber = 0.61f;
    private static bool dead = false;
    private string sceneName;
    private static GameObject pauseButton;
    private static float finishTime;
    private static bool paused;
    private static bool won;
    private static bool justWon;
    private static bool lost;
    private static float timeTaken;
    // prendre tous les trucs qui compensent le menu pause (static !) puis initialiser après dans Start() !
    private static GameObject quitButton;
    private static GameObject quitFinishButton;
    private static GameObject nextLevelButton;
    private static GameObject restartButton;
    private static GameObject text;
    private static GameObject gameVersion;
    private static GameObject scores;
    private static GameObject gameOver;
    private static GameObject pauseBackground;
    private static GameObject currentTime;
    public GameObject starPrefabEditor;
    private static GameObject starPrefab; // grosse arnaque pour que starPrefab soit statique et éditable dans l'Editor
    private static int totalStarsNb;
    private static int gottenStarsNb;
    public static Image[] starsFinish;
    private static GameObject pad;
    private GameObject powers;

    public static void SetTotalStarsNb(int nb) { totalStarsNb = nb; }

    public static void SetDead(bool state)
    {
        dead = state;
    }

    public static bool GetDead()
    {
        return dead;
    }

    public static bool GetPaused()
    {
        return paused;
    }

    public static bool GetWon()
    {
        return won;
    }

    public static void SetJustWon(bool paramWon)
    {
        justWon = paramWon;
        if (justWon)
        {
            won = true;
        }  // si on vient de gagné, on a gagné !
    }

    void Awake()
    {
        //        StartCoroutine(SetCurrentTimeFontSize());
    }

    IEnumerator SetCurrentTimeFontSize() { 
        currentTime = GameObject.FindWithTag("CurrentTime");
        // currentTime est en bestFit donc s'adapte puis on enlève le best fit pour éviter qu'il bouge qd le temps change
        while (currentTime.GetComponent<Text>().cachedTextGenerator.fontSizeUsedForBestFit == 0)
            yield return new WaitForSeconds(0);
        int fontSize = currentTime.GetComponent<Text>().cachedTextGenerator.fontSizeUsedForBestFit;
        Debug.print(fontSize);
        currentTime.GetComponent<Text>().resizeTextForBestFit = false;
        currentTime.GetComponent<Text>().fontSize = fontSize;
    }
    void Start()
    {
        gottenStarsNb = 0;
        if (SceneManager.GetActiveScene().name != "MainMenu") { pad = gameObject.GetComponent<StartZoom>().pad; }
        sceneName = SceneManager.GetActiveScene().name;
        // s'il y a de quoi mettre la version
        gameVersion = GameObject.FindWithTag("GameVersion");
        if (gameVersion != null) { gameVersion.GetComponent<Text>().text = "v " + System.Convert.ToString(gameVersionNumber); }
        // Si il y a menu pause
        if (!(new List<string>(new string[] {"MainMenu", "LevelSelection","Credits", "Help", "Options", "PlayerCreation", "Ruby", "Ruby_use"})).Contains(sceneName))
        {
            pauseButton = GameObject.FindWithTag("PauseButton");
            pauseButton.GetComponent<Button>().onClick.AddListener(PauseButton);
            gameOver = GameObject.FindWithTag("GameOver");
            gameOver.SetActive(false);
            pauseBackground = GameObject.FindWithTag("PauseBackground");
            pauseBackground.SetActive(false);
            quitButton = GameObject.FindWithTag("PauseQuit");
            quitButton.GetComponent<Button>().onClick.AddListener(BackToMenu);
            quitButton.SetActive(false);
            quitFinishButton = GameObject.FindWithTag("FinishQuit");
            quitFinishButton.GetComponent<Button>().onClick.AddListener(BackToMenu);
            quitFinishButton.SetActive(false);
            nextLevelButton = GameObject.FindWithTag("NextLevel");
            if (SceneManager.GetActiveScene().name == "ChallengeLevel")
            {
                nextLevelButton.GetComponent<Button>().onClick.AddListener(BackToMenu);
            }  else
            {
                nextLevelButton.GetComponent<Button>().onClick.AddListener(NextLevel);
            }
            nextLevelButton.SetActive(false);
            restartButton = GameObject.FindWithTag("PauseRestart");
            restartButton.GetComponent<Button>().onClick.AddListener(Restart);
            restartButton.SetActive(false);
            text = GameObject.FindWithTag("Text");
            currentTime = GameObject.FindWithTag("CurrentTime");
            scores = GameObject.FindWithTag("Scores");
            scores.SetActive(false);
            starsFinish = GameObject.FindWithTag("StarsFinish").GetComponentsInChildren<Image>();
            starPrefab = starPrefabEditor;
            foreach (Image star in starsFinish) { star.enabled = false; }
            powers = GameObject.FindWithTag("Powers");
        }
        paused = false;
        won = false;
        justWon = false;
        lost = false;
        Time.timeScale = 1;
    }

    public static void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public static void LoadOnClick(string str)
    {
        SceneManager.LoadScene(str);
    }

    public static void LoadOnClick(int nb)
    {
        SceneManager.LoadScene(nb);
    }

    void Update()
    {
        if (currentTime != null)
        {
            if (!StartZoom.GetBegin()) // on n'a pas encore commencé
                currentTime.GetComponent<Text>().text = "Time   00.000s";
            else if (Time.time - StartZoom.Gett0() < 10)
                currentTime.GetComponent<Text>().text = "Time   0" + System.Convert.ToString(System.Math.Round(Time.time - StartZoom.Gett0(), 3)) + "s";
            else
                currentTime.GetComponent<Text>().text = "Time   " + System.Convert.ToString(System.Math.Round(Time.time - StartZoom.Gett0(), 3)) + "s";
        }
        // Pour mettre en pause tant qu'on peut pas toucher avec l'autre doigt !
        if (Input.GetKeyDown("space"))
        {
            Pause();
        }
        if (dead)
        {
            SoundEffect.Instance.MakeExplosionSound();
            lost = true;
            Pause();
            text.GetComponent<Text>().text = "Tu en étais à " + System.Convert.ToString(System.Math.Round(Time.time - StartZoom.Gett0(), 3)) + "s";
            gameOver.SetActive(true);
            dead = false;
        }
        if (won)
        {
            if (justWon)
            {
                Debug.print("justWon !");
                finishTime = Time.time;
                Finish(finishTime);
                justWon = false; // On a déjà gagné !
                // On gère les scores :
                string pseudo = (PlayerPrefs.GetString("PlayerPseudo") == "") ? "Anonymous" : PlayerPrefs.GetString("PlayerPseudo");
                // Local
                Debug.print("On update les scores locaux");
                if (GetStage() == 0)  {
                    LocalHighScores.UpdateLocalChallengeScores(GetLevel(), gottenStarsNb, (int)(1000 * timeTaken));
                } else { LocalHighScores.UpdateLocalScores(GetStage(), GetLevel(), gottenStarsNb, (int)(1000 * timeTaken)); }
                // Online
                // Il a battu un score ?
                Debug.print("Score battu ?");
                Highscore worldScore = SQLScores.GetLevelScores(GetStage(), GetLevel())[gottenStarsNb];
                if (worldScore.data) { // on a bien pu récupérer des données
                    if (1000 * timeTaken < worldScore.time) {  // Score battu !
                        GameObject.FindWithTag("ScoresScript").GetComponent<DisplayScores>().ScoreBeatten(gottenStarsNb);
                    }
                } else if (worldScore.pseudo == Highscore.noDataYetString) { // c'est la première valeur !
                    GameObject.FindWithTag("ScoresScript").GetComponent<DisplayScores>().ScoreBeatten(gottenStarsNb);
                }
                // On upload
                Debug.print("On uploade le score");
                StartCoroutine(UploadScore.insertScore(pseudo, GetStage(), GetLevel(), (int)(1000 * timeTaken), gottenStarsNb));
            }
            else if (Time.time - finishTime > 3)
            {
                NextLevel();
            }
        }
    }
    
    public void Pause()
    {
        if (paused && !won && !lost)
        {
            Time.timeScale = 1;
            quitButton.SetActive(false);
            restartButton.SetActive(false);
            scores.SetActive(false);
            pad.SetActive(true);
            currentTime.SetActive(true);
            pauseBackground.SetActive(false);
            pauseButton.GetComponent<RectTransform>().anchorMax = new Vector2(0.97f, 0.98f);
            pauseButton.GetComponent<RectTransform>().anchorMin = new Vector2(0.7f, 0.85f);
            powers.GetComponent<PowerManager>().Pause(false);
        }
        else 
        {
            Time.timeScale = 0;
            quitButton.SetActive(true);
            restartButton.SetActive(true);
            pad.SetActive(false);
            currentTime.SetActive(false);
            pauseBackground.SetActive(true);
            if (won || lost) { pauseButton.SetActive(false); }
            else { 
                pauseButton.GetComponent<RectTransform>().anchorMax = new Vector2(0.75f, 1);
                pauseButton.GetComponent<RectTransform>().anchorMin = new Vector2(0.25f, 0.65f);
            }
            if (!StartZoom.GetBegin()) // on n'a pas encore commencé
                text.GetComponent<Text>().text = "Pause : il faut y aller !";
            else
                text.GetComponent<Text>().text = "Pause : tu en es à " + System.Convert.ToString(System.Math.Round(Time.time - StartZoom.Gett0(), 3)) + " secondes !";
            scores.SetActive(true);
            // On actualise l'affichage des scores
            GameObject.FindWithTag("ScoresScript").GetComponent<LocalHighScores>().DisplayLocalScores(GetStage(), GetLevel());
            GameObject.FindWithTag("Script").GetComponent<SQLScores>().Start();
            GameObject.FindWithTag("ScoresScript").GetComponent<DisplayScores>().displayScores(SQLScores.GetLevelScores(GetStage(), GetLevel()));
            powers.GetComponent<PowerManager>().Pause(true);
        }
        paused = !paused;
    }

    public void PauseButton()
    {
        if (!won && !lost)  // car dans ces cas, il n'y a plus besoin de passer en mode pause !
            Pause();
    }

    public void Finish(float finishTime)
    {
        Pause();
        for (int i = 0; i < gottenStarsNb; i++) { starsFinish[i].enabled = true; }
        for (int i = gottenStarsNb + starsFinish.Length/2; i < starsFinish.Length/2 + totalStarsNb; i++) { starsFinish[i].enabled = true; }
        quitButton.SetActive(false);
        if (SceneManager.GetActiveScene().name != "ChallengeLevel") { quitFinishButton.SetActive(true); } // on met le MainMenu en haut
        nextLevelButton.SetActive(true);
        timeTaken = finishTime - StartZoom.Gett0();
        text.SetActive(true);
        text.GetComponent<Text>().text = "Tu as mis " + System.Convert.ToString(timeTaken)+" secondes !";
        // on débloque le niveau suivant : nom scene = Level stage.level : Level 1.8
        if (SceneManager.GetActiveScene().name.Split(' ')[0] == "Level")
        {
            int stage = GetStage();
            int level = GetLevel();
            // quel est le niveau suivant ?
            if (level == 9) // si on était au dernier du stage
            {
                level = 0;
                stage += 1;
            }
            else { level += 1; }
            // est-ce qu'on est allé plus loin qu'avant ?
            Debug.print("Level updated " + level);
            if (stage != 0 && (stage > PlayerPrefs.GetInt("Stage") || level > PlayerPrefs.GetInt("Level")))
            {
                PlayerPrefs.SetInt("Level", level);
                PlayerPrefs.SetInt("Stage", stage);
                PlayerPrefs.Save();
            }
        }
    }
        
    static void NextLevel() {
        if (SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1) // +1 et > car ça commence à 0 et on veut savoir s'il y a la suivante
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }


/// <summary>
/// Faire apparaitre, en fonction du nb qu'il en avait, les étoiles à la mort d'un ennemi
/// </summary>
/// <param name="nb"> nb d'étoiles contenues dans l'ennemi </param>
/// <param name="position"> la position où il est mort : là où vont apparaitre les étoiles </param>
    public static void StarsPoping(int nb, Vector3 position)
    {
        switch (nb)
        {
            case 1:
                GameObject star = Object.Instantiate(starPrefab, position, Quaternion.identity) as GameObject;
                break;
            default:
                for (int i = 0; i < nb; i++) { Object.Instantiate(starPrefab, position, Quaternion.identity); }
                break;
        }
    }

    public static void StarCatched() { gottenStarsNb++; }
    public static int GottenStarsNb() { return gottenStarsNb; }

    public static int GetStage()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "ChallengeLevel")
        {
            return 0;
        }
        else
        { // de la forme Level 1.3
          // débute à 1
            return int.Parse(sceneName.Split(' ')[1].Split('.')[0]);
        }
    }

    public static int GetLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "ChallengeLevel")
        {
            return int.Parse(PlayerPrefs.GetString(PlayerPrefs.GetString("ChallengeLevel")).Split('|')[0]);
        }
        else
        { // de la forme Level 1.3
          // débute à 1
            return int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1].Split('.')[1]); // de 0 à 9
        }
    }

    public static int GetLevelNumber() { return int.Parse(GetStage() + "" + GetLevel()); }
}