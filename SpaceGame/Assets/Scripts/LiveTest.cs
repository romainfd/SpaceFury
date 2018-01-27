using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LiveTest : MonoBehaviour {
    private static string defaultTestFile = @"C:\Users\Romain Fouilland\Documents\Romain\Travail\Fiches\Jeux Android\Jeu Alexis\Niveaux\LiveTest.txt";

    void Start()
    {
        Time.timeScale = 1;
        gameObject.GetComponent<CreatedLevel>().sceneInfo = File.ReadAllText(@defaultTestFile);
        gameObject.GetComponent<CreatedLevel>().setUpScene();
        StartCoroutine(reloadScene());
    }

    public void Update()
    {

    }

    public IEnumerator reloadScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
