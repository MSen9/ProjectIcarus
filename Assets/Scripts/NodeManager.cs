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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        isClicked = true;
    }

    void OnMouseUp()
    {
        if (isClicked)
        {
            //go to that level
            Debug.Log("Trying to go to level: " + nodeId.ToString());
            DontDestroyOnLoad(this.gameObject);
            RunRouteManager.current.GoToMap(nodeId, gameObject);
        }
    }

    private void OnMouseExit()
    {
        isClicked = false;
    }
}
