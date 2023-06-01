using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scared : Living
{

    protected override void SelectTarget()
    {
        base.SelectTarget();

        if (closest != null)
        {

            Vector2 dif = transform.position - closest.position;

            dif = dif.normalized;

            target = (Vector2)transform.position + (dif * visionRadius);
        }
        else
        {
            target = transform.position;
        }

    }

}
