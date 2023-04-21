using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Account
{
    public int id = 0;
    public string serverAuthToken = "";
    public string email = "";
    
    // public Calendar calendar = new Calendar();
}

public class AccountManager : MonoBehaviour
{
    public static AccountManager instance;
    public static bool loggedIn = false;

    [SerializeField] public Account currentAccount;
    [SerializeField] public List<Account> savedAccounts = new List<Account>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple AccountManager instances on scene.");
            Destroy(gameObject);
        }
        instance = this;
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
