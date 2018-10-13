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

    //public List<string> walkAnimations = new List<string>();
    //public List<string> attackAnimations = new List<string>();
    //public List<string> hitAnimations = new List<string>();
    //public List<string> deathAnimations = new List<string>();

    public bool isDead;

    private string currentAttack;
    private string currentWalk;
    void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        player = (FindObjectOfType(typeof(PlayerController)) as PlayerController).transform;

        navAgent.stoppingDistance = stopDistance;
        navAgent.SetDestination(player.position);

        //currentAttack = attackAnimations[Random.Range(0, attackAnimations.Count)];
        //currentWalk = walkAnimations[Random.Range(0, walkAnimations.Count)];
        //animator.Play(currentWalk);

    }

    public void Update()
    {
        if (isDead)
            return;

        navAgent.speed = speedMove;

        if (navAgent.remainingDistance < navAgent.stoppingDistance)
        {
            navAgent.isStopped = true;
            animator.SetTrigger("OnAttack");
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
            animator.SetTrigger("isDead");
            health.healthAmount = 0;
            Destroy(gameObject, 3f);
        }
        else
        {
            animator.SetTrigger("OnHit");
        }

    }

}
