using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [Header("Stats")]

    public int maxHealth = 10;
    protected float health;

    protected Material mat;
    protected Animator animator;
    protected Rigidbody2D rb;
    public bool dead = false;

    public virtual void TakeDamage(float amount)
    {

        if (health > 0)
        {
            health -= amount;
            StartCoroutine(HurtColor(0.2f));
        }
        else
        {
            Death();
        }

    }

    private IEnumerator HurtColor(float time)
    {
        mat.SetInt("_Hurt", 1);
        yield return new WaitForSeconds(time);
        mat.SetInt("_Hurt", 0);
    }

    protected virtual void Death()
    {

        animator.SetBool("Dead", true);
        dead = true;
        rb.velocity = Vector2.zero;
        CancelInvoke();
    }

}
