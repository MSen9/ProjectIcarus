using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirTypes
{
    Up,
    Right,
    Down,
    Left
}
public class PlayerMovement : MonoBehaviour
{
    Vector2 currSpeed;
    float maxSpeed = 3f;
    float accelRate = 1/2f;
    float frameMult = 1 / 50f;
    KeyCode upKey;
    KeyCode downKey;
    KeyCode leftKey;
    KeyCode rightKey;
    public Rigidbody2D rb;
    Collider2D currShopDescSource;
    


    //code for managing walls that the player is touching 
    Dictionary<GameObject, bool[]> restrictedMovement;

    // Start is called before the first frame update
    void Start()
    {
        upKey = KeyCode.W;
        downKey = KeyCode.S;
        leftKey = KeyCode.A;
        rightKey = KeyCode.D;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (MapManager.current.doneLoading == false)
        {
            return;
        }
        if (Input.GetKey(rightKey))
        {
            if(currSpeed.x < 0)
            {
                currSpeed.x += accelRate;
            }
            currSpeed.x += accelRate;
        } else if(Input.GetKey(leftKey)){
            if (currSpeed.x > 0)
            {
                currSpeed.x -= accelRate;
            }
            currSpeed.x -= accelRate;
        } else
        {
            if (currSpeed.x > 0)
            {
                currSpeed.x -= accelRate;
                if(currSpeed.x < 0)
                {
                    currSpeed.x = 0;
                }
            }
            else if (currSpeed.x < 0)
            {
                currSpeed.x += accelRate;
                if (currSpeed.x > 0)
                {
                    currSpeed.x = 0;
                }
            }
        }

        if (Input.GetKey(upKey))
        {
            if (currSpeed.y < 0)
            {
                currSpeed.y += accelRate;
            }
            currSpeed.y += accelRate;
        }
        else if (Input.GetKey(downKey))
        {
            if (currSpeed.y > 0)
            {
                currSpeed.y -= accelRate;
            }
            currSpeed.y -= accelRate;
        }
        else
        {
            if (currSpeed.y > 0)
            {
                currSpeed.y -= accelRate;
                if (currSpeed.y < 0)
                {
                    currSpeed.y = 0;
                }
            }
            else if (currSpeed.y < 0)
            {
                currSpeed.y += accelRate;
                if (currSpeed.y > 0)
                {
                    currSpeed.y = 0;
                }
            }
        }
        currSpeed.x = Mathf.Clamp(currSpeed.x, maxSpeed * -1, maxSpeed);
        currSpeed.y = Mathf.Clamp(currSpeed.y, maxSpeed * -1, maxSpeed);
        /*
        //move using X then Y, check for blockage
        if(restrictedMovement.Count > 0)
        {
            //if movement is blocked by one touching wall then stop all movement
            int[] blockedCount = { 0, 0, 0, 0 };
            foreach(bool[] blocks in restrictedMovement.Values)
            {
                for (int i = 0; i < blocks.Length; i++)
                {
                    if(blocks[i] == true)
                    {
                        blockedCount[i]++;
                    }
                }
            }
            if(blockedCount[(int)DirTypes.Up] > 0)
            {
                if (Input.GetKey(upKey))
                {
                    RemoveWallRestrictions(DirTypes.Up);
                }
            }

        } 
        */
            rb.MovePosition(new Vector2(transform.position.x + currSpeed.x * frameMult,
                transform.position.y + currSpeed.y * frameMult));
        

    }
    /*
    void RemoveWallRestrictions(DirTypes dir)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject blocksGo in restrictedMovement.Keys)
        {
            if(restrictedMovement[blocksGo][(int)dir] == false)
            {
                toRemove.Add(blocksGo);
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            restrictedMovement.Remove(toRemove[i]);
        }
    }
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Main bullet has hit trigger");
        if (collision.tag == "ShopItem") {
            collision.gameObject.GetComponent<ShopItem>().PurchaseItem();
            ShopManager.current.RemoveItemDesc();
        } else if (collision.tag == "ShopItemDesc")
        {
            currShopDescSource = collision;
            string textDesc = collision.transform.parent.GetComponent<ShopItem>().itemDescription;
            ShopManager.current.MakeItemDesc(textDesc);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "ShopItemDesc")
        {
            if(currShopDescSource == collision)
            {
                ShopManager.current.RemoveItemDesc();
            }
        }
    }
}
