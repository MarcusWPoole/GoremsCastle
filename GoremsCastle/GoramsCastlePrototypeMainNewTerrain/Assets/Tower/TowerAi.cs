using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the tower's targeting and firing behaviour

// Both this script and the porjectile behaviour script were built using this tutorial:
// https://www.youtube.com/watch?v=QKhn2kl9_8I
// by Brackeys. Many thanks to them!

public class TowerAI : MonoBehaviour
{
    
    [Header("Variables")]

    public float towerRange = 15.0f;
    public float towerROF = 1.0f;
    private float towerCooldown = 0.0f;

    [Header("Enemy Details")]
    
    public Transform currentTarget;
    public string enemyTag = "Enemy";

    [Header("Weapon")]

    public GameObject projectilePrefab;
    public Transform towerFirePoint;
    private AudioSource towerSound;

    [Header("Round")]

    public bool roundStart;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);

        ParticleSystem rangeBoundary = GetComponentInChildren<ParticleSystem>();
        var rangeBoundaryShape = rangeBoundary.shape;
        rangeBoundaryShape.radius = towerRange;
        rangeBoundary.Pause();

        towerSound = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget == null)
        {
            return;
        }

        // Lock-on
        Vector3 targetDirection = currentTarget.position - transform.position;
        Quaternion firingAngle = Quaternion.LookRotation(targetDirection);

        if (towerCooldown <= 0 && roundStart)
        {
            Fire(firingAngle);
            towerSound.Play();
            towerCooldown = 1f / towerROF;
        }

        towerCooldown -= Time.deltaTime;

    }

    void Fire(Quaternion firingAngle)
    {
        GameObject projectileObject = (GameObject)Instantiate(projectilePrefab, towerFirePoint.position, firingAngle);
        ProjectileBehaviour projectile = projectileObject.GetComponent<ProjectileBehaviour>();

        if (projectile != null)
        {
            projectile.Seek(currentTarget);
        }    
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;
        
        foreach(GameObject enemy in enemies)
        {
            float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);

            if (enemyDistance < shortestDistance)
            {
                shortestDistance = enemyDistance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null && shortestDistance <= towerRange)
        {
            currentTarget = closestEnemy.transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }

}
