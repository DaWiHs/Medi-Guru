using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[System.Serializable] public class WebResponse
{
    public long code = 0;
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
        string requestResult = "";
        WebResponse response = new WebResponse();
        yield return StartCoroutine(Request("POST", url + "/sign_in",
            "{ \"doctor\": { \"email\": \"" + email +  "\", \"password\":\"" + password + "\" } }",
            response));
            //result => requestResult = result));
        Debug.Log("Login ended, result: " + requestResult);
    }
    public void _DRegister()
    {
        StartCoroutine(_Register());
    }
    IEnumerator _Register()
    {
        string requestResult = "";
        WebResponse response = new WebResponse();
        yield return StartCoroutine(Request("POST", url + "",
            "{ \"doctor\": { \"email\": \"" + email + "\", \"password\":\"" + password + "\" } }",
            response));
        Debug.Log("Register ended, result: " + requestResult);
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

    IEnumerator MyFunction()
    {

        string requestResult = "";
        WebResponse response = new WebResponse();

        yield return StartCoroutine(Request("DELETE", url + "/sign_out", "", response));
        Debug.Log("Delete ended, result: " + requestResult);

        //yield return StartCoroutine(Request("POST", url + "",
        //    "{ \"doctor\": { \"email\": \"doctor9@mail.com\", \"password\": \"123123\" } }",
        //    result => requestResult = result));

        //Debug.Log("Func ended, result: " + requestResult);
        
    }

    IEnumerator Login()
    {
        string requestResult = "";
        WebResponse response = new WebResponse();
        yield return StartCoroutine(Request("POST", url + "/sign_in", 
            "{ \"doctor\": { \"email\": \"doctor6@mail.com\", \"password\":\"123123\" } }", 
            response));

        AccountManager.instance.currentAccount.serverAuthToken = response.authToken;

        Debug.Log("Login ended, result: " + requestResult);
        // Login UI
    }

    IEnumerator Logout()
    {

        string requestResult = "";
        WebResponse response = new WebResponse();

        yield return StartCoroutine(Request("DELETE", url + "/sign_out", "", response));
        Debug.Log("Logout ended, result: " + requestResult);
        // Logout ui
    }

    public IEnumerator Request(string method, string url, string bodyJson, WebResponse response)
    {
        Debug.Log("Sending request:\nMethod: " + method + "\nURL:" + url + "\nDATA:" + bodyJson);
        
        var request = new UnityWebRequest(url, method);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        if (auth != "") request.SetRequestHeader("Authorization", auth);
        yield return request.SendWebRequest();
        //Encoding.UTF8.(request.downloadHandler.data)

        response.code = request.responseCode;
        response.error = request.error;
        response.content = request.downloadHandler.text;

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
        
    }

}
