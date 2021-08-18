using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class characterBase : MonoBehaviour
{
    [Header("°òÂ¦¶Ë®`")]
    public int AttackMax = 50;
    public int AttackMin = 30;

    public abstract void OnDamage(float damage);

    public abstract int DamageValue(float attackMultiple);
}
