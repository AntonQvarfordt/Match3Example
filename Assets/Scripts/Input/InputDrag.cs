using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDrag : InputEvent
{
    private readonly Vector2 _startPos;

    public InputDrag(Vector2 startPos)
    {
        _startPos = startPos;
        EventStartTime = Time.time;
        InputType = InputTypes.Drag;
    }

    public float DragDistance(Vector2 currentPos)
    {
        return Vector2.Distance(_startPos, currentPos);
    }

    public void DragUpdate(RaycastHit hit, InputInstance parentInstance)
    {
        if (hit.collider == null)
            return;

        if (!parentInstance.JewelsDraggedOver.Contains(hit.collider.gameObject.GetComponent<Jewel>()))
        {
            parentInstance.AddDraggedOver(hit.collider.GetComponent<Jewel>());
        }

    }
}