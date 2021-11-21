using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RunRouteManager.current.BuildRunMap();
    }
}
