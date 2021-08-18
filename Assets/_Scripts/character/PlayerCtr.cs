using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtr : characterBase
{
    public static PlayerCtr Instance;

    private CharacterController _controller;
    public Camera MainCamera;
    public float Gravity = -15.0f;

    [Header("基礎能力")]
    public int HpMax = 300; //初始最大HP
    public int HP;
    public int EnergyMax = 250; //初始最大Energy
    public int Energy;
    public float SpeedBase = 2f;
    [Tooltip("旋轉速度")]
    public float RotationSmoothTime = 0.12f;

    public float EnergyRecoverySpeed = 3f; //能量恢復速度

    private float speed;
    private float SpeedChangeRate = 10.0f;
    private float targetRotation = 0.0f;
    private float rotationVelocity;

    [Header("技能能力")]
    public float DodgeDistance = 15f;
    public GameObject DodgingEffect;
    public float DodgeOverTime = 2.0f;
    private float DodgeCountdown = 2f;

    [Header("攻擊狀態")]
    public AttackState attackState = AttackState.attack1;
    public enum AttackState { attack1, attack2, attack3 };
    private float attackRe = 5;

    private Vector3 DodgeStartPos;
    private Vector3 newDodgePos; //閃避位移座標
    private float movementReached = 0;

    [HideInInspector]
    public bool isDead = false;
    // 攻擊
    public bool isAttack;
    // 能量
    bool isEnergy;
    // 閃避
    public bool isDodge;

    private float _verticalVelocity;
    private float _terminalVelocity = 50;

    [Header("Player Grounded")]
    public bool isGround = true;
    public float GroundedOffset = -0.14f;
    [Tooltip("腳接地檢查的半徑。 應該匹配 CharacterController 的半徑")]
    public float GroundedRadius = 0.28f;
    [Tooltip("角色使用哪些層作為地面")]
    public LayerMask GroundLayers;
    /* ---------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    // 每招攻擊力倍率
    [SerializeField]
    private float AttackRate1 = 1.0f;
    [SerializeField]
    private float AttackRate2 = 1.5f;
    [SerializeField]
    private float AttackRate3 = 2.0f;

    // 招式花費的能量
    [SerializeField]
    private int EnergyAtt1 = 30;
    [SerializeField]
    private int EnergyAtt2 = 40;
    [SerializeField]
    private int EnergyAtt3 = 50;
    [SerializeField]
    private int EnergyDodge = 50; //閃避
    // 給怪物的傷害
    public int PlayerHurt;



    /* ---------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    //public int HPRecovery = 5;



    [System.Serializable]
    public class BossHut
    {
        public int 揮擊 = 100;
        public int 垂 = 120;
        public int 墜石 = 20;
    }
    public BossHut _BossHut;
    /* ---------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [Header("動畫")]
    public AniClip _AnimClip;
    [System.Serializable]
    public class AniClip
    {
        public AnimationClip IdleClip;
        public AnimationClip RunClip;
        public AnimationClip Attack1Clip;
        public AnimationClip Attack2Clip;
        public AnimationClip Attack3Clip;
        public AnimationClip DeathClip;
        public AnimationClip RollClip;
    }

    public Animation _animation;
    /* ---------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public AudioClip DodgeAudio;
    public AudioClip AttAudio;
    public AudioClip DeadAudio;
    public AudioSource _AudioSource;
    public AudioSource GAPaudiosource;

    float _AudioVul;

    private void Awake()
    {
        Instance = this;
        GM_Level.characterBaseDictionary.Add(this.gameObject, this);
        MainCamera = Camera.main;
    }
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        HP = HpMax;
        Energy = EnergyMax;
        StartCoroutine(EnergyAutoIncrease());
    }

    void Update()
    {
        if (isDead)
            return;

        yAxisModify();
        AttackAndDodge();
        Move();
    }

    public void yAxisModify()
    {
        if (isGround)
        {
            if (_verticalVelocity < 0.0f)
                _verticalVelocity = -2f;
        }
        else
        {
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        GroundedCheck();
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        isGround = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    // 角色移動
    void Move()
    {
        if (!isAttack && !isDodge)
        {
            Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;

            float targetSpeed = SpeedBase;
            if (inputDirection == Vector3.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = 1f;

            if (CapsuleRay(transform.forward * Input.GetAxisRaw("Vertical")))
            {
                if ((currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset))
                {
                    speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                    speed = Mathf.Round(speed * 1000f) / 1000f;
                }
                else
                {
                    speed = targetSpeed;
                }
            }
            else
            {
                speed = 0;
            }

            if (inputDirection != Vector3.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + MainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * speed * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (inputDirection != Vector3.zero)
                _animation.CrossFade(_AnimClip.RunClip.name, 0.16f);
            else
                _animation.Play(_AnimClip.IdleClip.name);
        }
    }

    public void AttackAndDodge()
    {
        if (Input.GetMouseButtonUp(0) && !isAttack && !isDodge)
        {
            switch (attackState)
            {
                case AttackState.attack1:
                    Attack(_AnimClip.Attack1Clip.name, EnergyAtt1);
                    break;
                case AttackState.attack2:
                    Attack(_AnimClip.Attack2Clip.name, EnergyAtt2);
                    break;
                case AttackState.attack3:
                    Attack(_AnimClip.Attack3Clip.name, EnergyAtt3);
                    break;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isAttack && !isDodge)
        {
            if (!UseSkillEnergy(EnergyDodge)) return;
            isDodge = true;
            _animation.Play(_AnimClip.RollClip.name);

            if (_AudioSource.clip == null)
            {
                _AudioSource.volume = _AudioVul * 0.25f;
                _AudioSource.PlayOneShot(DodgeAudio);
            }

            DodgeStartPos = transform.position;
            newDodgePos = DodgeStartPos + transform.forward * DodgeDistance;
        }
        else if (isDodge)
        {
            if (DodgeCountdown > 0)
            {
                float moveDistance = DodgeDistance / DodgeOverTime * Time.deltaTime;
                if (CapsuleRay(transform.forward))
                    transform.position = Vector3.MoveTowards(transform.position, newDodgePos, moveDistance);

                movementReached += moveDistance;

                DodgeCountdown -= Time.deltaTime;

                if (movementReached >= DodgeDistance)
                {
                    isDodge = false;
                    movementReached = 0;
                    DodgeCountdown = DodgeOverTime;
                }
            }
            else
            {
                isDodge = false;
                DodgeCountdown = DodgeOverTime;
            }
        }
    }

    public bool CapsuleRay(Vector3 dir)
    {
        float distanceToPoint = _controller.height / 2 - _controller.radius;
        Vector3 point1 = transform.position + _controller.center + Vector3.up * distanceToPoint;
        Vector3 point2 = transform.position + _controller.center - Vector3.up * distanceToPoint;

        float radius = _controller.radius * 0.95f;
        float castDistance = 2f;

        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, radius, dir, castDistance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.distance != 0)
                if (hit.transform.CompareTag("Wall"))
                    return false;
        }
        return true;
    }

    // 角色攻擊
    public void Attack(string animationName, int enemyDeplete)
    {
        if (UseSkillEnergy(enemyDeplete))
        {
            isAttack = true;
            _animation.Play(animationName);
            transform.Translate(0, 0, 1);

            if (_AudioSource.clip == null)
            {
                _AudioSource.volume = _AudioVul * 0.25f;
                _AudioSource.PlayOneShot(AttAudio);
            }

            if (attackState == AttackState.attack3)
                attackState = 0;
            else
                attackState += 1;
        }
        else
        {
            if (attackRe == 0)
            {
                attackRe = 5;
                attackState = 0;
            }
            else
                attackRe -= Time.deltaTime;
        }
    }

    // 揮刀中不能移動
    void StopAtt()
    {
        isAttack = false;
        isDodge = false;
    }

    // 受傷
    private void OnTriggerEnter(Collider other)
    {

    }

    // 死亡控制
    void isDeath()
    {
        StopAllCoroutines();
        isDead = true;
        _AudioSource.volume = _AudioVul * 0.25f;
        _AudioSource.PlayOneShot(DeadAudio);
        _animation.Play(_AnimClip.DeathClip.name);
    }

    // 吸血控制 要改
    void Bloodsucking()
    {
        return;
    }

    // 能量控制
    public bool UseSkillEnergy(int consume)
    {
        if (Energy > 0 && consume <= Energy)
        {
            Energy -= consume;
            return true;
        }
        else
            return false;
    }

    // 慢慢恢復能量
    IEnumerator EnergyAutoIncrease()
    {
        while (true)
        {
            if (Energy < EnergyMax && !isAttack && !isDodge)
            {
                Energy = (int)Mathf.MoveTowards(Energy, EnergyMax, EnergyRecoverySpeed);
                Energy = Mathf.Clamp(Energy, 0, EnergyMax);
                yield return new WaitForSeconds(0.5f * Time.deltaTime);
            }
            else
            {
                yield return new WaitForSeconds(1f * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGround) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + transform.forward, new Vector3(2, 2, 2));
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