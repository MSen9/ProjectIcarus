using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    //public GameObject point1;
    public GameObject endPoint;
    //public float wallWidth = 4;
    
    //The widths of both unveiled points on the ends of each wall
    float POINT_WIDTHS = 64 / 100f;
    BoxCollider2D[] wallColliders;
    // Start is called before the first frame update
    void Start()
    {
        wallColliders = GetComponents<BoxCollider2D>();
        for (int i = 0; i < wallColliders.Length; i++)
        {
            wallColliders[i].size = new Vector2(endPoint.transform.localPosition.x + POINT_WIDTHS, wallColliders[i].size.y);
            wallColliders[i].offset = new Vector2(endPoint.transform.localPosition.x / 2, wallColliders[i].offset.y);
        }
        
    }


}
