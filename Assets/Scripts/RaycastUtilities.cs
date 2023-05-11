using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RaycastUtilities
{
    public static bool PointerIsOverUI()
    {
        var hitObject = UIRaycast(ScreenPosToPointerData(Input.mousePosition));
        return hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
    }
    public static bool PointerIsOverUI(Vector2 screenPos)
    {
        var hitObject = UIRaycast(ScreenPosToPointerData(screenPos));
        return hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
    }

    public static bool PointerOverUIElement(GameObject element)
    {
        List<GameObject> hits = new List<GameObject>();
        UIRaycasts(ScreenPosToPointerData(Input.mousePosition)).ForEach((item) => { hits.Add(item.gameObject); });

        if (hits.Contains(element)) return true;
        return false;
    }
    public static bool PointerOverAnyUIElement(ICollection<GameObject> elements)
    {
        List<GameObject> hits = new List<GameObject>();
        UIRaycasts(ScreenPosToPointerData(Input.mousePosition)).ForEach((item) => { hits.Add(item.gameObject); });

        foreach (GameObject e in elements)
        {
            if (hits.Contains(e)) return true;
        }
        return false;
    }

    static List<RaycastResult> UIRaycasts(PointerEventData pointerData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results;
    }
    static GameObject UIRaycast(PointerEventData pointerData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count < 1 ? null : results[0].gameObject;
    }

    static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
       => new PointerEventData (EventSystem.current) { position = screenPos };
}
