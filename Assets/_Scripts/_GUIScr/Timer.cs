using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    public Text 計時板;
    private float StartTime;
    private bool finnished = false;

    private int BossHp;

    // Use this for initialization
    void Start()
    {
        StartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (finnished)
        {
            return;
        }

        float t = Time.time - StartTime;

        string minutes = ((int)t / 60).ToString("D2");
        string seconds = (t % 60).ToString("f2");

        計時板.text = minutes + ":" + seconds;
    }

    public void Finnish()
    {
		
        finnished = true;
        計時板.color = Color.yellow;

    }

}
