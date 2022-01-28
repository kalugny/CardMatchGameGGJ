using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    new private Collider2D collider;
    public State state;
    public Vector3 originalPos;
    private bool selected = false;
    public bool placed = false;
    
    public GameObject cardRoot;
    public float scaleTo = 1.4f;
    public float animationDuration = 1f;
    public AnimationCurve scaleCurve;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponentInChildren<Collider2D>();
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {  
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && !placed){
            if (collider.OverlapPoint(mousePosition)){
                if (!selected){
                    StartCoroutine(BringToFront());
                }
                selected = true;
                state.selectedCard = this;
            }
            else {
                if (selected){
                    StartCoroutine(SendToBack());
                }
                selected = false;
                if (state.selectedCard == this){
                    state.selectedCard = null;
                }
            }
        }
        
    }

    IEnumerator BringToFront(){
        float time = 0f;
        while (time <= 1f){
            float scale = scaleCurve.Evaluate(time);
            time += Time.deltaTime / animationDuration;
            
            cardRoot.transform.localScale = new Vector3(scale, scale, 1);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator SendToBack(){
        float time = 1f;
        while (time >= 0f){
            float scale = scaleCurve.Evaluate(time);
            time -= Time.deltaTime / animationDuration;
            
            cardRoot.transform.localScale = new Vector3(scale, scale, 1);
            yield return new WaitForFixedUpdate();
        }
    }
}
