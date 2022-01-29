using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler
{
    public State state;

    public Vector3 originalPos;
    public int baseSortingOrder = 12;

    public SpriteRenderer icon;

    public enum CardState {
        InHand,
        Selected,
        InSpot,
        InMatch
    };

    public CardState cardState;

    public GameCard gameCard;
    
    public GameObject cardRoot;
    public float scaleTo = 1.6f;
    public float animationDuration = 1f;
    public AnimationCurve scaleCurve;

    public ProfileImage profileImage;

    public MatchSpot spot = null;

    public static Card GenerateNewCard(GameCard gc, State state, GameObject cardPrefab){
        GameObject newCard = Instantiate(cardPrefab);
        Card c = newCard.GetComponent<Card>();
        c.state = state;
        c.gameCard = gc;
        ProfileImage p = newCard.GetComponentInChildren<ProfileImage>();
        UnityEngine.Random.InitState(gc.id);
        if (gc.gender == Gender.Male){
            p.CreateMenProfile();
        }
        else {
            p.CreateWomenProfile();
        }
        Text t = newCard.GetComponentInChildren<Text>();
        t.text = String.Format("Desperation: {0}, Traits: {1}, {2}, {3}", gc.desperation, gc.traits[0], gc.traits[1], gc.traits[2]);
        
        return c;
    }

    void Awake(){
        cardRoot.GetComponent<SpriteRenderer>().sortingOrder = baseSortingOrder;
        profileImage = GetComponentInChildren<ProfileImage>();

        cardState = CardState.InHand;
    }
    
    void Start()
    {
        originalPos = transform.position;


        
    }

    // Update is called once per frame
    void Update()
    {  
        
    }

    public void OnPointerDown(PointerEventData eventData){

        Debug.Log(cardState);
        
        if (state.selectedCard != this && state.selectedCard != null){
            StartCoroutine(state.selectedCard.SendToBack());
            state.selectedCard = null;
        }

        switch (cardState){
            case CardState.InHand:
                StartCoroutine(BringToFront());
                break;
            case CardState.Selected:
                StartCoroutine(SendToBack());
                break;
            case CardState.InSpot:
                StartCoroutine(Move(originalPos, CardState.InHand, spot.flip));
                spot.placedCard = null;
                spot = null;
                break;
        }
    }

    public IEnumerator Move(Vector3 target, CardState endState, bool flip){
        float time = 0f;

        cardState = endState;
        if (state.selectedCard == this && endState != CardState.Selected){
            state.selectedCard = null;
        }
        
        while (time <= 1f){
            float x = scaleCurve.Evaluate(time);
            time += Time.deltaTime / animationDuration;
            
            if (time > 0.5f){
                if (flip){
                    flip = false;  
                    Vector3 s = profileImage.transform.localScale;
                    s.x = -s.x;
                    profileImage.transform.localScale = s;
                }
                SetLayer("default");
            }

            transform.position = Vector2.Lerp(transform.position, target, x);        
            cardRoot.transform.localScale = Vector3.Lerp(cardRoot.transform.localScale, Vector3.one, x);
            yield return new WaitForFixedUpdate();
        }



    }

    IEnumerator BringToFront(){
        float time = 0f;

        cardState = CardState.Selected;
        state.selectedCard = this;
        
        while (time <= 1f){
            float scale = scaleCurve.Evaluate(time);
            time += Time.deltaTime / animationDuration;

            if (time > 0.5f){
                SetLayer("SelectedCard");
            }
            
            cardRoot.transform.localScale = Vector3.Lerp(cardRoot.transform.localScale, new Vector3(scaleTo, scaleTo, 1), scale);
            yield return new WaitForFixedUpdate();
        }
        
    }

    public IEnumerator SendToBack(){
        float time = 0f;

        cardState = CardState.InHand;
        if (state.selectedCard == this){
            state.selectedCard = null;
        }

        while (time <= 1f){
            float scale = scaleCurve.Evaluate(time);
            time += Time.deltaTime / animationDuration;

            if (time > 0.5f){
                SetLayer("default");
            }
            
            cardRoot.transform.localScale = Vector3.Lerp(cardRoot.transform.localScale, Vector3.one, scale);
            yield return new WaitForFixedUpdate();
        }
        
    }

    void SetLayer(string layer){
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>()){
            sr.sortingLayerName = layer;
        }
        foreach (var c in GetComponentsInChildren<Canvas>()){
            c.sortingLayerName = layer;
        }   
    }
}
