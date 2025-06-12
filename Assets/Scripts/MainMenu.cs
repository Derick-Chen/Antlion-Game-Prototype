using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] int targetLevel;
        public void PlayEasy ()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayMedium ()
    {
        SceneManager.LoadScene(2);
    }
    public void PlayHard ()
    {
        SceneManager.LoadScene(3);
    }
    public void ReturnToMenu ()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayTutorial ()
    {
        SceneManager.LoadScene(4);
    }

    public void QuitGame ()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
