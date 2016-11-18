using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyJewelAnimation : MonoBehaviour
{

    private float _startTime;

    public void StartDestroyAnimation(Color toColor, float time, Renderer rend)
    {
        StartCoroutine(AnimateEmission(toColor, time, rend));
    }

    private IEnumerator AnimateEmission(Color toColor, float time, Renderer rend)
    {
        _startTime = Time.time;
        var currentColor = rend.material.GetColor("_EmissionColor");

        while (currentColor != toColor)
        {
            var timePlayed = Time.time - _startTime;
            currentColor = Color.Lerp(currentColor, toColor, timePlayed/time);
            rend.material.SetColor("_EmissionColor", currentColor);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
        Destroy(this);
    }
}
