using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class RangeAI : MonoBehaviour
{
    [SerializeField]
    private float MaxDis;

    private Transform playerTransform;
    private NavMeshAgent agent;
    Vector3 StartPosition;

    public GameObject bomb;
    public Transform cannonPoint;
    float nextBomb;
    public float fireRate;
    public float bombForce = 200;

    public int enemyHP;
    public GameObject DestroyFX;

    public AudioClip explosionSound;
    public AudioSource explosionAudioSource;

    // 🔒 CỰC KỲ QUAN TRỌNG
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false; // CHỐT HẠ LỖI
    }

    void Start()
    {
        StartPosition = transform.position;

        // ===== ĐẶT AGENT LÊN NAVMESH ĐÚNG CÁCH =====
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 100f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            agent.enabled = true;
        }
        else
        {
            Debug.LogError("[RangeAI] Không tìm thấy NavMesh cho enemy: " + gameObject.name);
            return;
        }
        // ==========================================

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // ===== BẢO VỆ NAVMESH TUYỆT ĐỐI =====
        if (agent == null) return;
        if (!agent.enabled) return;
        if (!agent.isOnNavMesh) return;
        // ====================================

        if (playerTransform != null)
        {
            float currentDis = Vector3.Distance(transform.position, playerTransform.position);

            if (currentDis <= MaxDis)
            {
                Vector3 targetToLookAt = new Vector3(
                    playerTransform.position.x,
                    transform.position.y,
                    playerTransform.position.z
                );
                transform.LookAt(targetToLookAt);

                if (Time.time >= nextBomb)
                {
                    nextBomb = Time.time + 1f / fireRate;
                    Shoot();
                }
            }
            else if (currentDis > MaxDis && currentDis < MaxDis + 8)
            {
                agent.SetDestination(playerTransform.position);
            }
            else
            {
                agent.SetDestination(StartPosition);
            }
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        if (enemyHP <= 0)
        {
            Instantiate(DestroyFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void Shoot()
    {
        GameObject b = Instantiate(bomb, cannonPoint.position, transform.rotation);

        Vector3 dir = playerTransform.position - transform.position;
        dir = dir.normalized;

        Rigidbody rb = b.GetComponent<Rigidbody>();
        rb.AddForce(dir * bombForce);
        rb.useGravity = true;

        Destroy(b, 10);

        if (explosionSound != null && explosionAudioSource != null)
        {
            explosionAudioSource.PlayOneShot(explosionSound);
        }
    }

    public void TakeDamage(int dmg)
    {
        enemyHP -= dmg;
    }
}
