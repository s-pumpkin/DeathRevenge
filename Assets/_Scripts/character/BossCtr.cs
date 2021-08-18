using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossCtr : characterBase
{
    public static BossCtr Instance;
    public int HpMax = 1500;
    // [HideInInspector]
    public int Hp;
    public float 招式間隔CD = 2f;
    private Transform PlayerTr; // 玩家位置

    /* ------------------------------------------------------------------------------------------------------- */
    // 動畫控制                         1        2        3          4        5       6
    public enum GoblinState { idel, attack1, attack2, attack3, SumMonter, SumSkill, hurt };
    public GoblinState goblinState = GoblinState.idel;

    private Animator animator;
    private bool animEnd = false;
    /* ------------------------------------------------------------------------------------------------------------ */

    public Collider[] BossCollider;

    [Header("召喚")]
    public GameObject 生成生物;
    public int 生成次數;
    public Transform 生成地點L;
    public Transform 生成地點R;
    public float 生成間隔 = 1.0f;

    [Header("技能")]
    public GameObject 墜石;
    public Transform 生成地點UP;

    public int 攻擊模式 = 0;
    private int 上個攻擊模式 = 0;
    private bool 是否進行攻擊 = false;
    [HideInInspector]
    public bool isDead = false;

    private int 切受傷 = 0;//滿5次受傷

    bool StartSum = false;
    bool StartSkill = false;

    [Space(10)]
    private float _AudioVul;
    public AudioSource Boss揮擊RSource;
    public AudioSource Boss揮擊LSource;
    public AudioClip Boss揮擊;

    public AudioSource 通用Source;
    public AudioClip BOSS垂地板;
    public AudioClip BOSS倒下來;
    public AudioClip 砍BOSS骨頭;


    public AudioClip BOSS死亡;



    private void Awake()
    {
        Instance = this;
        GM_Level.characterBaseDictionary.Add(this.gameObject, this);
    }

    // Use this for initialization
    void Start()
    {
        Hp = HpMax;
        animator = this.GetComponent<Animator>();
        PlayerTr = PlayerCtr.Instance.transform; //取得玩家Transform
        StartCoroutine(RandomSelectionMode());
        StartCoroutine(NowState());
        // StartCoroutine(攻擊Mode());
    }

    // Update is called once per frame
    void Update()
    {
        _AudioVul = PlayerPrefs.GetFloat("AudioEffects", 0.5f);
        // Debug.Log(攻擊模式);
    }


    //選擇招式
    IEnumerator RandomSelectionMode()
    {
        while (!isDead)
        {
            Debug.Log("attmode");
            攻擊模式 = Random.Range(1, 6);
            while (攻擊模式 == 上個攻擊模式)
            {
                攻擊模式 = Random.Range(1, 6);
                yield return null;
            }
            上個攻擊模式 = 攻擊模式;
            切受傷 += 1;
            if (切受傷 == 5)
            {
                切受傷 = 0;
                攻擊模式 = 6;
            }
            yield return StartCoroutine(攻擊Mode());
            // Debug.Log("MODE回歸");
            yield return new WaitForSeconds(招式間隔CD);
        }
    }

    //動畫控制
    IEnumerator NowState()
    {
        while (!isDead)
        {
            switch (goblinState)
            {
                case GoblinState.idel:
                    animator.SetTrigger("idel");
                    break;
                case GoblinState.attack1:
                    animator.SetTrigger("Att1");
                    break;
                case GoblinState.attack2:
                    animator.SetTrigger("Att2");
                    break;
                case GoblinState.attack3:
                    animator.SetTrigger("Att3");
                    break;
                case GoblinState.SumMonter:
                    animator.SetTrigger("SumMonter");
                    break;
                case GoblinState.SumSkill:
                    animator.SetTrigger("SumSkill");
                    break;
                case GoblinState.hurt:
                    animator.SetTrigger("hurt");
                    break;
            }
            yield return null;
        }
    }

    IEnumerator 攻擊Mode()
    {
        switch (攻擊模式)
        {
            case 1: //att1
                goblinState = GoblinState.attack1;
                Boss揮擊RSource.volume = _AudioVul * 0.5f;  //音效
                Boss揮擊RSource.PlayOneShot(Boss揮擊);
                while (!animEnd)
                {
                    yield return null;
                }
                animEnd = false;
                break;
            case 2: //att2
                goblinState = GoblinState.attack2;
                Boss揮擊LSource.volume = _AudioVul * 0.5f;  //音效
                Boss揮擊LSource.PlayOneShot(Boss揮擊);
                while (!animEnd)
                {
                    yield return null;
                }
                animEnd = false;
                break;
            case 3: //att3
                goblinState = GoblinState.attack3;
                while (!animEnd)
                {
                    yield return null;
                }
                animEnd = false;
                break;
            case 4:      //招喚   寫召喚
                goblinState = GoblinState.SumMonter;
                while (!animEnd)
                {
                    yield return null;
                }
                animEnd = false;
                break;
            case 5:      //法術
                goblinState = GoblinState.SumSkill;
                while (!animEnd)
                {
                    yield return null;
                }
                animEnd = false;
                break;
            case 6:      //受傷
                goblinState = GoblinState.hurt;
                while (!animEnd)
                {
                    yield return null;
                }
                animEnd = false;
                break;
        }
    }

    IEnumerator Summon()
    {
        // Debug.Log("生怪");

        for (int i = 0; i < 生成次數; i++)
        {
            Instantiate(生成生物, 生成地點L);
            Instantiate(生成生物, 生成地點R);
            // Debug.Log("迴圈");
            yield return new WaitForSeconds(生成間隔);
        }
        // Debug.Log("出來迴圈");
        StartSum = false;

    }

    IEnumerator Skin()
    {
        Debug.Log("法術");

        while (StartSkill)
        {
            生成地點UP.transform.position = new Vector3(PlayerTr.position.x, 生成地點UP.transform.position.y, PlayerTr.position.z);
            Instantiate(墜石, 生成地點UP);
            yield return new WaitForSeconds(0.5f);
        }
    }



    void Anim()
    {
        StartSum = false;
        StartSkill = false;
        animEnd = true;
        goblinState = GoblinState.idel;
    }

    void OnSum()
    {
        StartSum = true;
        StartCoroutine(Summon());
    }

    void OnSkill()
    {
        StartSkill = true;
        StartCoroutine(Skin());
    }

    void BossHut()
    {
        if (BossCollider[0].enabled)
        {
            BossCollider[0].enabled = false;
            BossCollider[1].enabled = false;
            BossCollider[2].enabled = false;
            BossCollider[3].enabled = false;
        }
        else
        {
            BossCollider[0].enabled = true;
            BossCollider[1].enabled = true;
            BossCollider[2].enabled = true;
            BossCollider[3].enabled = true;
        }

    }

    void BossAudioHum()
    {
        通用Source.volume = _AudioVul * 0.5f;  //音效
        通用Source.PlayOneShot(BOSS垂地板);
    }

    void BossAudioHut()
    {
        通用Source.volume = _AudioVul * 0.5f;  //音效
        通用Source.PlayOneShot(BOSS倒下來);
    }


    //HP&Dead
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerAtt1" || other.tag == "PlayerAtt2" || other.tag == "PlayerAtt3")
        {
            if (砍BOSS骨頭 != null)
            {
                通用Source.volume = _AudioVul * 0.5f;  //音效
                通用Source.PlayOneShot(砍BOSS骨頭);
            }
        }
    }

    public void isDeath()
    {
        StopAllCoroutines();
        animator.SetTrigger("Dead");
        if (BOSS死亡 != null)
        {
            通用Source.volume = _AudioVul * 0.5f;  //音效
            通用Source.PlayOneShot(BOSS死亡);
        }
    }

    public override void OnDamage(float damage)
    {
        if (!isDead && Hp > 0)
        {
            Hp -= (int)damage;
            if (Hp <= 0)
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
