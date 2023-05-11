using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleController : MonoBehaviour
{
    public bool Active {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnActivate()
    {
        Active = true;
        // TODO
    }
    public void OnDeactivate()
    {
        Active = false;
        // TODO
    }
}
