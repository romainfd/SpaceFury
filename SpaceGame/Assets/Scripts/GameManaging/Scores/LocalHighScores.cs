using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LocalHighScores : MonoBehaviour {
    public static int nbLevels = 30;
    public static int nbChallengeLevels = 50;
    public static int[][] localScores;
    public static int[][] localChallengeScores;
    public Text[] localDisplayLines;

    private static string localScoresStg;
    private static bool localScoresDownloaded;

    void Start()
    {
        localScoresDownloaded = false;
        localScores = new int[nbLevels][]; // pour chaque niveau, les meilleurs temps avec 0 à 3 étoiles
        localChallengeScores = new int[nbChallengeLevels][]; // Id|et les 4 tps
//        CleanLocalScores();
//        CleanLocalChallengeScores();

        DownloadLocalScores();
        DownloadLocalChallengeScores();
    }

    public static void DownloadLocalScores()
    {
        if (!localScoresDownloaded)
        {
            localScoresStg = PlayerPrefs.GetString("PlayerScores");
            string[] scoresByLevel = new string[nbLevels];
            int j = 0;
            if (localScoresStg.Contains("#"))
            {
                scoresByLevel = localScoresStg.Split('#');
                j = scoresByLevel.Length;
            }
            for (; j < nbLevels; j++)
            {
                scoresByLevel[j] = "0;0;0;0";
            }
            // C'est bon on a une liste de longueur nbLevels qui vaut un bon truc où "0;0;0;0"
            for (int i = 0; i < nbLevels; i++)
            {
                int[] tempLevelScores = new int[4];
                int stars = 0;
                foreach (string scoreByStar in scoresByLevel[i].Split(';'))
                    tempLevelScores[stars++] = int.Parse(scoreByStar);
                localScores[i] = tempLevelScores;
            }
            localScoresDownloaded = true;
        }
    }

    public static void DownloadLocalChallengeScores()
    {
        localScoresStg = PlayerPrefs.GetString("ChallengeScores");
        string[] scoresByLevel = new string[nbChallengeLevels];
        int j = 0;
        if (localScoresStg.Contains("#"))
        {
            scoresByLevel = localScoresStg.Split('#');
            j = scoresByLevel.Length;
        }
        for (; j < nbChallengeLevels; j++)
        {
            scoresByLevel[j] = "0|0;0;0;0";
        }
        // C'est bon on a une liste de longueur nbChallengeLevels qui vaut un bon truc où "0|0;0;0;0"
        for (int i = 0; i < nbChallengeLevels; i++)
        {
            int[] tempLevelScores = new int[5];
            tempLevelScores[0] = int.Parse(scoresByLevel[i].Split('|')[0]); // on récupère l'Id
            int stars = 1;
            foreach (string scoreByStar in scoresByLevel[i].Split('|')[1].Split(';'))
                tempLevelScores[stars++] = int.Parse(scoreByStar);
            localChallengeScores[i] = tempLevelScores;
        }
    }

    public static void UploadLocalScores()
    {
        string scores = "";
        for (int i = 0; i < nbLevels - 1; i++)
        {
            scores += localScores[i][0] + ";" + localScores[i][1] + ";" + localScores[i][2] + ";" + localScores[i][3] + "#";
        }
        // le dernier n'a pas de #
        scores += localScores[nbLevels - 1][0] + ";" + localScores[nbLevels - 1][1] + ";" + localScores[nbLevels - 1][2] + ";" + localScores[nbLevels - 1][3];
        PlayerPrefs.SetString("PlayerScores", scores);
        PlayerPrefs.Save();
    }

    public static void UploadLocalChallengeScores()
    {
        string scores = "";
        for (int i = 0; i < nbChallengeLevels - 1; i++)
        {
            scores += localChallengeScores[i][0] + "|" + localChallengeScores[i][1] + ";" + localChallengeScores[i][2] + ";" + localChallengeScores[i][3] + ";" + localChallengeScores[i][4] + "#";
        }
        // le dernier n'a pas de #
        scores += localChallengeScores[nbChallengeLevels - 1][0] + "|" + localChallengeScores[nbChallengeLevels - 1][1] + ";" + localChallengeScores[nbChallengeLevels - 1][2] + ";" + localChallengeScores[nbChallengeLevels - 1][3] + ";" + localChallengeScores[nbChallengeLevels - 1][4];
        PlayerPrefs.SetString("ChallengeScores", scores);
        print(scores);
        PlayerPrefs.Save();
    }

    public void DisplayLocalScores(int stage, int level)
    {
        if (stage == 0)  // C'est un challenge : on parcourt les temps en local et si on a pas de valeur (ie 0) on affiche No data yet
        {
            // on recherche le résultat avec la bonne Id
            int index = 0;
            for (int i = 0; i < localChallengeScores.Length; i++)
            {
                if (localChallengeScores[i][0] == level) {// bonne Id
                    index = i;
                    break;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                int temp = localChallengeScores[index][1+i];
                if (temp == 0) { 
                    localDisplayLines[i].text = "No data yet";
                } else {
                    localDisplayLines[i].text = "" + temp / 1000f + "s";
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                int nb = 10 * (stage - 1) + level;
                if (0 <= nb && nb < nbLevels)
                {
                    int temp = localScores[10 * (stage - 1) + level][i];
                    if (temp == 0)
                    {
                        localDisplayLines[i].text = "No data yet";
                    }
                    else
                    {
                        localDisplayLines[i].text = "" + temp / 1000f + "s";
                    }
                }
            }
        }
    }

    public static void UpdateLocalScores(int stageNb, int levelNb, int stars, int timeMS)
    {
        DownloadLocalScores();
        int nb = 10 * (stageNb - 1) + levelNb;
        if (localScores[nb][stars] > timeMS || localScores[nb][stars] == 0) {  // si on a battu l'ancien score, on met à jour et on affiche 
            localScores[nb][stars] = timeMS;
            UploadLocalScores();
            GameObject.FindWithTag("ScoresScript").GetComponent<LocalHighScores>().localDisplayLines[stars].GetComponent<Text>().color = new Color(186f / 255, 24f / 255, 14f / 255);

        }
    }

    public static int GetLocalChallengeScore(int levelId, int stars)
    {
        DownloadLocalChallengeScores();
        for (int i = 0; i < localChallengeScores.Length; i++)
        {
            if (localChallengeScores[i][0] == levelId)
            {// bonne Id
                return localChallengeScores[i][1 + stars];
            }
        }
        return 999999999;
    }

    public static void UpdateLocalChallengeScores(int levelId, int stars, int timeMS)
    {
        DownloadLocalChallengeScores();
        int index = 0;
        for (int i = 0; i < localChallengeScores.Length; i++)
        {
            if (localChallengeScores[i][0] == levelId)
            {// bonne Id
                index = i;
                break;
            }
        }
        print(localChallengeScores[index][1 + stars]);
        print(timeMS);
        if (localChallengeScores[index][1+stars] > timeMS || localChallengeScores[index][1+stars] == 0) { localChallengeScores[index][1+stars] = timeMS; }
        UploadLocalChallengeScores();
    }

    public static void CleanLocalScores()
    {
        for (int i = 0; i < nbLevels; i++)
        {
            localScores[i] = new int[] { 0, 0, 0, 0 };
        }
        UploadLocalScores();
    }

    public static void CleanLocalChallengeScores()
    {
        for (int i = 0; i < nbChallengeLevels; i++)
        {
            localChallengeScores[i] = new int[] { i, 0, 0, 0, 0 };
        }
        UploadLocalChallengeScores();
    }
}
