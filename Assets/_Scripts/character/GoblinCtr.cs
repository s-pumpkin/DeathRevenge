using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class GoblinCtr : characterBase
{

    public int HpMax = 500;

    private Transform PlayerTr; // 玩家位置
    private Vector3 NewGoPos;

    public PathData movePath = new PathData();
    public Transform PatrolTrPosA; // 巡邏座標A
    public Transform PatrolTrPosB; // 巡邏座標B

    private NavMeshAgent NvAgent;

    // private float distance; // 與玩家的距離 
    private int HP;

    // public float Speed = 15f;
    public float 距離1 = 100; // 玩家小於距離1使用衝撞
    public float 衝撞CD = 30.0f;
    public float 衝撞速度 = 5f;
    public GameObject 衝撞Object;
    [Range(0, 100)]
    public int 衝撞發動機率 = 40;

    private float reCD;
    private Vector3 newPlayerPos;
    public float 揮擊距離; // 小於距離2使用揮打
    private float distance;


    private int 模式 = 0; // 1揮擊 2衝撞 3追玩家
    public bool 是否巡邏 = true;
    public bool 是否使用衝撞 = true; //準許
    public bool 是否追玩家 = true; //準許
    bool 是否CD衝撞 = false;
    private bool StopmoveAtoB = false;
    public bool isDetectedPlayer = false;
    private bool isDead = false;

    float 硬質CD;

    public GameObject bloodEffect;
    public GameObject _ReHpBall;

    /* ------------------------------------------------------------------------------------------------------- */
    // 動畫控制
    public enum GoblinState { walk, idel, attack1, attack2, dead };
    public GoblinState goblinState = GoblinState.walk;

    private Animator animator;

    /*---------------------------------------------------------------------------------------------------------------*/

    private float _AudioVul;
    public AudioSource _AudioSource;
    public AudioClip _AttAudio;
    public AudioClip _被打擊聲音;
    public AudioClip _DeadAudio;
    public float 播放聲音比 = 0.25f;

    private void Awake()
    {
        GM_Level.characterBaseDictionary.Add(this.gameObject, this);

    }

    // Use this for initialization
    void Start()
    {
        HP = HpMax;
        reCD = 衝撞CD;
        PlayerTr = PlayerCtr.Instance.transform; //取得玩家Transform
        animator = this.GetComponent<Animator>();
        NvAgent = this.GetComponent<NavMeshAgent>(); //抓取導航
        NewGoPos = PatrolTrPosA.position; //初始巡邏位置
        newPlayerPos = Vector3.zero;
        StartCoroutine(CheckGoblinState());
        StartCoroutine(NowState());
        StartCoroutine(moveAtoB());
        StartCoroutine(reatt2());
        StartCoroutine(random());
        StartCoroutine(movetoPlayer());
        StartCoroutine(att1CD());
    }


    void Update()
    {
        硬質刷新();
        _AudioVul = PlayerPrefs.GetFloat("AudioEffects", 0.5f);
    }


    // 偵測玩家距離&控制技能使用
    IEnumerator CheckGoblinState()
    {
        while (!isDead)
        {
            //計算玩家是否在怪物視角
            Vector3 direction = PlayerTr.position - this.gameObject.transform.position;
            float degree = Vector3.Angle(direction, this.gameObject.transform.forward);
            Ray r = new Ray();
            r.origin = this.gameObject.transform.position;
            r.direction = direction;
            RaycastHit hitInfo;

            if (Physics.Raycast(r, out hitInfo, 距離1 + 30)) //發出點,儲存資料,距離,忽略
            {
                if (hitInfo.transform == PlayerTr)
                {
                    distance = Vector3.Distance(transform.position, PlayerTr.position);
                }
                else
                {
                    distance = 1000;
                }
            }

            // 與玩家距離
            yield return null;

            if (distance < 距離1 && distance > 距離1 / 1.1f && !是否CD衝撞 && 是否使用衝撞 && isAtt2 < 衝撞發動機率)
            {
                NvAgent.isStopped = true; // 停止導航
                isDetectedPlayer = true;
                模式 = 2;
                StartCoroutine(Att2());
                是否CD衝撞 = true;
            }
            else if (distance < 距離1 && distance > 揮擊距離 && 模式 != 2 && 模式 != 3)
            {
                NvAgent.isStopped = false;
                isDetectedPlayer = true;
                模式 = 3;
            }

            yield return null;
        }
    }

    int isAtt2;
    // 機率使用
    IEnumerator random()
    {
        while (!isDead)
        {
            isAtt2 = Random.Range(0, 100);
            yield return new WaitForSeconds(5f);
        }
    }

    // CD Att2
    IEnumerator reatt2()
    {
        while (!isDead)
        {
            if (是否CD衝撞)
            {
                yield return new WaitForSeconds(1f);
                reCD -= 1;
                if (reCD == 0)
                {
                    reCD = 衝撞CD;
                    是否CD衝撞 = false;
                }
            }
            yield return null;
        }
    }

    //目前狀態 動畫時間要改
    IEnumerator NowState()
    {
        while (!isDead)
        {
            switch (goblinState)
            {
                case GoblinState.idel:
                    animator.SetTrigger("idel");
                    break;
                case GoblinState.walk:
                    animator.SetTrigger("Run");
                    break;
                case GoblinState.attack1:
                    animator.SetTrigger("Att1");
                    break;
                case GoblinState.attack2:
                    animator.SetTrigger("Crash");
                    break;
            }
            yield return null;
        }
    }

    // 巡邏 A to B
    IEnumerator moveAtoB()
    {
        while (!isDead)
        {
            if (!isDetectedPlayer && 是否巡邏)
            {
                NvAgent.stoppingDistance = 0f;
                NvAgent.destination = NewGoPos;
                goblinState = GoblinState.walk;
                yield return null;
                if (NvAgent.remainingDistance < 1.5f)
                {
                    // Debug.Log(1);
                    if (NewGoPos == PatrolTrPosA.position)
                    {
                        NewGoPos = PatrolTrPosB.position;
                    }
                    else if (NewGoPos == PatrolTrPosB.position)
                    {
                        NewGoPos = PatrolTrPosA.position;
                    }
                    goblinState = GoblinState.idel; //到達目的地 等待X秒
                    yield return new WaitForSeconds(5.0f);
                    // Debug.Log(2);
                }
            }
            else
            {
                yield break;
            }
        }
    }

    public GameObject 衝刺特效;
    // 技能 衝撞
    IEnumerator Att2()
    {
        if (模式 == 2 && 是否使用衝撞)
        {
            if (newPlayerPos == Vector3.zero)
            {
                newPlayerPos = PlayerTr.localPosition - new Vector3(0, 0, 3f); //取得玩家座標 
            }
            // GameObject DE = (GameObject)Instantiate(衝刺特效, transform.position, Quaternion.identity);
            while (true)
            {
                transform.LookAt(newPlayerPos); // 面相座標玩家移動前的座標A
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                goblinState = GoblinState.attack2;
                transform.position = Vector3.MoveTowards(transform.position, newPlayerPos, 衝撞速度);
                float newPosDis = Vector3.Distance(transform.position, newPlayerPos);
                if (newPosDis < 0.2f || 碰到牆壁)
                {
                    衝撞Object.SetActive(false);
                    // Destroy(DE);
                    newPlayerPos = Vector3.zero;
                    goblinState = GoblinState.idel;
                    碰到牆壁 = false;
                    模式 = 0;
                    yield break;
                }
                yield return null;
            }
        }
    }

    // 追玩家 & 攻擊
    IEnumerator movetoPlayer()
    {
        while (!isDead)
        {
            // 會連續攻擊                          下面
            if (模式 == 3 && 是否追玩家)
            {
                while (true)
                {
                    if (模式 == 2 || PlayerCtr.Instance.isDead)
                    {
                        yield break;
                    }
                    NvAgent.stoppingDistance = 揮擊距離;
                    NvAgent.destination = PlayerTr.position;

                    if (distance <= 揮擊距離) //如果要改在這以下改
                    {
                        NvAgent.SetDestination(transform.position); //(停止移動) 定位成自己 不能砍
                        Vector3 dir = PlayerTr.position - transform.position;
                        Quaternion lookPlayer = Quaternion.LookRotation(dir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookPlayer, 0.6f);
                        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

                        if (isatt1)
                        {
                            goblinState = GoblinState.attack1;
                            isatt1 = false;
                        }
                        else
                        {
                            goblinState = GoblinState.idel;
                        }

                    }
                    else if (distance > 揮擊距離)
                    {
                        goblinState = GoblinState.walk;
                    }
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    bool isatt1 = true;
    IEnumerator att1CD()
    {
        while (!isDead)
        {
            if (!isatt1)
            {
                yield return new WaitForSeconds(2f);
                isatt1 = true;
            }
            yield return null;
        }

    }


    private bool 碰到牆壁 = false;
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag != "isground")
        {
            // Debug.Log("TOne");
            碰到牆壁 = true;
        }
        else
        {
            碰到牆壁 = false;
        }
    }

    void 硬質刷新()
    {
        硬質CD += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerAtt1" || other.tag == "PlayerAtt2" || other.tag == "PlayerAtt3")
        {
            CreateBloodEffect();
            PlayerReHP();
            _AudioSource.volume = _AudioVul * 播放聲音比;  //音效
            _AudioSource.PlayOneShot(_被打擊聲音);
            if (硬質CD >= 3) //硬質CD
            {
                硬質CD = 0;
                NvAgent.SetDestination(transform.position);
                animator.SetTrigger("Hard");
            }
        }
    }

    public void isDeath()
    {
        StopAllCoroutines();
        NvAgent.isStopped = true;
        isDead = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        _AudioSource.volume = _AudioVul * 播放聲音比;
        _AudioSource.PlayOneShot(_DeadAudio);
        animator.SetTrigger("Dead");
        if (LevelCtr.instance != null)
            LevelCtr.instance.RemoveMonster(this.gameObject);
        Destroy(gameObject, 5.0f);
    }

    void CreateBloodEffect()
    {
        Debug.Log("噴寫");
        //噴血粒子特效
        GameObject blood = (GameObject)Instantiate(bloodEffect, new Vector3(transform.position.x, transform.position.y + Random.Range(1, 4), transform.position.z), Quaternion.identity);
        Destroy(blood, 2.5f); //銷毀噴血粒子效果
    }

    void PlayerReHP()
    {
        int nYpos = Random.Range(6, 10);
        int LorR = Random.Range(1, 3);
        float nXpos = 0;

        switch (LorR)
        {
            case 1:
                nXpos = Random.Range(-7f, -4f);
                break;
            case 2:
                nXpos = Random.Range(4f, 7f);
                break;
        }

        if (_ReHpBall != null)
        {
            // Instantiate(_ReHpBall, new Vector3(transform.localPosition.x + nXpos, transform.localPosition.y + nYpos, transform.localPosition.z), Quaternion.identity);
            Instantiate(_ReHpBall, new Vector3(transform.position.x + nXpos, transform.position.y + nYpos, transform.position.z), Quaternion.identity);
            // Instantiate(_ReHpBall, new Vector3(nXpos, nYpos, 0), Quaternion.identity);
        }
    }

    void attaudio()
    {
        _AudioSource.volume = _AudioVul * 播放聲音比;
        _AudioSource.PlayOneShot(_AttAudio);
    }

    public override void OnDamage(float damage)
    {
        if (!isDead && HP > 0)
        {
            HP -= (int)damage;
            if (HP <= 0)
            {
                isDead = true;
                isDeath();
            }
        }
    }

    public override int DamageValue(float attackMultiple)
    {
        return Mathf.RoundToInt(Random.Range(AttackMin, AttackMax) * attackMultiple);
    }

    private void OnDestroy()
    {
        GM_Level.characterBaseDictionary.Remove(this.gameObject);
    }
}