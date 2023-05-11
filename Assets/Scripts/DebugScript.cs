using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{

    public GameObject appView;
    public GameObject loginView;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
