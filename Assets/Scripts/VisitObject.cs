using UnityEngine.UI;

[System.Serializable]
public class VisitObject
{
    public Button button;
    public int startTime;
    public int duration;

    public VisitObject(int startTime, int duration, Button buttonObject)
    {
        this.startTime = startTime;
        this.duration = duration;
        this.button = buttonObject;
    }
}
