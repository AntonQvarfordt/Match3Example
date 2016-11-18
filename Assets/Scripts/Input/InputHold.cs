using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHold : InputEvent
{
    public float HoldTime;

    public InputHold(GameObject hitObject)
    {
        HitObject = hitObject;
        EventStartTime = Time.time;
        InputType = InputTypes.Hold;
    }
}
