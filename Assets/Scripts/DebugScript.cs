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

    // Start is called before the first frame update
    void Start()
    {
        urlInput.text = WebRequest.instance.url;
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
    public void OpenDebugMenu()
    {
        debugMenu.SetActive(true);
    }
    public void CloseDebugMenu()
    {
        debugMenu.SetActive(false);
    }
    public void ApplyURL()
    {
        WebRequest.instance.url = urlInput.text;
    }
    public void TestConnectivity()
    {

    }

}
