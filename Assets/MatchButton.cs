using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MatchButton : MonoBehaviour, IPointerDownHandler
{

    Matcher matcher;
    bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        matcher = GetComponentInParent<Matcher>();
    }

    // Update is called once per frame
    void Update()
    {
        active = matcher.spot1.placedCard != null && matcher.spot2.placedCard != null;
        var sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = active ? 1 : 0.5f;
        sr.color = c;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!active) return;
        
        matcher.state.logic.CreateNewMatch(matcher.spot1.placedCard.gameCard, matcher.spot2.placedCard.gameCard);

        StartCoroutine(matcher.spot1.placedCard.Move(Vector3.zero, Card.CardState.InMatch, false));
        StartCoroutine(matcher.spot2.placedCard.Move(Vector3.zero, Card.CardState.InMatch, false));;
    
    }
}
