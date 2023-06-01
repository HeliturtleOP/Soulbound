using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject[] Enemies;

    private Transform player;
    private int spawned = 0;

    public AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating("Spawn", 1, 0.1f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn() {

        spawned++;

        if (spawned > 100) {

            CancelInvoke();
            return;

        }

        float xPos = Random.Range(-25f, 25f);
        float yPos = Random.Range(-25f, 25f);

        Vector2 pos = new Vector2(xPos, yPos);

        if (Vector2.Distance(pos, player.position) <= 12.5)
        {

            Spawn();
            return;

        }
        else
        {

            Instantiate(selectEnemy(), pos, Quaternion.identity);

        }

    }

    GameObject selectEnemy() {

        float selection = Random.Range(0f, 1f);
        selection = curve.Evaluate(selection);
        selection *= (Enemies.Length - 1);

        int selectInt = Mathf.RoundToInt(selection);

        Debug.Log(selectInt);

        return Enemies[selectInt];

    }

}
