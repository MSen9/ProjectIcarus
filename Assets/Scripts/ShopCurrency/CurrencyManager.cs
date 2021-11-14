using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{

    public int currency = 0;
    public GameObject currencyPos;
    public GameObject currencySymbol;
    GameObject activeSymbol;
    GameObject currencyVectors;
    string moneyString = "";
    float currencyStringXOffset = 0.75f;
    string lastMoneyString;
    StringToVectorManager stvManager;
    public static float[] moneyCols = new float[] { 228/255f, 255/255f, 51/255f };
    public static CurrencyManager current;
    // Start is called before the first frame update
    void Start()
    {
        current = this;
        lastMoneyString = moneyString;
        stvManager = GameObject.FindGameObjectWithTag("StoVManager").GetComponent<StringToVectorManager>();
        if (BetweenMapInfo.current != null && BetweenMapInfo.current.hasInfoSaved)
        {
            currency = BetweenMapInfo.current.savedInfo.currency;
        }
    }

    // Update is called once per frame
    void Update()
    {
        moneyString = ":" + currency.ToString();
        if(lastMoneyString != moneyString)
        {
            if(currencyVectors != null)
            {
                stvManager.DestroyString(currencyVectors);
            }
            currencyVectors = stvManager.StringToVectors(moneyString, 1, StringAlignment.left, moneyCols);
            currencyVectors.transform.position = currencyPos.transform.position + new Vector3(currencyStringXOffset,0);
            currencyVectors.transform.parent = currencyPos.transform;
        }

        if(activeSymbol == null)
        {
            activeSymbol = Instantiate(currencySymbol, currencyPos.transform.position, Quaternion.identity);
            activeSymbol.transform.parent = currencyPos.transform;
            activeSymbol.GetComponent<AllPointManager>().SetAllColors(new Color(moneyCols[0],moneyCols[1],moneyCols[2]));
        }
    }


}
