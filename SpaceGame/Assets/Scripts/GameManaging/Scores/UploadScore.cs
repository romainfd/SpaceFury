using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadScore : MonoBehaviour {
    private static string insertURL = "spacefury.000webhostapp.com/InsertScore.php";

    public static IEnumerator insertScore(string pseudo, int stage, int level, int time, int stars)
    {
        WWWForm form = new WWWForm();
        form.AddField("pseudoPost", pseudo);
        form.AddField("stagePost", stage);
        form.AddField("levelPost", level);
        form.AddField("timePost", time);
        if (stars > 3) { stars = 3; }
        form.AddField("starsPost", stars);

        WWW www = new WWW("https://" + insertURL, form);
        yield return www;         // on attend que les scores soient téléchargés
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.print("Upload Error : " + www.error);
        }
        else
        {
            Debug.print("Upload OK of "+time);
        }
    }

    public static IEnumerator insertScore(Highscore score)
    {
        WWWForm form = new WWWForm();
        form.AddField("pseudoPost", score.pseudo);
        form.AddField("stagePost", score.stage);
        form.AddField("levelPost", score.level);
        form.AddField("timePost", score.time);
        form.AddField("starsPost", score.stars);

        WWW www = new WWW(insertURL, form);
        yield return www;         // on attend que les scores soient téléchargés
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.print("Upload Error : " + www.error);
        }
        else
        {
            Debug.print("Upload OK");
        }
    }
}
