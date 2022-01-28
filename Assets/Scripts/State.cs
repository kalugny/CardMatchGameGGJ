using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Params {
    public  int NumNewCardsInTurn = 1;
    public  int NumInitialCards = 6;

    public  int MaxAttractiveness = 100;
    public  int InitialAttractivenessnMean = 50;
    public  int InitialAttractivenessRange = 40;
    public  int AttractivenessDecreaseStep = 2;

    public  int MaxDesparation = 100;
    public  int InitialDesparationMean = 50;
    public  int InitialDesparationRange = 40;
    public  int DesparationIncreaseStep = 10;
    public  int DesparationDecreaseStep = 5;
    public  int DesparationBreakupStep = 10;

    public  int NumberOfTraits = 3;

    public  int MaxCards = 30;

    public  double DatingDesparationThreshold = 0.7;
    public  double DatingFullDesparationBias = 1;
    public  double DatingSemiDesparationBias = 0.2;

    public  double InLoveDesparationThreshold = 0.7;
    public  double InLoveFullDesparationBias = 1;
    public  double InLoveSemiDesparationBias = 0.2;

    public  double MarriedDesparationThreshold = 0.7;
    public  double MarriedFullDesparationBias = 1;
    public  double MarriedSemiDesparationBias = 0.2;
}

public class State : MonoBehaviour
{
 
    public Card selectedCard;

    public Params params_;

    GameLogic logic;

    void Start()
    {
        logic = new GameLogic(params_);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
