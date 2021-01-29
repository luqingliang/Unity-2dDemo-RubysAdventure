using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialog : MonoBehaviour
{
    private GameObject dialogBox;
    private AudioSource audioSource;
    private float dialogShowTime = 3.0f;
    private float dialogTimer;
    private bool hasPlaySound;

    public Text textDialog;
    public AudioClip taskCompleteSound;

    // Start is called before the first frame update
    void Start()
    {
        dialogBox = GameObject.Find("DialogCanvas");
        audioSource = GetComponent<AudioSource>();
        dialogBox.SetActive(false);
        dialogTimer = -1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogTimer > 0)
        {
            dialogTimer -= Time.deltaTime;
            if (dialogTimer <= 0)
            {
                dialogBox.SetActive(false);
            }
        }
    }

    public void showDialog()
    {
        dialogTimer = dialogShowTime;
        dialogBox.SetActive(true);
        UIHPBar.instance.hasTask = true;
        if (UIHPBar.instance.fixedNum >= 5)
        {
            // 任务完成了
            textDialog.text = "谢谢你";
            if (!hasPlaySound)
            {
                audioSource.PlayOneShot(taskCompleteSound);
                hasPlaySound = true;
            }
        }
        else
        {
            // 任务还没完成
            textDialog.text = "帮我去修理机器人吧";
        }
    }
}
