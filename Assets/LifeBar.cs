using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeBar : MonoBehaviour
{

    public Sprite[] lifeSprites;

    public int life = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoseLife()
    {
        life--;
        var sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = lifeSprites[life];
    }
}
