using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject reward0; 
    public float shootDelay = 3f;
    public float bulletSpeed = 10f;

    private Transform player;
    public PlayerController playerObj;
    private float moveSpeed = 0.7f;
    private float stoppingDistance = 0.2f;
    private bool isDead = false;
    
    
    private int level = 0;
    private int rewardLevel = 0;
    

    private void Start()
    {
        playerObj = FindObjectOfType<PlayerController>();
        player = playerObj.transform;
        //StartCoroutine(ShootAtPlayer());
        if (level == 0)
        	StartCoroutine(ChasePlayer());
        if (level == 1)
        {
        	turnOffPhysics();
        	StartCoroutine(ChasePlayer());
        }
        	
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
    
    private IEnumerator ChasePlayer()
    {
        while (!GameManager.Instance.isGameOver)
        {
            if (player != null)
            {
                // Calculate distance to player
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                
                // Only move if we're further than stopping distance
                if (distanceToPlayer > stoppingDistance)
                {
                    // Calculate direction to player
                    Vector3 direction = (player.position - transform.position).normalized;
                    
                    // Move towards player
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    
                    // Optional: Rotate to face player
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }

            }
            
            // Wait for next frame
            yield return null;
        }
    }


    public void TakeDamage()
    {
        StopAllCoroutines();
        turnOnPhysics();
        StartCoroutine(EnemyDeath());
        
    }
    
    private IEnumerator EnemyDeath()
	{
	    isDead = true;
	    SphereCollider sphereCollider = GetComponent<SphereCollider>();
	    // Store original scale
	    Vector3 originalScale = transform.localScale;
	    
	    // Squash effect: flatten y-scale and expand x/z scale
	    Vector3 squashScale = new Vector3(
		originalScale.x * 1.5f,  // Expand width
		originalScale.y * 0.3f,  // Flatten height
		originalScale.z * 1.5f   // Expand depth
	    );
	    
	    // Apply squash scale
	    transform.localScale = squashScale;
	    sphereCollider.radius = 0.3f * sphereCollider.radius;
	    turnOffRotationalPhysics();
	    
	    // Disable any movement/shooting scripts if you have them
	    // GetComponent<Rigidbody>().isKinematic = true;  // Optional: freeze movement
	    
	    // Wait for 3 seconds
	    yield return new WaitForSeconds(3f);
	    
	    generateReward();
	    
	    // Destroy the object
	    Destroy(gameObject);
	}
    
    private void OnCollisionEnter(Collision collision)
    {
    	GameObject hitObject = collision.gameObject;
    	if (hitObject.CompareTag("Player"))
    	{
    	    Debug.Log($"enemy y {gameObject.transform.position.y}, player y {hitObject.transform.position.y}");
    	    if (level < 2 && !isDead)
    	    {
    	    	if (gameObject.transform.position.y + (transform.localScale.y/2) <= hitObject.transform.position.y -(hitObject.transform.localScale.y/2) || playerObj.isNowInvicible())
                {
            	    TakeDamage();
                }
                else
                {
		    hitObject.SendMessage("PlayerDead");
                }	
    	    }
    	    else if (level >= 2 && !isDead)
    	    {
    	    	if (playerObj.isNowInvicible())
                {
            	    TakeDamage();
                }
                else
                {
		    hitObject.SendMessage("PlayerDead");
                }	
    	    }
            
    	}
    	
    	
    }
    
    private void turnOffRotationalPhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    
    private void generateReward()
    {
    	if (rewardLevel == 0 && isDead)
    	{
    		GameObject rewardObj = Instantiate(reward0, transform.position, Quaternion.identity);
    		rewardObj.AddComponent<ItemSpin>();
    		
    	}
    }
    
    public void setLevel(int num)
    {
    	level = num;
    }
    
    public void setRewardLevel(int num)
    {
    	rewardLevel = num;
    }
    
    public void turnOffPhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
    
    public void turnOnPhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
}
