using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{

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
    }

    public void OpenApp()
    {
        appView.SetActive(true);
        loginView.SetActive(false);
    }
    public void OpenLogin()
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
        WebResponse response = new WebResponse();
        MGLoginCredentials credentials = new MGLoginCredentials();
        credentials.email = "";
        credentials.password = "";
        MGAccount account = new MGAccount();

        yield return MGApi.Login(credentials, account);

    }

}
