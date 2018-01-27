using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CreatedLevel : MonoBehaviour {
    public string sceneInfo;
    private Camera cam;

    public void Awake()
    {
        cam = Camera.main;
        if (SceneManager.GetActiveScene().name == "ChallengeLevel") // On doit charger le bon niveau
        {
            string IdData = PlayerPrefs.GetString(PlayerPrefs.GetString("ChallengeLevel"));
            if (IdData.Contains("|"))
            {
                sceneInfo = IdData.Split('|')[1];
            }
            else
            {
                GameManager.LoadOnClick("MainMenu");
            }
            Debug.print(IdData);
        }
        if (sceneInfo == "") { sceneInfo = "0#2.222222; -1.25; 3.375#False; 3; -1#False; False; False; False; False; False#"; }
        setUpScene();
    }

    //// On s'occupe de l'ouverture d'une scène créée
    public void setUpScene()
    {
        // détruire tous les anciens trucs contenus dans GameObject.FindWithTag("LevelObjects");
        string[] infos = sceneInfo.Split('#');
        string sceneStars = infos[0];
        string[] sceneCamera = infos[1].Split(new char[] { ';' });
        string[] sceneDarkness = infos[2].Split(new char[] { ';' });
        string[] scenePowers = infos[3].Split(new char[] { ';' });
        string[] sceneObjects = infos[infos.Length-1].Split(new char[] { '!' });
        // on gère les étoiles
        GameManager.SetTotalStarsNb(int.Parse(sceneStars));
        // On gère la caméra
        cam.GetComponent<RectTransform>().position = new Vector3(float.Parse(sceneCamera[0]), float.Parse(sceneCamera[1]), -10);
        cam.orthographicSize = float.Parse(sceneCamera[2]);
        // On gère le darkness
        if (sceneDarkness[0] == "True") { StartZoom.SetTDarkness(float.Parse(sceneDarkness[2])); } // sinon on fait rien => T = -1 => apparait jamais
        else { StartZoom.SetTDarkness(-1); }  // inutile mais pour être sûr
        StartZoom.SetSize(float.Parse(sceneDarkness[1]));
        // On gère les powers
        PowerManager.powersTaken = new bool[scenePowers.Length];
        for (int i = 0; i < scenePowers.Length; i++) { PowerManager.powersTaken[i] = scenePowers[i] == "True"; }
        // On gère les objets
        foreach (string item in sceneObjects)
        {
            if (item.Split(';').Length > 1)   // pour éviter les "" et " " à la fin MAIS ATTENTION => 1 valeur seule sera coupée !
            {
                string[] info = item.Split(';');
                float x = float.Parse(info[2]), y = float.Parse(info[3]);
                int index = Int32.Parse(info[0]), rotation = Int32.Parse(info[1]);
                if (index > 3) // on avait du être très précis
                {
                    x /= 100;
                    y /= 100;
                }
                GameObject obj = Instantiate(GetComponent<Editor>().listObjects[index], new Vector3(x, y, 0), GetComponent<Editor>().listObjects[index].GetComponent<Transform>().rotation) as GameObject;
                // pour les paths l'incrément de rotation est de 90°, 1° pour le reste
                int increment = (index <= 3) ? 90 : 1;
                obj.GetComponent<Transform>().Rotate(rotation * new Vector3(0, 0, increment));
                // on le met à la bonne taille
                if ((new List<int> {4, 5, 7, 10, 11, 12}).Contains(index)) { obj.GetComponent<Transform>().localScale *= float.Parse(info[info.Length - 1]) / 100; }
                if (index == 0)
                {
                    obj.GetComponentsInChildren<Transform>()[1].localPosition *= float.Parse(info[info.Length - 1]) / 100;  // ok car x et z = 0
                    obj.GetComponentsInChildren<Transform>()[2].localPosition *= float.Parse(info[info.Length - 1]) / 100;
                }
                if (index == 12) { obj.GetComponent<AsymptoteFix>().v *= float.Parse(info[info.Length - 1]) / 100; }
                if ((new List<int> { 7, 10 }).Contains(index)) { obj.GetComponentInChildren<SnowRock>().T /= float.Parse(info[info.Length - 2])/100; }
                if (index == 8) { obj.GetComponent<MagmaRock>().TMove /= float.Parse(info[info.Length - 1]) / 100; }
                if (index == 10) { obj.GetComponentInChildren<Following>().v = float.Parse(info[info.Length - 1])/100; }
                if (index == 11) { obj.GetComponent<Harponer>().vGoRelative = float.Parse(info[info.Length - 2]) / 100; }
                if (index == 12) { obj.GetComponent<AsymptoteFix>().s *= float.Parse(info[info.Length - 2]) / 100; }
                
                // on place les enfants le cas échéant
                if (7 <= index && index <= 10)
                {
                    Transform[] childrenTransform = obj.GetComponentsInChildren<Transform>();
                    int i = 2;
                    foreach (Transform childTransform in childrenTransform)
                    {
                        childTransform.position = new Vector3(float.Parse(info[i]) / 100, float.Parse(info[i + 1]) / 100, 0);
                        i += 2;
                    }
                    if (index == 7) { obj.GetComponentInChildren<SnowRock>().Refresh(); }
                    if (index == 8) { obj.GetComponent<MagmaRock>().Refresh(); }
                }

                //on met les étoiles dans les ennemis
                if (new List<int> { 10, 11, 12 }.Contains(index)) { obj.GetComponentInChildren<CollisionFoe>().starsNb = int.Parse(info[info.Length - 3]); }

                obj.GetComponent<Transform>().SetParent(GameObject.FindWithTag("LevelObjects").GetComponent<Transform>());
            }
        }
    }
}
