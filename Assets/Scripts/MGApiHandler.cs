using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class MGApiHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] InputField emailField;
    [SerializeField] InputField passwordField;
    [SerializeField] Text specialtyIdText;


    [Header("DEBUG")]
    public GameObject appView;
    public GameObject loginView;

    public GameObject debugMenu;
    public InputField urlInput;

    public Image connectionStatus;
    public Sprite connectionTesting;
    public Sprite connectionOk;
    public Sprite connectionError;

    // Start is called before the first frame update
    void Start()
    {
        urlInput.text = WebRequest.instance.url;
        MGApi.serverURL = WebRequest.instance.url;
    }

    public void OpenAppView()
    {
        appView.SetActive(true);
        loginView.SetActive(false);
    }
    public void OpenLoginView()
    {
        appView.SetActive(false);
        loginView.SetActive(true);
    }
    public void OpenDebugMenu() { debugMenu.SetActive(true); }
    public void CloseDebugMenu() { debugMenu.SetActive(false); }
    public void ApplyURL()
    {
        Debug.Log("API URL set");
        MGApi.serverURL = urlInput.text;
        WebRequest.instance.url = urlInput.text;
    }

    //// TEST CONNECTION
    public void TestConnectivity()
    {
        Debug.Log("DBS_Test");
        connectionStatus.sprite = connectionTesting;

        WebResponse response = new WebResponse();
        StartCoroutine(MGApi.TestConnection(response, delegate { ConnectionOk(); }, delegate { ConnectionError(); }));
    }
    void ConnectionOk() { connectionStatus.sprite = connectionOk; }
    void ConnectionError() { connectionStatus.sprite = connectionError; }

    //// LOGIN
    public void Login() { StartCoroutine(_Login()); }
    private IEnumerator _Login()
    {
        // Initialize variables
        MGLogin credentials = new MGLogin();
        credentials.doctor.email = emailField.text;
        credentials.doctor.password = passwordField.text;

        WebResponse response = new WebResponse();

        yield return MGApi.Login(credentials, response);

        (string message, bool success) = MGApi.MessageTranslate(response);

        if (success)
        {
            MGApi.account.email = credentials.doctor.email;
            MGApi.account.serverAuthToken = response.authToken;
            OnLoginSuccess();
        }
        else
        {
            MGApi.account.email = "";
            MGApi.account.serverAuthToken = "";
            OnLoginFail(message);
        }
        
    }
    private void OnLoginSuccess()
    {
        PopupController.MakePopup("Zalogowano pomyślnie.", OpenAppView);
    }
    private void OnLoginFail(string message)
    {
        PopupController.MakePopup("Błąd logowania: " + message, null);
    }

    //// REGISTER
    public void Register() { StartCoroutine(_Register()); }
    private IEnumerator _Register()
    {
        // Initialize variables
        MGLogin credentials = new MGLogin();
        credentials.doctor.email = emailField.text;
        credentials.doctor.password = passwordField.text;
        credentials.doctor.specialty_id = int.Parse(specialtyIdText.text);

        WebResponse response = new WebResponse();

        yield return MGApi.Register(credentials, response);

        (string message, bool success) = MGApi.MessageTranslate(response);

        if (success) OnRegisterSuccess();
        else OnRegisterFail(message);
    }
    private void OnRegisterSuccess()
    {
        PopupController.MakePopup("Rejestracja pomyślna.", null);
    }
    private void OnRegisterFail(string message)
    {
        PopupController.MakePopup("Błąd rejestracji:\n" + message, null);
    }

    //// PROFILE
    public static IEnumerator GetProfile()
    {
        WebResponse response = new WebResponse();

        yield return MGApi.GetProfile(response);

        (string message, bool success) = MGApi.MessageTranslate(response);

        if (success)
        {
            ProfileController.instance.tempProfile = JsonConvert.DeserializeObject<MGProfile>(response.content);

        } else
        {
            PopupController.MakePopup(message, null);
            ProfileController.instance.tempProfile.description = "ERROR";
        }
    }
    public static IEnumerator SaveProfile(MGProfile profile)
    {
        WebResponse response = new WebResponse();

        yield return MGApi.UpdateProfile(response, profile);

        (string message, bool success) = MGApi.MessageTranslate(response);

        if (success)
        {
            PopupController.MakePopup(message, null);
        }
        else
        {
            PopupController.MakePopup(message, null);
            ProfileController.instance.tempProfile.description = "ERROR";
        }
    }

    //// REVIEWS
    public static IEnumerator GetReviews(List<MGReview> reviews)
    {
        WebResponse response = new WebResponse();
        yield return MGApi.GetReviews(response);

        if (response.code == 401)
        {
            reviews.Add(new MGReview("Błąd autoryzacji."));
        } else
        {
            reviews.AddRange(JsonConvert.DeserializeObject<List<MGReview>>(response.content));
        }

    }
}
