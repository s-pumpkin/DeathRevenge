using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Set背景音 : MonoBehaviour
{

    public AudioSource AudioSource;
    public AudioClip Level;
    public AudioClip Level_2;

    // Use this for initialization
    private void Awake()
    {

    }


    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Level_2")
        {
            if (AudioSource.clip != Level_2)
            {
                AudioSource.clip = Level_2;
                AudioSource.Play();
            }
        }
        else if (scene.name == "Level" || scene.name == "Start")
        {
            if (AudioSource.clip != Level)
            {
                AudioSource.clip = Level;
                AudioSource.Play();
            }
        }
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("AudioBack", 0.5f);

    }
}
