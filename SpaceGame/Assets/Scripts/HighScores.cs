using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HighScores : MonoBehaviour
{
/*    static string[] privateCode;
    static string[] publicCode;
    // faire le lien entre le nom du niveau et le rang du niveau/serveur
    public static Dictionary<string, int> link;
    const string webURL = "http://dreamlo.com/lb/";

    //METHODE : toujours ajouter les infos devant !! => dans link aussi !

    public Highscore[] highscoresList;  // stocke les scores !! Variables publiques => la modifier dans les fctions (cf FormatHighscores)
    public Highscore yourHighscore;
    private DisplayScores displayScores;
    static HighScores instance;  // on crée une instance de notre truc pour mettre une coroutine dans static addnewhighscore

    void Awake()
    {
        int i = 0;
        privateCode = new string[] {"nqpqWWn_zUG-xFKoDclyAA4Ld4D4lAwkK-WnE4wGC8-g", "tO_fDuBZ50-dGPf34qfh-wUG9LxsUH3UCq2RWKFhoAWQ", "SMqNU1xJdE6NPu8U6HJLsAQdwIh-Hp4UypUSetgXtz3g", "Jit4mbPuuEmbILSrTIne4AM9lzT-v1XkmtfpQ3yaSf_Q", "_FbKbl1nA0aCb1gid1GNUgvpbtNUWIs0ST1iOoz4P61Q", "U69Za_-OnkmitXd50Z7vCQVqfGsSD48ECAGGbZsnBa9g" };
        publicCode = new string[] {"581efcfd8af60311c8ea1f77", "581b59fa8af6031358209ccc", "581b59e18af6031358209baa", "581b59878af6031358209741", "580f60178af60313c0e63a75", "581b590e8af6031358209124" };
        link = new Dictionary<string, int>()
        {
            {"Level 6", i++ }, {"Level 5", i++}, {"Level 4", i++}, {"Level 3", i++}, {"Level 2", i++}, {"Level 1", i++}
        };
        displayScores = GetComponent<DisplayScores>();
        instance = this;
    }

    public static void AddNewHighScore(string username, float time)  // static => appelable facilement
    {
        Highscore score = new Highscore(username, time);
        instance.StartCoroutine(instance.UploadNewHighscore(username, score.GetInvTime()));
    }

    IEnumerator UploadNewHighscore(string username, long inv_time)
    {
        DownloadYourHighscore();

        WWW www = new WWW(webURL + privateCode[link[SceneManager.GetActiveScene().name]] + "/add/" + WWW.EscapeURL(username) + "/" + inv_time);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            DownloadHighscores(); // ==> tjs à jour !!
        }
        else
        {
            Debug.print("Error uploading" + www.error);
        }
    }

    public void DownloadHighscores()
    {
        StartCoroutine("DownloadHighscoresFromDatabase");
    }

    public void DownloadYourHighscore() { StartCoroutine("DownloadYourHighscoreFromDatabase"); }
    
    IEnumerator DownloadHighscoresFromDatabase()
    {
        WWW www = new WWW(webURL + publicCode[link[SceneManager.GetActiveScene().name]] + "/pipe/0/10");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            FormatHighscores(www.text);
            displayScores.OnDownloadedHighscores(highscoresList);
        }
        else
        {
            Debug.print("Error downloading" + www.error);
        }
    }

    IEnumerator DownloadYourHighscoreFromDatabase()
    {
        WWW www = new WWW(webURL + publicCode[link[SceneManager.GetActiveScene().name]] + "/pipe-get/" + PlayerPrefs.GetString("PlayerPseudo"));
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            string[] entryInfo = (www.text).Split(new char[] { '|' }); // on sépare toutes les infos fournies
            string username = entryInfo[0];
            long inv_time = long.Parse(entryInfo[1]);
            yourHighscore = new Highscore(username, inv_time);
//            displayScores.OnDownloadedYourHighscore(yourHighscore);
        }
        else
        {
            Debug.print("Error downloading" + www.error);
        }
    }

    void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries); // listes des entrées
        highscoresList = new Highscore[entries.Length];  // on va les mettre dans une liste de scores
        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' }); // on sépare toutes les infos fournies
            string username = entryInfo[0];
            long inv_time = long.Parse(entryInfo[1]);
            highscoresList[i] = new Highscore(username, inv_time);
        }
    }
*/
}
 
/*
public struct Highscore
{
    private const long key = 12345678910; // grande clée pour avoir des scores entiers
    public string username;
    private long inv_time;
    private float time;

    public Highscore(string param_username, long param_inv_time)
    {
        username = param_username;
        inv_time = param_inv_time;
        time = ((float)key) / ((float)inv_time);
    }

    public Highscore(string param_username, float param_time)
    {
        username = param_username;
        time = param_time;
        inv_time = System.Convert.ToInt64(key / time);
    }

    public float GetTime() { return time; }
    public long GetInvTime() { return inv_time; }
}
*/