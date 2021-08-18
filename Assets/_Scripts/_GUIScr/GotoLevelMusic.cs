using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoLevelMusic : MonoBehaviour
{

    public GameObject Manager;

    void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Music");
        if (obj == null)
        {
            obj = (GameObject)Instantiate(Manager);
        }
    }

}
