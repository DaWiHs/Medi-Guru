using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReviewsController : MonoBehaviour
{
    [Header("References")]
    public GameObject reviewPrefab;
    public RectTransform reviewsParent;
    
    [Header("Reviews")]
    [SerializeField] List<MGReview> reviews;

    [Header("Scroll")]
    [SerializeField] private GameObject scrollCatch;
    private int maxScrollDown;
    [SerializeField] private GraphicRaycaster raycaster;
    private EventSystem eventSystem;


    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();

        
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
                    reviewsParent.anchoredPosition -= new Vector2(0, Input.mouseScrollDelta.y * 10);
                }
            }
        }


        if (reviewsParent.anchoredPosition.y > maxScrollDown)
            reviewsParent.anchoredPosition = new Vector2(0, Mathf.LerpUnclamped(reviewsParent.anchoredPosition.y, maxScrollDown, 0.2f));
        if (reviewsParent.anchoredPosition.y < 0)
            reviewsParent.anchoredPosition = new Vector2(0, Mathf.LerpUnclamped(reviewsParent.anchoredPosition.y, 0, 0.2f));

    }
    void ClearReviews()
    {
        foreach (Transform child in reviewsParent.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject == reviewsParent.gameObject) continue;
            if (child != null) Destroy(child.gameObject);
        }
    }

    private IEnumerator RenderReviews()
    {
        // Clear actual reviews
        ClearReviews();
        reviews.Clear();

        // Get reviews from server
        yield return MGApiHandler.GetReviews(reviews);

        // Render
        for (int i = 0; i < reviews.Count; i++) 
        {

            GameObject obj = Instantiate(reviewPrefab, reviewsParent);
            obj.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -110 * i);
            obj.transform.GetChild(1).GetComponent<Image>().fillAmount = reviews[i].score * 1.0f / 10;
            obj.transform.GetChild(2).GetComponent<Text>().text = reviews[i].author;
            obj.transform.GetChild(3).GetComponent<Text>().text = reviews[i].posted_on;
            obj.transform.GetChild(4).GetComponent<Text>().text = reviews[i].body;
        }
        maxScrollDown = reviews.Count * 110 - 200;
    }

    public void OnActivate() { StartCoroutine(RenderReviews()); }
    public void OnDeactivate() { }
}
