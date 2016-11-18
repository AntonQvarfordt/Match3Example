using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyingState : MonoBehaviour, IGameState
{
    private const float _destroyAnimationTime = 0.75f;

    public void Init()
    {
        CheckForMarked();
    }

    private void CheckForMarked()
    {
        var gameManager = GameManager.GetGameManager();
        var marked = gameManager.MarkForDeath();

        if (marked.Count > 2)
            DestroyMarked(marked);
        else
        {
            gameManager.SetActiveState(GameStates.Idle);
        }
    }

    private void DestroyMarked(List<Jewel> markedJewels)
    {
        StartCoroutine(DestroyMarkedCoroutine(markedJewels));
    }

    private IEnumerator DestroyMarkedCoroutine(List<Jewel> markedJewels)
    {
        for (var i = 0; i < markedJewels.Count; i++)
        {
            var jewel = markedJewels[i];
            jewel.DestroySelf(_destroyAnimationTime);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(_destroyAnimationTime);
        DestroyedMarkedCallback();
    }

    public void DestroyedMarkedCallback()
    {
        GameManager.GetGameManager().SetActiveState(GameStates.Moving);
    }
}
