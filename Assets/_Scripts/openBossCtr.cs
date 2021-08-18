using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openBossCtr : MonoBehaviour
{

    public BossCtr BossCtr;
    public bool OpBossLine = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(OpenBossCtr());
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }


    IEnumerator OpenBossCtr()
    {
        yield return new WaitForSeconds(2f);
        BossCtr.enabled = true;
        OpBossLine = true;
    }

}
