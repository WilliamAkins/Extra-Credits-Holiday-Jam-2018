using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuNav : MonoBehaviour
{
    public Button PlayGame;
    public Button ExitGame;

    void Start()
    {
        PlayGame.onClick.AddListener(Play);
        ExitGame.onClick.AddListener(Exit);
    }

    void Play()
    {
        SceneManager.LoadScene("MainScene");
    }

    void Exit()
    {
        Application.Quit();
    }

}
