using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoneEventHandler : MonoBehaviour
{
    public GameObject textObject;
    public GameObject player; // 引用玩家物件
    public GameObject otherObject; // 引用其他需要啟用/停用的物件
    public AudioSource audioSource; // 用於播放音效的 AudioSource

    private PlayerMovement playerMovement;

    [Header("動畫事件設定")]
    [SerializeField] UnityEvent onAnimationEvent;

    void Start()
    {
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    public void ShowText()
    {
        textObject.SetActive(true);
    }

    // 動畫事件方法：停止玩家移動
    public void PlayerStun()
    {
        if (playerMovement != null)
        {
            playerMovement.Stun(4);
        }
    }

    // 動畫事件方法：啟用其他物件
    public void EnableOtherObject()
    {
        if (otherObject != null)
        {
            otherObject.SetActive(true);
        }
    }

    // 動畫事件方法：停用其他物件
    public void DisableOtherObject()
    {
        if (otherObject != null)
        {
            otherObject.SetActive(false);
        }
    }

    // 動畫事件方法：播放音效
    public void PlaySound()
    {
        if (audioSource != null )
        {
            audioSource.Play();
        }
    }

    // 動畫事件方法：調用 UnityEvent
    public void InvokeAnimationEvent()
    {
        onAnimationEvent.Invoke();
    }
}
