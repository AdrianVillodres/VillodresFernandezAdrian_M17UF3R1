using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseBehaviour : MonoBehaviour
{
    public float Speed;
    private NavMeshAgent agent;
    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Speed;
    }
    public void Chase(Transform target)
    {
        agent.SetDestination(target.position);
    }
    public void Run(Transform target, Transform self)
    {
        Vector3 direction = (self.position - target.position).normalized;
        Vector3 newDestination = self.position + direction * 5f; 
        agent.SetDestination(newDestination);
    }


    public void StopChasing()
    {
        agent.ResetPath();
        _rb.velocity = Vector2.zero;
    }
}
