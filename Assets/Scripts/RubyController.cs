using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    private Rigidbody2D rBoby2d;
    private int maxHP = 5;
    private int currentHP;
    private Vector2 restartPosition; // 复活点

    private float timeInvincible = 2.0f;
    private float invincibleTimer;
    private bool isInvincible = false;

    public int speed = 3;
    public int HP { get { return currentHP; } }
    public int MHP { get { return maxHP; } }

    // 方向动画控制相关属性
    private Vector2 lookDirection = new Vector2(1, 0); // 默认看向右边
    private Animator animator;

    public GameObject bulletPrefab; // 子弹预制体, 在gui中设置
    public AudioSource audioSource;
    public AudioSource walkAudioSource;
    public AudioClip playerHit;
    public AudioClip playerAttack;
    public AudioClip playerWalk;

    // Start is called before the first frame update
    void Start()
    {
        rBoby2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHP = maxHP;
        restartPosition = transform.position;
        Debug.Log("Ruby当前血量为:" + HP);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        // 只要有某个方向上的输入值不为0，则说明要移动
        if (!Mathf.Approximately(move.x, 0) || !Mathf.Approximately(move.y, 0))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.clip = playerWalk;
                walkAudioSource.Play();
            }
        }
        else
        {
            walkAudioSource.Stop();
        }

        // 更新动画状态
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        Vector2 position = transform.position;
        //position.x = position.x + speed * horizontal * Time.deltaTime; // *Time.deltaTime是为了每秒移动多少而不是每帧多少
        //position.y = position.y + speed * vertical * Time.deltaTime;
        // 移动
        position = position + speed * move * Time.deltaTime;
        //transform.position = position;
        rBoby2d.MovePosition(position);

        if (isInvincible)
        {
            //Debug.Log("无敌时间剩余：" + invincibleTimer);
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false;
            }
        }

        // 监听按下子弹发射
        if (Input.GetKeyDown(KeyCode.J))
        {
            Launch();
        }

        // 检测与NPC对话
        if (Input.GetKeyDown(KeyCode.T))
        {
            RaycastHit2D hit = Physics2D.Raycast(rBoby2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                Debug.Log("射线处罚到的游戏物体是：", hit.collider.gameObject);
                NPCDialog npcDialog = hit.collider.gameObject.GetComponent<NPCDialog>();
                if (npcDialog != null)
                {
                    npcDialog.showDialog();
                }
            }
        }
    }

    public void ChangeHP(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }

            // 受到伤害开始无敌状态
            isInvincible = true;
            invincibleTimer = timeInvincible;
            animator.SetTrigger("Hit");
            PlaySound(playerHit);
        }

        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        //Debug.Log(currentHP + "/" + maxHP);
        UIHPBar.instance.SetHpValue(currentHP / (float)maxHP);
        if (currentHP <= 0)
        {
            Restart();
        }
    }

    // 发射子弹
    private void Launch()
    {
        if (!UIHPBar.instance.hasTask) // 没有任务不能发射
        {
            return;
        }

        GameObject bulletObject = Instantiate(bulletPrefab, rBoby2d.position + Vector2.up * 0.5f, Quaternion.identity);
        BulletController bulletController = bulletObject.GetComponent<BulletController>();
        bulletController.Launch(lookDirection, 300);
        animator.SetTrigger("Launch");
        PlaySound(playerAttack);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    /// <summary>
    /// 死亡后复活回到初始位置
    /// </summary>
    private void Restart()
    {
        ChangeHP(maxHP);
        transform.position = restartPosition;
    }
}
