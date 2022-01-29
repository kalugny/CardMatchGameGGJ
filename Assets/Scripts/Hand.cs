using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Hand : MonoBehaviour
{

    public State state;
    public GameObject cardPrefab;

    public List<Card> cards = new List<Card>();
    public int initialCards = 8;
    public float paddingBetweenCards = 0.1f;
    public float maxWidth = 9f;

    void Start()
    {
        NewTurn();
    }

    public void NewTurn(){
        cards.Clear();

        List<GameCard> gameCards = state.logic.GetHand();

        foreach (GameCard c in gameCards){
            Card newCard = Card.GenerateNewCard(c, state, cardPrefab);
            Debug.Log("Hand Card: " + c.id + ", " + newCard.cardState);
            
            if (newCard.cardState == Card.CardState.InMatch) continue;
            
            newCard.icon.enabled = false;
            cards.Add(newCard);
            newCard.transform.parent = transform;
            newCard.transform.localPosition = Vector3.zero;
            newCard.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            if (c.isTooDespearate){
                newCard.MakeDesperate();
            }
            else{
                newCard.cardState = Card.CardState.InHand;
            }
        }
        SpreadCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void SpreadCards(){
        float cardWidth = cards[0].cardRoot.GetComponent<SpriteRenderer>().bounds.size.x;
        float totalWidth = cardWidth * cards.Count + paddingBetweenCards * (cards.Count - 1);
        if (totalWidth > maxWidth){
            paddingBetweenCards = (maxWidth - cardWidth * cards.Count) / (cards.Count - 1);
            totalWidth = maxWidth;
        }
        for (int i = 0; i < cards.Count; i++){
            cards[i].transform.localPosition = new Vector3((i + 0.5f) * (cardWidth + paddingBetweenCards) - totalWidth / 2, 0, 0);
            cards[i].originalPos = cards[i].transform.position;
        }
    }
}
