using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType{
    damage,
    powerUp
}
public class BulletMove : MonoBehaviour
{
    BulletType bulletType = BulletType.damage;
    float moveSpeed = 1/60f;
    //the rotation on the z axis with which the bullet is moving
    
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = transform.rotation * Vector2.up;
        //move the BIG SHOT (or regular shot)
        transform.position += new Vector3(movement.x*moveSpeed,movement.y*moveSpeed);
    }
}
