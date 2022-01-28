using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Hand : MonoBehaviour
{

    public GameObject cardPrefab;
    public State state;

    public List<Card> cards = new List<Card>();
    public int initialCards = 8;
    public float paddingBetweenCards = 0.1f;
    public float maxWidth = 9f;

    void Start()
    {
        List<GameCard> gameCards = state.logic.GetHand();
        foreach (GameCard c in gameCards){
            GenerateNewCard(c);
        }
        SpreadCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateNewCard(GameCard gc){
        GameObject newCard = Instantiate(cardPrefab);
        Card c = newCard.GetComponent<Card>();
        c.state = state;
        c.gameCard = gc;
        ProfileImage p = newCard.GetComponentInChildren<ProfileImage>();
        if (gc.gender == Gender.Male){
            p.CreateMenProfile();
        }
        else {
            p.CreateWomenProfile();
        }
        Text t = newCard.GetComponentInChildren<Text>();
        t.text = String.Format("Desperation: {0}, Traits: {1}", gc.desperation, gc.traits[0]);
        cards.Add(c);
        newCard.transform.parent = transform;
        newCard.transform.localPosition = Vector3.zero;
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
        }
    }
}
