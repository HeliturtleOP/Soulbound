using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarDisplay : MonoBehaviour
{
    public float value, maxValue;

    public Image filler;

    public float lerpTime = 1;
    float elapsedTime, fill;

    private void Update()
    {
        LerpValue();
    }

    public void UpdateValue(float inputValue) {

        if (inputValue > maxValue)
        {
            value = maxValue;
        }
        else {
            value = inputValue;
        }

        elapsedTime = 0;

    }

    void LerpValue() {

        if (value <= maxValue + 0.01f)
        {

            if (elapsedTime < lerpTime)
            {

                float currentFill = filler.fillAmount;

                fill = Mathf.Lerp(currentFill, value/maxValue, elapsedTime / lerpTime);
                elapsedTime += Time.deltaTime;
                filler.fillAmount = fill;

            }

        }



    }

}
