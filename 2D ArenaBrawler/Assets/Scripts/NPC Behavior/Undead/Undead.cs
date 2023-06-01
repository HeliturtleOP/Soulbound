using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead : ArtificialCharacter
{

    protected Transform Player;

    protected override void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        contact.SetLayerMask(1 << 7);

        base.Start();

    }

    protected override void SelectTarget()
    {
        base.SelectTarget();

        //
        if (Input.GetKey(KeyCode.Space))
        {
            target = (Vector2)Player.position + transformOffset;
        }
        else if (closest)
        {

            targetScript = null;

            targetScript = closest.gameObject.GetComponent<Character>();

            target = closest.position;

        }
        else
        {
            animator.speed = 1;
            animator.SetBool("AtEnemy", false);

            target = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + transformOffset;
        }


        if (closest != null)
        {
            target = closest.position;
        }
        else if (Vector2.Distance(transform.position, closest.position) <= attackRange)
        {
            animator.speed = attackSpeed;
            animator.SetBool("AtEnemy", true);
        }
        else
        {
            animator.speed = 1;
            animator.SetBool("AtEnemy", false);
            target = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + transformOffset;
        }

    }

}
