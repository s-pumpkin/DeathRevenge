using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBall : MonoBehaviour
{

    private Transform PlayerTr;
    public float speed = 0.5f;
    [Tooltip("延遲(??)開始移動")]
    public float 延遲 = 1f;

    // Use this for initialization
    void Start()
    {
        PlayerTr = PlayerCtr.Instance.transform;
        StartCoroutine(move());
    }

    IEnumerator move()
    {
        yield return new WaitForSeconds(延遲);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerTr.position, speed);
            yield return null;
        }
    }


}
