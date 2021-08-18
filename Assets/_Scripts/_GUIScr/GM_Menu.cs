using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GM_Menu : MonoBehaviour
{

    public GameObject Setting_GUI;
    
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(string StartGame)
    {
        SceneManager.LoadScene(StartGame);
    }

      public void ReGame(string ReGame)
    {
        SceneManager.LoadScene(ReGame);
    }

      public void ReturnTitle(string ReturnTitle)
    {
        SceneManager.LoadScene(ReturnTitle);
    }

    // public void Setting()
    // {
    //     if (Setting_GUI.activeSelf == false)
    //     {
    //         Setting_GUI.SetActive(true);
    //     }
    //     else
    //     {
    //         Setting_GUI.SetActive(false);
    //     }
    // }

    public void ExitGame()
    {
        Application.Quit();
    }

    
}
