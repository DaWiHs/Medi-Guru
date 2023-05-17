using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

[System.Serializable]
public class MGAccount
{
    public int id = 0;
    public string serverAuthToken = "";
    public string email = "";
}

public class MGLogin
{
    [SerializeField] public MGCredentials doctor;

    public MGLogin()
    {
        doctor = new MGCredentials();
    }
}
public class MGCredentials
{
    public string email;
    public string password;
    public int specialty_id = 1;
}
public class MGMessage
{
    public string message;
}

public class MGApi : MonoBehaviour
{
    public static string serverURL = "";

    /// <summary>
    /// Attempts to log in to API using given credentials.
    /// <para>Account auth token will be null if failed.</para>
    /// </summary>
    /// <param name="creditials">Credentials to serialize into JSON</param>
    /// <param name="account">Reference account</param>
    /// <param name="onSuccess">Called on Success (HTTP 200) with no arguments</param>
    /// <param name="onFail">Called otherwise with string argument</param>
    public static IEnumerator Login(MGLogin credentials, MGAccount account, System.Action onSuccess = null, System.Action<string> onFail = null) 
    {
        WebResponse response = new WebResponse();

        yield return WebRequest.Request("POST", serverURL + "/doctors/sign_in",
            JsonConvert.SerializeObject(credentials), response);

        if (response.code == 200)
        {
            account.email = credentials.doctor.email;
            account.serverAuthToken = response.authToken;
            if (onSuccess != null) onSuccess.Invoke();
        } else
        {
            account.email = "";
            account.serverAuthToken = "";
            if (onFail != null) onFail.Invoke(response.content);
        }
    }
    /// <summary>
    /// Attempts to register on server.
    /// </summary>
    /// <param name="credentials">Credentials</param>
    /// <param name="onSuccess">Called on Success (HTTP 200)</param>
    /// <param name="onFail">Called otherwise with string argument</param>
    /// <returns></returns>
    public static IEnumerator Register(MGLogin credentials, System.Action onSuccess = null, System.Action<string> onFail = null)
    {
        WebResponse response = new WebResponse();

        yield return WebRequest.Request("POST", serverURL + "/doctors",
            JsonConvert.SerializeObject(credentials), response);

        if (response.code == 200)
        {
            if (onSuccess != null) onSuccess.Invoke();
        }
        else
        {
            if (onFail != null) onFail.Invoke(response.content);
        }
    }

    /// <summary>
    /// Attempts connection to server.
    /// </summary>
    /// <param name="onSuccess">Called on connection success (code 200)</param>
    /// <param name="onFail">Called otherwise</param>
    /// <returns></returns>
    public static IEnumerator TestConnection(System.Action onSuccess = null, System.Action onFail = null)
    {
        WebResponse response = new WebResponse();

        yield return WebRequest.Request("GET", serverURL + "/doctors", "", response);

        if (response.code == 200)
        {
            if (onSuccess != null) onSuccess.Invoke();
        }
        else
        {
            if (onFail != null) onFail.Invoke();
        }
    }


}
