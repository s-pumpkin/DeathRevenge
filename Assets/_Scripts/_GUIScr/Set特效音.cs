using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set特效音 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.GetComponent<AudioSource>().volume=PlayerPrefs.GetFloat("AudioEffects", 0.5f);
	}
}
