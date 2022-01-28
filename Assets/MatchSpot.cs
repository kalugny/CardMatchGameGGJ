using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSpot : MonoBehaviour
{
    public State state;
    public AnimationCurve animationCurve;
    public float animationDuration = 0.5f;
    public Vector3 extraPos = new Vector3(0, 0.5f, 0);
    new private Collider2D collider;
    public Card placedCard;

    
    void Start()
    {
        collider = GetComponentInChildren<Collider2D>();    
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)){
            if (collider.OverlapPoint(mousePosition)){
                Debug.Log("Clicked on " + gameObject.name);
                Card card = placedCard != null ? placedCard : state.selectedCard;
                if (card != null){
                    if (!card.placed){
                        StartCoroutine(moveCard(card, true));
                    }
                    else {
                        StartCoroutine(moveCard(card, false));
                    }
                }
            }
        }
    }

    IEnumerator moveCard(Card card, bool toMatchSpot){
        float time = 0f;
        Vector3 startPos = card.transform.position;
        Vector3 endPos = toMatchSpot ? transform.position + extraPos : card.originalPos;
        while (time <= 1f){
            float x = animationCurve.Evaluate(time);
            time += Time.deltaTime / animationDuration;
            
            card.transform.position = Vector2.Lerp(card.transform.position, endPos, x);
            // card.transform.localScale = Vector2.Lerp(card.transform.localScale, Vector2.one, x);
            yield return new WaitForFixedUpdate();
        }
        placedCard = toMatchSpot ? card : null;
        card.placed = toMatchSpot;
    }
}
