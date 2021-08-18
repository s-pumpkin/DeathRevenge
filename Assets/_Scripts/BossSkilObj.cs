using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkilObj : MonoBehaviour
{

    public float speed = 100;
    bool 碰到地面 = false;

    public AudioSource AudioSource;
    public AudioClip 落下;
    public AudioClip 擊中地面;
    float _AudioVul;

    // Use this for initialization
    void Start()
    {
        if (落下 != null)
        {
            AudioSource.volume = _AudioVul * 0.5f;
            AudioSource.PlayOneShot(落下);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _AudioVul = PlayerPrefs.GetFloat("AudioEffects", 0.5f);
        if (!碰到地面)
        {
            transform.Translate(0, speed * Time.deltaTime, 0);
        }
        else
        {
            transform.position = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "階梯")
        {
            碰到地面 = true;
            gameObject.GetComponent<MeshCollider>().enabled = false;
            if (擊中地面)
            {
                AudioSource.volume = _AudioVul * 0.5f;
                AudioSource.PlayOneShot(擊中地面);
            }
            Destroy(gameObject, 3f);
        }
        else if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject, 0.1f);
        }
    }

}
