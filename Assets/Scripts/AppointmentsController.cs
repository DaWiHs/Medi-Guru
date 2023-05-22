using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AppointmentsController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject hourPrefab;
    [SerializeField] private GameObject visitPrefab;

    [Header("References")]
    [SerializeField] private RectTransform todayView;
    [SerializeField] private GameObject previewObject;
    [SerializeField] private GameObject connectImage;
    [SerializeField] private List<VisitObject> visitObjects = new List<VisitObject>();

    [Header("PreviewReferences")]
    [SerializeField] Text previewHour;
    [SerializeField] Text previewName;
    [SerializeField] Text previewDescription;
    
    [Header("GenValues")]
    public List<MGAppointment> appointments;
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
        ScrollControl();
    }

    public void OnActivate() { StartCoroutine(_GetAppointments()); }
    public void OnDeactivate() { RemoveRender(); }

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
        int currentHour = 0;
        int lastHour = 0;

        int i = 0;
        foreach (MGAppointment item in appointments)
        {
            // Render line and hour
            currentHour = item.time.Hour;
            if (currentHour != lastHour)
            {
                lastHour = currentHour;
                GameObject l = Instantiate(linePrefab, todayView);
                l.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);

                GameObject h = Instantiate(hourPrefab, todayView);
                h.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);
                h.GetComponentInChildren<Text>().text = currentHour + ":00";
            }

            // Render visit obj
            GameObject v = Instantiate(visitPrefab, todayView);
            v.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);
            v.GetComponentInChildren<Text>().text = VisitTimeText(item.time.Hour * 60 + item.time.Minute, item.duration / 60);

            visitObjects.Add(new VisitObject(
                i,
                item.time.Hour * 60 + item.time.Minute,
                minutesPerVisit,
                v
                ));

            v.name = "Visit_" + item.time.Hour + "_" + item.time.Minute;

            // Delegate function
            int _i = i++;
            v.GetComponentInChildren<Button>().onClick.AddListener(delegate { ButtonDelegated(_i); });

            // Placement
            currentY -= 15;
        }

        maxScrollDown = (currentY * -1) - 200;
    }

    public void RemoveRender()
    {
        connectImage.transform.SetParent(previewObject.transform);
        foreach(Transform child in todayView.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject == todayView.gameObject) continue;
            if (child != null) Destroy(child.gameObject);
        }
        visitObjects.Clear();
    }

    public void ClosePreview()
    {
        previewObject.SetActive(false);
        connectImage.SetActive(false);
    }
    public void ButtonDelegated(int index)
    {
        previewObject.SetActive(true);
        connectImage.SetActive(true);
        connectImage.transform.SetParent(visitObjects[index].objectReference.transform);
        connectImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(100f, -0.5f);

        MGAppointment a = appointments[index];

        previewHour.text = VisitTimeText(a.time.Hour * 60 + a.time.Minute, a.duration / 60);
        previewName.text = a.patient_name;
        previewDescription.text = "";

    }

    private string VisitTimeText(int time, int duration)
    {
        string ret = "";

        int hS = time / 60;
        int hE = (time + duration) / 60;

        int mS = time - (hS * 60);
        int mE = time + duration - (hE * 60);

        ret += hS + ":";
        if (mS < 10) ret += "0" + mS;
        else ret += "" + mS;

        ret += " - ";

        ret += hE + ":";
        if (mE < 10) ret += "0" + mE;
        else ret += "" + mE;

        return ret;
    } 

    private IEnumerator _GetAppointments()
    {
        yield return MGApiHandler.GetAppointments(appointments);
        appointments.ForEach((t) => { Debug.Log("Appointment: " + t.time + " " + (t.duration / 60f) + " " + t.patient_name); });
        RemoveRender();
        RenderVisits();
    }

}
[System.Serializable] public class VisitObject
{
    public int index;
    public GameObject objectReference;
    public int startTime;
    public int duration;

    public VisitObject(int index, int startTime, int duration, GameObject objectReference)
    {
        this.index = index;
        this.startTime = startTime;
        this.duration = duration;
        this.objectReference = objectReference;
    }
}