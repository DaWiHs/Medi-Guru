using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public void TestConnectivity()
    {
        Debug.Log("DBS_Test");
        connectionStatus.sprite = connectionTesting;
        StartCoroutine(MGApi.TestConnection(delegate { ConnectionOk(); }, delegate { ConnectionError(); }));
    }
    void ConnectionOk() { connectionStatus.sprite = connectionOk; }
    void ConnectionError() { connectionStatus.sprite = connectionError; }

    public void Login() { StartCoroutine(_Login()); }
    private IEnumerator _Login()
    {
        // Initialize variables
        MGLogin credentials = new MGLogin();
        credentials.doctor.email = emailField.text;
        credentials.doctor.password = passwordField.text;

        MGAccount account = new MGAccount();


        yield return MGApi.Login(credentials, account, OnLoginSuccess, OnLoginFail);

        //if (account.serverAuthToken != "")
        //{
        //    Debug.Log("Login Success");
        //} else
        //{
        //    Debug.Log("Login Fail");
        //}

    }
    private void OnLoginSuccess()
    {
        Debug.Log("Login success");
        // TODO UI
    }
    private void OnLoginFail(string message)
    {
        Debug.Log("Login fail: " + message);
        // TODO UI
    }
    public void Register() { StartCoroutine(_Register()); }
    private IEnumerator _Register()
    {
        // Initialize variables
        MGLogin credentials = new MGLogin();
        credentials.doctor.email = emailField.text;
        credentials.doctor.password = passwordField.text;
        credentials.doctor.specialty_id = int.Parse(specialtyIdText.text);

        yield return MGApi.Register(credentials, OnRegisterSuccess, OnRegisterFail);

    }
    private void OnRegisterSuccess()
    {
        Debug.Log("Register success");
        // TODO UI
    }
    private void OnRegisterFail(string message)
    {
        Debug.Log("Register fail: " + message);
        // TODO UI
    }
}
