using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitController : MonoBehaviour
{
    public characterBase mycharacter;
    public float attack;
    public float AttackMultiple = 1;
    private float hitCD = 0;
    private float hitRecovery = 0.5f;

    private void Update()
    {
        hitCD -= Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(this.tag))
            return;

        if (hitCD > 0)
            return;

        characterBase cb = GM_Level.characterBaseDictionary[other.gameObject.transform.parent.gameObject];
        if (cb != null)
        {
            float damage = mycharacter == null ? attack : mycharacter.DamageValue(AttackMultiple);
            cb.OnDamage(damage);
            hitCD = hitRecovery;
        }
        else
        {
            Debug.LogError(other.gameObject.transform.parent.gameObject + "is not in the characterBaseDictionary");
        }
    }
}
