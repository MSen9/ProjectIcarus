using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum StringAlignment
{
    left,
    right,
    center
}
public class StringToVectorManager : MonoBehaviour
{
    public Dictionary<char,GameObject> vectorChars;
    public GameObject emptyGameObject;
    string CHAR_LIST = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ+-!()";
    string[] SPEC_CHARS = new string[]{ ":colon", "%percent","?questionMark","/fSlash","'apostrophe",",comma",".period" };
    string VECTOR_PREFAB_DIRECTORY = "VectorLetters";
    string FULL_FILE_PATH = "Assets/Resources/VectorLetters";
    public static StringToVectorManager current;
    float generalXOffsetBoost = 0.2f;
    char spaceLetter = ' ';
    List<GameObject> vectorStrings;
    Color WHITE = new Color(1, 1, 1);
    // Start is called before the first frame update
    void OnEnable()
    {
        if (current != null)
        {
            return;
        }
        current = this;
        DontDestroyOnLoad(this.gameObject);
        vectorChars = new Dictionary<char, GameObject>();
        vectorStrings = new List<GameObject>();
        for (int i = 0; i < CHAR_LIST.Length; i++)
        {

            string path = Path.Combine(VECTOR_PREFAB_DIRECTORY, CHAR_LIST[i].ToString());
            try
            {
                GameObject currChar = Resources.Load(path) as GameObject;
                vectorChars.Add(CHAR_LIST[i], currChar);
            
            } catch
            {
                Debug.LogError("No valid prefab for: " + CHAR_LIST[i].ToString());
            }
                
            
            
        }

        for (int i = 0; i < SPEC_CHARS.Length; i++)
        {
            string path = Path.Combine(VECTOR_PREFAB_DIRECTORY, SPEC_CHARS[i].Substring(1, SPEC_CHARS[i].Length - 1));
            
            try
            {
                GameObject currChar = Resources.Load(path) as GameObject;
                vectorChars.Add(SPEC_CHARS[i][0], currChar);

            }
            catch
            {
                Debug.LogError("No valid prefab for: " + SPEC_CHARS[i][0].ToString());
            }
        }

    }

    public GameObject StringToVectors(string inputString, float stringScale = 1f, 
        StringAlignment align = StringAlignment.left, float[] textCols = null)
    {
        string[] lineStrings = inputString.Split(new string[] { "\n","\\n" }, System.StringSplitOptions.None);
        for (int i = 0; i < lineStrings.Length; i++)
        {
            lineStrings[i] = lineStrings[i].ToUpper();
        }

        float baseYGap = 1.75f;

       
        
        float[] overallWidths = new float[lineStrings.Length];
        
        List<List<GameObject>> stringChars = new List<List<GameObject>>();
        GameObject vectorHolder = Instantiate(emptyGameObject, new Vector3(0,0,0), Quaternion.identity);


        for (int i = 0; i < lineStrings.Length; i++)
        {
            float lineWidth = 0;
            stringChars.Add(new List<GameObject>());
        
            for (int j = 0; j < lineStrings[i].Length; j++)
            {
                char currLetter = lineStrings[i][j];
                if(currLetter != spaceLetter)
                {
                    if (vectorChars.ContainsKey(currLetter))
                    {
                        GameObject matchingPrefab = vectorChars[currLetter];
                        VectorCharTracker vct = matchingPrefab.GetComponent<VectorCharTracker>();
                        stringChars[i].Add(matchingPrefab);
                        lineWidth += (vct.width + generalXOffsetBoost) * stringScale;
                    } else
                    {
                        Debug.LogError("No appropriate letter for: " + currLetter);
                    }
                  
                } else
                {
                    stringChars[i].Add(emptyGameObject);
                    lineWidth += (0.8f + generalXOffsetBoost) * stringScale;
                }
            

            }
            overallWidths[i] = lineWidth;
        }

        
        float yOffset = 0;
        for (int i = 0; i < lineStrings.Length; i++)
        {
            float xOffset = 0;

            float alignmentXOffset = 0;
            if(align == StringAlignment.center)
            {
                alignmentXOffset = overallWidths[i] / 2;
                float trueCenterFixing = 0.6f;
                alignmentXOffset -= trueCenterFixing;
            } else if (align == StringAlignment.right)
            {
                alignmentXOffset = overallWidths[i];
            }


            for (int j = 0; j < stringChars[i].Count; j++)
            {
                if(stringChars[i][j] != emptyGameObject)
                {
                    GameObject madeChar = Instantiate(stringChars[i][j], vectorHolder.transform.position + new Vector3(xOffset - alignmentXOffset, yOffset), Quaternion.identity);
                    madeChar.transform.localScale = new Vector3(stringScale, stringScale, 1);
                    madeChar.transform.parent = vectorHolder.transform;
                    VectorCharTracker vct = madeChar.GetComponent<VectorCharTracker>();
                    xOffset += (vct.width + generalXOffsetBoost) * stringScale;
                    madeChar.GetComponent<AllPointManager>().SetToUiLayer();
                    if (textCols != null && textCols.Length == 3)
                    {
                        madeChar.GetComponent<AllPointManager>().SetAllColors(new Color(textCols[0], textCols[1], textCols[2]));
                    }
                } else
                {
                    xOffset += (1 + generalXOffsetBoost) * stringScale;
                }
           
            }
            yOffset -= baseYGap * stringScale;
        }
        vectorStrings.Add(vectorHolder);
        return vectorHolder;
    }
    public void StringExplode(GameObject vectorHolder, float destroyTime, float baseDestroyMove = 0.5f, float destroyRotate = 30f)
    {
        vectorStrings.Remove(vectorHolder);
        for (int i = 0; i < vectorHolder.transform.childCount; i++)
        {
            GameObject currLetter = vectorHolder.transform.GetChild(i).gameObject;
            AllPointManager apm = currLetter.GetComponent<AllPointManager>();
            if(apm != null)
            {
                Destroy(vectorHolder, destroyTime);
                apm.DeathAnimation(baseDestroyMove * currLetter.transform.localScale.x, destroyRotate, destroyTime);
            }
            
        }
    }

    public void DestroyString(GameObject vectorHolder)
    {
        vectorStrings.Remove(vectorHolder);
        Destroy(vectorHolder);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
