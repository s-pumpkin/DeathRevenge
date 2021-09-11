using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class GoblinCtr : characterBase
{

    public int HpMax = 500;
    private int HP;

    private Transform PlayerTr; // 玩家位置
    private Vector3 NowGoPos;

    public Transform PatrolTrPosA; // 巡邏座標A
    public Transform PatrolTrPosB; // 巡邏座標B

    private NavMeshAgent NvAgent;

    /// <summary>
    /// 與玩家的距離 
    /// </summary>
    private float distance = 99999;
    public float checkPlayerDistance;

    [Header("手動准許功能")]
    public bool CanPatrol = true;
    public bool CanBump = true;
    public bool CanRunPlayer = true;

    /// <summary>
    /// 怪物狀態
    /// </summary>
    public enum GoblinState { None, attack, bump, runToPlayer }
    public GoblinState goblinState = GoblinState.runToPlayer;

    public bool isIdel;
    private float IdelCD;

    /// <summary>
    /// 近戰攻擊
    /// </summary>
    bool isAttack1 = true;
    public float SwingDistance;

    /// <summary>
    /// 衝刺
    /// </summary>
    // public float Speed = 15f;
    public float CheckbumpDistance = 100;
    public float 衝撞CD = 30.0f;
    private float reCD;
    public float BumpSpeed = 5f;
    public GameObject 衝撞Object;
    [Range(0, 100)]
    public int BumpProbability = 40;

    bool isBumpCD = false;
    public bool isDetectedPlayer = false;
    private bool isDead = false;

    bool isHard = false;
    float HardCD;

    private bool hirWall = false;

    public GameObject bloodEffect;
    public GameObject _ReHpBall;

    public LayerMask CheckHitLayer;
    /// <summary>
    ///  動畫控制
    /// </summary>
    [Header("動畫控制")]
    private Animator animator;
    public enum GoblinAnimState { walk, idel, attack1, attack2, dead };
    public GoblinAnimState goblinAnimState = GoblinAnimState.walk;



    private float _AudioVul = 0.5f;
    public AudioSource _AudioSource;
    public AudioClip _AttAudio;
    public AudioClip _被打擊聲音;
    public AudioClip _DeadAudio;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkPlayerDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SwingDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, CheckbumpDistance);
    }

    private void Awake()
    {
        GM_Level.characterBaseDictionary.Add(this.gameObject, this);
        _AudioVul = PlayerPrefs.GetFloat("AudioEffects", 0.5f);
    }

    // Use this for initialization
    void Start()
    {
        HP = HpMax;
        reCD = 衝撞CD;
        PlayerTr = PlayerCtr.Instance.transform; //取得玩家Transform
        animator = this.GetComponent<Animator>();

        NvAgent = this.GetComponent<NavMeshAgent>(); //抓取導航
        NvAgent.SetDestination(PatrolTrPosA.position); //初始巡邏位置
        NowGoPos = PatrolTrPosA.position;

        StartCoroutine(CheckDirection());
    }

    void Update()
    {
        if (isDead)
            return;

        if (PlayerCtr.Instance.isDead)
            return;

        HardTime();

        moveTarget();
        CheckAtteck();
    }

    private void LateUpdate()
    {
        if (isDead)
            return;

        SetAnim();
    }

    /// <summary>
    /// 目前動畫狀態
    /// </summary>
    private void SetAnim()
    {
        if (!isHard)
        {
            switch (goblinAnimState)
            {
                case GoblinAnimState.idel:
                    animator.SetTrigger("idel");
                    break;
                case GoblinAnimState.walk:
                    animator.SetTrigger("Run");
                    break;
                case GoblinAnimState.attack1:
                    animator.SetTrigger("Att1");
                    break;
                case GoblinAnimState.attack2:
                    animator.SetTrigger("Crash");
                    break;
            }
        }
        else
            animator.SetTrigger("Hard");
    }
    //檢查玩家距離
    IEnumerator CheckDirection()
    {
        while (!isDead)
        {
            RaycastHit hit;
            Vector3 Dir = (new Vector3(PlayerTr.position.x, 0, PlayerTr.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
            if (Physics.Raycast(transform.position + new Vector3(0, 3, 0), Dir, out hit, checkPlayerDistance, CheckHitLayer))
            //if (Physics.CheckSphere(transform.position, checkPlayerDistance, PlayerLayer))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    distance = Vector3.Distance(transform.position, PlayerTr.position);
                    goblinState = GoblinState.runToPlayer;
                }
                else
                {
                    distance = 99999;
                    goblinState = GoblinState.None;
                }
            }


            if (distance < SwingDistance)
            {
                goblinState = GoblinState.attack;
            }
            else if (distance < CheckbumpDistance && !isBumpCD && BumpProbability > Random.Range(0.0f, 100.0f))
            {
                goblinState = GoblinState.bump;
            }

            yield return null;
        }
    }

    private void moveTarget()
    {
        if (goblinState == GoblinState.attack || goblinState == GoblinState.bump)
            return;

        NvAgent.stoppingDistance = 0.1f;

        if (goblinState == GoblinState.runToPlayer && CanRunPlayer || !CanPatrol)
        {
            NvAgent.SetDestination(PlayerTr.position);
        }
        else if (CanPatrol)
        {
            IdelCD -= Time.deltaTime;
            if (NvAgent.remainingDistance < 1.5f)
                if (!isIdel)
                {
                    isIdel = true;
                    IdelCD = 5;
                    goblinAnimState = GoblinAnimState.idel;
                    return;
                }
                else
                {
                    if (IdelCD > 0)
                    {
                        goblinAnimState = GoblinAnimState.idel;
                        return;
                    }

                    isIdel = false;

                    //A to B , B to A
                    NowGoPos = NowGoPos == PatrolTrPosA.position ? PatrolTrPosB.position : PatrolTrPosA.position;
                    NvAgent.SetDestination(NowGoPos);
                }
        }
        goblinAnimState = GoblinAnimState.walk;
    }
    private void CheckAtteck()
    {
        if (goblinState == GoblinState.attack)
        {
            NvAgent.stoppingDistance = SwingDistance;//(停止移動) 定位成自己
            NvAgent.SetDestination(transform.position);

            Vector3 dir = PlayerTr.position - transform.position;
            Quaternion lookPlayer = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookPlayer, 0.6f);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            if (isAttack1)
            {
                goblinAnimState = GoblinAnimState.attack1;
                isAttack1 = false;
                StartCoroutine(Attack1CD());
            }
            else
            {
                goblinAnimState = GoblinAnimState.idel;
            }
        }
        else if (goblinState == GoblinState.bump && CanBump)
        {
            Vector3 BumpGoToPoition = PlayerTr.localPosition - PlayerTr.forward * 3; //取得玩家座標 

            transform.LookAt(BumpGoToPoition); // 面相座標玩家移動前的座標A
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            isBumpCD = true;
            StartCoroutine(GoblinBump(BumpGoToPoition));
        }
    }

    float bumpTime = 5;
    IEnumerator GoblinBump(Vector3 target)
    {
        while (!isDead)
        {
            bumpTime -= Time.deltaTime;
            float newPosDis = Vector3.Distance(transform.position, target);
            if (newPosDis < 0.2f || hirWall || bumpTime <= 0)
            {
                hirWall = false;
                衝撞Object.SetActive(false);
                goblinAnimState = GoblinAnimState.idel;
                bumpTime = 5;
                goblinState = GoblinState.None;

                StartCoroutine(BumpCD());
                yield break;
            }

            goblinAnimState = GoblinAnimState.attack2;
            transform.position = Vector3.MoveTowards(transform.position, target, BumpSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator Attack1CD()
    {
        yield return new WaitForSeconds(2f);
        isAttack1 = true;
    }

    // CD Att2
    IEnumerator BumpCD()
    {
        yield return new WaitForSeconds(衝撞CD);
        isBumpCD = false;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag != "isground" && goblinState == GoblinState.bump)
        {
            hirWall = true;
        }
    }

    public void isDeath()
    {
        StopAllCoroutines();
        NvAgent.isStopped = true;
        isDead = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        AudioPlay(_DeadAudio);
        animator.SetTrigger("Dead");
        if (LevelCtr.instance != null)
            LevelCtr.instance.RemoveMonster(this.gameObject);
        Destroy(gameObject, 5.0f);
    }

    /// <summary>
    /// 噴血特效
    /// </summary>
    void CreateBloodEffect()
    {
        //噴血粒子特效
        GameObject blood = Instantiate(bloodEffect, new Vector3(transform.position.x, transform.position.y + Random.Range(1, 4), transform.position.z), Quaternion.identity);
        Destroy(blood, 2.5f); //銷毀噴血粒子效果
    }

    /// <summary>
    /// 玩家生命回復球
    /// </summary>
    void PlayerReHP()
    {
        int nYpos = Random.Range(6, 10);
        float nXpos = Random.Range(1, 3) == 1 ? Random.Range(-7f, -4f) : Random.Range(4f, 7f);

        if (_ReHpBall != null)
            Instantiate(_ReHpBall, new Vector3(transform.position.x + nXpos, transform.position.y + nYpos, transform.position.z), Quaternion.identity);
    }

    void attaudio()
    {
        AudioPlay(_AttAudio);
    }

    #region 傷害&受傷
    public override void OnDamage(float damage)
    {
        if (!isDead && HP > 0)
        {
            PlayerReHP();
            CreateBloodEffect();
            AudioPlay(_被打擊聲音);

            HP -= (int)damage;
            if (HP <= 0)
            {
                isDead = true;
                isDeath();
            }
        }
    }
    void HardTime()
    {
        HardCD -= Time.deltaTime;
    }

    public void HardAnimRe()
    {
        isHard = false;
    }

    public void CheckHard()
    {
        if (HardCD <= 0) //硬質CD
        {
            HardCD = 3;
            isHard = true;
            NvAgent.SetDestination(transform.position);
        }
    }

    public override int DamageValue(float attackMultiple)
    {
        return Mathf.RoundToInt(Random.Range(AttackMin, AttackMax) * attackMultiple);
    }
    #endregion

    public void AudioPlay(AudioClip audioClip)
    {
        _AudioSource.volume = _AudioVul * .25f;
        _AudioSource.PlayOneShot(audioClip);
    }

    private void OnDestroy()
    {
        GM_Level.characterBaseDictionary.Remove(this.gameObject);
    }
}