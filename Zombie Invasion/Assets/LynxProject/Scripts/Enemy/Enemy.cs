using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour {

    [HideInInspector]
    public Health health;
    [HideInInspector]
    public Animator animator;

    public float speedMove = 0.25f;
    public float stopDistance = 1.5f;

    private NavMeshAgent navAgent;
    private Transform player;

    public bool isDead;
    public float attackRate = 1;
    private float nextAttackTime;

    void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        player = (FindObjectOfType(typeof(PlayerController)) as PlayerController).transform;

        navAgent.stoppingDistance = stopDistance;
        navAgent.SetDestination(player.position);

    }

    public void Update()
    {
        if (isDead)
            return;

        navAgent.speed = speedMove;

        if (navAgent.remainingDistance < navAgent.stoppingDistance)
        {
            navAgent.isStopped = true;

            if(Time.time > nextAttackTime)
            {
                nextAttackTime = Time.time + attackRate;
                animator.SetTrigger("OnAttack");
            }

        }
    }

    public virtual void OnHitArea(float damage)
    {
        if (isDead)
            return;

        health.OnHitTaken(damage);
        
        if (health.healthAmount <= 0)
        {
            isDead = true;
            navAgent.isStopped = true;
            animator.SetBool("isDead", isDead);
            health.healthAmount = 0;
            Destroy(gameObject, 3f);
        }
        else
        {
            animator.SetTrigger("OnHit");
        }

    }

}
