using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQLScores : MonoBehaviour
{
    private string url = "spacefury.000webhostapp.com/DisplayLevelScores.php";
    public static string[] levelScores;
    private static bool scoresDownloaded;
    public static bool serverAccessed;

    public IEnumerator Start()
    {
        scoresDownloaded = false;
        bool scoresDownloadedTemp = true;
        serverAccessed = false;
        levelScores = new string[4];
        int stage = GameManager.GetStage();
        int level = GameManager.GetLevel();

        bool notUpdated = false;

        for (int stars = 0; stars < 4; stars++) {
            WWWForm form = new WWWForm();
            form.AddField("stagePost", stage);
            form.AddField("levelPost", level);
            form.AddField("starsPost", stars);

            WWW highscoresData = new WWW("https://" + url, form);
            yield return highscoresData;         // on attend que les scores soient téléchargés
            if (!string.IsNullOrEmpty(highscoresData.error))
            {
                Debug.print(highscoresData.error);
                levelScores[stars] = "Unable to access server";
                scoresDownloadedTemp = false; // si un est faux, il devient faux
                // ET METTRE EN PLACE LE STOCKAGE DES NOUVEAUX TEMPS EN LOCAL POUR LES ENVOYER PLUS TARD QD LA CONNEXION SE REFAIT

            }
            else
            {
                levelScores[stars] = highscoresData.text;
            }

            print("enter");
            int localScore = (stage == 0 ? LocalHighScores.GetLocalChallengeScore(level, stars) : LocalHighScores.localScores[10 * (stage - 1) + level][stars]);
            if (localScore != 0 && int.Parse(GetDataValue(levelScores[stars], "Time")) > localScore)  //temps local meilleur => on update
            {
                notUpdated = true;
                yield return UploadScore.insertScore(PlayerPrefs.GetString("PlayerPseudo"), stage, level, localScore, stars);
                Debug.print("LocalScore put online");
            }
        }
        if (notUpdated)
        {
            print("Restart");
            yield return Start();  // on retélécharge les scores !
        }
        scoresDownloaded = scoresDownloadedTemp;
        serverAccessed = true;
        Debug.print("Highscores Downloaded in " + Time.time + ", " + scoresDownloaded);
        GameObject.FindWithTag("ScoresScript").GetComponent<DisplayScores>().displayScores(SQLScores.GetLevelScores(stage, level));
    }

    public static string GetDataValue(string score, string index)
    {
        string value = score.Substring(score.IndexOf(index) + index.Length + 1); // +index.Length à cause du index & +1 à cause du ':'
        if (value.Contains("|")) { value = value.Remove(value.IndexOf('|')); }
        else if (index=="Time") { // on veut un Time alors que c'est la première fois qu'il y a une valeur pour ce truc !
            return "1000000000"; // on retourne un temps énorme car on veut que le joueur fasse mieux !
        }
        return value;
    }

    public static Highscore[] GetLevelScores(int stage, int level)   //scores[nbétoiles], les param sont inutiles car levelScores contient les scorees du niveau en cours
    {
        Highscore[] levelScoresHigh = new Highscore[4]; // pour chaque nb d'étoiles
        if (scoresDownloaded)
        {
            for (int stars = 0; stars < 4; stars++)
            {
                levelScoresHigh[stars] = new Highscore(levelScores[stars], stage, level, stars);
            }
        }
        else
        {
            for (int stars = 0; stars < 4; stars++)
            {
                levelScoresHigh[stars] = new Highscore(false, stage, level, stars);
            }
        }
        return levelScoresHigh;
    }
}

public struct Highscore
{
    public static string noDataYetString = "No data yet", noConnectionString = "No connection";
    public int id, stage, level, time, stars;  // time en ms !
    public string pseudo;
    public bool data;

    override
    public string ToString()
    {
        if (time == 0)
        {
            return pseudo;
        }
        else
        {
            return "" + time / 1000f + "s ( " + pseudo + " )";
        }
    }

    public Highscore(string scoreInfo)
    {
        data = false;
        id = 0;
        pseudo = noDataYetString;
        stage = 0;
        level = 0;
        time = 0;
        stars = 0;
        if (scoreInfo.Contains("|"))
        {
            data = true;
            string[] values = scoreInfo.Split('|');
            if (values.Length < 6) { data = false; }
            else
            {
                id = int.Parse(values[0].Substring(values[0].IndexOf(":") + 1));
                pseudo = values[1].Substring(values[1].IndexOf(":") + 1);
                stage = int.Parse(values[2].Substring(values[2].IndexOf(":") + 1));
                level = int.Parse(values[3].Substring(values[3].IndexOf(":") + 1));
                time = int.Parse(values[4].Substring(values[4].IndexOf(":") + 1));
                stars = int.Parse(values[5].Substring(values[5].IndexOf(":") + 1));
                if (stars > 3) { stars = 3; }
            }
        }
    }

    public Highscore(string scoreInfo, int _stage, int _level, int _stars)
    {
        data = false;
        id = 0;
        pseudo = noDataYetString;
        stage = _stage;
        level = _level;
        time = 0;
        stars = _stars;
        if (stars > 3) { stars = 3; }
        if (scoreInfo.Contains("|"))
        {
            data = true;
            string[] values = scoreInfo.Split('|');
            if (values.Length < 5) { data = false; }
            else
            {
                id = int.Parse(values[0].Substring(values[0].IndexOf(":") + 1));
                pseudo = values[1].Substring(values[1].IndexOf(":") + 1);
                time = int.Parse(values[4].Substring(values[4].IndexOf(":") + 1));
            }
        }
    }

    // data = false qd on a pas réussi à télécharger les scores => "No connection en pseudo"
    public Highscore(bool Data, int _stage, int _level, int _stars)
    {
        data = Data;
        id = 0;
        pseudo = noConnectionString;
        stage = _stage;
        level = _level;
        time = 0;
        stars = _stars;
        if (stars > 3) { stars = 3; }
    }
}

/* VERSION AVEC TOUS LES SCORES TELECHARGES
public class SQLScores : MonoBehaviour
{
    private string url = "spacefury.000webhostapp.com/DisplayScores.php";
    public static string[] highscores;
    private static bool scoresDownloaded;
    public static bool serverAccessed;

    public IEnumerator Start()
    {
        scoresDownloaded = false;
        serverAccessed = false;
        WWW highscoresData = new WWW("https://" + url);
        yield return highscoresData;         // on attend que les scores soient téléchargés
        if (!string.IsNullOrEmpty(highscoresData.error))
        {
            Debug.print(highscoresData.error);
            highscores = new string[4 * LocalHighScores.nbLevels];
            for (int i = 0; i < highscores.Length; i++)
            {
                highscores[i] = "Unable to access server";
            }

            // ET METTRE EN PLACE LE STOCKAGE DES NOUVEAUX TEMPS EN LOCAL POUR LES ENVOYER PLUS TARD QD LA CONNEXION SE REFAIT

        }
        else
        {
            string highscoresString = highscoresData.text;
            highscores = highscoresString.Split(';');
            scoresDownloaded = true;
        }
        serverAccessed = true;
        Debug.print("Highscores Downloaded in " + Time.time + ", " + scoresDownloaded)
; GameObject.FindWithTag("ScoresScript").GetComponent<DisplayScores>().displayScores(SQLScores.GetLevelScores(GameManager.GetStage(), GameManager.GetLevel()));
    }

    string GetDataValue(string score, string index)
    {
        string value = score.Substring(score.IndexOf(index) + index.Length + 1); // +index.Length à cause du index & +1 à cause du ':'
        if (value.Contains("|")) { value = value.Remove(value.IndexOf('|')); }
        return value;
    }

    public static Highscore[] GetLevelScores(int stage, int level)   //scores[nbétoiles]
    {
        Highscore[] levelScores = new Highscore[4]; // pour chaque nb d'étoiles
        if (scoresDownloaded)
        {
            for (int stars = 0; stars < 4; stars++)
            {
                levelScores[stars] = new Highscore(highscores[10 * (stage - 1) + 4 * level + stars], stage, level, stars);
            }
        }
        else
        {
            for (int stars = 0; stars < 4; stars++)
            {
                levelScores[stars] = new Highscore(false, stage, level, stars);
            }
        }
        return levelScores;
    }
}

public struct Highscore
{
    public int id, stage, level, time, stars;
    public string pseudo;
    public bool data;

    public Highscore(string scoreInfo)
    {
        data = false;
        id = 0;
        pseudo = "No data available";
        stage = 0;
        level = 0;
        time = 0;
        stars = 0;
        if (scoreInfo.Contains("|"))
        {
            data = true;
            string[] values = scoreInfo.Split('|');
            if (values.Length < 6) { data = false; }
            else
            {
                id = int.Parse(values[0].Substring(values[0].IndexOf(":") + 1));
                pseudo = values[1].Substring(values[1].IndexOf(":") + 1);
                stage = int.Parse(values[2].Substring(values[2].IndexOf(":") + 1));
                level = int.Parse(values[3].Substring(values[3].IndexOf(":") + 1));
                time = int.Parse(values[4].Substring(values[4].IndexOf(":") + 1));
                stars = int.Parse(values[5].Substring(values[5].IndexOf(":") + 1));
            }
        }
    }

    public Highscore(string scoreInfo, int _stage, int _level, int _stars)
    {
        data = false;
        id = 0;
        pseudo = "No data available";
        stage = _stage;
        level = _level;
        time = 0;
        stars = _stars;
        if (scoreInfo.Contains("|"))
        {
            data = true;
            string[] values = scoreInfo.Split('|');
            if (values.Length < 5) { data = false; }
            else
            {
                id = int.Parse(values[0].Substring(values[0].IndexOf(":") + 1));
                pseudo = values[1].Substring(values[1].IndexOf(":") + 1);
                time = int.Parse(values[4].Substring(values[4].IndexOf(":") + 1));
            }
        }
    }

    public Highscore(bool Data, int _stage, int _level, int _stars)
    {
        data = Data;
        id = 0;
        pseudo = "No data available";
        stage = _stage;
        level = _level;
        time = 0;
        stars = _stars;
    }
} 
*/