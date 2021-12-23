using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyMover : MonoBehaviour
{
    // Start is called before the first frame update
    public int value = 1;
    public AudioClip moneySound;
    float MAX_SPEED = 20;
    float acceleration = 2f;
    float speed = 2;
    float rotationGoal = 0;
    float rotationRate = 45f;
    float rotateAccel = 45f;
    GameObject player;
    Rigidbody2D rb;
    Vector2 moveVals;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(player == null)
        {
            Destroy(gameObject);
            return;
        }
        rotationRate += rotateAccel * Time.fixedDeltaTime;
        float currAngle = rb.rotation;
        rotationGoal = HelperFunctions.AngleBetween(player.transform.position, transform.position);
        currAngle = HelperFunctions.RotateTowards(currAngle, rotationGoal, rotationRate, Time.fixedDeltaTime);
        rb.MoveRotation(currAngle);
        speed += acceleration*Time.fixedDeltaTime;
        if(speed > MAX_SPEED)
        {
            speed = MAX_SPEED;
        }
        moveVals = (transform.rotation * Vector2.up) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVals);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<CurrencyManager>().currency += value;
            collision.gameObject.GetComponent<SoundPlayer>().PlaySound(moneySound, 0.40f);
            Destroy(gameObject);
        }
    }
}
