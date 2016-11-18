using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    Destroying,
    Moving,
    Idle
}

public interface IGameState
{
    void Init();
}
