using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public AudioClip audioClip;
    public GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("触碰到血包的对象是：" + collision);
        RubyController rubyController = collision.GetComponent<RubyController>();
        if (rubyController != null)
        {
            if (rubyController.HP < rubyController.MHP)
            {
                rubyController.ChangeHP(1);
                rubyController.PlaySound(audioClip);
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
