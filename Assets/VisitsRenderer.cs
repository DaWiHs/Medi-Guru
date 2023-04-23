using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class VisitsRenderer : MonoBehaviour
{
    public bool active = true;
    [Header("Prefabs")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject hourPrefab;
    [SerializeField] private GameObject visitPrefab;

    [Header("References")]
    [SerializeField] private RectTransform todayView;
    [SerializeField] private List<VisitObject> visitObjects = new List<VisitObject>();
    
    [Header("GenValues")]
    public int minutesPerVisit = 15;

    public int startWorkHour;
    public int endWorkHour;

    [Header("Scroll")]
    [SerializeField] private RectTransform scrollObj;
    [SerializeField] private GameObject scrollCatch;
    private int maxScrollDown;
    [SerializeField] private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        ScrollControl();
    }

    private void ScrollControl()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            // Setup PointerEvent in place
            PointerEventData pointerEvent = new PointerEventData(eventSystem);
            pointerEvent.position = Input.mousePosition;

            // Raycast using PointerEvent
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            raycaster.Raycast(pointerEvent, raycastResults);

            foreach (RaycastResult r in raycastResults)
            {
                //Debug.Log(r);
                if (r.gameObject == scrollCatch)
                {
                    scrollObj.anchoredPosition -= new Vector2(0, Input.mouseScrollDelta.y * 10);
                }
            }
        }


        if (scrollObj.anchoredPosition.y > maxScrollDown)
            scrollObj.anchoredPosition = new Vector2(0, Mathf.LerpUnclamped(scrollObj.anchoredPosition.y, maxScrollDown, 0.2f));
        if (scrollObj.anchoredPosition.y < 0)
            scrollObj.anchoredPosition = new Vector2(0, Mathf.LerpUnclamped(scrollObj.anchoredPosition.y, 0, 0.2f));

    }

    public void RenderVisits()
    {
        int currentY = 0;
        int currentMinutes = 60;
        int currentHour = startWorkHour - 1;

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
                h.GetComponentInChildren<Text>().text = currentHour + ":00";

            }

            if (i == visits)
            {
                maxScrollDown = (currentY * -1) - 200;
                break;
            }
            // Create visit plate
            // Set visit time text
            GameObject v = Instantiate(visitPrefab, todayView);
            v.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);
            v.GetComponentInChildren<Text>().text = VisitTimeText(currentHour * 60 + currentMinutes);

            visitObjects.Add(new VisitObject(
                currentHour * 60 + currentMinutes, 
                minutesPerVisit, 
                v.GetComponentInChildren<Button>()
                ));

            v.name = "Visit_" + currentHour + "_" + currentMinutes;

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
