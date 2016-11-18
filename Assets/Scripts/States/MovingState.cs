using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : MonoBehaviour, IGameState
{
    public void Init()
    {
        StartCoroutine(Move());
    }

    public IEnumerator Move()
    {
        var timeOutAfterAttempts = 100;
        while (GameManager.GetGameManager().HasEmptySlots)
        {
            if (timeOutAfterAttempts < 1)
            {
                break;
            }
            timeOutAfterAttempts--;
            var doMove = MoveDown();

            if (doMove)
            {
                yield return new WaitForSeconds(GameManager.GetGameManager().MoveAnimationTime);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
            
        }

        var marked = GameManager.GetGameManager().MarkForDeath();

        if (marked.Count > 2)
        {
            GameManager.GetGameManager().SetActiveState(GameStates.Destroying);
        }
        else
        {
            GameManager.GetGameManager().SetActiveState(GameStates.Idle);
        }
        yield return null;
    }

    private static bool MoveDown()
    {
        var returnValue = false;
        for (var i = GameManager.JewelSlots.Count-1; i >= 0; i--)
        {
            for (var j = 0; j < GameManager.JewelSlots[i].Count; j++)
            {
                var jewelSlot = GameManager.JewelSlots[i][j];
                jewelSlot.CheckMove();
                returnValue = true;
            }
        }
        return returnValue;
    }
}