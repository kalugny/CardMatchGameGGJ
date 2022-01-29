using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchArea : MonoBehaviour
{
    public State state;

    public GameObject cardPrefab;

    public List<Tuple <Card, Card>> matches = new List<Tuple<Card, Card>>();

    public float paddingBetweenCards = 0.1f;
    public float distanceBetweenCardsInMatch = 0.1f;
    public float maxHeight = 9f;

    [Serializable]
    public struct MatchLevelImage {
        public MatchLevel matchLevel;
        public Sprite image;
    }
    public MatchLevelImage[] matchLevelImages;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMatches(){

        matches.Clear();

        var ms = state.logic.GetMatches();
        foreach (var match in ms){
            Card card1 = Card.GenerateNewCard(match.card1, state, cardPrefab);
            card1.cardState = Card.CardState.InMatch;
            Card card2 = Card.GenerateNewCard(match.card2, state, cardPrefab);
            card2.cardState = Card.CardState.InMatch;
            card1.transform.parent = transform;
            card1.transform.localScale = Vector3.one;
            ProfileImage pi = card1.profileImage;
            Vector3 s = pi.transform.localScale;
            s.x = -s.x;
            pi.transform.localScale = s;
            card2.transform.parent = transform;
            card2.transform.localScale = Vector3.one;
            card2.icon.sprite = matchLevelImages[(int)match.level].image;
            card2.icon.enabled = true;
            card2.icon.sortingOrder = 20000;

            matches.Add(new Tuple<Card, Card>(card1, card2));
        }
        if (ms.Count > 0){
            OrderMatches();
        }
    }

    public void OrderMatches(){
        float cardWidth = matches[0].Item1.cardRoot.GetComponent<SpriteRenderer>().bounds.size.x;
        float cardHeight = matches[0].Item1.cardRoot.GetComponent<SpriteRenderer>().bounds.size.y;
        float totalHeight = cardWidth * matches.Count + paddingBetweenCards * (matches.Count - 1);
     
        if (totalHeight > maxHeight){
            paddingBetweenCards = (maxHeight - cardHeight * matches.Count) / (matches.Count - 1);
            totalHeight = maxHeight;
        }
        
        for (int i = 0; i < matches.Count; i++){
            Tuple<Card, Card> match = matches[i];

            match.Item1.transform.localPosition = new Vector3(-distanceBetweenCardsInMatch, (i + 0.5f) * (cardHeight + paddingBetweenCards) - totalHeight / 2, 0);
            match.Item2.transform.localPosition = new Vector3(distanceBetweenCardsInMatch, (i + 0.5f) * (cardHeight + paddingBetweenCards) - totalHeight / 2, 0);    
        }
     
    }
}
