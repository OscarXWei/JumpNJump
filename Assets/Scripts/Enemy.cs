using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float shootDelay = 3f;
    public float bulletSpeed = 10f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ShootAtPlayer());
    }

    private IEnumerator ShootAtPlayer()
    {
        while (!GameManager.Instance.isGameOver)
        {
            yield return new WaitForSeconds(shootDelay);
            if (player != null && !GameManager.Instance.isGameOver)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
            }
        }
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}