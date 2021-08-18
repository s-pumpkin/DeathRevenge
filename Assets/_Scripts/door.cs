using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{

    public bool 生成 = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        生成 = true;
        
    }

    private void OnTriggerExit(Collider other)
    {
        生成 = false;
        gameObject.GetComponent<Collider>().enabled = false;
    }
}
