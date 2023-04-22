using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VisitsRenderer : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject hourPrefab;
    [SerializeField] private GameObject visitPrefab;

    [Header("References")]
    [SerializeField] private RectTransform todayView;
    
    [Header("GenValues")]
    public int minutesPerVisit = 15;

    public int startWorkHour;
    public int endWorkHour;

    [Header("Scroll")]
    [SerializeField] private RectTransform scrollObj;
    [SerializeField] private LayerMask hoverMask;
    [SerializeField] private int maxScrollDown;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderVisits()
    {
        int currentY = 0;
        int currentMinutes = 60;
        int currentHour = -1;

        int visits = (endWorkHour - startWorkHour) * 60 / minutesPerVisit;

        for (int i = 0; i <= visits; i++)
        {
            
            if (currentMinutes >= 60)
            {
                currentMinutes -= 60;
                currentHour++;
                GameObject l = Instantiate(linePrefab, todayView);
                l.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);

                GameObject h = Instantiate(hourPrefab, todayView);
                h.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);
                h.GetComponentInChildren<Text>().text = (startWorkHour + currentHour) + ":00";

            }

            if (i == visits) break;

            // Create visit plate
            // Set visit time text
            GameObject v = Instantiate(visitPrefab, todayView);
            v.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);
            v.GetComponentInChildren<Text>().text = VisitTimeText((startWorkHour + currentHour) * 60 + currentMinutes);
            
            currentMinutes += minutesPerVisit;
            currentY -= 15;


        }


    }

    private string VisitTimeText(int currentTime)
    {
        string ret = "";

        int hS = currentTime / 60;
        int hE = (currentTime + minutesPerVisit) / 60;

        int mS = currentTime - (hS * 60);
        int mE = currentTime + minutesPerVisit - (hE * 60);

        ret += hS + ":";
        if (mS < 10) ret += "0" + mS;
        else ret += "" + mS;

        ret += " - ";

        ret += hE + ":";
        if (mE < 10) ret += "0" + mE;
        else ret += "" + mE;

        return ret;
    } 

}
