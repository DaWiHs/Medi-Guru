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
    public static IEnumerator Login(MGLogin credentials, WebResponse response) 
    {
        yield return WebRequest.Request("POST", serverURL + "/doctors/sign_in",
            JsonConvert.SerializeObject(credentials), response);
    }
    /// <summary>
    /// Attempts to register on server.
    /// </summary>
    /// <param name="credentials">Credentials</param>
    /// <param name="onSuccess">Called on Success (HTTP 200)</param>
    /// <param name="onFail">Called otherwise with string argument</param>
    /// <returns></returns>
    public static IEnumerator Register(MGLogin credentials, WebResponse response)
    {
        yield return WebRequest.Request("POST", serverURL + "/doctors",
            JsonConvert.SerializeObject(credentials), response);
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

    public static (string, bool) MessageTranslate(WebResponse response)
    {
        // HTTP Errors
        if (response.code == 404) return ("404 Nie znaleziono strony.", false); // 404 Not Found
        if (response.code == 401) return ("401 Nie masz dostępu do tej strony.", false); // 401 Unauthorized
        
        if (response.code == 200)
        {
            try
            {
                Dictionary<string,string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.content);

                if (payload.ContainsKey("message"))
                {
                    if (payload["message"] == "Signed up.") return ("", true);
                    if (payload["message"] == "Sign up failure. Email is invalid") return ("Niepoprawny e-mail.", false);
                    if (payload["message"] == "Sign up failure. Password is too short (minimum is 6 characters)") return ("Hasło musi mieć długość conajmniej 6 znaków.", false);
                    if (payload["message"] == "Sign up failure. Email has already been taken") return ("Ten e-mail jest już zajęty.", false);
                    
                    if (payload["message"] == "Signed in successfully") return ("", true);
                    if (payload["message"] == "Invalid Email or password.") return ("Niepoprawny e-mail i/lub hasło.", false);
                }

            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Deserializing error: " + e.Message);
                throw;
            }
        }

        return ("Unhandled error:\nCode " + response.code + "\nError: " + response.error + "\nContent: " + response.content, false);
    }
}
