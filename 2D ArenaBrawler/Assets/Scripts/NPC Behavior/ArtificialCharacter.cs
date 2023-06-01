using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class ArtificialCharacter : Character
{

    public float baseSpeed = 200f;

    public int attackDamage = 1;

    public float attackSpeed = 1;

    public float attackRange = 2;

    [Header("A.I. Controlls")]

    
    protected ContactFilter2D contact;

    [SerializeField]
    protected float targetVariablity = 1;

    [SerializeField]
    protected float stoppingDistance = 1;

    [SerializeField]
    protected float visionRadius = 3;

    [SerializeField]
    protected float nextWaypointDistance = .5f;

    [SerializeField]
    protected float refreshFrequency = .5f;

    protected Vector2 target;
    protected Character targetScript;


    protected Vector2 transformOffset;


    protected float speed;

    protected Path path;
    protected int currentWaypoint = 0;
    protected bool reachedEndOfPath = false;

    protected Seeker seeker;

    protected Transform closest;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mat = GetComponentInChildren<SpriteRenderer>().material;
    }

    protected virtual void Start()
    {
        stoppingDistance = 0.5f;
        attackRange = stoppingDistance + 1f;

        health = maxHealth;

        contact.useTriggers = true;
        contact.useLayerMask = true;

        target = transform.position;

        speed = baseSpeed;

        transformOffset = new Vector2(Random.Range(-targetVariablity, targetVariablity), Random.Range(-targetVariablity, targetVariablity));
       
        InvokeRepeating("UpdatePath", 0f, refreshFrequency);

        animator.speed = attackSpeed;

    }

    private void Update()
    {
        if (!dead)
        {
            SelectTarget();

            Flip();

            animator.SetFloat("Velocity", rb.velocity.magnitude);
        }
        else {

            target = transform.position;

        }

    }

    protected virtual void SelectTarget() {

        closest = FindClosestTarget();

        if (closest)
        {

            targetScript = null;

            targetScript = closest.gameObject.GetComponent<Character>();

        }

        if (closest != null && Vector2.Distance(transform.position, closest.position) <= attackRange)
        {
            animator.speed = attackSpeed;
            animator.SetBool("AtEnemy", true);
        }
        else
        {
            animator.speed = 1;
            animator.SetBool("AtEnemy", false);
        }



    }

    void FixedUpdate()
    {
        if (!dead)
        {
            PathFind();
        }

    }

    private Transform FindClosestTarget()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        if (Physics2D.CircleCast(transform.position, visionRadius, Vector2.zero, contact, hits) != 0)
        {

            float currentClosest = Mathf.Infinity;
            GameObject closestObject = null;

            foreach (RaycastHit2D _target in hits)
            {

                float dist = Vector2.Distance(_target.collider.gameObject.transform.position, transform.position);

                if (_target.collider.gameObject.GetComponent<Character>())
                {

                    if (dist < currentClosest && !_target.collider.gameObject.GetComponent<Character>().dead)
                    {
                        currentClosest = dist;
                        closestObject = _target.collider.gameObject;
                    }
                }
            }

            if (closestObject != null)
            {
                return closestObject.transform;
            }
            else
            {
                return null;
            }



        }
        else
        {
            return null;
        }


    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void PathFind() {

        if (path == null)
            return;

        if (Vector2.Distance(path.vectorPath[path.vectorPath.Count - 1], transform.position) <= stoppingDistance)
        {
            reachedEndOfPath = true;
            rb.velocity = Vector2.zero;

            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * baseSpeed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
            currentWaypoint++;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }

    private void Flip()
    {
        if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (rb.velocity.x > 0)
        {
            transform.localScale = Vector2.one;
        }
    }

    protected void ModifySpeed(float modifier)
    {
        speed = baseSpeed * modifier;
    }

    private void DealDamage()
    {

        targetScript.TakeDamage(attackDamage);

    }

}
