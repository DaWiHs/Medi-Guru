using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MGAccount
{
    public int id = 0;
    public string serverAuthToken = "";
    public string email = "";
}

public class MGLoginCredentials
{
    public string email;
    public string password;
}
public class MGRegisterCredentials
{
    public string email;
    public string password;
    public int specialty_id = 1;
}

public class MGApi : MonoBehaviour
{
    public static string serverURL = "";

    public static IEnumerator Login(MGLoginCredentials creditials, MGAccount account, UnityAction<string> onSuccess = null, UnityAction<string> onFail = null) 
    {
        WebResponse response = new WebResponse();

        yield return WebRequest.Request("POST", serverURL + "/doctors/sign_in", 
            JsonUtility.ToJson(creditials), response);

        if (response.code == 200)
        {
            account.email = creditials.email;
            account.serverAuthToken = response.authToken;
            if (onSuccess != null) onSuccess.Invoke("");
        } else
        {
            account.email = "";
            account.serverAuthToken = "";
            if (onFail != null) onFail.Invoke(response.content);
        }
    }

    public static IEnumerator TestConnection(UnityAction<string> onSuccess = null, UnityAction<string> onFail = null)
    {
        WebResponse response = new WebResponse();

        yield return WebRequest.Request("GET", serverURL + "/doctors", "", response);

        if (response.code == 200)
        {
            if (onSuccess != null) onSuccess.Invoke("");
        }
        else
        {
            if (onFail != null) onFail.Invoke("");
        }
    }


}
