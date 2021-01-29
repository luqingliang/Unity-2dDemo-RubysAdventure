using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    public static UIHPBar instance { get; private set; }

    public bool hasTask;
    public int fixedNum;

    public Image mask;
    private float originalWidth;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        originalWidth = mask.rectTransform.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 更新血条血量
    public void SetHpValue(float fillValue)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalWidth * fillValue);
    }
}
