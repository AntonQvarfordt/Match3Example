using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum InputTypes
{
    Down,
    Up,
    Tap,
    Drag,
    Hold
}

public class InputEvent {

    public GameObject HitObject;
    public InputTypes InputType;
    public float EventStartTime;
}
