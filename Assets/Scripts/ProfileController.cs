using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;


public class ProfileController : MonoBehaviour
{
    public static ProfileController instance;

    [Header("Profile")]
    [SerializeField] bool unsavedChanges = false;
    [SerializeField] MGProfile currentProfile;
    [SerializeField] public MGProfile tempProfile;

    [Header("References")]
    [SerializeField] SpecialitiesController profileSpecialities;
    [SerializeField] Text profileFullName;
    [SerializeField] InputField profileName;
    [SerializeField] InputField profileSurname;
    [SerializeField] InputField profileDescription;
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
        profileDescription.onValueChanged.AddListener(delegate { UpdateDescription(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (unsavedChanges)
        {
            SaveButtonBlink();
        }
    }

    public void OnActivate()
    {
        UpdateProfile();
    }
    public void OnDeactivate() {}
    public void UpdateProfileUI()
    {
        profileName.text = currentProfile.first_name;
        profileSurname.text = currentProfile.last_name;
        profileDescription.text = currentProfile.description;
        profileSpecialities.SelectSpeciality(currentProfile.specialty, currentProfile.specialty_id, false);
    }

    public void SetSpeciality(string speciality, int id)
    {
        tempProfile.specialty = speciality;
        tempProfile.specialty_id = id;
        unsavedChanges = true;
    }

    public void UpdateName()
    {
        string fullName = "";

        fullName += profileName.text + " ";
        tempProfile.first_name = profileName.text;
        fullName += profileSurname.text;
        tempProfile.last_name = profileSurname.text;

        if (fullName == " ") fullName = "Jan Kowalski";
        profileFullName.text = fullName;

        unsavedChanges = true;
    }

    public void UpdateDescription()
    {
        tempProfile.description = profileDescription.text;
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
        yield return MGApiHandler.SaveProfile(tempProfile);

        currentProfile = new MGProfile(tempProfile);

        unsavedChanges = false;

        saveButton.color = normalColor;
    }

    public void UpdateProfile()
    {
        if (currentProfile.specialty_id == 0)
        {
            StartCoroutine(_GetProfile());
        }
        else
        {
            UpdateProfileUI();
        }
    }

    private IEnumerator _GetProfile()
    {
        // Wait for specialities to render
        while (profileSpecialities.rendered == false) { yield return null; }

        // Get profile from server
        yield return MGApiHandler.GetProfile();

        if (tempProfile.description != "ERROR")
        {
            tempProfile.specialty_id = SpecialitiesController.SpecialityId(tempProfile.specialty);
            currentProfile = new MGProfile(tempProfile);
        }
        else
        {
            tempProfile.first_name = "";
            tempProfile.last_name = "";
            tempProfile.description = "";
        }
        
        UpdateProfileUI();

    }

}
