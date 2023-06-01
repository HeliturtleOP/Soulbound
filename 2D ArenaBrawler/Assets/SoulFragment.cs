using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulFragment : MonoBehaviour
{

    float soulAmount = 1f/6f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {

            collision.gameObject.GetComponent<Player>().CollectSoul(soulAmount);

            Destroy(gameObject);

        }
    }
}
