using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StoneEventHandler : MonoBehaviour
{
    public GameObject textObject;
    public GameObject player; // 引用玩家物件
    public List<GameObject> objectsToEnable; // 要啟用的物件列表
    public List<GameObject> objectsToDisable; // 要停用的物件列表
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

    // 動畫事件方法：啟用指定的物件
    public void EnableObjects()
    {
        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    // 動畫事件方法：停用指定的物件
    public void DisableObjects()
    {
        foreach (var obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    // 動畫事件方法：播放音效
    public void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    // 動畫事件方法：切換場景
    public void SwitchScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("場景名稱無效或為空！");
        }
    }

    // 動畫事件方法：調用 UnityEvent
    public void InvokeAnimationEvent()
    {
        onAnimationEvent.Invoke();
    }
}
