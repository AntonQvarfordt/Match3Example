using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputState
{
    Holding,
    Dragging,
    Idle
}

public class InputInstance
{
    public InputInstance(Jewel inputOn)
    {
        InputDown = new InputDown(inputOn.gameObject);
    }

    public List<Jewel> JewelsDraggedOver = new List<Jewel>();

    public InputState CurrentState;

    public InputTap InputTap;
    public InputDrag InputDrag;
    public InputHold InputHold;
    public InputUp InputUp;
    public InputDown InputDown;

    public void CreateDrag()
    {
        InputDrag = new InputDrag(Input.mousePosition);
    }

    public void AddDraggedOver(Jewel jewel)
    {
        JewelsDraggedOver.Add(jewel);



        if (!jewel.IsLit)
            jewel.LightOn();
    }

    public void UnlightDraggedOverJewels()
    {
        for (var i = 0; i < JewelsDraggedOver.Count; i++)
        {
            var jewel = JewelsDraggedOver[i];
            if (jewel.IsLit)
            {
                jewel.LightOff();
            }
        }
    }
}
