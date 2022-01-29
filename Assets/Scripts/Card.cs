using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler
{
    public State state;

    public static int sortingOrder = 14;

    public Vector3 originalPos;
    public int baseSortingOrder = 12;

    public SpriteRenderer icon;

    public Text[] texts = new Text[3];

    public Text nameText;

    public Color[] colors = new Color[2];

    public Sprite[] bgs = new Sprite[4];
    public SpriteRenderer bg;

    public AudioClip clickSound;
    public AudioClip putSound;

    public enum CardState {
        NotPlaced,
        InHand,
        Selected,
        InSpot,
        InMatch,
        Desperate
    };

    public CardState cardState = CardState.NotPlaced;

    public GameCard gameCard;
    
    public GameObject speechBubble;
    public static int speechBubbleSortingOrder = 0;
    public float speechBubbleDuration = 2f;

    public GameObject cardRoot;
    public float scaleTo = 1.6f;
    public float animationDuration = 1f;
    public AnimationCurve scaleCurve;

    public ProfileImage profileImage;

    public MatchSpot spot = null;

    public static Card GenerateNewCard(GameCard gc, State state, GameObject cardPrefab){
        Card c;
        if (state.cards.ContainsKey(gc.id)){
            c = state.cards[gc.id];
        }
        else {
            GameObject newCard = Instantiate(cardPrefab);
            c = newCard.GetComponent<Card>();
            c.state = state;
            c.gameCard = gc;
            c.bg.sprite = c.bgs[UnityEngine.Random.Range(0, c.bgs.Length)];
            foreach (var sr in newCard.GetComponentsInChildren<SpriteRenderer>()){
                sr.sortingOrder = Card.sortingOrder++;
            }
            newCard.GetComponentInChildren<Canvas>().sortingOrder = Card.sortingOrder++;
            ProfileImage p = newCard.GetComponentInChildren<ProfileImage>();
            if (gc.gender == Gender.Male){
                p.CreateMenProfile();
            }
            else {
                p.CreateWomenProfile();
            }
            for (int i = 0; i < 3; i++){
                c.texts[i].text = TraitSentence(gc.traits[i], state);
                
            }
            foreach (var i in c.GetComponentsInChildren<Image>()){
                i.color = c.colors[(int)gc.gender];
            }
            var genderNames = state.firstNames_.Where(x => x.gender == gc.gender).ToList();
            c.nameText.text = genderNames[UnityEngine.Random.Range(0, genderNames.Count)].firstName + ", " + gc.age;
            c.nameText.color = c.colors[(int)gc.gender];
            state.cards[gc.id] = c;

        }   
        c.gameObject.SetActive(true); 
        return c;
    }

    public static string TraitSentence(Trait t, State state){
        var sentences = state.sentences_.Where(s => s.trait == t);
        List<string> strings = sentences.Select(s => s.sentence).ToList();
        return strings[UnityEngine.Random.Range(0, strings.Count)];
    }

    void Awake(){
        // cardRoot.GetComponent<SpriteRenderer>().sortingOrder = baseSortingOrder;
        profileImage = GetComponentInChildren<ProfileImage>();

        // cardState = CardState.InHand;
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
        
        if (state.selectedCard != this && state.selectedCard != null){
            StartCoroutine(state.selectedCard.SendToBack());
            state.selectedCard = null;
        }

        var audioSource = GetComponent<AudioSource>();
        audioSource.clip  = clickSound;
        switch (cardState){
            case CardState.InHand:
                StartCoroutine(BringToFront());
                
                audioSource.Play();
                break;
            case CardState.Selected:
                StartCoroutine(SendToBack());
                
                break;
            case CardState.InSpot:
                StartCoroutine(Move(originalPos, CardState.InHand, spot.flip));
                spot.placedCard = null;
                spot = null;
                audioSource.Play();
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

    public void MakeDesperate(){
        // Say something
        cardState = CardState.Desperate;
        var srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs){                     
            sr.color = Color.gray;
        }
    }

    public IEnumerator Say(string text){
        speechBubble.GetComponentInChildren<Text>().text = text;
        speechBubble.GetComponent<SpriteRenderer>().sortingOrder = Card.speechBubbleSortingOrder++;
        speechBubble.GetComponentInChildren<Canvas>().sortingOrder = Card.speechBubbleSortingOrder++;
        speechBubble.SetActive(true);
        yield return new WaitForSeconds(speechBubbleDuration);
        speechBubble.SetActive(false);
        
    }
}
