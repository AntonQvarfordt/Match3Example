using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDown : InputEvent {

    public InputDown(GameObject hitObject)
    {
        HitObject = hitObject;
        EventStartTime = Time.time;
        InputType = InputTypes.Down;
    }
}
