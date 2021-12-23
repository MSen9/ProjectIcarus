using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FireType {
    normal,
    wavey, //shoots bullets in a ossiclating wave, life an over-the-top machinegun
    random5,
    totalSpin,
    totalChaos,
    oneBullet,
    tripleShot,
    eightShot,
    sevenShot,
    finalBoss
}

public class EnemyShooting : MonoBehaviour
{
    AllPointManager apm;
    public GameObject bullet;
    public float reloadTime = 1f;
    float currReloadTime = 0;
    public FireType fireType = FireType.normal;
    public bool canShoot = false;
    public bool clearActive = false;
    public float shootDistOffset = .5f;
    float waveyOffset = 0;
    float WAVEY_OFFSET_MAX = 30;
    float waveyOffsetIncrement = 8;
    public float scaleMod = 1;
    public float speedMod = 1;
    float spinOffset = 0;
    float spinOffsetChange = 15;
    public GameObject oneBullet;
    public bool alwaysShoot = false;
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
        if ((canShoot || alwaysShoot) && clearActive == false)
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
            case FireType.random5:
                CreateBullet(transform.rotation.eulerAngles.z + Random.Range(-2,3)*45);
                break;
            case FireType.totalSpin:
                spinOffset += spinOffsetChange;
                CreateBullet(transform.rotation.eulerAngles.z + spinOffset);
                break;
            case FireType.totalChaos:
                CreateBullet(transform.rotation.eulerAngles.z + Random.Range(-90, 91));
                break;
            case FireType.oneBullet:
                if(oneBullet == null)
                {
                    oneBullet = CreateBullet(transform.rotation.eulerAngles.z);
                }
                
                break;
            case FireType.tripleShot:
                CreateBullet(transform.rotation.eulerAngles.z);
                CreateBullet(transform.rotation.eulerAngles.z + 35);
                CreateBullet(transform.rotation.eulerAngles.z - 35);
                break;
            case FireType.eightShot:
                for (int i = 0; i < 8; i++)
                {
                    CreateBullet(transform.rotation.eulerAngles.z + 45*i);
                }
                break;
            case FireType.sevenShot:
                float randomAddon = Random.Range(0, 51f);
                for (int i = 0; i < 7; i++)
                {
                    
                    CreateBullet(transform.rotation.eulerAngles.z + randomAddon + 51.42f * i);
                }
                break;
        }

    }

    GameObject CreateBullet(float rotation)
    {
        GameObject madeBullet = Instantiate(bullet, transform.position + Quaternion.Euler(0,0,rotation) * new Vector2(0, shootDistOffset), Quaternion.Euler(0, 0, rotation));
        madeBullet.transform.parent = GameObject.FindGameObjectWithTag("BulletList").transform;
        madeBullet.transform.localScale *= scaleMod;
        madeBullet.GetComponent<BulletMove>().moveSpeed *= speedMod;
        return madeBullet;
    }
}
