using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleController : MonoBehaviour
{
    public bool Active {get; private set;}

    [SerializeField] Calendar currentCalendar;
    [SerializeField] Calendar tempCalendar;

    // Start is called before the first frame update
    void Start()
    {

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
}


public enum WeekDay
{
    monday,
    tuesday,
    wednesday,
    thursday,
    friday,
    saturday,
    sunday
}

[System.Serializable]
public class HourMinutePair
{
    public HourMinutePair() { }
    public HourMinutePair(int _h, int _m) { hour = _h; minute = _m; }
    [SerializeField] public int hour;
    [SerializeField] public int minute;
};
[System.Serializable]
public class Calendar
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