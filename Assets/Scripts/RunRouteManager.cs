using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct RunNode
{
    public Vector3 nodePos;
    public int waves;
    public float mapSize;
    public int mapWalls;
    public Vector3[] wallPositions;
    public Vector3[] wallAngles;
    public List<RunNode> forwardConnections;

    public RunNode(Vector3 nodePos, int waves, float mapSize, int mapWalls, Vector3[] wallPositions, Vector3[] wallAngles, List<RunNode> forwardConnections)
    {
        this.nodePos = nodePos;
        this.waves = waves;
        this.mapSize = mapSize;
        this.mapWalls = mapWalls;
        this.wallPositions = wallPositions;
        this.wallAngles = wallAngles;
        this.forwardConnections = forwardConnections;
    }
}
public class RunRouteManager : MonoBehaviour
{
    public GameObject baseNode;
    public GameObject wallObj;

    // Start is called before the first frame update
    public static RunRouteManager current;
    List<RunNode> runNodes;
    List<GameObject> nodeObjList;
    int mapDepth = 10;
    Vector3 TOP_NODE_POS = new Vector3(0, -10);
    int BASE_WALL_COUNT = 4;
    float BASE_MAP_SIZE = 18;

    float MIN_SIZE_BOOST = 3f;
    float MAX_SIZE_BOOST = 6f;
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
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //run generation rules:
    //start with a single node
    //branch out to 2-3 nodes
    //use mimi maps to show what kind of map it is, put symbols underneath
    void MakeRunMap()
    {
        for (int i = 0; i < mapDepth; i++)
        {
            
            Vector3 nodePos;
            int waves;
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
                mapWalls = BASE_WALL_COUNT;
                
            } else
            {
                nodePos = TOP_NODE_POS + new Vector3(0,-5)*i;
                mapSize = BASE_MAP_SIZE + Random.Range(MIN_SIZE_BOOST, MAX_SIZE_BOOST)*i;
                mapWalls = BASE_WALL_COUNT + Random.Range(0, i + 1);
            }
            angleOffset = MakeMapStartingAngle();
            wallPositions = HelperFunctions.GetEquilateralShapePointPositions(mapWalls, mapSize, angleOffset).ToArray();
            wallAngles =  GetWallAngles(wallPositions);
            GameObject mapNode = makeMap(wallPositions, wallAngles);
            mapNode.transform.position = nodePos;

            
            nodeObjList.Add(mapNode);
        }

        //make connections
    }

    GameObject makeMap(Vector3[] wallPositions, float[] wallAngles)
    {
        GameObject mapParent = Instantiate(baseNode);



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
    void LoadRunMap()
    {

    }
}
