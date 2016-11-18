using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUp : InputEvent {

    public InputUp(GameObject hitObject)
    {
        HitObject = hitObject;
        EventStartTime = Time.time;
        InputType = InputTypes.Up;
    }
}
