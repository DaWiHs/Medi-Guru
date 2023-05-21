using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ScheduleController : MonoBehaviour
{
    public bool Active {get; private set;}
    public static ScheduleController instance;

    [Header("Generator References")]
    [SerializeField] InputField startHour;
    [SerializeField] InputField endHour;
    [SerializeField] InputField minPerVisit;
    [Space(5)]
    [SerializeField] GameObject visitPrefab;
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject hourPrefab;

    [Header("SideScroll")]
    [SerializeField] GameObject sideScrollMask;
    [SerializeField] GameObject sideScrollObj;

    [Header("Week Days Objects")]
    [SerializeField] WeekDayObject monday;
    [SerializeField] WeekDayObject tuesday;
    [SerializeField] WeekDayObject wednesday;
    [SerializeField] WeekDayObject thursday;
    [SerializeField] WeekDayObject friday;
    [SerializeField] WeekDayObject saturday;
    [SerializeField] WeekDayObject sunday;

    [Space(20)]
    [Header("Calendar")]
    [SerializeField] public MGSchedule currentCalendar;
    [SerializeField] public MGSchedule tempCalendar;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Debug.LogError("Multiple instances of " + this); Destroy(gameObject); }
    }

    // Start is called before the first frame update
    void Start()
    {

        InitiateWeekObjects();
        InitiateButtons();

        currentCalendar = new MGSchedule();
        tempCalendar = new MGSchedule();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnActivate()
    {
        Active = true;
        RenderDays();
        // TODO
    }
    public void OnDeactivate()
    {
        Active = false;
        // TODO
    }

    public void RenderDays()
    {
        StartCoroutine(_RenderDays());
    }
    IEnumerator _RenderDays()
    {

        yield return MGApiHandler.GetSchedule();
        
        RenderDay(currentCalendar.monday, monday);
        RenderDay(currentCalendar.tuesday, tuesday);
        RenderDay(currentCalendar.wednesday, wednesday);
        RenderDay(currentCalendar.thursday, thursday);
        RenderDay(currentCalendar.friday, friday);
        RenderDay(currentCalendar.saturday, saturday);
        RenderDay(currentCalendar.sunday, sunday);
    }

    void RenderDay(List<MGHourMinuteDuration> list, WeekDayObject obj)
    {
        // Clear to avoid overlap
        DerenderDay(obj);

        int currentY = 0;
        int currentHour = 0;
        int lastHour = 0;

        foreach (MGHourMinuteDuration item in list)
        {
            // Render line and hour
            currentHour = item.hour;
            if (currentHour != lastHour)
            {
                lastHour = currentHour;
                GameObject l = Instantiate(linePrefab, obj.scrollObj.transform);
                l.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);

                GameObject h = Instantiate(hourPrefab, obj.scrollObj.transform);
                h.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);
                h.GetComponentInChildren<Text>().text = currentHour + ":00";
            }

            // Render obj
            GameObject o = Instantiate(visitPrefab, obj.scrollObj.transform);
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);
            o.GetComponentInChildren<Text>().text = VisitTimeText(item.hour * 60 + item.minute, item.duration);

            o.name = "Visit_" + item.hour + "_" + item.minute;

            // Placement
            currentY -= 15;
        }

        obj.maxScroll = (currentY * -1) - 200;
    }

    public void UpdateSchedule()
    {
        // Send web request PUT
        StartCoroutine(MGApiHandler.SetSchedule(tempCalendar));
        // Copy (no reference) from temp to current
        currentCalendar = JsonUtility.FromJson<MGSchedule>(JsonUtility.ToJson(tempCalendar));
    }

    /// <summary> Regenerates scheduled visits for given week day </summary>
    public void GenerateDay(WeekDayObject day)
    {
        // Clear to avoid overlapping
        ClearDay(day);


        // Variables
        int startWorkHour, endWorkHour, minutesPerVisit, currentY = 0;
        int currentMinutes = 60;
        try
        {
            startWorkHour = int.Parse(startHour.text);
            endWorkHour = int.Parse(endHour.text);
            minutesPerVisit = int.Parse(minPerVisit.text);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Parse error: " + e.Message);
            return;
            throw;
        }
        int currentHour = startWorkHour - 1;
        int visits = (endWorkHour - startWorkHour) * 60 / minutesPerVisit;

        for (int i = 0; i <= visits; i++)
        {
            // Create lines and hours
            if (currentMinutes >= 60)
            {
                currentMinutes -= 60;
                currentHour++;
                GameObject l = Instantiate(linePrefab, day.scrollObj.transform);
                l.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);

                GameObject h = Instantiate(hourPrefab, day.scrollObj.transform);
                h.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);
                h.GetComponentInChildren<Text>().text = currentHour + ":00";
            }

            // Stop if all done
            if (i == visits)
            {
                day.maxScroll = (currentY * -1) - 200;
                break;
            }

            // Create visit plate
            GameObject v = Instantiate(visitPrefab, day.scrollObj.transform);
            v.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);

            // Set visit time text
            v.GetComponentInChildren<Text>().text = VisitTimeText(currentHour * 60 + currentMinutes, minutesPerVisit);

            // Save object
            tempCalendar.DayAdd(day.weekDay, currentHour, currentMinutes, minutesPerVisit);

            v.name = "Visit_" + currentHour + "_" + currentMinutes;

            // Increase timer and placement
            currentMinutes += minutesPerVisit;
            currentY -= 15;
        }

    }
    /// <summary> Translate hour and minute to "HH:MM" string with leading zeros </summary>
    private string VisitTimeText(int currentTime, int minutesPerVisit)
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

    /// <summary> Clears all scheduled visits on given day </summary>
    public void DerenderDay(WeekDayObject day)
    {
        //Debug.Log("Clear day " + day.applyButton.transform.parent.name);

        foreach (Transform child in day.scrollObj.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject == day.scrollObj.gameObject) continue; // Self exception
            if (child != null) Destroy(child.gameObject);
        }
    }
    public void ClearDay(WeekDayObject day)
    {
        DerenderDay(day);
        tempCalendar.ClearDay(day.weekDay);
    }


    /// <summary> Initiate weekdays buttons with delegated functions </summary>
    private void InitiateButtons()
    {
        monday.applyButton.onClick.AddListener(delegate { GenerateDay(monday); });
        monday.clearButton.onClick.AddListener(delegate { ClearDay(monday); });

        tuesday.applyButton.onClick.AddListener(delegate { GenerateDay( tuesday); });
        tuesday.clearButton.onClick.AddListener(delegate { ClearDay(tuesday); });

        wednesday.applyButton.onClick.AddListener(delegate { GenerateDay(wednesday); });
        wednesday.clearButton.onClick.AddListener(delegate { ClearDay(wednesday); });

        thursday.applyButton.onClick.AddListener(delegate { GenerateDay(thursday); });
        thursday.clearButton.onClick.AddListener(delegate { ClearDay(thursday); });

        friday.applyButton.onClick.AddListener(delegate { GenerateDay(  friday); });
        friday.clearButton.onClick.AddListener(delegate { ClearDay(friday); });

        saturday.applyButton.onClick.AddListener(delegate { GenerateDay(saturday); });
        saturday.clearButton.onClick.AddListener(delegate { ClearDay(saturday); });

        sunday.applyButton.onClick.AddListener(delegate { GenerateDay(  sunday); });
        sunday.clearButton.onClick.AddListener(delegate { ClearDay(sunday); });
    }
    private void InitiateWeekObjects()
    {
        monday.weekDay = WeekDay.monday;
        monday.applyButton = sideScrollObj.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        monday.clearButton = sideScrollObj.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        monday.scrollMask = sideScrollObj.transform.GetChild(0).GetChild(3).gameObject;
        monday.scrollObj = sideScrollObj.transform.GetChild(0).GetChild(3).GetChild(0).gameObject;

        tuesday.weekDay = WeekDay.tuesday;
        tuesday.applyButton = sideScrollObj.transform.GetChild(1).GetChild(1).GetComponent<Button>();
        tuesday.clearButton = sideScrollObj.transform.GetChild(1).GetChild(2).GetComponent<Button>();
        tuesday.scrollMask = sideScrollObj.transform.GetChild(1).GetChild(3).gameObject;
        tuesday.scrollObj = sideScrollObj.transform.GetChild(1).GetChild(3).GetChild(0).gameObject;

        wednesday.weekDay = WeekDay.wednesday;
        wednesday.applyButton = sideScrollObj.transform.GetChild(2).GetChild(1).GetComponent<Button>();
        wednesday.clearButton = sideScrollObj.transform.GetChild(2).GetChild(2).GetComponent<Button>();
        wednesday.scrollMask = sideScrollObj.transform.GetChild(2).GetChild(3).gameObject;
        wednesday.scrollObj = sideScrollObj.transform.GetChild(2).GetChild(3).GetChild(0).gameObject;

        thursday.weekDay = WeekDay.thursday;
        thursday.applyButton = sideScrollObj.transform.GetChild(3).GetChild(1).GetComponent<Button>();
        thursday.clearButton = sideScrollObj.transform.GetChild(3).GetChild(2).GetComponent<Button>();
        thursday.scrollMask = sideScrollObj.transform.GetChild(3).GetChild(3).gameObject;
        thursday.scrollObj = sideScrollObj.transform.GetChild(3).GetChild(3).GetChild(0).gameObject;

        friday.weekDay = WeekDay.friday;
        friday.applyButton = sideScrollObj.transform.GetChild(4).GetChild(1).GetComponent<Button>();
        friday.clearButton = sideScrollObj.transform.GetChild(4).GetChild(2).GetComponent<Button>();
        friday.scrollMask = sideScrollObj.transform.GetChild(4).GetChild(3).gameObject;
        friday.scrollObj = sideScrollObj.transform.GetChild(4).GetChild(3).GetChild(0).gameObject;

        saturday.weekDay = WeekDay.saturday;
        saturday.applyButton = sideScrollObj.transform.GetChild(5).GetChild(1).GetComponent<Button>();
        saturday.clearButton = sideScrollObj.transform.GetChild(5).GetChild(2).GetComponent<Button>();
        saturday.scrollMask = sideScrollObj.transform.GetChild(5).GetChild(3).gameObject;
        saturday.scrollObj = sideScrollObj.transform.GetChild(5).GetChild(3).GetChild(0).gameObject;

        sunday.weekDay = WeekDay.sunday;
        sunday.applyButton = sideScrollObj.transform.GetChild(6).GetChild(1).GetComponent<Button>();
        sunday.clearButton = sideScrollObj.transform.GetChild(6).GetChild(2).GetComponent<Button>();
        sunday.scrollMask =  sideScrollObj.transform.GetChild(6).GetChild(3).gameObject;
        sunday.scrollObj =   sideScrollObj.transform.GetChild(6).GetChild(3).GetChild(0).gameObject;

    }
}





[System.Serializable] public class WeekDayObject
{
    public WeekDay weekDay;
    [SerializeField] public Button applyButton;
    [SerializeField] public Button clearButton;
    [SerializeField] public GameObject scrollMask;
    [SerializeField] public GameObject scrollObj;
    public int maxScroll = 100;
} 