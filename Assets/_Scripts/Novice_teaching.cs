using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Novice_teaching : MonoBehaviour {
    public Flowchart teaching;
    public string onCollosionEnter;
   
    Rigidbody PlayerRigidbody;
    // Use this for initialization
   
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
   
    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.CompareTag("Player"))
            {
            
                Block targetBlock = teaching.FindBlock(onCollosionEnter);
                teaching.ExecuteBlock(targetBlock);
            }
        Destroy(gameObject);
    }
    }




