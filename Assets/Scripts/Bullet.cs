using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    public bool isPlayerBullet = false;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isPlayerBullet && collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage();
            // collision.gameObject.GetComponent<PlayerController>().TakeDamage(10);
            Destroy(gameObject);
        }
        else if (!isPlayerBullet && collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(100);
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}