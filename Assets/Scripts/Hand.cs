using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        for (int i = 0; i < initialCards; i++){
            GenerateNewCard();
        }
        SpreadCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateNewCard(){
        GameObject newCard = Instantiate(cardPrefab);
        Card c = newCard.GetComponent<Card>();
        c.state = state;
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
