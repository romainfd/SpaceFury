using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ButtonClick : MonoBehaviour {

	public void OnClickLoad(int nb)
    {
        SceneManager.LoadScene(nb);
	}

	public void OnClickLoad(string nom)
    {
        SceneManager.LoadScene(nom);
	}

    public void LoadCreatedLevel()
    {
        name = GameObject.FindWithTag("CreatedLevelName").GetComponent<InputField>().text;
        SceneManager.LoadScene(name);
    }

    public void LoadGameLevel()
    {
        name = GameObject.FindWithTag("GameLevelNumber").GetComponent<InputField>().text;
        SceneManager.LoadScene("Level "+name);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("PlayerPseudo"))
            SceneManager.LoadScene("Level 1");
        else
            SceneManager.LoadScene("PlayerCreation");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
