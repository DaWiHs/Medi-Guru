using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReviewsController : MonoBehaviour
{
    public bool Active { get; private set; }
    public GameObject prefab;
    public RectTransform parent;
    

    [SerializeField]public List<review> r;

    [SerializeField] private GameObject scrollCatch;
    private int maxScrollDown;
    [SerializeField] private GraphicRaycaster raycaster;
    private EventSystem eventSystem;


    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();

        renderReview();
    }

    // Update is called once per frame
    void Update()
    {
        ScrollControl();
    }
    private void ScrollControl()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            // Setup PointerEvent in place
            PointerEventData pointerEvent = new PointerEventData(eventSystem);
            pointerEvent.position = Input.mousePosition;

            // Raycast using PointerEvent
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            raycaster.Raycast(pointerEvent, raycastResults);

            foreach (RaycastResult r in raycastResults)
            {
                Debug.Log(r);
                if (r.gameObject == scrollCatch)
                {
                    parent.anchoredPosition -= new Vector2(0, Input.mouseScrollDelta.y * 10);
                }
            }
        }


        if (parent.anchoredPosition.y > maxScrollDown)
            parent.anchoredPosition = new Vector2(0, Mathf.LerpUnclamped(parent.anchoredPosition.y, maxScrollDown, 0.2f));
        if (parent.anchoredPosition.y < 0)
            parent.anchoredPosition = new Vector2(0, Mathf.LerpUnclamped(parent.anchoredPosition.y, 0, 0.2f));

    }
    void renderReview()
    {
        // GET REVIEWS
        //WebResponse siema = new WebResponse();

        //WebRequest.instance.Request("GET", "/reviews", " ", siema);


        for (int i = 0; i < r.Count; i++) //petla wysiwetla
        {

        GameObject obj = Instantiate(prefab, parent);
            obj.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -110 * i);
            obj.transform.GetChild(0).GetComponent<Text>().text = r[i].nick;
            obj.transform.GetChild(1).GetComponent<Text>().text = r[i].opinia;
            obj.transform.GetChild(2).GetComponent<Image>().fillAmount = r[i].ocena*1.0f/10;


             
        }
        maxScrollDown = r.Count * 110 - 200;

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
[System.Serializable] public class review //klasa przechowujaca informacje o ocenach
 {
    public string nick;
    public string opinia;
    public int ocena; //1-10 co pol gwiazki



 }