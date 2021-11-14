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
    BoxCollider2D wallColliders;
    // Start is called before the first frame update
    void Start()
    {
        UpdateWallCollider();
        
    }
    public void UpdateWallCollider()
    {
        wallColliders = GetComponent<BoxCollider2D>();
        wallColliders.size = new Vector2(endPoint.transform.localPosition.x + POINT_WIDTHS, wallColliders.size.y);
        wallColliders.offset = new Vector2(endPoint.transform.localPosition.x / 2, wallColliders.offset.y);
    }

}
