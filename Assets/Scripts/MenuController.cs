using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum MenuState
{
    schedule,
    profile,
    visits,
    reviews
}

public class MenuController : MonoBehaviour
{
    [Header("Schedule")]
    [SerializeField] private Button _scheduleButton;
    [SerializeField] private ScheduleController _scheduleScript;
    [SerializeField] private GameObject _scheduleParent;

    [Header("Profile")]
    [SerializeField] private Button _profileButton;
    [SerializeField] private ProfileController _profileScript;
    [SerializeField] private GameObject _profileParent;

    [Header("Visits")]
    [SerializeField] private Button _visitsButton;
    [SerializeField] private VisitsRenderer _visitsScript;
    [SerializeField] private GameObject _visitsParent;

    [Header("Reviews")]
    [SerializeField] private Button _reviewsButton;
    [SerializeField] private ReviewsController _reviewsScript;
    [SerializeField] private GameObject _reviewsParent;

    [Header("Variables")]
    [SerializeField] private Color _activeButtonColor;
    [SerializeField] private Color _inactiveButtonColor;
    [SerializeField] private Color _activeTextColor;
    [SerializeField] private Color _inactiveTextColor;

    // Start is called before the first frame update
    void Start()
    {
        _scheduleButton.onClick.AddListener(delegate { OpenMenu(MenuState.schedule); });
        _profileButton.onClick.AddListener(delegate { OpenMenu(MenuState.profile); });
        _visitsButton.onClick.AddListener(delegate { OpenMenu(MenuState.visits); });
        _reviewsButton.onClick.AddListener(delegate { OpenMenu(MenuState.reviews); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseAll()
    {
        _scheduleParent.SetActive(false);
        _scheduleButton.GetComponent<Image>().color = _inactiveButtonColor;
        _scheduleButton.GetComponentInChildren<Text>().color = _inactiveTextColor;
        _scheduleScript.OnDeactivate();

        _profileParent.SetActive(false);
        _profileButton.GetComponent<Image>().color = _inactiveButtonColor;
        _profileButton.GetComponentInChildren<Text>().color = _inactiveTextColor;
        _profileScript.OnDeactivate();

        _visitsParent.SetActive(false);
        _visitsButton.GetComponent<Image>().color = _inactiveButtonColor;
        _visitsButton.GetComponentInChildren<Text>().color = _inactiveTextColor;
        _visitsScript.OnDeactivate();

        _reviewsParent.SetActive(false);
        _reviewsButton.GetComponent<Image>().color = _inactiveButtonColor;
        _reviewsButton.GetComponentInChildren<Text>().color = _inactiveTextColor;
        _reviewsScript.OnDeactivate();
    }

    void OpenMenu(MenuState state)
    {
        CloseAll();

        switch (state)
        {
            case MenuState.schedule:
                _scheduleParent.SetActive(true);
                _scheduleButton.GetComponent<Image>().color = _activeButtonColor;
                _scheduleButton.GetComponentInChildren<Text>().color = _activeTextColor;

                break;

            case MenuState.profile:
                _profileParent.SetActive(true);
                _profileButton.GetComponent<Image>().color = _activeButtonColor;
                _profileButton.GetComponentInChildren<Text>().color = _activeTextColor;

                break;

            case MenuState.visits:
                _visitsParent.SetActive(true);
                _visitsButton.GetComponent<Image>().color = _activeButtonColor;
                _visitsButton.GetComponentInChildren<Text>().color = _activeTextColor;
                _visitsScript.OnActivate();
                break;

            case MenuState.reviews:
                _reviewsParent.SetActive(true);
                _reviewsButton.GetComponent<Image>().color = _activeButtonColor;
                _reviewsButton.GetComponentInChildren<Text>().color = _activeTextColor;

                break;

        }

    }

}
