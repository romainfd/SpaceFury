using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectionScrolling : MonoBehaviour {
    public GameObject[] buttons;
    public GameObject StageButtons, LevelButtons;
    public Sprite[] stageNumbers;

    private int selectedStage, selectedLevel;
    private int firstNb = 1;  // le plus bas numéro affiché
    private int maxNb = 8;
    private Vector3 translation, translationRight, translationLeft; // toutes positives quand on se déplace vers la droite (ie partent à gauche)
    private bool moving;
    private int sens; //+ ou -1
    private float t0, T = 0.5f, lambda = 0;

	void Start ()
    {
        StageButtons.SetActive(true);
        LevelButtons.SetActive(false);
        translation = buttons[1].GetComponent<Transform>().position - buttons[2].GetComponent<Transform>().position;
        moving = false;
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].GetComponentsInChildren<Image>()[1].sprite = stageNumbers[firstNb - 1 + i];
            buttons[i].GetComponentsInChildren<Image>()[1].preserveAspect = true;
        }
        buttons[0].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 0);
        buttons[buttons.Length - 1].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        if (moving)
        {
            lambda += Time.deltaTime / T;
            foreach (GameObject button in buttons) { button.GetComponent<Transform>().Translate(sens * Time.deltaTime / T * translation); }
            if (sens == 1)
            {
                buttons[buttons.Length - 1].GetComponent<Image>().color = new Color(1, 1, 1, lambda);
                buttons[buttons.Length - 1].GetComponent<Transform>().localScale = new Vector3(2 - lambda, 2 - lambda, 2 - lambda);
                buttons[1].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
                buttons[buttons.Length - 1].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, lambda);
                buttons[1].GetComponent<Image>().color = new Color(1, 1, 1, 1 - lambda);
                buttons[1].GetComponent<Transform>().localScale = new Vector3(1+lambda, 1+ lambda, 1+ lambda);
                buttons[1].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1-lambda);
            }
            else if (sens == -1)
            {
                buttons[buttons.Length - 2].GetComponent<Image>().color = new Color(1, 1, 1, 1-lambda);
                buttons[buttons.Length - 2].GetComponent<Transform>().localScale = new Vector3(1+ lambda, 1+ lambda, 1+ lambda);
                buttons[buttons.Length - 2].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1-lambda);
                buttons[0].GetComponent<Image>().color = new Color(1, 1, 1, lambda);
                buttons[0].GetComponent<Transform>().localScale = new Vector3(2 - lambda, 2 - lambda, 2 - lambda);
                buttons[0].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, lambda);
            }
            if (lambda >= 1)
            {
                foreach (GameObject button in buttons) { button.GetComponent<Transform>().Translate( - lambda * sens * translation); }  // on remet à la position initiale
                lambda = 0; 
                moving = false;
                // On remet tout en ordre 
                // bonne taille et bonne transparence pour le 2é et l'avant dernier
                buttons[buttons.Length - 2].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                buttons[buttons.Length - 2].GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
                buttons[buttons.Length - 2].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
                buttons[1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                buttons[1].GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
                for (int i = 0; i < buttons.Length; i++) { buttons[i].GetComponentsInChildren<Image>()[1].sprite = stageNumbers[firstNb - 1 + i]; }
                buttons[1].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
                // bonne taille et bonne transparence pour le 1er et le dernier
                buttons[buttons.Length - 1].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                buttons[buttons.Length - 1].GetComponent<Transform>().localScale = new Vector3(2, 2, 2);
                buttons[buttons.Length - 1].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 0);
                buttons[0].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                buttons[0].GetComponent<Transform>().localScale = new Vector3(2, 2, 2);
                buttons[0].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 0);
                // on actualise les numéros
                firstNb += sens;
                for (int i = 0; i < buttons.Length; i++) { buttons[i].GetComponentsInChildren<Image>()[1].sprite = stageNumbers[firstNb - 1 + i]; }
            }
        }
    }

    public void Right()
    {
        if (firstNb + (buttons.Length - 2) -1  < maxNb)  // on a encore des stage à afficher (et -2 et -1 car les comings affichent rien !)
        {
            moving = true;
            sens = +1;
        }
    }

    public void Left()
    {
        if (firstNb > 1)  // on a encore des stage à afficher (et -1 car le coming affiche rien !)
        {
            moving = true;
            sens = -1;
        }
    }

    public void LoadLevelSelection(int nbShift)
    {
        int stageNb = firstNb + nbShift;
        if (stageNb <= PlayerPrefs.GetInt("Stage") || Application.platform == RuntimePlatform.WindowsEditor) 
        {
            selectedStage = stageNb;
            StageButtons.SetActive(false);
            LevelButtons.SetActive(true);
        }
    }

    public void LoadLevel(int levelNb)
    {  // levelNb - 1 car numérotés 1, 2, ...
        if (selectedStage < PlayerPrefs.GetInt("Stage") || levelNb-1 <= PlayerPrefs.GetInt("Level") || Application.platform == RuntimePlatform.WindowsEditor) // Stage déjà fini ou alors niveau fait
        {
            selectedLevel = levelNb - 1;
            SceneManager.LoadScene("Level " + selectedStage + "." + selectedLevel);
            Debug.print("Charging " + "Level " + selectedStage + "." + selectedLevel);  
        }
    }

    public void BackToStageSelection()
    {
        StageButtons.SetActive(true);
        LevelButtons.SetActive(false);
    }
}
