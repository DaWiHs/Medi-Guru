using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class Profile
{
    public string name;
    public string surname;
    public string prefix;
    public string speciality;
    //[SerializeField] public Calendar calendar; // Moved to ScheduleController
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
