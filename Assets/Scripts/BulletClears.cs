using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletClears : MonoBehaviour
{
    KeyCode clearKey = KeyCode.Q;
    float CLEAR_RANGE = 15;
    public int clears = 3;
    float currBulletStopTime = -1;
    float bulletStopTime = 2f;
    bool eShootingDisabled = false;
    HealthPowerUpTracker healthPupTracker;
    public GameObject dropper;
    GameObject droppedClear;
    AllPointManager droppedApm;
    float DROPPED_FADE_RATE = 1f;
    public AudioClip clearSound;
    // Start is called before the first frame update
    void Start()
    {
        if (BetweenMapInfo.current != null && BetweenMapInfo.current.hasInfoSaved)
        {
            clears = BetweenMapInfo.current.savedInfo.clears;
        }
        healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        for (int i = 0; i < clears; i++)
        {
            healthPupTracker.MakeUIObj(trackedTypes.clears);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MapManager.current == null || MapManager.current.doneLoading == false || MapManager.current.mapOver)
        {
            return;
        }
        if (Input.GetKeyDown(clearKey) && currBulletStopTime < 0 && clears > 0)
        {
            ClearBullets();
        }
        currBulletStopTime -= Time.deltaTime;
        if(eShootingDisabled && currBulletStopTime < 0)
        {
            SetEnemyShooting(true);
        }
        if(droppedClear != null)
        {
            Color currColor = droppedApm.GetPointColor();
            if(currColor.a <= 0)
            {
                Destroy(droppedClear);
            } else
            {
                droppedApm.SetAllColors(currColor - new Color(0, 0, 0, DROPPED_FADE_RATE * Time.deltaTime));
            }
            
        }
    }

    void ClearBullets()
    {
        //play a powerful sound effect and break the bullets
        clears--;
        Transform bulletList = GameObject.FindGameObjectWithTag("BulletList").transform;
        for (int i = 0; i < bulletList.childCount; i++)
        {
            GameObject currBullet = bulletList.GetChild(i).gameObject;
            if(Vector3.Distance(transform.position, currBullet.transform.position) < CLEAR_RANGE)
            {
                AllPointManager apm = currBullet.GetComponent<AllPointManager>();
                if(apm != null)
                {
                    apm.BreakBullet(1f,45f,1);
                }
                else
                {
                    Debug.LogError("Error: No apm component for: " + currBullet.name);
                }
            }
        }
        
        currBulletStopTime = bulletStopTime;
        SetEnemyShooting(false);
        healthPupTracker.RemoveUIObj(trackedTypes.clears);
        droppedClear = Instantiate(dropper,transform.position,Quaternion.identity);
        droppedApm = droppedClear.GetComponent<AllPointManager>();
        SoundPlayer.MakeOnlySound(clearSound, 0.35f);
    }

    void SetEnemyShooting(bool enable)
    {
        GameObject eList = GameObject.FindGameObjectWithTag("EnemyList");
        for (int i = 0; i < eList.transform.childCount; i++)
        {
            GameObject currChild = eList.transform.GetChild(i).gameObject;
            EnemyShooting[] es = currChild.GetComponents<EnemyShooting>();
            for (int j = 0; j < es.Length; j++)
            {
                es[j].clearActive = !enable;
            }
        }
        eShootingDisabled = !enable;
    }
}
