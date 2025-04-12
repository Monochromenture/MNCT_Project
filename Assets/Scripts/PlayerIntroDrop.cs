using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntroDrop2D : MonoBehaviour
{
    public GameObject realPlayer;         // 真正的角色
    public GameObject dropImage;          // 掉落用圖像
    public float dropSpeed = 5f;          // 掉落速度
    public float groundY = -2f;           // 地板 Y 軸
    public GameObject landingEffect;      // 落地粒子（預製物件）
    public Camera mainCamera;             // 主攝影機
    public float particleLifetime = 1f;   // 粒子效果持續時間
    public GameObject landingAnimationObject; // 落地動畫物件

    private bool isDropping = true;
    private float shakeDuration = 0.2f;   // 震動持續時間
    private float shakeStrength = 0.3f;   // 震動幅度，增大到 0.3f
    private Vector3 originalCamPos;

    private bool hasLanded = false;       // 確保只播放一次音效

    void Start()
    {
        realPlayer.SetActive(false);
        dropImage.SetActive(true);
        originalCamPos = mainCamera.transform.position;

        // 禁用玩家控制
        if (realPlayer.TryGetComponent<PlayerMovement>(out var pc))
            pc.enabled = false;
    }

    void Update()
    {
        if (isDropping)
        {
            Vector3 pos = dropImage.transform.position;
            pos.y -= dropSpeed * Time.deltaTime;

            if (pos.y <= groundY)
            {
                pos.y = groundY;
                dropImage.transform.position = pos;
                isDropping = false;
                OnLanding();
            }
            else
            {
                dropImage.transform.position = pos;
            }
        }
    }

    public AudioSource audioSource; // 可是角色身上的 AudioSource

    void OnLanding()
    {
        if (hasLanded) return; // 確保只播放一次音效
        hasLanded = true;

        // 粒子
        if (landingEffect)
        {
            GameObject effect = Instantiate(landingEffect, dropImage.transform.position, Quaternion.identity);
            Destroy(effect, particleLifetime); // 在指定時間後銷毀粒子效果
        }

        // 音效
        if (audioSource && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        // 相機震動
        StartCoroutine(CameraShake());

        // 播放落地動畫
        if (landingAnimationObject)
        {
            dropImage.SetActive(false);
            landingAnimationObject.SetActive(true);
            StartCoroutine(WaitForAnimation(landingAnimationObject));
        }
        else
        {
            // 如果沒有設置動畫，直接啟動玩家
            ActivatePlayer();
        }
    }

    IEnumerator WaitForAnimation(GameObject animationObject)
    {
        Animator animator = animationObject.GetComponent<Animator>();
        if (animator != null)
        {
            // 等待動畫播放完成
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
        }

        // 動畫播放完成後關閉動畫物件並啟動玩家
        animationObject.SetActive(false);
        ActivatePlayer();
    }

    void ActivatePlayer()
    {
        dropImage.SetActive(false);
        realPlayer.SetActive(true);
        if (realPlayer.TryGetComponent<PlayerMovement>(out var pc))
            pc.enabled = true;
    }

    IEnumerator CameraShake()
    {
        float timer = 0f;
        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            Vector3 shakeOffset = Random.insideUnitCircle * shakeStrength;
            mainCamera.transform.position = originalCamPos + new Vector3(shakeOffset.x, shakeOffset.y, 0);
            yield return null;
        }
        mainCamera.transform.position = originalCamPos;
    }
}
