using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused : MonoBehaviour
{
    [SerializeField]
    public bool PauseGame;
    public GameObject pause;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseGame)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Continue()
    {
        pause.SetActive(false);
        Time.timeScale = 1;
        PauseGame = false;
    }

    public void Pause()
    {
        pause.SetActive(true);
        Time.timeScale = 0;
        PauseGame = true;
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
