using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public void Leave()
    {
        GameManager.Instance.Leave();
    }	
}
