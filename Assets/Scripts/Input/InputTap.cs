using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTap : InputEvent {



    public InputTap(GameObject hitObject)
    {
        HitObject = hitObject;
        EventStartTime = Time.time;
        InputType = InputTypes.Tap;
    }

}
