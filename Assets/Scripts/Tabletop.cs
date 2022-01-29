using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tabletop : MonoBehaviour, IPointerDownHandler
{

    public State state;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (state.selectedCard != null){
            StartCoroutine(state.selectedCard.SendToBack());
        }
    }
}
