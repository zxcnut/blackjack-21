using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSound : MonoBehaviour
{
    public bool isOn;
    public AudioSource ad;
    public AudioClip[] musicTracks; // ������ ����������� ������
    private int currentTrackIndex; // ������ �������� �����

    // Start is called before the first frame update
    void Start()
    {
        isOn = true;
        currentTrackIndex = 0;
        PlayMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("music") == 0)
        {
            ad.enabled = true;
            isOn = true;
        }
        else if (PlayerPrefs.GetInt("music") == 1)
        {
            ad.enabled = false;
            isOn = false;
        }

        if (!ad.isPlaying && isOn) // ���� ������� ���� ����������, ��������� ���������
        {
            currentTrackIndex++;
            if (currentTrackIndex >= musicTracks.Length)
            {
                currentTrackIndex = 0; // ��������� � ������� �����
            }
            PlayMusic();
        }
    }

    public void offSound()
    {
        if (!isOn)
        {
            PlayerPrefs.SetInt("music", 0);
        }
        else if (isOn)
        {
            PlayerPrefs.SetInt("music", 1);
        }
    }

    public void PlayMusic()
    {
        ad.clip = musicTracks[currentTrackIndex]; // ���������� ������� ����
        ad.Play(); // ��������� ���������������
    }
}
