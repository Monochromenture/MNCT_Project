using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgirlHealthHeartBar : MonoBehaviour
{
    public GameObject heartPrefab;  // 血量條的愛心預製體
    public EgirlController bossHealth;  // Boss 的血量
    private List<HealthHeart> hearts = new List<HealthHeart>();

    private void OnEnable()
    {
        EgirlController.OnBossDamage += DrawHearts;  // 訂閱 Boss 受傷事件
        EgirlController.OnBossRespawn += DrawHearts; // 訂閱 Boss 復活事件
    }

    private void OnDisable()
    {
        EgirlController.OnBossDamage -= DrawHearts;
        EgirlController.OnBossRespawn -= DrawHearts;
    }

    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();

        float maxHealthRemainder = bossHealth.maxHealth % 2;
        int heartsToMake = (int)((bossHealth.maxHealth / 2) + maxHealthRemainder);

        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(bossHealth.currentHealth - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }
}
