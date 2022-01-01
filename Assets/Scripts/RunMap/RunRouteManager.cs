using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public struct RunNode
{
    public int nodeId;
    public Vector3 nodePos;
    public int waves;
    public int difficulty;
    public float mapSize;
    public int mapWalls;
    public Vector3[] wallPositions;
    public float[] wallAngles;
    public List<int> forwardConnections;
    public int widthPos;
    public int heightPos;
    public bool bossLevel;
    public RunNode(int nodeId, Vector3 nodePos, int waves, int difficulty, float mapSize, int mapWalls, Vector3[] wallPositions, float[] wallAngles, List<int> forwardConnections, int widthPos, int heightPos, bool bossLevel)
    {
        this.nodeId = nodeId;
        this.nodePos = nodePos;
        this.waves = waves;
        this.difficulty = difficulty;
        this.mapSize = mapSize;
        this.mapWalls = mapWalls;
        this.wallPositions = wallPositions;
        this.wallAngles = wallAngles;
        this.forwardConnections = forwardConnections;
        this.widthPos = widthPos;
        this.heightPos = heightPos;
        this.bossLevel = bossLevel;
    }

}
public class RunRouteManager : MonoBehaviour
{
    public GameObject baseNode;
    public GameObject wallObj;
    public int activeNode = -1;
    public GameObject activeNodeObj;
    // Start is called before the first frame update
    public static RunRouteManager current;
    public List<RunNode> runNodes;
    List<GameObject> nodeObjList;
    int mapDepth = 10;
    Vector3 TOP_NODE_POS = new Vector3(0, 10);
    int BASE_WALL_COUNT = 4;
    float BASE_MAP_SIZE = 18;
    GameObject player;
    float MIN_SIZE_BOOST = 3f;
    float MAX_SIZE_BOOST = 5f;
    public float scaleMod = 0.15f;
    float MAP_SIZE_POS_MOD = 5f;
    float CONNECTION_SCALE_MOD = 0.4f;
    int BASE_DIFFICULTY = 10;
    public bool movingPlayer = false;

    Vector3 startMovePos;
    float travelTime;
    float MAX_TRAVEL_TIME = 3f;
    public bool firstOpen = true;
    public List<int> beatenNodes;
    void Start()
    {
        if (current != null)
        {
            Destroy(this.gameObject);
            return;
        }
        current = this;
        nodeObjList = new List<GameObject>();
        
        if (SaveAndLoad.current.loadingRun)
        {
            runNodes = new List<RunNode>(SaveAndLoad.current.runSInfo.runNodes);
            beatenNodes = new List<int>(SaveAndLoad.current.runSInfo.beatenNodes);
            activeNode = SaveAndLoad.current.runSInfo.activeNode;
            BuildRunMap();
        }
        else
        {
            beatenNodes = new List<int>();
            runNodes = new List<RunNode>();
            MakeRunMap();
        }



        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Pauser.current.isPaused)
        {
            return;
        }
        if (movingPlayer)
        {
            travelTime += Time.deltaTime;
            player.transform.position = Vector3.Lerp(startMovePos, activeNodeObj.transform.position, travelTime / MAX_TRAVEL_TIME);
            if(travelTime > MAX_TRAVEL_TIME)
            {
                GoToMap();
            }
        }
    }

    void UpdateCameraPosition()
    {
        Vector3 camPos = Camera.main.transform.position;
        CamManager.current.camPos = new Vector3(camPos.x, player.transform.position.y - 10, camPos.z);
    }

    //run generation rules:
    //start with a single node
    //branch out to 2-3 nodes
    //use mimi maps to show what kind of map it is, put symbols underneath
    void MakeRunMap()
    {
        int nodeIdCount = 0;
        float yDistNeeded = 0;
        int mapWidth = 1; 
        for (int i = 0; i < mapDepth; i++)
        {
            yDistNeeded += (BASE_MAP_SIZE + MAX_SIZE_BOOST * i) * scaleMod*1.5f;
        }
        for (int i = mapDepth-1; i >= 0; i--)
        {
            int prevWidth = mapWidth;

            bool bossLevel = false;
            if (i == mapDepth - 1)
            {
                bossLevel = true;
            }
            if (i==0 || i == mapDepth - 1)
            {
                mapWidth = 1;
                
            } else
            {
                mapWidth = Random.Range(2, 4);
            }
            float avgWidth = (mapWidth - 1) / 2f;
            for (int j = 0; j < mapWidth; j++)
            {
                
                Vector3 nodePos;
                int waves;
                int difficulty;
                float mapSize;
                int mapWalls;
                float angleOffset = 0;
                Vector3[] wallPositions = { };
                float[] wallAngles = { };
                
                if (i == 0)
                {
                    //just make 1 center node with basic map
                    nodePos = TOP_NODE_POS;
                    mapSize = BASE_MAP_SIZE;
                    mapWalls = 0;

                }
                else
                {

                    mapSize = BASE_MAP_SIZE + Random.Range(MIN_SIZE_BOOST * i, MAX_SIZE_BOOST * i);
                    mapWalls = BASE_WALL_COUNT + Random.Range(0, i + 1);
                    nodePos = TOP_NODE_POS - new Vector3(0,yDistNeeded);
                    nodePos += new Vector3(mapSize/2.6f,0) * (j - avgWidth);
                    angleOffset = MakeMapStartingAngle();
                    wallPositions = HelperFunctions.GetEquilateralShapePointPositions(mapWalls, mapSize, angleOffset).ToArray();
                    wallAngles = GetWallAngles(wallPositions);
                }

                difficulty = BASE_DIFFICULTY + i * 10;
                waves = i/2 + 3; 
                List<int> forwardConnections = new List<int>();
                int prevNodeCount = prevWidth-1;
                float avgConnections = mapWidth / prevWidth;
                
                Dictionary<int, float> nodeDistIds = new Dictionary<int, float>();
                for (int k = runNodes.Count-1; k >= 0; k--)
                {
                    if(i == runNodes[k].heightPos-1)
                    {
                        float nodeDist = Vector3.Distance(nodePos, runNodes[k].nodePos);
                        nodeDistIds.Add( runNodes[k].nodeId, nodeDist);

                        forwardConnections.Add(runNodes[k].nodeId);
                    } else if(i != runNodes[k].heightPos)
                    {
                        //node must be at a lower depth, exit the loop
                        break;
                    }
                }
                int minConnections = (prevWidth + 1) - mapWidth;
                if(minConnections < 1)
                {
                    minConnections = 1;
                }
                if(Random.Range(0f,1f) > 0.5f)
                {
                    minConnections++;
                }
                int connectionsLeft = forwardConnections.Count;
                foreach(KeyValuePair<int,float> nodeDist in nodeDistIds.OrderByDescending(key => key.Value))
                {
                    if(connectionsLeft <= minConnections)
                    {
                        break;
                    }
                    forwardConnections.Remove(nodeDist.Key);
                    connectionsLeft--;
                }

                for (int k = 0; k < forwardConnections.Count; k++)
                {

                }
                runNodes.Add(new RunNode(nodeIdCount, nodePos, waves,difficulty, mapSize, mapWalls, wallPositions, wallAngles,forwardConnections,j,i,bossLevel));
                
                nodeIdCount++;
                
            }
            yDistNeeded -= (BASE_MAP_SIZE + MAX_SIZE_BOOST * i) * scaleMod*1.5f;
        }
        activeNode = nodeIdCount - 1;

        BuildRunMap();
    }

    
    public void BuildRunMap()
    {
        CamManager.current.SetScrollBounds(runNodes[0].nodePos.y);
        nodeObjList.Clear();
        foreach (RunNode rNode in runNodes)
        {
            Vector3 startPos = rNode.nodePos;
            bool nodeBeaten;
            if (beatenNodes.Contains(rNode.nodeId))
            {
                nodeBeaten = true;
            } else
            {
                nodeBeaten = false;
            }
            GameObject mapNode = makeMap(rNode.wallPositions, rNode.wallAngles, nodeBeaten);
            mapNode.transform.position = startPos;
            mapNode.GetComponent<NodeManager>().nodeId = rNode.nodeId;
            nodeObjList.Add(mapNode);
            for (int i = 0; i < rNode.forwardConnections.Count; i++)
            {

                Vector3 endPos = runNodes[rNode.forwardConnections[i]].nodePos;
                Vector3 nodeDist = startPos - endPos;
                float connectionAngle = HelperFunctions.AngleBetween(startPos, endPos) - 90f;
                GameObject madeConnection = Instantiate(wallObj, rNode.nodePos - nodeDist * 0.25f, Quaternion.Euler(0, 0, connectionAngle));
                madeConnection.transform.localScale = new Vector3(CONNECTION_SCALE_MOD, CONNECTION_SCALE_MOD, 1);
                madeConnection.GetComponent<AllPointManager>().pointObjs[1].transform.localPosition = new Vector3(nodeDist.magnitude * 0.5f/ CONNECTION_SCALE_MOD, 0);
                madeConnection.transform.parent = mapNode.transform;

            }
        }

        //place the main character at the active node
        player = GetPlayer();
        activeNodeObj = nodeObjList[activeNode];
        player.transform.position = runNodes[activeNode].nodePos;
        UpdateCameraPosition();

        foreach (int node in runNodes[activeNode].forwardConnections)
        {
            nodeObjList[node].GetComponent<NodeManager>().selectableNode = true;
        }
    }

    public void BeatLevel()
    {
        if(activeNode != -1)
        {
            beatenNodes.Add(activeNode);
        }
    }
    GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
    GameObject makeMap(Vector3[] wallPositions, float[] wallAngles, bool nodeBeaten)
    {
        GameObject mapParent = Instantiate(baseNode, Vector3.zero,Quaternion.identity);
        if (nodeBeaten)
        {
            return mapParent;
        }

        GameObject wallContainer = mapParent.GetComponent<NodeManager>().wallContainer;
        mapParent.transform.localScale *= 1 / scaleMod;
        
        for (int i = 0; i < wallPositions.Length; i++)
        {
            GameObject madeWall = Instantiate(wallObj, wallPositions[i], Quaternion.Euler(0, 0, wallAngles[i]-90f));
            madeWall.transform.parent = wallContainer.transform;
            int prevIndex = i - 1;
            if (i == 0)
            {
                prevIndex = wallPositions.Length - 1;
            }
            float wallLen = Vector3.Magnitude(wallPositions[i] - wallPositions[prevIndex]);
            madeWall.GetComponent<AllPointManager>().pointObjs[1].transform.localPosition = new Vector3(wallLen, 0);
            
        }
        mapParent.transform.localScale *= scaleMod;
        return mapParent;
    }
    float MakeMapStartingAngle(int diagonalOdds = 60, int cardinalOdds = 30, int otherOdds = 10)
    {
        float startingAngle;
        int pointAllignment = Random.Range(0, diagonalOdds + cardinalOdds + otherOdds + 1);
        if (pointAllignment < diagonalOdds)
        {
            startingAngle = 45;
        }
        else if (pointAllignment < diagonalOdds + cardinalOdds)
        {
            startingAngle = 0;
        }
        else
        {
            startingAngle = Random.Range(1, 90);
        }

        //See if you should flip the x and y values
        //flip across x axis
        if (Random.Range(0, 2) == 1)
        {
            startingAngle =  360 - startingAngle;
        }
        //flip across y axis
        if (Random.Range(0, 2) == 1)
        {
            startingAngle = 180 - startingAngle;
        }
        
        return startingAngle;
    }
   
    float[] GetWallAngles(Vector3[] wallPositions)
    {
        float[] wallAngles = new float[wallPositions.Length];
        for (int i = 0; i < wallPositions.Length-1; i++)
        {
            wallAngles[i] = HelperFunctions.AngleBetween(wallPositions[i], wallPositions[i + 1]);
        }
        wallAngles[wallPositions.Length-1] = HelperFunctions.AngleBetween(wallPositions[wallPositions.Length - 1], wallPositions[0]);

        return wallAngles;
    }
    public void GoToMap()
    {
        movingPlayer = false;
        SceneTransition.current.GoToScene("SampleScene");
    }

    public void MoveTowardsMap(int nodeID, GameObject nodeObj)
    {
        activeNode = nodeID;
        player.transform.rotation = Quaternion.Euler(0,0,HelperFunctions.AngleBetween(activeNodeObj.transform.position, nodeObj.transform.position) + 180);
        startMovePos = activeNodeObj.transform.position;
        activeNodeObj = nodeObj;
        travelTime = 0;
        LightTransition.current.StartFadeOut(MAX_TRAVEL_TIME);
        movingPlayer = true;
    }

    public RunNode GetCurrentRunNode()
    {
        return runNodes[activeNode];
    }

}
