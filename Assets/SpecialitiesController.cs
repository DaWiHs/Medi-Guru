using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialitiesController : MonoBehaviour
{

    [SerializeField] WebResponse myResponse = new WebResponse();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetSpecialities());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator GetSpecialities()
    {
        
        yield return StartCoroutine(WebRequest.instance.Request(
            "GET", 
            "https://raw.githubusercontent.com/hykare/mediguru/devise-api/lib/specialties.txt",
            "",
            myResponse));

        Debug.Log(myResponse);
        Debug.Log(myResponse.content);
    }

}
