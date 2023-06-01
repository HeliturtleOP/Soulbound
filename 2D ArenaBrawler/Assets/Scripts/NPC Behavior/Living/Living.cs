using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Living : ArtificialCharacter
{
    [SerializeField]
    protected GameObject soulFragment;
    [SerializeField]
    protected int soulDrop = 1;

    public bool summoned;

    protected override void Start()
    {
        contact.SetLayerMask(1 << 8);

        base.Start();   
    }

    protected override void SelectTarget()
    {
        base.SelectTarget();

        if (closest != null)
        {
            target = closest.position;
        }
        else
        {
            target = transform.position;
        }

    }

    protected override void Death()
    {
        base.Death();

        for (int i = 0; i < soulDrop; i++)
        {
            Rigidbody2D fragRb = Instantiate(soulFragment, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Rigidbody2D>();

            fragRb.velocity += (Vector2)fragRb.transform.up.normalized * Random.Range(0.5f, 2f);
        }

        mat.SetInt("_Outline", 1);

    }

    public void Summon(GameObject character) {

        summoned = true;

        Instantiate(character, transform.position + new Vector3(0,0.05f,0), Quaternion.identity);

        mat.SetInt("_Outline", 0);
        mat.SetInt("_BlackAndWhite", 1);

    }

}
