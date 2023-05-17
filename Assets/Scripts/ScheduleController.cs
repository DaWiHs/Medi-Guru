using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleController : MonoBehaviour
{
    public bool Active {get; private set;}

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
    [SerializeField] Calendar currentCalendar;
    [SerializeField] Calendar tempCalendar;


    // Start is called before the first frame update
    void Start()
    {
        InitiateWeekObjects();
        InitiateButtons();

        tempCalendar.PopulateDay(WeekDay.monday, 8, 10, 15);
        tempCalendar.PopulateDay(WeekDay.tuesday, 8, 10, 15);
        tempCalendar.PopulateDay(WeekDay.wednesday, 8, 10, 15);
        tempCalendar.PopulateDay(WeekDay.thursday, 8, 10, 15);
        tempCalendar.PopulateDay(WeekDay.friday, 8, 9, 15);
        tempCalendar.ClearDay(WeekDay.saturday);
        tempCalendar.ClearDay(WeekDay.sunday);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnActivate()
    {
        Active = true;
        // TODO
    }
    public void OnDeactivate()
    {
        Active = false;
        // TODO
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
            //visitObjects.Add(new VisitObject(
            //    i,
            //    currentHour * 60 + currentMinutes,
            //    minutesPerVisit,
            //    v
            //    ));

            v.name = "Visit_" + currentHour + "_" + currentMinutes;

            // Delegate function
            //int _i = i;
            //v.GetComponentInChildren<Button>().onClick.AddListener(delegate { ButtonDelegated(_i); });


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
    public void ClearDay(WeekDayObject day)
    {
        Debug.Log("Clear day " + day.applyButton.transform.parent.name);

        foreach (Transform child in day.scrollObj.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject == day.scrollObj.gameObject) continue; // Self exception
            if (child != null) Destroy(child.gameObject);
        }
        // Clear all saved
        //visitObjects.Clear();

    }

    /// <summary> Initiate weekdays buttons with delegated functions </summary>
    private void InitiateButtons()
    {
        monday.applyButton.onClick.AddListener(delegate { GenerateDay(  monday); });
        monday.clearButton.onClick.AddListener(delegate { ClearDay(     monday); });

        tuesday.applyButton.onClick.AddListener(delegate { GenerateDay( tuesday); });
        tuesday.clearButton.onClick.AddListener(delegate { ClearDay(    tuesday); });

        wednesday.applyButton.onClick.AddListener(delegate { GenerateDay(wednesday); });
        wednesday.clearButton.onClick.AddListener(delegate { ClearDay(   wednesday); });

        thursday.applyButton.onClick.AddListener(delegate { GenerateDay(thursday); });
        thursday.clearButton.onClick.AddListener(delegate { ClearDay(   thursday); });

        friday.applyButton.onClick.AddListener(delegate { GenerateDay(  friday); });
        friday.clearButton.onClick.AddListener(delegate { ClearDay(     friday); });

        saturday.applyButton.onClick.AddListener(delegate { GenerateDay(saturday); });
        saturday.clearButton.onClick.AddListener(delegate { ClearDay(   saturday); });

        sunday.applyButton.onClick.AddListener(delegate { GenerateDay(  sunday); });
        sunday.clearButton.onClick.AddListener(delegate { ClearDay(     sunday); });
    }
    private void InitiateWeekObjects()
    {
        monday.applyButton = sideScrollObj.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        monday.clearButton = sideScrollObj.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        monday.scrollMask = sideScrollObj.transform.GetChild(0).GetChild(3).gameObject;
        monday.scrollObj = sideScrollObj.transform.GetChild(0).GetChild(3).GetChild(0).gameObject;

        tuesday.applyButton = sideScrollObj.transform.GetChild(1).GetChild(1).GetComponent<Button>();
        tuesday.clearButton = sideScrollObj.transform.GetChild(1).GetChild(2).GetComponent<Button>();
        tuesday.scrollMask = sideScrollObj.transform.GetChild(1).GetChild(3).gameObject;
        tuesday.scrollObj = sideScrollObj.transform.GetChild(1).GetChild(3).GetChild(0).gameObject;

        wednesday.applyButton = sideScrollObj.transform.GetChild(2).GetChild(1).GetComponent<Button>();
        wednesday.clearButton = sideScrollObj.transform.GetChild(2).GetChild(2).GetComponent<Button>();
        wednesday.scrollMask = sideScrollObj.transform.GetChild(2).GetChild(3).gameObject;
        wednesday.scrollObj = sideScrollObj.transform.GetChild(2).GetChild(3).GetChild(0).gameObject;

        thursday.applyButton = sideScrollObj.transform.GetChild(3).GetChild(1).GetComponent<Button>();
        thursday.clearButton = sideScrollObj.transform.GetChild(3).GetChild(2).GetComponent<Button>();
        thursday.scrollMask = sideScrollObj.transform.GetChild(3).GetChild(3).gameObject;
        thursday.scrollObj = sideScrollObj.transform.GetChild(3).GetChild(3).GetChild(0).gameObject;

        friday.applyButton = sideScrollObj.transform.GetChild(4).GetChild(1).GetComponent<Button>();
        friday.clearButton = sideScrollObj.transform.GetChild(4).GetChild(2).GetComponent<Button>();
        friday.scrollMask = sideScrollObj.transform.GetChild(4).GetChild(3).gameObject;
        friday.scrollObj = sideScrollObj.transform.GetChild(4).GetChild(3).GetChild(0).gameObject;

        saturday.applyButton = sideScrollObj.transform.GetChild(5).GetChild(1).GetComponent<Button>();
        saturday.clearButton = sideScrollObj.transform.GetChild(5).GetChild(2).GetComponent<Button>();
        saturday.scrollMask = sideScrollObj.transform.GetChild(5).GetChild(3).gameObject;
        saturday.scrollObj = sideScrollObj.transform.GetChild(5).GetChild(3).GetChild(0).gameObject;

        sunday.applyButton = sideScrollObj.transform.GetChild(6).GetChild(1).GetComponent<Button>();
        sunday.clearButton = sideScrollObj.transform.GetChild(6).GetChild(2).GetComponent<Button>();
        sunday.scrollMask =  sideScrollObj.transform.GetChild(6).GetChild(3).gameObject;
        sunday.scrollObj =   sideScrollObj.transform.GetChild(6).GetChild(3).GetChild(0).gameObject;

    }
}


public enum WeekDay
{
    monday      ,
    tuesday     ,
    wednesday   ,
    thursday    ,
    friday      ,
    saturday    ,
    sunday
}

[System.Serializable] public class HourMinutePair
{
    public HourMinutePair() { }
    public HourMinutePair(int _h, int _m) { hour = _h; minute = _m; }
    [SerializeField] public int hour;
    [SerializeField] public int minute;
};
[System.Serializable] public class Calendar
{
    [SerializeField] List<HourMinutePair> monday;
    [SerializeField] List<HourMinutePair> tuesday;
    [SerializeField] List<HourMinutePair> wednesday;
    [SerializeField] List<HourMinutePair> thursday;
    [SerializeField] List<HourMinutePair> friday;
    [SerializeField] List<HourMinutePair> saturday;
    [SerializeField] List<HourMinutePair> sunday;

    public void PopulateDay(WeekDay day, int startHour, int endHour, int jump)
    {
        int m = 0;
        for (int h = startHour; h < endHour;)
        {
            DayAdd(day, h, m);

            m += jump;
            if (m >= 60)
            {
                m -= 60;
                h++;
            }
        }
    }
    private void DayAdd(WeekDay day, int hour, int min)
    {
        switch (day)
        {
            case WeekDay.monday:
                monday.Add(new HourMinutePair(hour, min));
                break;
            case WeekDay.tuesday:
                tuesday.Add(new HourMinutePair(hour, min));
                break;
            case WeekDay.wednesday:
                wednesday.Add(new HourMinutePair(hour, min));
                break;
            case WeekDay.thursday:
                thursday.Add(new HourMinutePair(hour, min));
                break;
            case WeekDay.friday:
                friday.Add(new HourMinutePair(hour, min));
                break;
            case WeekDay.saturday:
                saturday.Add(new HourMinutePair(hour, min));
                break;
            case WeekDay.sunday:
                sunday.Add(new HourMinutePair(hour, min));
                break;
        }
    }
    public void ClearDay(WeekDay day)
    {
        switch (day)
        {
            case WeekDay.monday:
                monday.Clear();
                break;
            case WeekDay.tuesday:
                tuesday.Clear();
                break;
            case WeekDay.wednesday:
                wednesday.Clear();
                break;
            case WeekDay.thursday:
                thursday.Clear();
                break;
            case WeekDay.friday:
                friday.Clear();
                break;
            case WeekDay.saturday:
                saturday.Clear();
                break;
            case WeekDay.sunday:
                sunday.Clear();
                break;
        }
    }
}
[System.Serializable] public class WeekDayObject
{
    [SerializeField] public Button applyButton;
    [SerializeField] public Button clearButton;
    [SerializeField] public GameObject scrollMask;
    [SerializeField] public GameObject scrollObj;
    public int maxScroll = 100;
    [SerializeField] public Dictionary<HourMinutePair, GameObject> dayVisits;
} 