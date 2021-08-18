using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GM_Level : MonoBehaviour
{
    public bool 設定;
    bool playerDie = false;
    bool BossDie = false;

    public static Dictionary<GameObject, characterBase> characterBaseDictionary = new Dictionary<GameObject, characterBase>();

    void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Level")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        SceneManager.LoadScene("HP_GUI", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        DownESC();

        if (PlayerCtr.Instance.isDead && !playerDie)
        {
            playerDie = true;
            SceneManager.LoadScene("Level_Dead", LoadSceneMode.Additive);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (BossCtr.Instance && BossCtr.Instance.isDead && !BossDie)
        {
            BossDie = true;
            SceneManager.LoadScene("Level_clear", LoadSceneMode.Additive);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    private void DownESC()
    {
        if (!設定 && !playerDie && !BossDie)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("Setting", LoadSceneMode.Additive);
                設定 = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.UnloadScene("Setting");
            設定 = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }
}


