using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using UnityEngine.XR;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private Button startBtn, startWithBtn, exitBtn, soundsBtn, playWithBtn;
    [SerializeField]
    private Text text;
    [SerializeField] 
    private TMP_Dropdown dropdown;
    [SerializeField]
    private TMP_InputField[] inputFields;
    [SerializeField]
    private Image Image1, Image2;
    [SerializeField]
    private Image[] Panels;

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayWith()
    {
        PlayerNames.countNames = 0;
        startBtn.gameObject.SetActive(false);
        startWithBtn.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);
        soundsBtn.gameObject.SetActive(false);
        Image1.gameObject.SetActive(false);

        Image2.gameObject.SetActive(true);
        dropdown.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        playWithBtn.gameObject.SetActive(true);
        Panels[0].gameObject.SetActive(true);
        inputFields[0].gameObject.SetActive(true);

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        OnDropdownValueChanged(dropdown.value);
        // —крываем все панели и InputField в начале
        //for (int i = 0; i < Panels.Length; i++)
        //{
        //    Panels[i].gameObject.SetActive(false);
        //    inputFields[i].gameObject.SetActive(false);
        //}
    }

    private void OnDropdownValueChanged(int value)
    {
        // ѕоказываем или скрываем панели и InputField в соответствии с выбранным значением
        for (int i = 0; i < Panels.Length; i++)
        {
            if (i <= value)
            {
                Panels[i].gameObject.SetActive(true);
                inputFields[i].gameObject.SetActive(true);
            }
            else
            {
                Panels[i].gameObject.SetActive(false);
                inputFields[i].gameObject.SetActive(false);
            }
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            Image2.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            playWithBtn.gameObject.SetActive(false);
            dropdown.value = 0;
            for (int i = 0; i < Panels.Length; i++)
            {
                Panels[i].gameObject.SetActive(false);
                inputFields[i].gameObject.SetActive(false);
            }

            startBtn.gameObject.SetActive(true);
            startWithBtn.gameObject.SetActive(true);
            exitBtn.gameObject.SetActive(true);
            soundsBtn.gameObject.SetActive(true);
            Image1.gameObject.SetActive(true);
            //PlayerNames.countNames = 0;
            //PlayerNames.names;
        }
    }
    
    public void PlayAfterSelectPlayers()
    {
        SubmitNames();
        SceneManager.LoadScene(1);
    }

    private void SubmitNames()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            PlayerNames.names[i] = inputFields[i].text;
            if(inputFields[i].text != "")
            {
                PlayerNames.countNames++;
            }
        }
        //for (int i = 0; i < PlayerNames.names.Length; i++)
        //{
        //    Debug.Log(PlayerNames.names[i]);

        //}
        Debug.Log(PlayerNames.countNames);
    }
}
