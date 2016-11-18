using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MonoBehaviour, IGameState
{
    private bool _isFirstRun = true;

    public void Init()
    {
        if (!_isFirstRun)
            return;

        SetAllJewelsInPlay();
        _isFirstRun = false;
    }

    public void SetAllJewelsInPlay()
    {
        var allJewels = GameManager.GetGameManager().DrawnJewels;
        for (var i = 0; i < allJewels.Count; i++)
        {
            var jewel = allJewels[i];
            if (jewel != null)
                jewel.InPlay = true;
        }
    }
}
