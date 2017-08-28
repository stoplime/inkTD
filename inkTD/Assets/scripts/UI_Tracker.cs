using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Tracker : MonoBehaviour
{
    

    public void PointerEnter()
    {
        helper.Help.MouseOnUI = true;
    }

    public void PointerExit()
    {
        helper.Help.MouseOnUI = false;
    }

}
