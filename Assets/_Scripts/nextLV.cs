using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextLV : MonoBehaviour {

	private void OnTriggerEnter(Collider other) {
		SceneManager.LoadScene("Level_2");
	}

}
