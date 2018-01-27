using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    public float cameraSize;
    public GameObject[] listObjects;
    public GameObject darknessPrefab;
    public Slider slider, slider1, sliderZoom, sliderDarkness;
    public InputField sliderInput, slider1Input;
    public InputField viewTime;
    public static bool editing;
    public GameObject inputFields, sliders;
    public InputField speedInputField;
    public Button[] powersButtons;

    private bool sliderEdition = true;  // true = slider et false = input
    private GameObject darkness;
    private static bool creating ; // true quand on sélectionne qqchose puis false une fois qu'on a placé le truc
    private static bool saved;
    private Camera cam;
    private string fileLocation = null;
//    private static float screenRatio = 16 / 9;
    private static GameObject lastObject;
    private static int index = -1;
    private static List<int> newItem;
    private static List<List<int>> savingList; // [[index, rotation (+ pour left), x, y],[..], ...]
    private List<Transform> childrenTransform;
    int i = 0, n = 1;
    private float minFollow, maxFollow, minHarpon, maxHarpon, minVHarpon, maxVHarpon, minAsympt, maxAsympt, minVAsympt, maxVAsympt;
    // [nom, effet slider, effet slider1]
    private string[,] objectsInfo = new string[,] { {"Straight Path", "Space", "" }, {"Turn Path", "", "" }, {"Vertical Line Path", "", "" }, {"Oblique Line Path", "", "" }, {"Comet", "Size", "Rotation" }, {"Explosion Rock", "Size", "" }, {"Black Hole", "", "" }, {"Snow Rock", "Size", "Speed" }, {"Magma Rock", "Speed", "" }, {"Teleport", "", "" }, {"Following Foe", "Size & Pursuit Speed", "Patrol Speed" }, {"Harpon Foe", "Size", "Harpon Speed" }, {"Asymptotic Foe", "Size & Speed", "Accuracy" }, { "Base", "", "" }, { "Star", "", "" } };
    // [minSlider, maxSlider, minSlider1, maxSlider1]
    private float[,] limits = new float[,] { { 0.5f, 5, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0.2f, 5, 0, 360}, { 0.2f, 5, 0, 0 }, { 0, 0, 0, 0 }, { 0.2f, 5, 0.2f, 5 }, { 0.2f, 5, 0, 0 }, { 0, 0, 0, 0 }, { 0.7f, 1.3f, 0.2f, 5 }, { 0.7f, 1.3f, 0.5f, 3 }, { 0.7f, 1.3f, 0.8f, 1.3f }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
    private bool developer = true;
    private int totalStarsNb = 0;
    private string defaultSavingPath = @"C:\Users\Romain Fouilland\Documents\Romain\Travail\Fiches\Jeux Android\Jeu Alexis\Niveaux";
    private bool[] powersTaken;
    private Vector2 dimensions10; // dimensions pour orthographicSize = 10

    public void SetIndicePlacing(int i)
    {
        if (!saved && lastObject != null) { AddData(); } // on met l'objet qu'on vient de placer puis on passe au suivant
        saved = true; // on a saved le dernier truc
        index = i;
        creating = true;
        slider.minValue = limits[index, 0];
        slider.maxValue = limits[index, 1];
        slider.value = 1;
        slider.interactable = false;
        slider.GetComponentInChildren<Text>().text = objectsInfo[i,1];
        sliderInput.GetComponentsInChildren<Text>()[1].text = ((objectsInfo[i, 1] != "") ? objectsInfo[i, 1] + " -->" : ""); // on met la flèche que si quelque chose
        sliderInput.interactable = false;
        slider1.minValue = limits[index, 2];
        slider1.maxValue = limits[index, 3];
        slider1.value = 1;
        slider1.interactable = false;
        slider1.GetComponentInChildren<Text>().text = objectsInfo[i, 2];
        slider1Input.GetComponentsInChildren<Text>()[1].text = ((objectsInfo[i, 2] != "") ? "<-- " + objectsInfo[i, 2]: "");
        slider1Input.interactable = false;
    }

    void Start()
    {
        savingList = new List<List<int>>(); // [[index, rotation (+ pour left), x, y],[..], ...]
        childrenTransform = new List<Transform>(0);
        if (developer) { limits = new float[,] { { 0, 10, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0.1f, 10, 0, 360 }, { 0.1f, 10, 0, 0 }, { 0, 0, 0, 0 }, { 0.1f, 10, 0.1f, 10 }, { 0.1f, 10, 0, 0 }, { 0, 0, 0, 0 }, { 0.1f, 10, 0.1f, 10 }, { 0.1f, 10, 0.1f, 10 }, { 0.1f, 10, 0.1f, 10 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } }; }
        cam = Camera.main;
        dimensions10 = 2*cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));  // orthographicSize = 10 à ce moment
        creating = false;
        editing = (GetComponent<CreatedLevel>() == null);      // null => on édite
        powersTaken = new bool[powersButtons.Length + 1]; // +1 car powersTaken[0] = empty
        // on instancie le fond noir et on l'enlève
        if (editing)
        {
            Time.timeScale = float.Parse(speedInputField.text);
            darkness = Instantiate(darknessPrefab, GameObject.FindWithTag("Player").GetComponent<Transform>().position, Quaternion.identity) as GameObject;
            darkness.SetActive(false);
        }
    

        saved = true; // on a rien à enregistrer pour l'instant
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = 0;
            slider1.minValue = 0;
            slider1.maxValue = 0;
            slider.interactable = false;
            sliderInput.interactable = false;
            slider1.interactable = false;
            slider1Input.interactable = false;
            sliderZoom.minValue = Mathf.Log10(0.5f); // on peut grossir jusqu'à 10 fois
            sliderZoom.maxValue = Mathf.Log10(25); // on peut réduire jusqu'à 5 fois et on double de taille logarithmiquement 
            sliderZoom.value = Mathf.Log10(5); // on part de la taille standard
            sliderDarkness.minValue = 1;
            sliderDarkness.maxValue = 10;
            sliderDarkness.value = 3;
            sliders.SetActive(sliderEdition);
            inputFields.SetActive(!sliderEdition);
        }

    }

    void Update()
    {
        if (editing) { Zoom(); }
        // on peut créer des paths en continu : si on clique dans la zone, qu'on est en train de placer du path et que la position a changé (x ou y) => on met un nouveau path 
        if (Input.GetMouseButtonDown(0) && !IsSelecting() && !creating && new List<int> { 0, 1, 2, 3 }.Contains(index) && (newItem[2] != arrondi(Camera.main.ScreenToWorldPoint(Input.mousePosition).x) || newItem[3] != arrondi(Camera.main.ScreenToWorldPoint(Input.mousePosition).y)))
        {
            AddData();  // on ajoute celui qu'on était en train de faire
            Create(index, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        if (Input.GetMouseButtonDown(0) && creating && !IsSelecting())  // on veut continuer à placer le même truc
        {
            if (index <= 3) { Create(index, Camera.main.ScreenToWorldPoint(Input.mousePosition)); }  // on place du chemin
            else
            {
                CreateRock(index, Camera.main.ScreenToWorldPoint(Input.mousePosition));   // on place du rock
            }
            i = 0;  // pas d'enfants
            saved = false; // on est en train de placer un autre objet
        }
        else if (Input.GetMouseButtonDown(0) && !IsSelecting() && (new List<int> { 7, 8, 9, 10 }).Contains(index)) // liste de ceux avec des sous parties
        {
            childrenTransform = new List<Transform>(lastObject.GetComponentsInChildren<Transform>());
            childrenTransform.RemoveAt(0); // on enlève l'empty gameObject
            n = childrenTransform.Count;
            childrenTransform[i].position = cam.ScreenToWorldPoint(Input.mousePosition);
            childrenTransform[i].Translate(new Vector3(0, 0, 10));
            i = (i + 1) % n;
            if ((new List<int> { 7, 10 }).Contains(index)) { lastObject.GetComponentInChildren<SnowRock>().Refresh(); }
            else if (index == 8)
            {
                lastObject.GetComponent<MagmaRock>().Refresh();
                i = i % (n - 1);   // on ne déplace pas la flamme
            }
        }
        // Puis on désactive tous les colliders
        if (lastObject != null) { foreach (Collider2D coll in lastObject.GetComponentsInChildren<Collider2D>()) { coll.enabled = false; } }
        if (lastObject != null) { foreach (Collider2D coll in lastObject.GetComponentsInChildren<Collider2D>()) { coll.enabled = false; } }
        if ((new List<int> {0, 4, 5, 7, 8, 10, 11, 12 }).Contains(index) && slider.interactable == false) // on met les sliders et Input
        {
            slider.interactable = true;
            sliderInput.interactable = true;
            if ((new List<int> {4, 7, 10, 11, 12 }).Contains(index)) { slider1.interactable = true;  slider1Input.interactable = true; }
        }
        if ((new List<int> {0, 4, 5, 7, 8, 10, 11, 12 }).Contains(index) && !creating && lastObject != null) // on affecte les effets des sliders une fois qu'on a créé le truc !
        {
            // slider affecte la taille
            if (index == 0)  // on rapproche les 2 côtés du path
            {
                lastObject.GetComponentsInChildren<Transform>()[1].localPosition = slider.value * listObjects[index].GetComponentsInChildren<Transform>()[1].localPosition;  // ok car x et z =0;
                lastObject.GetComponentsInChildren<Transform>()[2].localPosition = slider.value * listObjects[index].GetComponentsInChildren<Transform>()[2].localPosition;
            }
            if ((new List<int> { 4, 5 }).Contains(index)) { lastObject.GetComponent<Transform>().localScale = slider.value * listObjects[index].GetComponent<Transform>().localScale; } // on dilate la taille initiale
            if (index == 4) { lastObject.GetComponent<Transform>().localEulerAngles = new Vector3(0, 0, slider1.value); }
            if (index == 7)
            {
                lastObject.GetComponentInChildren<SnowRock>().T = 1/slider1.value * listObjects[index].GetComponentInChildren<SnowRock>().T;
                lastObject.GetComponentInChildren<SnowRock>().gameObject.GetComponent<Transform>().localScale = slider.value * listObjects[index].GetComponentInChildren<SnowRock>().gameObject.GetComponent<Transform>().localScale;
            }
            else if (index == 8) { lastObject.GetComponent<MagmaRock>().TMove = 1/slider.value * listObjects[index].GetComponent<MagmaRock>().TMove; }
            else if (index == 10)
            {
                lastObject.GetComponentInChildren<SnowRock>().T = 1 / slider1.value * listObjects[index].GetComponentInChildren<SnowRock>().T;
                lastObject.GetComponentInChildren<SnowRock>().gameObject.GetComponent<Transform>().localScale = slider.value * listObjects[index].GetComponentInChildren<SnowRock>().gameObject.GetComponent<Transform>().localScale;
                lastObject.GetComponentInChildren<Following>().v = slider.value;
            }
            else if (index == 11)
            {
                lastObject.GetComponent<Harponer>().vGoRelative = slider1.value;
                lastObject.GetComponent<Transform>().localScale = slider.value * listObjects[index].GetComponent<Transform>().localScale;
            }
            else if (index == 12)
            {
                lastObject.GetComponent<AsymptoteFix>().v = slider1.value;
                lastObject.GetComponent<Transform>().localScale = slider.value * listObjects[index].GetComponent<Transform>().localScale;
            }
        }
    }

    // On s'occupe de la darkness
    public void Dark() { darkness.SetActive(!darkness.activeSelf); }
    public void changeDarkSize() { darkness.GetComponent<Transform>().localScale = (sliderDarkness==null ? 1 : sliderDarkness.value) * darknessPrefab.GetComponent<Transform>().localScale; }

    public void Zoom() { cam.orthographicSize = Mathf.Pow(10,sliderZoom.value); }

    public void xUp()
    {
        cam.GetComponent<RectTransform>().position += new Vector3(dimensions10.x / 10 * cam.orthographicSize / 10,0,0);  // dimensions correspond à camSize = 10 => on se déplace d'un dixième de l'écran    
    }

    public void xDown() { cam.GetComponent<RectTransform>().position += new Vector3(-dimensions10.x / 10 * cam.orthographicSize / 10, 0, 0); }

    public void yUp() { cam.GetComponent<RectTransform>().position += new Vector3(0, dimensions10.y / 10 * cam.orthographicSize / 10, 0); }

    public void yDown() { cam.GetComponent<RectTransform>().position += new Vector3(0, -dimensions10.y / 10 * cam.orthographicSize / 10, 0); }

    public int arrondi(float x) { return 2 * (int) Mathf.Floor((x + 1) / 2); }    // pour placer les paths

    void Create(int index, Vector3 position)
    {
        int x = arrondi(position.x), y = arrondi(position.y);
        lastObject = Instantiate(listObjects[index], new Vector3(x, y, 0), listObjects[index].GetComponent<Transform>().rotation) as GameObject;
        newItem = new List<int> { index, 0, x, y };
        creating = false;
    }

    void CreateRock(int index, Vector3 position) // les positions doivent être / 100
    {
        lastObject = Instantiate(listObjects[index], new Vector3(position.x, position.y, 0), listObjects[index].GetComponent<Transform>().rotation) as GameObject;
        newItem = new List<int> { index, 0 };
        creating = false;
    }

    void AddData() {
        // on met la rotation pour Comet
        if (index == 4) { newItem[1] = (int)slider1.value; }
        // tout est déjà bon pour eux les paths
        if (4 <= index)
        {
            foreach (Transform transform in lastObject.GetComponentsInChildren<Transform>()) // on met l'empty game Object puis les enfants
            {
                newItem.Add((int) (100 * transform.position.x));
                newItem.Add((int) (100 * transform.position.y));
            }
        }
        newItem.Add(-1);  // au cas où
        newItem.Add(-1);
        newItem.Add(-1);
        newItem.Add(-1);
        newItem.Add(-1);
        if (new List<int> { 10, 11, 12 }.Contains(index)) { newItem.Add(lastObject.GetComponentInChildren<CollisionFoe>().starsNb); }  // on met les étoiles pour les ennemis
        if (new List<int> { 7, 10, 11, 12 }.Contains(index)) { newItem.Add((int)(100 * slider1.value)); }
        if ((new List<int> {0, 4, 5, 7, 8, 10, 11, 12 }).Contains(index)) { newItem.Add((int)(100 * slider.value)); }
        // Si c'est une étoile et qu'on la garde bien => nbStars ++
        if (index == 14) { totalStarsNb++; }
        savingList.Add(newItem);
    }

    string ConvertToString(List<int> item) { return "Object " + item[0] +" ("+objectsInfo[item[0], 0]+") has been rotated by " + item[1] + " located at " + item[2] + ":" + item[3]; }

    string ConvertToStore(List<int> item, char endingChar = '!')  // par défaut ligne normale
    {
        string data = "";
        for (int k = 0; k < item.Count - 1; k++) // le dernier s'ajoute avec '!' pas ';' =>  count - 1
        { data += item[k] + ";"; }
        return data + item[item.Count - 1] + endingChar + "\r\n";
    }
    string ConvertToStore(bool[] item, char endingChar = '!')  // par défaut ligne normale
    {
        string data = "";
        for (int k = 0; k < item.Length - 1; k++) // le dernier s'ajoute avec '!' pas ';' =>  count - 1
        { data += item[k] + ";"; }
        return data + item[item.Length - 1] + endingChar + "\r\n";
    }

    public void Save()
    {
        if (!saved) { AddData(); saved = true; lastObject = null; }
        if (viewTime.text=="") { viewTime.text = "-1"; }   // par défaut, si on a rien rentré le darkness n'apparait qu'après le start zoom
        string dataUtile = totalStarsNb + "#\r\n";
        float[] camInfos = camInfo();
        dataUtile += camInfos[0] + ";" + camInfos[1] + ";" + camInfos[2] + "#\r\n" + darkness.activeSelf.ToString() + ";" + sliderDarkness.value + ";" + viewTime.text + "#\r\n";
        dataUtile += ConvertToStore(powersTaken, '#');
        string data = "------------- NEW LEVEL EDITION ---------------\r\n";
        data += System.DateTime.Now.ToShortDateString() + "\r\n";
        string levelName = GameObject.FindWithTag("CreatedLevelName").GetComponent<InputField>().text;
        data += levelName + "\r\n";
        // caméra
        data += "The camera is situated in " + camInfos[0] + ":" + camInfos[1] + " with an orthographic size of " + camInfos[2] + "\r\n";
        // darkness
        data += "Darkness : " + darkness.activeSelf.ToString() + " for a viewSize of " + sliderDarkness.value + " and a clean view of " + viewTime.text + "seconds\r\n";
//        if (GameObject.FindWithTag("GameLevelNumber").GetComponent<InputField>().text == "") { GameObject.FindWithTag("GameLevelNumber").GetComponent<InputField>().text = defaultSavingPath; }
//        fileLocation = Path.Combine(GameObject.FindWithTag("GameLevelNumber").GetComponent<InputField>().text, levelName+".txt");
        fileLocation = Path.Combine(defaultSavingPath, levelName+".txt");
        foreach (List<int> item in savingList)
        {
            dataUtile += ConvertToStore(item);
            data += ConvertToString(item)+"\r\n";
        }
        string timeScale = speedInputField.text;
        Time.timeScale = 1;
        speedInputField.text = "1";
        if (Application.platform == RuntimePlatform.WindowsEditor) File.WriteAllText(@fileLocation,dataUtile + "\r\n" + "\r\n" + data);   // datautile est la ligne à recopier dans le CreatedLevel et data est les infos mis en forme
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (!PlayerPrefs.HasKey(PlayerPrefs.GetString("PlayerPseudo") + ":" + levelName))
            {
                PlayerPrefs.SetString(PlayerPrefs.GetString("PlayerPseudo") + ":" + levelName, dataUtile);
                StartCoroutine(GetComponent<Ruby_use>().DisplayMessage("Votre niveau a bien été enregistré. Mettez le en ligne avec 1 Ruby !", 2));
            } else
            {
                StartCoroutine(GetComponent<Ruby_use>().DisplayMessage("Un niveau avec ce nom existe déjà, choisissez un autre nom.", 2));
            }
        }
    }

    public void Delete()
    {
        if (lastObject != null)
        {
            Destroy(lastObject);
            lastObject = null;
        }
    }

    public void rotateRight()
    {
        if (index <= 3) // on tourne que les paths pas les rochers !!
        {
            lastObject.GetComponent<Transform>().Rotate(new Vector3(0, 0, -90));
            newItem[1] += -1;
        }
    }

    public void rotateLeft()
    {
        if (index <= 3)
        {
            lastObject.GetComponent<Transform>().Rotate(new Vector3(0, 0, 90));
            newItem[1] += 1;
        }
    }

    bool IsSelecting()
    {
        return Input.mousePosition.x < Screen.width/4 || Input.mousePosition.y > Screen.height*3/4;  // situé dans le 1er quart gauche OU dans le quart en haut
    }

    float[] camInfo() // retourne la position qu'il faut donner à la cam et la taille pour qu'elle donne la vue du rectangle noir
    {
        Vector2 dimensions = dimensions10 / 10 * cam.orthographicSize;
        Debug.print(dimensions.x+" : "+dimensions.y+" : "+ cam.orthographicSize);
        Vector3 newPos = cam.GetComponent<Transform>().position + new Vector3(0.125f * dimensions.x, -0.125f * dimensions.y, 0);
        return new float[] { newPos.x, newPos.y, 0.675f * cam.orthographicSize };
    }

    public void sliderEdit()  // editer le slider qd l'input a été modifié
    {
        slider.value = float.Parse(sliderInput.text);
        sliderInput.text = ""+slider.value;  // permet de s'assurer qu'on ne sort pas des bornes !  
    }
    public void slider1Edit() // editer le slider1 qd l'input1 a été modifié
    {
        slider1.value = float.Parse(slider1Input.text);
        slider1Input.text = ""+slider1.value;
    }
    public void inputEdit() // editer l'input quand le slider a été modifié
    {
        sliderInput.text = "" + slider.value; ;
     }
    public void input1Edit() { slider1Input.text = "" + slider1.value; }

    public void EditorSpeedEdit() { Time.timeScale = float.Parse(speedInputField.text);  }

    public void SwitchEditionMode()
    {
        sliderEdition = !sliderEdition;
        sliders.SetActive(sliderEdition);
        inputFields.SetActive(!sliderEdition);
    }

    public void IncludeStar()
    {
        if ((new List<int> { 10, 11, 12 }).Contains(index))  // si qqchose est sélectionné et que c'est un ennemi
        {
            lastObject.GetComponentInChildren<CollisionFoe>().starsNb++;
            totalStarsNb++;
        }
    }

    public void ActivatePower(int nb)
    {
        if (powersTaken[nb]) { powersButtons[nb-1].GetComponent<RectTransform>().localScale = new Vector3(0.3875f, 0.3875f, 0.3875f); }
        else { powersButtons[nb-1].GetComponent<RectTransform>().localScale = new Vector3(0.45f, 0.45f, 0.45f); }
        powersTaken[nb] = !powersTaken[nb];
    }
}