using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialitiesController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject specialityPrefab;
    [SerializeField] RectTransform specialitiesParent;
    [SerializeField] MyInputField specialitySearchInput;
    [SerializeField] GameObject dropdown;

    [SerializeField] Dictionary<string, GameObject> searchValues = new Dictionary<string, GameObject>();

    [Header("Variables")]
    public List<string> specialities = new List<string>();
    public Color toggleDisabled;
    public Color toggleEnabled;

    void Start()
    {
        RenderSpecialities();
        specialitySearchInput.onValueChanged.AddListener(delegate { SearchSpecialities(); });

        specialitySearchInput.OnFocus(delegate { ShowDropdown(); });
        //mySearchInput.OnLostFocus(delegate { Invoke("HideDropdown", 0.1f); });
    }

    void Update()
    {

        if (!ProfileController.instance.active) return;

        if(Input.GetMouseButtonDown(0)) OnClick();
        
    }

    void OnClick()
    {
        GameObject[] targets = { dropdown, specialitySearchInput.gameObject };
        if (!RaycastUtilities.PointerOverAnyUIElement(targets))
        {
            HideDropdown();
        }
    }

    public void RenderSpecialities() { StartCoroutine(_RenderSpecialities()); }
    private IEnumerator _RenderSpecialities()
    {
        // Get specialities if none
        if (specialities.Count < 1) yield return StartCoroutine(GetSpecialities());

        // Render
        int currentY = 0;
        foreach (string speciality in specialities)
        {
            // Generate
            GameObject obj = Instantiate(specialityPrefab, specialitiesParent);
            RectTransform t = obj.GetComponent<RectTransform>();

            searchValues.Add(speciality, obj);

            // Place
            t.anchoredPosition = new Vector2Int(0, currentY);

            // Obj update
            obj.name = "Speciality_" + speciality;
            obj.GetComponentInChildren<Text>().text = speciality;
            string _s = speciality;                             // Copy for delegate
            Button _b = obj.GetComponentInChildren<Button>();   // Copy for delegate

            _b.onClick.AddListener(delegate { SelectSpeciality(_s, _b); });


            // Render options update
            currentY -= 20 + 1;

        }
    }

    public IEnumerator GetSpecialities()
    {
        WebResponse response = new WebResponse();

        yield return StartCoroutine(WebRequest.instance.Request(
            "GET", 
            "https://raw.githubusercontent.com/hykare/mediguru/devise-api/lib/specialties.txt",
            "",
            response));

        specialities.AddRange(response.content.Split('\n'));
        specialities.Sort();

    }

    public void SelectSpeciality(string speciality, Button reference)
    {
        // Speciality searchbox update
        specialitySearchInput.text = speciality;

        // Profile update
        ProfileController.instance.SetSpeciality(speciality);

        // Remove focus
        //mySearchInput.interactable = false;
        //mySearchInput.interactable = true;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        HideDropdown();
    }

    private void SearchSpecialities()
    {
        List<string> found = new List<string>();
        string searchWord = specialitySearchInput.text;

        foreach (string item in specialities)
        {
            if (item.Contains(searchWord)) found.Add(item);
        }

        int currentY = 0;
        foreach (string speciality in specialities)
        {
            if (found.Contains(speciality))
            {
                // Place
                searchValues[speciality].SetActive(true);
                searchValues[speciality].GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, currentY);

                // Render options update
                currentY -= 20 + 1;
            }
            else
            {
                // Hide button
                searchValues[speciality].SetActive(false);
            }
        }
    }

    private void ShowDropdown()
    {
        dropdown.SetActive(true);
    }

    private void HideDropdown()
    {
        dropdown.SetActive(false);
    }

}
