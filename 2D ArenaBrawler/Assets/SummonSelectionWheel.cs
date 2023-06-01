using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonSelectionWheel : MonoBehaviour
{

    public GameObject point;
    public RectTransform barBacking;
    public float lerpTime = 1;
    float elapsedTime, fill;

    public int index = 0;

    private float[] barWidths = { 199.8f + 6.3f, 349.8f + 6.3f, 499.8f + 6.3f, 649.8f + 6.3f };

    private Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>() ;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y < 0)
        {
            if (index > 0)
            {
                index -= 1;
            }
            else
            {
                index = 3;
            }

            player.SetSummon(index);
            elapsedTime = 0;

        }

        if (Input.mouseScrollDelta.y > 0)
        {
            if (index < 3)
            {
                index += 1;
            }
            else
            {
                index = 0;
            }

            player.SetSummon(index);
            elapsedTime = 0;

        }

        if (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;

            float rot = Mathf.Lerp(point.GetComponent<RectTransform>().rotation.eulerAngles.z, index * 90, elapsedTime / lerpTime);
            
            point.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rot);

            float barScale = Mathf.Lerp(barBacking.rect.width, barWidths[index], elapsedTime / lerpTime);
            barBacking.sizeDelta = new Vector2(barScale ,barBacking.rect.height);

        }





    }

}
