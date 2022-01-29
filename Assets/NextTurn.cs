using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextTurn : MonoBehaviour, IPointerDownHandler
{
    public State state;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state.gameOver){
            gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData){
        if (!state.gameOver){
            
            state.PlayTurn();
            
        }        
    }
}
