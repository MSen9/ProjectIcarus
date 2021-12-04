using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    // Start is called before the first frame update
    float fullScale = 1.5f;
    bool isFullScale = false;
    Vector3 growthRate;
    public bool canGoToNextLevel = false;
    AllPointManager apm;
    float exitToOnLeeway = 1.2f;
    float playerCollisionRadius = 1.5f;
    float exitRadius;
    float enterRadius;
    float spinSpeed = 180f;
    float innerDist = 1.4f;
    float gravitateTime = 0;
    float maxGravitateTime = 5f;
    float gravityBoostRate = 5f;
    bool fadingOut = false;

    public static MapTransition current;
    public PlayerSaveInfo betweenMapInfo;
    GameObject player;
    
    void Start()
    {
        if (current == null)
        {
            current = this;
        } 
        
        player = GameObject.FindGameObjectWithTag("Player");
        transform.localScale = new Vector3(0.01f,0.01f,1f);
        growthRate = new Vector3(0.5f, 0.5f);
        apm = GetComponent<AllPointManager>();
        exitRadius = fullScale * playerCollisionRadius * exitToOnLeeway;
        enterRadius = fullScale * playerCollisionRadius;

    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector2.Distance(player.transform.position, transform.position);
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z - spinSpeed * Time.deltaTime);
        if (MapManager.current.goingToNextLevel)
        {
            gravitateTime += Time.deltaTime;
            player.GetComponent<PlayerMovement>().gravityRate += gravityBoostRate * Time.deltaTime;
            if (MapManager.current.nextLevelFadeOut)
            {
                transform.localScale -= new Vector3(fullScale, fullScale)*Time.deltaTime/MapManager.current.fadeOutTime;
                
            }
            else if((dist < innerDist || gravitateTime > maxGravitateTime) && fadingOut == false)
            {
                fadingOut = true;
                LightTransition.current.StartFadeOut(MapManager.current.fadeOutTime);
                MapManager.current.nextLevelFadeOut = true;
            }
            return;
        }

        if(isFullScale == false)
        {
            transform.localScale += growthRate * Time.deltaTime;
            if(transform.localScale.x > fullScale)
            {
                isFullScale = true;
                transform.localScale = new Vector3(fullScale, fullScale, 1f);
                
                if (dist < exitRadius)
                {
                    apm.SetAllColors(new Color(1,1,1,0.2f));
                    canGoToNextLevel = false;
                } else
                {
                    canGoToNextLevel = true;
                }
                
            }
        } else
        {
            
            if (canGoToNextLevel)
            {
                
                if (dist < enterRadius)
                {
                    StartLevelTransition();
                }
            }
            else
            {
                if (dist > exitRadius)
                {
                    apm.SetAllColors(Color.white);
                    canGoToNextLevel = true;
                }
            }
        }

        
    }

    void StartLevelTransition()
    {
        MapManager.current.goingToNextLevel = true;
        player.GetComponent<PlayerMovement>().endLevelPoint = transform.position;
    }

    

}
