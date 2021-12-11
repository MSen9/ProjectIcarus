using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FireType {
    normal,
    wavey, //shoots bullets in a ossiclating wave, life an over-the-top machinegun
    random8
}

public class EnemyShooting : MonoBehaviour
{
    AllPointManager apm;
    public GameObject bullet;
    public float reloadTime = 1f;
    float currReloadTime = 0;
    public FireType fireType = FireType.normal;
    public bool canShoot = false;
    public float shootDistOffset = .5f;
    float waveyOffset = 0;
    float WAVEY_OFFSET_MAX = 30;
    float waveyOffsetIncrement = 5;
    public float scaleMod = 1;
    public float speedMod = 1;
    // Start is called before the first frame update
    void Start()
    {
        currReloadTime = reloadTime;
        apm = GetComponent<AllPointManager>();
    }

    void Update()
    {
        if(MapManager.current.doneLoading == false || apm.doneExtending == false)
        {
            return;
        }
        if (canShoot)
        {
            if(currReloadTime < 0)
            {
                DoAttack(fireType);
            }
            currReloadTime -= Time.deltaTime;
            
        } 
    }

    void DoAttack(FireType currFireType)
    {
        currReloadTime += reloadTime;
        switch (currFireType){

            case FireType.normal:
                CreateBullet(transform.rotation.eulerAngles.z);
                break;
            case FireType.wavey:
                CreateBullet(transform.rotation.eulerAngles.z + waveyOffset);
                waveyOffset += waveyOffsetIncrement;
                if(Mathf.Abs(waveyOffset) >= WAVEY_OFFSET_MAX)
                {
                    waveyOffsetIncrement *= -1;
                }
                break;
            case FireType.random8:
                CreateBullet(transform.rotation.eulerAngles.z + Random.Range(0,8)*45);
                break;

        }

    }

    void CreateBullet(float rotation)
    {
        GameObject madeBullet = Instantiate(bullet, transform.position + Quaternion.Euler(0,0,rotation) * new Vector2(0, shootDistOffset), Quaternion.Euler(0, 0, rotation));
        madeBullet.transform.parent = GameObject.FindGameObjectWithTag("BulletList").transform;
        madeBullet.transform.localScale *= scaleMod;
        madeBullet.GetComponent<BulletMove>().moveSpeed *= speedMod;
    }
}
