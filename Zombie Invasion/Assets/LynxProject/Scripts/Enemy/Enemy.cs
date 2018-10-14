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

    public int deathAnimSize = 4;
    public int hitAnimSize = 2;
    public int attackAnimSize = 2;
    public int walkAnimSize = 2;

    private int currentDeathType;
    private int currentHitType;
    private int currentAttackType;
    private int currentWalkType;

    void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        player = (FindObjectOfType(typeof(PlayerController)) as PlayerController).transform;

        navAgent.stoppingDistance = stopDistance;
        navAgent.SetDestination(player.position);

        currentAttackType = Random.Range(0, attackAnimSize);
        currentWalkType = Random.Range(0, walkAnimSize);
        animator.SetFloat("WalkType", currentWalkType);
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
                animator.SetFloat("AttackType", currentAttackType);
            }

        }
    }

    public void AttackAction()
    {
        Debug.Log("Attack Player");
        player.SendMessage("OnHitTaken", 10, SendMessageOptions.DontRequireReceiver);
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
            navAgent.speed = 0;
            animator.SetTrigger("OnHit");
            animator.SetBool("isDead", isDead);

            currentDeathType = Random.Range(0, deathAnimSize);
            animator.SetFloat("DeathType", currentDeathType);

            health.healthAmount = 0;
            Destroy(gameObject, 3.5f);
        }
        else
        {
            StartCoroutine(OnHitStop());
            animator.SetTrigger("OnHit");
            currentHitType = Random.Range(0, hitAnimSize);
            animator.SetFloat("HitType", currentHitType);
        }

    }

    IEnumerator OnHitStop()
    {
        navAgent.isStopped = true;
        yield return new WaitForSeconds(1f);
        navAgent.isStopped = false;
    }

}
