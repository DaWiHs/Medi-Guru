using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialitiesController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject specialityPrefab;
    [SerializeField] RectTransform specialitiesParent;

    [Header("Variables")]
    public List<string> specialities = new List<string>();
    public Color toggleDisabled;
    public Color toggleEnabled;

    void Start()
    {
        RenderSpecialities();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RenderSpecialities() { StartCoroutine(_RenderSpecialities()); }
    private IEnumerator _RenderSpecialities()
    {
        // Get specialities if none
        if (specialities.Count < 1) yield return StartCoroutine(GetSpecialities());

        // Render
        int currentY = 0;
        bool isLeft = true;
        foreach (string speciality in specialities)
        {
            // Generate
            GameObject obj = Instantiate(specialityPrefab, specialitiesParent);
            RectTransform t = obj.GetComponent<RectTransform>();

            // Place
            t.anchorMin = new Vector2(isLeft ? 0f : 0.5f, 1);
            t.anchorMax = new Vector2(isLeft ? 0.5f : 1f, 1);

            t.anchoredPosition = new Vector2Int(0, currentY);

            // Obj update
            obj.GetComponentInChildren<Text>().text = speciality;
            string _s = speciality;
            Button _b = obj.GetComponentInChildren<Button>();
            _b.onClick.AddListener(delegate { ToggleSpeciality(_s, _b); });



            // Render options update
            if (isLeft)
            {
                isLeft = false;
            } else
            {
                isLeft = true;
                currentY -= 20;
            }
            
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

    public void ToggleSpeciality(string speciality, Button reference)
    {
        // Profile update
        // Toggle image update
        Image _img = reference.transform.GetChild(0).GetComponentInChildren<Image>();
        
        if (_img.color == toggleEnabled)
        {
            _img.color = toggleDisabled;
            // Profil update TODO
        } else
        {
            _img.color = toggleEnabled;
            // Profil Update TODO 
        }

    }

}
