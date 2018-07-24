using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SkillButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    // 最大持续时间
    public float maxChargeTime = 5f;

    // 按钮图像
    public Image buttonImage;

    // 是否可以交互
    public bool interactable = true;

    // 按下事件
    public System.Action onClick;

    // 离开或完成时的事件
    public System.Action onFinish;

    // 延迟时间  
    private float delay = 0.2f;
    // 按钮是否是按下状态  
    private bool isDown = false;
    // 按钮最后一次被按住的时间
    private float lastIsDownTime;

    private void Start()
    {
        if(buttonImage == null)
            buttonImage = GetComponent<Image>();
    }

    void Update()
    {
        if (!interactable) return;

        // 如果按钮是被按下状态  
        if (isDown)
        {
            // 当前时间 -  按钮最后一次被按下的时间 > 延迟时间0.2秒  
            if (Time.time - lastIsDownTime > delay)
            {
                // 触发长按方法  
                Debug.Log("长按");
                // 记录按钮最后一次被按下的时间  
                //lastIsDownTime = Time.time;

                buttonImage.color = Color.gray;

                if (Time.time - lastIsDownTime > maxChargeTime)
                {
                    isDown = false;
                    buttonImage.color = Color.white;
                }

            }
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;

        isDown = true;
        lastIsDownTime = Time.time;

        if(onClick != null)
            onClick();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) return;

        isDown = false;
        buttonImage.color = Color.white;

        if (onFinish != null)
            onFinish();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable) return;

        isDown = false;
        buttonImage.color = Color.white;

        if (onFinish != null)
            onFinish();
    }

}
