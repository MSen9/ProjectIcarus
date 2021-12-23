using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum trackedTypes
{
    health,
    clears,
    fireRate,
    shotSize,
    manaGen,
    shotPen,
    shotSpread,
    shotExplode,
    shotSplit,
    trackedTypeCount
}
public class HealthPowerUpTracker : MonoBehaviour
{
    //float WORLD_TO_CANVAS_SCALE = 13.64f;
    float xSpacing = 1.8f;

    // Start is called before the first frame update
    GameObject player;
    PlayerShooting ps;
    HealthHandling hh;
    BulletClears bc;
    bool listsSetUp = false;
    public GameObject[] objTypes = new GameObject[9];

    public GameObject[] groups = new GameObject[9];

    GameObject[] pupTexts = new GameObject[9];

    public float[] scales = new float[9];

    int[] counts = { 0, 0, 0, 0, 0, 0, 0, 0, 0};

    float destroyTime = 2f;
    float destroyVel = 1f;
    float destroyRotation = 60f;
    float yFixScaling = -0.4f;
    List<List<GameObject>> trackedObjs = new List<List<GameObject>>(4);
    public GameObject bossHPBar;
    public GameObject gameOverInfo;
    public static HealthPowerUpTracker current;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ps = player.GetComponent<PlayerShooting>();
        hh = player.GetComponent<HealthHandling>();
        bc = player.GetComponent<BulletClears>();
        current = this;
    }

    void SetUpLists()
    {
        if (listsSetUp)
        {
            return;
        }
        for (int i = 0; i < (int)trackedTypes.trackedTypeCount; i++)
        {
            trackedObjs.Add(new List<GameObject>());
        }
        listsSetUp = true;
    }
    // Update is called once per frame
    void Update()
    {

    }


    public void RemoveUIObj(trackedTypes type, int removeCount = 1)
    {
        SetUpLists();
        int tType = (int)type;
        if(tType == 0 || tType == 1)
        {
            ClassicRemoveUIObj(type);
            return;
        }
        if(trackedObjs[tType].Count <= 0)
        {
            Debug.LogError("Trying to remove UI object from group that has no objects");
            return;
        }
        //remove the last one on the list
        if (removeCount >= counts[tType])
        {
            GameObject toRemove = trackedObjs[tType][0];
            AllPointManager apm = toRemove.GetComponent<AllPointManager>();
            apm.DeathAnimation(destroyVel, destroyRotation, destroyTime);
            Destroy(toRemove, destroyTime);
            trackedObjs[tType].Remove(toRemove);
            StringToVectorManager.current.StringExplode(pupTexts[tType], 2);
        } else
        {

            StringToVectorManager.current.DestroyString(pupTexts[tType]);
            pupTexts[tType] = StringToVectorManager.current.StringToVectors(":" + (counts[tType] - removeCount).ToString());
            pupTexts[tType].transform.position = groups[tType].transform.position + new Vector3(xSpacing*.8f, (2.5f-scales[tType])*yFixScaling);
            pupTexts[tType].transform.parent = groups[tType].transform;
        }
        
        counts[tType]--;
    }

    void ClassicRemoveUIObj(trackedTypes type)
    {
        int tType = (int)type;
        if (trackedObjs[tType].Count <= 0)
        {
            Debug.LogError("Trying to remove UI object from group that has no objects");
            return;
        }
        //remove the last one on the list
        GameObject toRemove = trackedObjs[tType][trackedObjs[tType].Count - 1];
        AllPointManager apm = toRemove.GetComponent<AllPointManager>();
        apm.DeathAnimation(destroyVel, destroyRotation, destroyTime);
        Destroy(toRemove, destroyTime);
        trackedObjs[tType].Remove(toRemove);
        counts[tType]--;
    }
    public void MakeUIObj(trackedTypes type, int madeCount = 1)
    {
        SetUpLists();
        int tType = (int)type;
        if(tType == 0 || tType == 1)
        {
            ClassicMakeUIObj(type);
            return;
        }
        if (counts[tType] == 0)
        {
            GameObject madeObj = Instantiate(objTypes[tType], groups[tType].transform.position, Quaternion.identity);
            madeObj.transform.parent = groups[tType].transform;
            madeObj.transform.localScale *= scales[tType];
            //disable unneccesary aspects
            if (madeObj.GetComponent<BulletMove>() != null)
            {
                madeObj.GetComponent<BulletMove>().enabled = false;
            }
            PolygonCollider2D[] pCols = madeObj.GetComponents<PolygonCollider2D>();
            for (int i = 0; i < pCols.Length; i++)
            {
                pCols[i].enabled = false;
            }
            BoxCollider2D[] bCols = madeObj.GetComponents<BoxCollider2D>();
            for (int i = 0; i < bCols.Length; i++)
            {
                bCols[i].enabled = false;
            }
            madeObj.GetComponent<AllPointManager>().SetToUiLayer();
            pupTexts[tType] = StringToVectorManager.current.StringToVectors(":" + madeCount.ToString());
            pupTexts[tType].transform.position = groups[tType].transform.position + new Vector3(xSpacing, (2.5f - scales[tType]) * yFixScaling);
            pupTexts[tType].transform.parent = groups[tType].transform;
            trackedObjs[tType].Add(madeObj);
        } else
        {
            StringToVectorManager.current.DestroyString(pupTexts[tType]);
            pupTexts[tType] = StringToVectorManager.current.StringToVectors(":" + (counts[tType] + madeCount).ToString());
            pupTexts[tType].transform.position = groups[tType].transform.position + new Vector3(xSpacing, (2.5f-scales[tType])*yFixScaling);
            pupTexts[tType].transform.parent = groups[tType].transform;
        }
        counts[tType] += madeCount;


    }
    void ClassicMakeUIObj(trackedTypes type)
    {
        SetUpLists();
        int tType = (int)type;
        GameObject madeObj = Instantiate(objTypes[tType], groups[tType].transform.position, Quaternion.identity);
        madeObj.transform.parent = groups[tType].transform;
        madeObj.transform.localScale *= scales[tType];
        madeObj.transform.position += new Vector3(xSpacing * counts[tType], 0);
        //disable unneccesary aspects
        if (madeObj.GetComponent<BulletMove>() != null)
        {
            madeObj.GetComponent<BulletMove>().enabled = false;
        }
        PolygonCollider2D[] pCols = madeObj.GetComponents<PolygonCollider2D>();
        for (int i = 0; i < pCols.Length; i++)
        {
            pCols[i].enabled = false;
        }
        BoxCollider2D[] bCols = madeObj.GetComponents<BoxCollider2D>();
        for (int i = 0; i < bCols.Length; i++)
        {
            bCols[i].enabled = false;
        }
        madeObj.GetComponent<AllPointManager>().SetToUiLayer();
        trackedObjs[tType].Add(madeObj);
        counts[tType]++;
    }
}
