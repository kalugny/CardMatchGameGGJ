using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileImage : MonoBehaviour
{

    public Sprite[] menHeads;
    public Sprite[] menShirts;
    public Sprite[] menNeckShadows;
    public Sprite[] menNoses;
    public Sprite[] menEyes;
    public Sprite[] menMouths;
    public Sprite[] menHair;
    public Sprite[] menEyebrows;
    public Sprite[] menBeards;
    public Sprite[] womenHeads;
    public Sprite[] womenShirts;
    public Sprite[] womenNeckShadows;
    public Sprite[] womenFaces;
    public Sprite[] womenHair;
    public Sprite[] womenEyebrows;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGO(Sprite sprite, int sortingOrder){
        GameObject go = new GameObject();
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;
    }

    public void CreateMenProfile(){
        int head = Random.Range(0, menHeads.Length);
        int shirt = Random.Range(0, menShirts.Length);
        int neckShadow = Random.Range(0, menNeckShadows.Length);
        int nose = Random.Range(0, menNoses.Length);
        int eye = Random.Range(0, menEyes.Length);
        int mouth = Random.Range(0, menMouths.Length);
        int hair = Random.Range(0, menHair.Length + 1);
        int eyebrow = Random.Range(0, menEyebrows.Length);
        int beard = Random.Range(0, menBeards.Length + 1);

        CreateGO(menHeads[head], Card.sortingOrder++);
        CreateGO(menNeckShadows[neckShadow], Card.sortingOrder++);
        CreateGO(menShirts[shirt], Card.sortingOrder++);
        CreateGO(menNoses[nose], Card.sortingOrder++);
        CreateGO(menEyes[eye], Card.sortingOrder++);
        CreateGO(menEyebrows[eyebrow], Card.sortingOrder++);
        CreateGO(menMouths[mouth], Card.sortingOrder++);
        if (hair < menHair.Length){
            CreateGO(menHair[hair], Card.sortingOrder++);
        }
        if (beard < menBeards.Length){
            CreateGO(menBeards[beard], Card.sortingOrder++);
        }        
    }

    public void CreateWomenProfile(){
        int head = Random.Range(0, womenHeads.Length);
        int shirt = Random.Range(0, womenShirts.Length);
        int neckShadow = Random.Range(0, womenNeckShadows.Length);
        int face = Random.Range(0, womenFaces.Length);
        int hair = Random.Range(0, womenHair.Length);
        int eyebrow = Random.Range(0, womenEyebrows.Length);

        CreateGO(womenHeads[head], Card.sortingOrder++);
        CreateGO(womenNeckShadows[neckShadow], Card.sortingOrder++);
        CreateGO(womenFaces[face], Card.sortingOrder++);
        CreateGO(womenEyebrows[eyebrow], Card.sortingOrder++);
        CreateGO(womenHair[hair], Card.sortingOrder++);        
        CreateGO(womenShirts[shirt], Card.sortingOrder++);
    }
}
