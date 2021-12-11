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
    string CHAR_LIST = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ+-";
    string[] SPEC_CHARS = new string[]{ ":colon", "%percent","?questionMark","/fSlash" };
    string VECTOR_PREFAB_DIRECTORY = "VectorLetters";
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
            GameObject currChar = Resources.Load(Path.Combine(VECTOR_PREFAB_DIRECTORY, CHAR_LIST[i].ToString())) as GameObject;
            if(currChar != null)
            {
                vectorChars.Add(CHAR_LIST[i], currChar);
            } else
            {
                Debug.LogError("No valid prefab for: " + CHAR_LIST[i].ToString());
            }
        }

        for (int i = 0; i < SPEC_CHARS.Length; i++)
        {
            GameObject currChar = Resources.Load(Path.Combine(VECTOR_PREFAB_DIRECTORY, SPEC_CHARS[i].Substring(1,SPEC_CHARS[i].Length-1))) as GameObject;
            if (currChar != null)
            {
                vectorChars.Add(SPEC_CHARS[i][0], currChar);
            }
            else
            {
                Debug.LogError("No valid prefab for: " + CHAR_LIST[i].ToString());
            }
        }

    }

    public GameObject StringToVectors(string inputString, float stringScale = 1f, 
        StringAlignment align = StringAlignment.left, float[] textCols = null)
    {
        inputString = inputString.ToUpper();
        float xOffset = 0;
        float yOffset = 0;
        float overallWidth = 0;
        List<GameObject> stringChars = new List<GameObject>();
        GameObject vectorHolder = Instantiate(emptyGameObject, new Vector3(0,0,0), Quaternion.identity);

        

        for (int i = 0; i < inputString.Length; i++)
        {
            if(inputString[i] != spaceLetter)
            {
                GameObject matchingPrefab = vectorChars[inputString[i]];
                VectorCharTracker vct = matchingPrefab.GetComponent<VectorCharTracker>();
                stringChars.Add(matchingPrefab);
                overallWidth += (vct.width + generalXOffsetBoost) * stringScale;
            } else
            {
                stringChars.Add(emptyGameObject);
                overallWidth += (0.8f + generalXOffsetBoost) * stringScale;
            }
            
        }

        float alignmentXOffset = 0;
        if(align == StringAlignment.center)
        {
            alignmentXOffset = overallWidth / 2;
            float trueCenterFixing = 0.6f;
            alignmentXOffset -= trueCenterFixing;
        } else if (align == StringAlignment.right)
        {
            alignmentXOffset = overallWidth;
        }


        for (int i = 0; i < stringChars.Count; i++)
        {
            if(stringChars[i] != emptyGameObject)
            {
                GameObject madeChar = Instantiate(stringChars[i], vectorHolder.transform.position + new Vector3(xOffset - alignmentXOffset, yOffset), Quaternion.identity);
                madeChar.transform.localScale = new Vector3(stringScale, stringScale, 1);
                madeChar.transform.parent = vectorHolder.transform;
                VectorCharTracker vct = madeChar.GetComponent<VectorCharTracker>();
                xOffset += (vct.width + generalXOffsetBoost) * stringScale;
                madeChar.GetComponent<AllPointManager>().SetToUiLayer();
                if (textCols != null)
                {
                    madeChar.GetComponent<AllPointManager>().SetAllColors(new Color(textCols[0], textCols[1], textCols[2]));
                }
            } else
            {
                xOffset += (1 + generalXOffsetBoost) * stringScale;
            }
           
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
