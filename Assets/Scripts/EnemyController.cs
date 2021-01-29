using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private AudioSource audioSource;
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private ParticleSystem smokeEffect;
    private int direction = 1;
    private float changeDirectionTimer;
    // 是否是损坏的机器人
    private bool broken = true;

    public float directionChangeTime = 3.0f;
    public int speed = 3;
    public bool vertical = false;
    public AudioClip fixedSound;
    public AudioClip[] hitSounds;

    public GameObject hitEffect;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        smokeEffect = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        changeDirectionTimer = directionChangeTime;
        broken = true;
        //animator.SetBool("Vertical", vertical);
        //animator.SetInteger("MoveX", direction);
        PlayMoveAni();
    }

    // Update is called once per frame
    void Update()
    {
        if (!broken)
        {
            return; // 修好了就不再移动
        }

        changeDirectionTimer -= Time.deltaTime;
        if (changeDirectionTimer <= 0)
        {
            direction = -direction;
            changeDirectionTimer = directionChangeTime;
            //animator.SetInteger("MoveX", direction);
            PlayMoveAni();
        }

        Vector2 position = rigidbody2d.position;
        if (vertical) // 垂直移动
        {
            position.y = position.y + speed * Time.deltaTime * direction;
        }
        else
        {
            position.x = position.x + speed * Time.deltaTime * direction;
        }
        rigidbody2d.MovePosition(position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RubyController rubyController = collision.gameObject.GetComponent<RubyController>();
        if (rubyController != null)
        {
            rubyController.ChangeHP(-1);
        }
    }

    /**
     * 切换移动动画
     */
    private void PlayMoveAni()
    {
        if (vertical) // 垂直方向
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", direction);
        }
        else // 水平方向
        {
            animator.SetFloat("MoveY", 0);
            animator.SetFloat("MoveX", direction);
        }
    }

    // 修复机器人
    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false; // 修好了不再进行碰撞检测
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        animator.SetTrigger("Fixed");
        int randomNum = Random.Range(0, 2);
        audioSource.Stop();
        audioSource.volume = 0.6f;
        audioSource.PlayOneShot(hitSounds[randomNum]);
        Invoke("PlayFixedSound", 1f);
        smokeEffect.Stop();
        UIHPBar.instance.fixedNum++;
    }
    private void PlayFixedSound()
    {
        audioSource.PlayOneShot(fixedSound);
    }
}
