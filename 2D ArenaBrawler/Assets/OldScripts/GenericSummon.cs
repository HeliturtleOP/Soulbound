using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GenericSummon : MonoBehaviour
{

    [Header("Stats")]

    public float baseSpeed = 200f;

    public int attackDamage = 1;

    public float attackSpeed = 1;

    public bool Dead;

    [Header("A.I. Controlls")]

    public ContactFilter2D contact;

    [SerializeField]
    private float targetVariablity = 1;

    [SerializeField]
    private float stoppingDistance = 1;

    [SerializeField]
    private float visionRadius = 3;

    private Vector2 target;
    private GameObject enemy;

    private Transform Player;
    Vector2 transformOffset;

    public float nextWaypointDistance = 3f;

    public float refreshFrequency = .5f;

    private Animator animator;

    private float speed;

    private Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;
    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        speed = baseSpeed;

        Player = GameObject.FindGameObjectWithTag("Player").transform;

        transformOffset = new Vector2(Random.Range(-targetVariablity, targetVariablity), Random.Range(-targetVariablity, targetVariablity));

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, refreshFrequency);

        animator = GetComponent<Animator>();
        animator.speed = attackSpeed;

        mat = GetComponentInChildren<SpriteRenderer>().material;

    }

    private void Update()
    {
        Transform closest = FindClosestEnemy();

        if (closest) {

            enemy = closest.gameObject;

        }

        if (closest != null && Vector2.Distance(transform.position, closest.position) <= stoppingDistance && !closest.GetComponent<GenericEnemy>().Dead)
        {
            animator.SetBool("AtEnemy", true);
        }
        else
        {
            animator.SetBool("AtEnemy", false);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            target = (Vector2)Player.position + transformOffset;
        }
        else if (closest != null && !closest.GetComponent<GenericEnemy>().Dead)
        {
            target = closest.position;
        }
        else
        {
            target = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + transformOffset;
        }

        Flip();

        animator.SetFloat("Velocity", rb.velocity.magnitude);

    }

    private Transform FindClosestEnemy()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        if (Physics2D.CircleCast(transform.position, visionRadius, Vector2.zero, contact, hits) != 0)
        {

            float currentClosest = Mathf.Infinity;
            GameObject closestObject = null;

            foreach (RaycastHit2D enemy in hits)
            {

                float dist = Vector2.Distance(enemy.collider.gameObject.transform.position, transform.position);

                if (dist < currentClosest)
                {
                    currentClosest = dist;
                    closestObject = enemy.collider.gameObject;
                }
            }

            return closestObject.transform;

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

    void FixedUpdate()
    {
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

    private void ModifySpeed(float modifier)
    {
        speed = baseSpeed * modifier;
    }

    private void DealDamage(int amount) {

        enemy.GetComponent<GenericEnemy>().TakeDamage(amount);

    }

    private IEnumerator HurtColor(float time)
    {

        mat.SetInt("_Hurt", 1);
        yield return new WaitForSeconds(time);
        mat.SetInt("_Hurt", 0);
    }
}
