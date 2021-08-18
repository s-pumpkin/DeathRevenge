using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Play_GUILine : MonoBehaviour
{
    /* -------------------------------生命-------------------------------------- */

    private int MaxHP;

    private int HP;
    private float HPLine;
    private Image HP_RedImage;
    public Image HP_BlackImage;
    /* -------------------------------耐力--------------------------------------- */

    private int MaxEnergy;

    private int Energy;
    private float EnergyLine;
    private Image Energy_GreenImage;
    public Image Energy_BlackImage;
    /* -------------------------------BossHP-------------------------------------- */

    private int MaxBossHp;

    private int BossHp;
    private float BossHpLine;
    private Image BossHp_GreenImage;
    public Image BossHp_BlackImage;

    public GameObject BossHPLine;

    public openBossCtr OpenBossCtr;
    // Use this for initialization
    void Start()
    {
        HP_RedImage = GameObject.Find("Red").GetComponent<Image>();
        Energy_GreenImage = GameObject.Find("Green").GetComponent<Image>();
        BossHp_GreenImage = GameObject.Find("Orange").GetComponent<Image>();
        BossHPLine.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        血量條();
        能量條();
        Boss血量條();
    }

    void 血量條()
    {
        //讀取HP&HPMax
        MaxHP = PlayerCtr.Instance.HpMax;
        HP = PlayerCtr.Instance.HP;
        HPLine = (float)HP / MaxHP;
        HP_RedImage.fillAmount = HPLine;
        HP_BlackImage.fillAmount = Mathf.MoveTowards(HP_BlackImage.fillAmount, HPLine, Time.deltaTime * 0.2f);
    }

    void 能量條()
    {
        //讀取Energy&EnergyMax
        MaxEnergy = PlayerCtr.Instance.EnergyMax;
        Energy = PlayerCtr.Instance.Energy;
        EnergyLine = (float)Energy / MaxEnergy;
        Energy_GreenImage.fillAmount = EnergyLine;
        Energy_BlackImage.fillAmount = Mathf.MoveTowards(Energy_BlackImage.fillAmount, EnergyLine, Time.deltaTime * 0.2f);
    }
    void Boss血量條()
    {
        if (BossCtr.Instance)
        {
            if (OpenBossCtr == null)
                OpenBossCtr = GameObject.FindWithTag("橋").GetComponent<openBossCtr>();
            else if (OpenBossCtr.OpBossLine)
            {
                if (BossHPLine.activeSelf == false)
                {
                    BossHPLine.SetActive(true);
                }
            }

            //讀取Boss血量條&Boss血量條
            MaxBossHp = BossCtr.Instance.HpMax;
            BossHp = BossCtr.Instance.Hp;
            Debug.Log(BossHp);
            BossHpLine = (float)BossHp / MaxBossHp;
            BossHp_GreenImage.fillAmount = BossHpLine;
            BossHp_BlackImage.fillAmount = Mathf.MoveTowards(BossHp_BlackImage.fillAmount, BossHpLine, Time.deltaTime * 0.2f);
        }

    }
}