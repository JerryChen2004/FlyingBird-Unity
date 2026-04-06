using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour
{
    public TMP_Text flashingText;
    private float flashSpeed = 0.5f;

    void Start()
    {
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        while (true)
        {
            flashingText.enabled = !flashingText.enabled;
            yield return new WaitForSeconds(flashSpeed);
        }
    }
}
