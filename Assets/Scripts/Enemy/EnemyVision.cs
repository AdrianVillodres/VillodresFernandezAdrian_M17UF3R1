using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform player;
    public float visionRange = 10f;
    public float visionAngle = 90f;
    public float timeToStopFollowingPlayer = 2.5f;
    public LayerMask obstacleLayer;
    private bool playerInSight = false;

    public bool PlayerInSight { get => playerInSight; }

    public Coroutine CoroutineStopFollowing { get; set; }

    private EnemyIA enemyIA;

    void Start()
    {
        enemyIA = GetComponent<EnemyIA>();
    }

    void Update()
    {
        DetectPlayer();
    }

    private IEnumerator StopFollowingPlayer()
    {
        yield return new WaitForSeconds(timeToStopFollowingPlayer);
        playerInSight = false;
        enemyIA.LostPlayer();
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= visionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= visionAngle / 2f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, visionRange, ~obstacleLayer))
                {
                    if (hit.transform == player)
                    {
                        if (CoroutineStopFollowing != null)
                        {
                            StopCoroutine(CoroutineStopFollowing);
                            CoroutineStopFollowing = null;
                        }

                        playerInSight = true;
                        enemyIA.SeePlayer(player.gameObject);
                    }
                    else if (hit.transform.CompareTag("obstacle"))
                    {
                        if (playerInSight && CoroutineStopFollowing == null) CoroutineStopFollowing = StartCoroutine(StopFollowingPlayer());
                    }
                }
            }
            else
            {
                if (playerInSight && CoroutineStopFollowing == null) CoroutineStopFollowing = StartCoroutine(StopFollowingPlayer());
            }
        }
        else
        {
            if (playerInSight && CoroutineStopFollowing == null) CoroutineStopFollowing = StartCoroutine(StopFollowingPlayer());
        }

    }

void OnDrawGizmos()
    {
        Gizmos.color = playerInSight ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward * visionRange;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        if (playerInSight)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
