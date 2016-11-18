using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Jewel GetJewelToRight(this Jewel jewel)
    {
        return GameManager.JewelSlots[jewel.MatrixIndex.Key][jewel.MatrixIndex.Value - 1].GetJewel;
    }

    public static Jewel GetJewelToLeft(this Jewel jewel)    
    {
        return GameManager.JewelSlots[jewel.MatrixIndex.Key][jewel.MatrixIndex.Value + 1].GetJewel;
    }

    public static Jewel GetJewelAbove(this Jewel jewel)
    {
        return GameManager.JewelSlots[jewel.MatrixIndex.Key + 1][jewel.MatrixIndex.Value].GetJewel;
    }

    public static Jewel GetJewelBelow(this Jewel jewel)
    {
        return GameManager.JewelSlots[jewel.MatrixIndex.Key - 1][jewel.MatrixIndex.Value].GetJewel;
    }
}
