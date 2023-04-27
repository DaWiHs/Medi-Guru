using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class VisitObject
{
    public int index;
    public GameObject objectReference;
    public int startTime;
    public int duration;

    public VisitObject(int index, int startTime, int duration, GameObject objectReference)  {
        this.index = index;
        this.startTime = startTime;
        this.duration = duration;
        this.objectReference = objectReference;
    }
}
