using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeManager : MonoBehaviour
{
    public GameObject wallContainer;
    bool isClicked = false;
    public int nodeId;
    public bool selectableNode = false;
    float scaleMaxBonus = 0.05f;
    float ZOOM_CHANGE_RATE = 1 / 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selectableNode)
        {
            transform.localScale = Vector3.one * (1 + Mathf.PingPong(Time.time*ZOOM_CHANGE_RATE, scaleMaxBonus));
        }
        
    }

    void OnMouseDown()
    {
        if (selectableNode)
        {
            isClicked = true;
        } else
        {
            Debug.Log("trying to select an unselectable node");
        }
        
    }

    void OnMouseUp()
    {
        if (isClicked && RunRouteManager.current.movingPlayer == false)
        {
            //go to that level
            Debug.Log("Trying to go to level: " + nodeId.ToString() + ". Height is: " + RunRouteManager.current.runNodes[nodeId].heightPos.ToString());
            DontDestroyOnLoad(this.gameObject);
            RunRouteManager.current.MoveTowardsMap(nodeId, gameObject);
        }
    }

    private void OnMouseExit()
    {
        //isClicked = false;
    }
}
