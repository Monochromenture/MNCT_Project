using UnityEngine;
using System.Collections;


public class BulletHellSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float spawnRate = 0.2f;
    public int bulletCount = 50;

    private bool isSpawning = false;

    public void StartBulletHell()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnBullets());
        }
    }

    private IEnumerator SpawnBullets()
    {
        isSpawning = true;

        for (int i = 0; i < bulletCount; i++)
        {
            Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnRate);
        }

        isSpawning = false;
    }
}
