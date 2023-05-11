using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        for (int h = startHour; h < endHour; )
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
[System.Serializable] public class Profile
{
    public string name;
    public string surname;
    public string prefix;
    public string speciality;
    [SerializeField] public Calendar calendar;
}

public class ProfileController : MonoBehaviour
{
    public static ProfileController instance;
    public bool Active { get; private set; }


    [Header("Profile")]
    [SerializeField] bool unsavedChanges = false;
    [SerializeField] Profile currentProfile;
    [SerializeField] Profile tempProfile;

    [Header("References")]
    [SerializeField] Text profileFullName;
    [SerializeField] InputField profileName;
    [SerializeField] InputField profileSurname;
    [SerializeField] InputField profilePrefix;
    [SerializeField] Image saveButton;

    [Header("Variables")]
    [SerializeField] Color normalColor;
    [SerializeField] List<Color> blinkColors;
    int targetBlinkColor;
    float blinkValue = 0;
    float valInc = 0.025f;
    float valCut = 0.04f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Debug.LogError("Multiple instances of " + this); Destroy(gameObject); }
    }

    // Start is called before the first frame update
    void Start()
    {
        profileName.onValueChanged.AddListener(delegate { UpdateName(); });
        profileSurname.onValueChanged.AddListener(delegate { UpdateName(); });
        profilePrefix.onValueChanged.AddListener(delegate { UpdateName(); });

        tempProfile.calendar.PopulateDay(WeekDay.monday, 8, 10, 15);
        tempProfile.calendar.PopulateDay(WeekDay.tuesday, 8, 10, 15);
        tempProfile.calendar.PopulateDay(WeekDay.wednesday, 8, 10, 15);
        tempProfile.calendar.PopulateDay(WeekDay.thursday, 8, 10, 15);
        tempProfile.calendar.PopulateDay(WeekDay.friday, 8, 9, 15);
        tempProfile.calendar.ClearDay(WeekDay.saturday);
        tempProfile.calendar.ClearDay(WeekDay.sunday);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Active) return;

        if (unsavedChanges)
        {
            // Save button blink (?) TODO
            SaveButtonBlink();
        }
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

    public void SetSpeciality(string speciality)
    {
        tempProfile.speciality = speciality;
        unsavedChanges = true;
    }

    public void UpdateName()
    {
        string fullName = "";

        fullName += profilePrefix.text + " ";
        tempProfile.prefix = profilePrefix.text;
        fullName += profileName.text + " ";
        tempProfile.name = profileName.text;
        fullName += profileSurname.text;
        tempProfile.surname = profileSurname.text;

        profileFullName.text = fullName;

        unsavedChanges = true;
    }

    private void SaveButtonBlink()
    {
        Color c = saveButton.color * (1f - blinkValue) + blinkColors[targetBlinkColor] * blinkValue;

        saveButton.color = c;
        blinkValue += valInc * Time.deltaTime;

        if (saveButton.color == blinkColors[targetBlinkColor] || blinkValue > valCut)
        {
            blinkValue = 0;
            if (++targetBlinkColor >= blinkColors.Count) targetBlinkColor = 0;
        }
    }

    public void SaveChanges()
    {
        StartCoroutine(_SaveChanges());
    }
    private IEnumerator _SaveChanges()
    {
        yield return null;

        string json = JsonUtility.ToJson(tempProfile);

        Debug.Log("Profile:\n" + json);

        currentProfile = JsonUtility.FromJson<Profile>(json);

        unsavedChanges = false;

        saveButton.color = normalColor;
    }


}
