using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum trackedTypes
{
    health,
    fireRate,
    shotSize,
    manaGen,
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
    bool listsSetUp = false;
    public GameObject[] objTypes = new GameObject[4];

    public GameObject[] groups = new GameObject[4];

    public float[] scales = new float[4];

    int[] counts = { 0, 0, 0, 0 };

    float destroyTime = 2f;
    float destroyVel = 1f;
    float destroyRotation = 60f;

    List<List<GameObject>> trackedObjs = new List<List<GameObject>>(4);

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ps = player.GetComponent<PlayerShooting>();
        hh = player.GetComponent<HealthHandling>();
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


    public void RemoveUiObj(trackedTypes type)
    {
        SetUpLists();
        int tType = (int)type;
        if(trackedObjs[tType].Count <= 0)
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
    public void MakeUIObj(trackedTypes type)
    {
        SetUpLists();
        int tType = (int)type;
        GameObject madeObj = Instantiate(objTypes[tType],groups[tType].transform.position,Quaternion.identity);
        madeObj.transform.localScale *= scales[tType];
        madeObj.transform.position += new Vector3(xSpacing * counts[tType], 0,0);
        trackedObjs[tType].Add(madeObj);
        counts[tType]++;

        //disable unneccesary aspects
        if(madeObj.GetComponent<BulletMove>() != null)
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
    }
}
