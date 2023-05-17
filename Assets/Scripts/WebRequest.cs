using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[System.Serializable] public class WebResponse
{
    public long code = 0;
    public string result = "";
    public string error = "";
    public string content = "";
    public string authToken = "";
}

public class WebRequest : MonoBehaviour
{
    public static WebRequest instance;

    public string url = "https://1ac7-5-184-235-151.eu.ngrok.io/doctors";
    

    [Header("_Debug")]
    public string email = "doctor@mail.com";
    public string password = "123123";
    public string auth = "";

    public WebResponse lastResponse;

    private void Awake()
    {
        instance = this;

    }

    void Start()
    {
        UnityWebRequest.ClearCookieCache();
        //StartCoroutine(MyFunction());
    }

    void Update()
    {
        
    }

    ///////////////////// _DEBUG
    public void _DLogin()
    {
        StartCoroutine(_Login());
    }
    IEnumerator _Login()
    {
        WebResponse response = new WebResponse();
        yield return StartCoroutine(Request("POST", url + "/doctors/sign_in",
            "{ \"doctor\": { \"email\": \"" + email +  "\", \"password\":\"" + password + "\" } }",
            response));
        //result => requestResult = result));

        AccountManager.instance.currentAccount.serverAuthToken = response.authToken;

        Debug.Log("Login ended, result: " + response.result);
    }
    public void _DRegister()
    {
        StartCoroutine(_Register());
    }
    IEnumerator _Register()
    {
        WebResponse response = new WebResponse();
        yield return StartCoroutine(Request("POST", url + "/doctors",
            "{ \"doctor\": { \"email\": \"" + email + "\", \"password\":\"" + password + "\" , \"specialty_id\" : 1} }",
            response));
        
        AccountManager.instance.currentAccount.serverAuthToken = response.authToken;
        
        Debug.Log("Register ended, result: " + response.result);
    }
    public void _DLogout()
    {
        StartCoroutine(_Logout());
    }
    IEnumerator _Logout()
    {
        string requestResult = "";
        WebResponse response = new WebResponse();

        yield return StartCoroutine(Request("DELETE", url + "/sign_out",
            "",
            response));
        Debug.Log("Logout ended, result: " + requestResult);
    }

    ///////////////////// _Debug





    public static IEnumerator Request(string method, string url, string bodyJson, WebResponse response)
    {
        Debug.Log("Sending request:\nMethod: " + method + "\nURL:" + url + "\nDATA:" + bodyJson);
        
        var request = new UnityWebRequest(url, method);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        if (AccountManager.instance.currentAccount.serverAuthToken != "") 
            request.SetRequestHeader("Authorization", AccountManager.instance.currentAccount.serverAuthToken);

        // Wait for server response
        yield return request.SendWebRequest();

        // Set WebResponse
        response.code = request.responseCode;
        response.result = request.result.ToString();
        response.error = request.error;
        response.content = request.downloadHandler.text;

        instance.lastResponse = response;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Response Code: " + response.code + "\n Error: " + response.error + "\nReceived: \n" + response.content);
        }
        else
        {
            Debug.Log("Response Code: " + response.code + "\n Error: " + response.error + "\nReceived: \n" + response.content);
            if (request.GetResponseHeader("Authorization") != null)
            {
                response.authToken = request.GetResponseHeader("Authorization");
            }
        }
        request.Dispose();
    }

}
