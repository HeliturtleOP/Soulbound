using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    [SerializeField]
    private float baseSpeed;

    [SerializeField]
    private int attackDamage;

    [SerializeField]
    float summonRadius = 3;

    [SerializeField]
    private float attackRange = 1;

    private float speed;

    private Camera cam;
    //private Rigidbody2D rb;
    //private Animator animator;
    [SerializeField]
    private BarDisplay display;

    // public bool dead;

    [SerializeField]
    private GameObject[] Summonables;
    private int currentSummon;


    protected ContactFilter2D contact;
    protected GameObject closestEnemy;
    protected GameObject MeleeRangeEnemy;
    //protected Material mat;

    // Start is called before the first frame update
    void Start()
    {

        health = 1;

        display.UpdateValue(health);

        contact.useLayerMask = true;
        contact.SetLayerMask(1 << 7);

        contact.useTriggers = true;
        contact.useLayerMask = true;

        ModifySpeed(1);

        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mat = GetComponentInChildren<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            DetectMouse();
            Attack();
            SummonAnimation();
        }

    }

    //FixedUpdate called every physics iteration
    private void FixedUpdate()
    {
        if(!dead)
        Move();
    }

    private void Move()
    {
        //get keyboard input from player
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        //combine inputs into direction for movement
        Vector2 Direction = new Vector2(xInput, yInput).normalized;

        //moves the player in the desired location at the desired speed
        rb.velocity = Direction * speed;

        //triggers the walk animation
        animator.SetFloat("Velocity", rb.velocity.magnitude);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(((float)amount)/6f);

        display.UpdateValue(health);

    }

    private void DetectMouse()
    {
        //gets mouse position in world space coordinates
        float mouseX = cam.ScreenToWorldPoint(Input.mousePosition).x;

        //checks if the mouse is in front of or behind the player


        if (MeleeRangeEnemy)
        {
            if (MeleeRangeEnemy.transform.position.x < transform.position.x)
            {
                Flip(true);
            }
            else
            {
                Flip(false);
            }
        }
        else
        {
            if(mouseX < transform.position.x)
            {
                Flip(true);
            }
            else
            {
                Flip(false);
            }
        }

    }

    private void Flip(bool flip)
    {
        if (flip)
        {
            transform.localScale = new Vector2(-1,1);
        }
        else
        {
            transform.localScale = Vector2.one;
        }
    }

    private IEnumerator HurtColor(float time)
    {
        mat.SetInt("_Hurt", 1);
        yield return new WaitForSeconds(time);
        mat.SetInt("_Hurt", 0);
    }

    private void Attack() {

        MeleeRangeEnemy = FindClosestLiving();

        if (MeleeRangeEnemy)
        {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        animator.SetTrigger("Melee");
                    }     
        }

    }

    public void DealDamage() {

        MeleeRangeEnemy.GetComponent<Character>().TakeDamage(attackDamage);

    }

    public void SetSummon(int index) {

        currentSummon = index;

    }

    private void SummonAnimation()
    {

        closestEnemy = FindClosestDead();

        if (health >= currentSummon+1)
        {
            if (closestEnemy)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    health -= currentSummon + 1;
                    display.UpdateValue(health);
                    if (health <= 0)
                    {

                        Death();

                        

                        return;
                    }

                    animator.SetBool("Summoning", true);
                }
            }

        }
    }

    private IEnumerator SummonDead(float sec)
    {
        yield return new WaitForSeconds(sec);

        closestEnemy.GetComponent<Living>().Summon(Summonables[currentSummon]);

        animator.SetBool("Summoning", false);
    }

    private void ModifySpeed(float modifier)
    {
        speed = baseSpeed * modifier;
    }

    public void CollectSoul(float s) {

        health += s;

        health = Mathf.Clamp(health, 0f, maxHealth);

        display.UpdateValue(health);
    }

    private GameObject FindClosestDead()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        if (Physics2D.CircleCast(transform.position, summonRadius, Vector2.zero, contact, hits) != 0)
        {

            float currentClosest = Mathf.Infinity;
            GameObject closestObject = null;

            foreach (RaycastHit2D _target in hits)
            {

                float dist = Vector2.Distance(_target.collider.gameObject.transform.position, transform.position);

                if (dist < currentClosest && _target.collider.gameObject.GetComponent<ArtificialCharacter>().dead && !_target.collider.gameObject.GetComponent<Living>().summoned)
                {
                    currentClosest = dist;
                    closestObject = _target.collider.gameObject;
                }
            }

            if (closestObject != null)
            {
                return closestObject;
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

    private GameObject FindClosestLiving()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        if (Physics2D.CircleCast(transform.position, attackRange, Vector2.zero, contact, hits) != 0)
        {

            float currentClosest = Mathf.Infinity;
            GameObject closestObject = null;

            foreach (RaycastHit2D _target in hits)
            {

                float dist = Vector2.Distance(_target.collider.gameObject.transform.position, transform.position);

                if (dist < currentClosest && !_target.collider.gameObject.GetComponent<ArtificialCharacter>().dead && !_target.collider.gameObject.GetComponent<Living>().summoned)
                {
                    currentClosest = dist;
                    closestObject = _target.collider.gameObject;
                }
            }

            if (closestObject != null)
            {
                return closestObject;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, summonRadius);

        Gizmos.color = Color.red;
    }

}
