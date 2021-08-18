using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openA : MonoBehaviour
{

    
    public GM_Level GM_Level;
	public GameObject camera;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

	void TimelineStop()
	{
		GM_Level.enabled=true;
		camera.SetActive(true);		
	}
}
