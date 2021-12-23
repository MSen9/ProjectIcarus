using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    Rigidbody2D rb;
    public float rotateRate = 60f;
    BulletMove bm;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.LogError("No player detected for tracking bullet");
            enabled = false;
        }
        rb = GetComponent<Rigidbody2D>();
        bm = GetComponent<BulletMove>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.SetRotation(HelperFunctions.RotateTowards(transform.rotation.eulerAngles.z, HelperFunctions.AngleBetween(transform.position,player.transform.position)-180f, rotateRate, Time.deltaTime));
        bm.UpdateMovement();
    }
}
