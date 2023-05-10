using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyInputField : InputField
{
    UnityEngine.Events.UnityAction<string> _focusCall = null;
    UnityEngine.Events.UnityAction<string> _lostFocusCall = null;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (_focusCall != null) _focusCall.Invoke("");
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (_lostFocusCall != null) _lostFocusCall.Invoke("");
    }

    public void OnFocus(UnityEngine.Events.UnityAction<string> call)
    {
        _focusCall = call;
    }
    public void OnLostFocus(UnityEngine.Events.UnityAction<string> call)
    {
        _lostFocusCall = call;
    }
}
