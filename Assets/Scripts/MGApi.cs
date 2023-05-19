using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

[System.Serializable] public class MGAccount
{
    public string serverAuthToken = "";
    public string email = "";
}
[System.Serializable] public class MGProfile 
{
    public string first_name;
    public string last_name;
    public string specialty;
    public int specialty_id;
    public string description;
    public MGProfile() { }
    public MGProfile(MGProfile baseProfile)
    {
        first_name = baseProfile.first_name;
        last_name = baseProfile.last_name;
        specialty = baseProfile.specialty;
        specialty_id = baseProfile.specialty_id;
        description = baseProfile.description;
    }
}
[System.Serializable] public class MGLogin
{
    [SerializeField] public MGCredentials doctor;

    public MGLogin()
    {
        doctor = new MGCredentials();
    }
}
[System.Serializable] public class MGCredentials
{
    public string email;
    public string password;
    public int specialty_id = 1;
}
[System.Serializable] public class MGMessage
{
    public string message;
}
[System.Serializable] class APISpecialties
{
    public string name;
}

public class MGApi : MonoBehaviour
{
    public static string serverURL = "";
    public static MGAccount account = new MGAccount();

    /// <summary>
    /// Attempts to log in to API using given credentials.
    /// <para>Account auth token will be null if failed.</para>
    /// </summary>
    /// <param name="creditials">Credentials to serialize into JSON</param>
    public static IEnumerator Login(MGLogin credentials, WebResponse response) 
    {
        yield return WebRequest.Request("POST", serverURL + "/doctors/sign_in",
            JsonConvert.SerializeObject(credentials), response);
    }
    /// <summary>
    /// Attempts to register on server.
    /// </summary>
    /// <param name="credentials">Credentials for register</param>
    /// <returns></returns>
    public static IEnumerator Register(MGLogin credentials, WebResponse response)
    {
        yield return WebRequest.Request("POST", serverURL + "/doctors",
            JsonConvert.SerializeObject(credentials), response);
    }
    ///
    public static IEnumerator GetReviews(WebResponse response)
    {
        if (account.serverAuthToken != "")
        {
            yield return WebRequest.Request("GET", serverURL + "/reviews.json",
                "", response);
        }
        else
        {
            response.authToken = "";
            response.code = 401;
            yield return null;
        }
    }
    public static IEnumerator GetProfile(WebResponse response)
    {
        if (account.serverAuthToken != "")
        {
            yield return WebRequest.Request("GET", serverURL + "/doctor.json",
                "", response);
        }
        else
        {
            response.authToken = "";
            response.code = 401;
            yield return null;
        }
    }
    public static IEnumerator UpdateProfile(WebResponse response, MGProfile profile)
    {
        if (account.serverAuthToken != "")
        {
            // params: first_name, last_name, description, specialty_id
            yield return WebRequest.Request("PUT", serverURL + "/doctor.json",
                JsonConvert.SerializeObject(profile), response);
        }
        else
        {
            response.authToken = "";
            response.code = 401;
            yield return null;
        }
    }


    /// <summary>
    /// Attempts connection to server.
    /// </summary>
    /// <returns></returns>
    public static IEnumerator TestConnection(WebResponse response, System.Action onSuccess, System.Action onFail)
    {
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
        if (response.code == 401) return ("401 Nie masz dostępu do tej strony lub nie jesteś zalogowany.", false); // 401 Unauthorized
        
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

                    if (payload["message"] == "Doctor info updated successfuly") return ("Zaktualizowano dane.", true);
                    if (payload["message"] == "Error: couldn't update doctor") return ("Błąd aktualizacji danych.", false);
                } 
                else
                {
                    // Code 200, no message
                    return ("Operacja zakończona sukcesem.", true);
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
