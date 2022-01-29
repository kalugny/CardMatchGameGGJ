using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MatchSpot : MonoBehaviour, IPointerDownHandler
{
    public State state;
    public Vector3 extraPos = new Vector3(0, 1.75f, 0);
    public Card placedCard;

    public bool flip = false;

    
    void Start()
    {
        
    }

    void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData){
                
        if (placedCard == null && state.selectedCard != null){
            // Move it to the match spot            
            placedCard = state.selectedCard;
            state.selectedCard.spot = this;
            StartCoroutine(state.selectedCard.Move(transform.position + extraPos, Card.CardState.InSpot, flip));
            var audioSource = placedCard.GetComponent<AudioSource>();
            audioSource.clip = placedCard.putSound;
            audioSource.Play();
        }                
    }
}
