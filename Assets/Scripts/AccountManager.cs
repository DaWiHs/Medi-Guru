using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AccountManager : MonoBehaviour
{
    public static AccountManager instance;
    public static bool loggedIn = false;

    [SerializeField] public MGAccount currentAccount;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple AccountManager instances on scene.");
            Destroy(gameObject);
        }
        instance = this;
        currentAccount = new MGAccount();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SaveCurrentAccount(string email, string sererAuthToken)
    {
        currentAccount.email = email;
        currentAccount.serverAuthToken = sererAuthToken;
    }
}
