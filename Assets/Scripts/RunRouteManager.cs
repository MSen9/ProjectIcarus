using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    public RunNode(int nodeId, Vector3 nodePos, int waves, int difficulty, float mapSize, int mapWalls, Vector3[] wallPositions, float[] wallAngles, List<int> forwardConnections, int widthPos, int heightPos)
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
    }
}
public class RunRouteManager : MonoBehaviour
{
    public GameObject baseNode;
    public GameObject wallObj;
    public int activeNode;
    public GameObject activeNodeObj;
    // Start is called before the first frame update
    public static RunRouteManager current;
    List<RunNode> runNodes;
    List<GameObject> nodeObjList;
    int mapDepth = 10;
    Vector3 TOP_NODE_POS = new Vector3(0, 10);
    int BASE_WALL_COUNT = 4;
    float BASE_MAP_SIZE = 18;
    GameObject player;
    float MIN_SIZE_BOOST = 3f;
    float MAX_SIZE_BOOST = 6f;
    public float scaleMod = 0.15f;
    float MAP_SIZE_POS_MOD = 5f;
    float CONNECTION_SCALE_MOD = 0.4f;
    int BASE_DIFFICULTY = 10;
    bool movingPlayer = false;
    void Start()
    {
        if (current != null)
        {
            Destroy(this.gameObject);
            return;
        }
        current = this;
        runNodes = new List<RunNode>();
        nodeObjList = new List<GameObject>();
        MakeRunMap();
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (movingPlayer)
        {
            UpdateCameraPosition();
        }
    }

    void UpdateCameraPosition()
    {
        ector3 camPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(camPos.x, player.transform.position.y + TOP_NODE_POS.y, camPos.z);
    }

    //run generation rules:
    //start with a single node
    //branch out to 2-3 nodes
    //use mimi maps to show what kind of map it is, put symbols underneath
    void MakeRunMap()
    {
        int nodeIdCount = 0;
        float yDistNeeded = 0;
        for (int i = 0; i < mapDepth; i++)
        {
            yDistNeeded += (BASE_MAP_SIZE + MAX_SIZE_BOOST * i) * scaleMod*1.5f;
        }
        for (int i = mapDepth-1; i >= 0; i--)
        {
            int mapWidth = 1;
            if(i==0 || i == mapDepth - 1)
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
                float angleOffset;
                Vector3[] wallPositions;
                float[] wallAngles;
                if (i == 0)
                {
                    //just make 1 center node with basic map
                    nodePos = TOP_NODE_POS;
                    mapSize = BASE_MAP_SIZE;
                    mapWalls = 0;

                }
                else
                {
                    
                    mapSize = BASE_MAP_SIZE + Random.Range(MIN_SIZE_BOOST, MAX_SIZE_BOOST) * i;
                    mapWalls = BASE_WALL_COUNT + Random.Range(0, i + 1);
                    nodePos = TOP_NODE_POS - new Vector3(0,yDistNeeded);
                    nodePos += new Vector3(mapSize/2.6f,0) * (j - avgWidth);
                }

                difficulty = BASE_DIFFICULTY + i * 10;
                waves = i + 2;
                angleOffset = MakeMapStartingAngle();
                wallPositions = HelperFunctions.GetEquilateralShapePointPositions(mapWalls, mapSize, angleOffset).ToArray();
                wallAngles = GetWallAngles(wallPositions);
                List<int> forwardConnections = new List<int>();
                for (int k = runNodes.Count-1; k >= 0; k--)
                {
                    if(i == runNodes[k].heightPos-1)
                    {
                        forwardConnections.Add(runNodes[k].nodeId);
                    } else if(i != runNodes[k].heightPos)
                    {
                        //node must be at a lower depth, exit the loop
                        break;
                    }
                }
                runNodes.Add(new RunNode(nodeIdCount, nodePos, waves,difficulty, mapSize, mapWalls, wallPositions, wallAngles,forwardConnections,j,i));
                
                nodeIdCount++;
                
            }
            yDistNeeded -= (BASE_MAP_SIZE + MAX_SIZE_BOOST * i) * scaleMod*1.5f;
        }
        activeNode = nodeIdCount - 1;

        BuildRunMap();
    }

    public void BuildRunMap()
    {
        nodeObjList.Clear();
        foreach (RunNode rNode in runNodes)
        {
            Vector3 startPos = rNode.nodePos;
            GameObject mapNode = makeMap(rNode.wallPositions, rNode.wallAngles);
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
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = runNodes[activeNode].nodePos;
        UpdateCameraPosition();
    }

    GameObject makeMap(Vector3[] wallPositions, float[] wallAngles)
    {
        GameObject mapParent = Instantiate(baseNode, Vector3.zero,Quaternion.identity);
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
    public void GoToMap(int nodeID, GameObject nodeObj)
    {
        activeNode = nodeID;
        activeNodeObj = nodeObj;
        SceneManager.LoadScene("SampleScene");
    }

    public RunNode GetCurrentRunNode()
    {
        return runNodes[activeNode];
    }

}
