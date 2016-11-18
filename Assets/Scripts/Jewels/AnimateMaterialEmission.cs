using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateMaterialEmission : MonoBehaviour {

    public void AnimateLight(Color toColor, float speed, Renderer rend)
    {
        StartCoroutine(AnimateEmission(toColor, speed, rend));
    }

    private IEnumerator AnimateEmission (Color toColor, float speed, Renderer rend)
    {
        var currentColor = rend.material.GetColor("_EmissionColor");
        
        while (currentColor != toColor)
        {
            currentColor = Color.Lerp(currentColor, toColor, speed*Time.deltaTime);
            rend.material.SetColor("_EmissionColor", currentColor);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
        Destroy(this);
    }

    public void ForceDestroy()
    {
        StopAllCoroutines();
        Destroy(this);
    }
}
