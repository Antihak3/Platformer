using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public player player;
    public Text coinText;
    public Image[] hearts;
    public Sprite isLise, monLise;
    public GameObject PauseScreen;
    public GameObject invenScreen;
    public GameObject inveon; public GameObject inveoff; public GameObject pauseknop;
    public GameObject WinScreen;
    public GameObject LosesCreen;
    float timer = 0;
    public Text timetext;
    public TimeWork timeWork;
    public float countdown;
    public soundeff soundeff;
    public AudioSource musicSource, soundSource; 
  public void ReloadLvl()
    {
        Time.timeScale = 1f;
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    private void Start()
    {
        if ((int)timeWork == 2)
            timer = countdown;

        musicSource.volume = (float)PlayerPrefs.GetInt("MusicVolume") / 9;
        soundSource.volume = (float)PlayerPrefs.GetInt("SoundVolume") / 9;
    }

    public void Update()
    {
        coinText.text = player.GetCoins().ToString();
        for (int i = 0; i < hearts.Length; i++)
        {
            if (player.GetHP() > i)
                hearts[i].sprite = isLise;
            else
                hearts[i].sprite = monLise;
        }

        if ((int)timeWork == 1)
        {
            timer += Time.deltaTime;
            timetext.text = timer.ToString("F2").Replace(",", ":");
        }
        else if ((int)timeWork == 2)
        {
            timer -= Time.deltaTime;
            // timetext.text = timer.ToString("F2").Replace(",", ":");
            timetext.text = ((int)timer / 60).ToString() + ":" + ((int)timer - ((int)timer / 60) * 60).ToString("D2");
            if (timer <= 0)
                Lose();
        }
        else 
            timetext.gameObject.SetActive(false);
    }


    public void PauseOn()
    {
        Time.timeScale = 0f;
        player.enabled = false;
        PauseScreen.SetActive(true);
    }

    public void PauseOff()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        PauseScreen.SetActive(false);
    }

    public void Win()
    {
        soundeff.PlayWinSound();
        Time.timeScale = 0f;
        player.enabled = true;
        WinScreen.SetActive(true);
        if (!PlayerPrefs.HasKey("Lvl") || PlayerPrefs.GetInt("Lvl") < SceneManager.GetActiveScene().buildIndex)
            PlayerPrefs.SetInt("Lvl", SceneManager.GetActiveScene().buildIndex);
        print(PlayerPrefs.GetInt("Lvl"));

        if (PlayerPrefs.HasKey("coins"))
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + player.GetCoins());
        else
            PlayerPrefs.SetInt("coins", player.GetCoins());

        print(PlayerPrefs.GetInt("coins"));

        invenScreen.SetActive(false);
        inveon.SetActive(false);
        inveoff.SetActive(false);
        GetComponent<inventari>().RecountItems();
        pauseknop.SetActive(false);
    }

    public void Lose()
    {
        soundeff.PlayLoseSound();
        Time.timeScale = 0f;
        player.enabled = true;
        LosesCreen.SetActive(true);

        invenScreen.SetActive(false);
        inveon.SetActive(false);
        inveoff.SetActive(false);
        GetComponent<inventari>().RecountItems();
        pauseknop.SetActive(false);
    }

    public void MenuLvl()
    {
        SceneManager.LoadScene("Menu");
    }

    public void NextLvl()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void invenOn()
    {
        inveon.SetActive(false);
        inveoff.SetActive(true);
        invenScreen.SetActive(true);
    }

    public void invenOff()
    {
        inveon.SetActive(true);
        inveoff.SetActive(false);
        invenScreen.SetActive(false);
    }
}

public enum TimeWork
{
    None,
    Stopwatch,
    Timer
}