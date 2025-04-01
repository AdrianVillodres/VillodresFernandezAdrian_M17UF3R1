using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class EnemyIA : MonoBehaviour, Inputs.IEnemyActions, IHurteable
{
    public int HP = 5;
    private Inputs enemyInputs;
    public GameObject target;
    public bool attack;
    public bool chase;
    public StateSO currentNode;
    public List<StateSO> Nodes;
    public int LostHP;
    public Slider Healthbar;
    [Header("Patrol System")]
    public Transform mainPoint;
    public Transform[] patrolPoints;
    public float stopChaseTime = 1f;
    private int currentPatrolIndex = 0;
    private bool lostPlayer = false;
    private Vector3 lastSeenPosition;
    private float lostPlayerTimer = 0;

    private ChaseBehaviour chaseBehaviour;
    private NavMeshAgent agent;

    void Awake()
    {
        enemyInputs = new Inputs();
        enemyInputs.Enemy.SetCallbacks(this);
        chaseBehaviour = GetComponent<ChaseBehaviour>();
        agent = GetComponent<NavMeshAgent>();
        Healthbar.maxValue = HP;
        Healthbar.value = HP;
    }

    private void OnEnable()
    {
        enemyInputs.Enable();
    }

    private void OnDisable()
    {
        enemyInputs.Disable();
    }

    private void Update()
    {
        if (chase && target != null)
        {
            chaseBehaviour.Chase(target.transform);
        }
        else if (lostPlayer)
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= stopChaseTime)
            {
                MovePatrolArea(lastSeenPosition);
                lostPlayer = false;
                lostPlayerTimer = 0;
            }
            else
            {
                MovePatrolArea(lastSeenPosition);
                Patrol();
            }
        }
        else
        {
            Patrol();
        }

        CheckEndingConditions();
        currentNode.OnStateUpdate(this);
    }

    public void SeePlayer(GameObject player)
    {
        target = player;
        chase = true;
        lostPlayer = false;
    }

    public void LostPlayer()
    {
        chase = false;
        if (target != null)
        {
            lastSeenPosition = target.transform.position;
        }
        lostPlayer = true;
    }

   /* private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SeePlayer(collision.gameObject);
        }
        CheckEndingConditions();
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LostPlayer();
        }
        CheckEndingConditions();
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = true;
            IHurteable hurteable = collision.gameObject.GetComponent<IHurteable>();
            if (hurteable != null)
            {
                hurteable.Hurt(1);
            }
        }
        CheckEndingConditions();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = false;
        }
        CheckEndingConditions();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        chaseBehaviour.Chase(patrolPoints[currentPatrolIndex]);

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void MovePatrolArea(Vector3 lastSeenPosition)
    {
        if (HP >= 5) return;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        if (!NavMesh.SamplePosition(lastSeenPosition, out NavMeshHit mainHit, 2.0f, NavMesh.AllAreas))
        {
            return;
        }

        mainPoint.position = new Vector3(mainHit.position.x, mainPoint.position.y, mainHit.position.z);
        int mainArea = mainHit.mask;

        Vector3 offset = new Vector3(mainPoint.position.x - lastSeenPosition.x, 0, mainPoint.position.z - lastSeenPosition.z);

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Vector3 patrolPosition = patrolPoints[i].position + offset;
            float originalHeight = patrolPoints[i].position.y;

            if (NavMesh.SamplePosition(patrolPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                if (hit.mask == mainArea)
                {
                    patrolPoints[i].position = new Vector3(hit.position.x, originalHeight, hit.position.z);
                }
                else
                {
                    Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
                    Vector3 newPatrolPos = mainPoint.position + randomOffset;

                    if (NavMesh.SamplePosition(newPatrolPos, out NavMeshHit newHit, 2.0f, NavMesh.AllAreas))
                    {
                        patrolPoints[i].position = new Vector3(newHit.position.x, originalHeight, newHit.position.z);
                    }
                    else
                    {
                        patrolPoints[i].position = new Vector3(mainPoint.position.x, originalHeight, mainPoint.position.z);
                    }
                }
            }
            else
            {
                patrolPoints[i].position = new Vector3(mainPoint.position.x, originalHeight, mainPoint.position.z);
            }
        }

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    public void CheckEndingConditions()
    {
        foreach (ConditionSO condition in currentNode.EndConditions)
        {
            if (condition.CheckCondition(this) == condition.answer)
            {
                ExitCurrentNode();
            }
        }
    }
    public void Hurt(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;    
        }
        CheckEndingConditions();
        Healthbar.value = HP;
    }

    public void ExitCurrentNode()
    {
        foreach (StateSO stateSO in Nodes)
        {
            if (stateSO.StartCondition == null ||
                stateSO.StartCondition.CheckCondition(this) == stateSO.StartCondition.answer)
            {
                EnterNewState(stateSO);
                break;
            }
        }
        currentNode.OnStateEnter(this);
    }

    private void EnterNewState(StateSO state)
    {
        currentNode.OnStateExit(this);
        currentNode = state;
        currentNode.OnStateEnter(this);
    }

}
