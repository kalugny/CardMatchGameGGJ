using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using UnityEngine;



public class GameLogic {
    // This class represents the state of the game at a single turn

    public Deck deck;
    public List<Match> matches = new List<Match>();
    public TraitMatching traitMatching;

    public Params params_;

    public GameLogic(Params params_) {
        this.params_ = params_;
        traitMatching = new TraitMatching(@"Assets/Resources/traitMatchingV3.csv");
        deck = new Deck(params_, traitMatching);
    }

    public void CreateNewMatch(GameCard card1, GameCard card2) {
        matches.Add(new Match(card1, card2, traitMatching, params_));
    }

    public void PlayTurn() {
        Console.WriteLine ("Playing turn");
        foreach (Match match in matches) {
            match.PlayMatch();
        }

        matches.RemoveAll(m => !m.card1.isBeingMatched || !m.card2.isBeingMatched);

        deck.AgeCards();
        deck.RemoveDesparateCards();
        deck.DrawCards(params_.NumNewCardsInTurn);
    }

    public List<GameCard> GetHand() {
        return deck.GetHand();
    }

    public List<GameCard> GetDesperateCards(){
        return deck.GetDesperateCards();
    }

    public List<GameCard> GetAlmostDesparateCards() {
        return deck.GetAlmostDesparateCards();
    }

    public List<Match> GetMatches() {
        return matches;
    }

    public string GetDescription() {
        string desc = "";
        // desc += "Hand: \n\n";
        //
        // foreach (GameCard card in this.GetHand()) {
        //     if (!card.isBeingMatched) {
        //         desc += card.GetDescription() + "\n\n";
        //     }
        // }

        desc += "\nMatches:\n";

        for (int i = this.matches.Count - 1 ; i >= 0 ; i -- ) {
        //foreach (Match match in this.matches.Reverse()) {
            Match match = this.matches[i];
            desc += match.level + "\ntrait " + match.traitsScore + " prob: " + match.advanceProb + " toss: " + match.advanceToss +  "\n";
            desc += match.card1.GetDescription() + "\n";
            desc += match.card2.GetDescription() + "\n";
        }

        return desc;
    }
}

public class GameCard {
    public Gender gender;
    public int age;
    public Gender[] preferredGenders;
    public int attractiveness;
    public int desperation;
    public List<Trait> traits = new List<Trait>();
    public bool isBeingMatched;
    // System.Random _R = new System.Random();
    public bool isTooDespearate = false;

    Params params_;

    public int id;

    public GameCard(Params params_, int id, TraitMatching traitMatching) {
        this.params_ = params_;
        this.id = id;
        this.gender = this.RandomEnumValue<Gender>();

        this.preferredGenders = new Gender[1];
        this.preferredGenders[0] = this.RandomEnumValue<Gender>();

        // this.attractiveness = _R.Next(GameLogic.params_.InitialAttractivenessnMean - GameLogic.params_.InitialAttractivenessRange / 2,  GameLogic.params_.InitialAttractivenessnMean + GameLogic.params_.InitialAttractivenessRange / 2);
        this.desperation = UnityEngine.Random.Range(params_.InitialAttractivenessnMean - params_.InitialDesparationRange / 2, params_.InitialAttractivenessnMean + params_.InitialDesparationRange / 2);

        this.age = UnityEngine.Random.Range(params_.AgeMean - params_.AgeRange / 2, params_.AgeMean + params_.AgeRange / 2);

        for (int i=0 ; i < params_.NumberOfTraits ; i++) {
            Trait newTrait = this.RandomEnumValue<Trait>();

            while (this.traits.Contains(newTrait) || isTraitCollides(newTrait, traitMatching)) {
                newTrait = this.RandomEnumValue<Trait>();
            }

            this.traits.Add(newTrait);
        }

        this.isBeingMatched = false;
    }

    public bool isTraitCollides(Trait newTrait, TraitMatching traitMatching) {
        bool colission = false;
        foreach (Trait t in this.traits) {
            if (traitMatching.GetTraitPairScore(t, newTrait) < 0) {
                colission = true;
            }
        }

        return colission;
    }

    T RandomEnumValue<T> ()
    {
        var v = Enum.GetValues (typeof (T));
        return (T) v.GetValue(UnityEngine.Random.Range(0, v.Length));
    }

    public string GetDescription() {
        string desc = String.Format("Desperation: {0}\nTraits: ", desperation);

        foreach (Trait t in traits) {
            desc += t + " ";
        }

        return desc;

    }

    public void DecreaseDesparation(int step) {
        desperation -= step;
        if (desperation < 0) {
            desperation = 0;
        }
    }

    public void IncreaseDesparation(int step) {
        desperation += step;
        if (desperation > params_.MaxDesparation) {
            desperation = params_.MaxDesparation;
        }
    }

    public void DecreaseAttractiveness(int step) {
        attractiveness -= step;
        if (attractiveness < 0) {
            attractiveness = 0;
        }
    }
}

public class Deck {
    public static int cardCounter = 0;
    public List<GameCard> cards = new List<GameCard>();
    public TraitMatching traitMatching;

    Params params_;

    public Deck(Params params_, TraitMatching traitMatching) {
        this.params_ = params_;
        this.traitMatching = traitMatching;
        this.DrawCards(params_.NumInitialCards);
    }

    public void DrawCards(int numCards) {
        // TODO: how many cards? how do me make sure there are enough possible matches, at least in the sense of preferred gender?
        for (int i = 0 ; i < numCards ; i++) {
            if (cards.Count >= params_.MaxCards) {
                return;
            }

            Console.WriteLine ("Adding new card");
            cards.Add(new GameCard(params_, cardCounter++, traitMatching));
        }
    }

    public List<GameCard> GetHand() {
        List<GameCard> hand = new List<GameCard>();

        foreach (GameCard card in this.cards) {
            if (!card.isBeingMatched) {
                hand.Add(card);
            }
        }

        return hand;
    }

    public void AgeCards() {
        foreach (GameCard card in this.cards) {
            if (!card.isBeingMatched) {
                card.desperation += params_.DesparationIncreaseStep;
            }

            card.DecreaseAttractiveness(params_.AttractivenessDecreaseStep);
        }
    }

    public void RemoveDesparateCards() {
        Debug.Log("All cards = " + cards.Count);
        var desperateCards = this.cards.Where(card => card.desperation > params_.MaxDesparation);
        foreach (GameCard card in desperateCards) {
            card.isTooDespearate = true;
        }
    }

    public List<GameCard> GetDesperateCards() {
        return this.cards.Where(card => card.isTooDespearate).ToList();
    }

    public List<GameCard> GetAlmostDesparateCards() {
        return this.cards.Where(card => card.desperation > (params_.MaxDesparation - params_.DesparationIncreaseStep)).ToList();
    }
}

public class Match {
    public MatchLevel level;
    public GameCard card1, card2;
    public double advanceProb;
    public double advanceToss;
    public double traitsScore;

    public TraitMatching traitMatching;

    public Params params_;

    public Match(GameCard card1, GameCard card2, TraitMatching traitMatching, Params params_) {
        this.params_ = params_;
        this.level = MatchLevel.Dating;
        this.card1 = card1;
        this.card2 = card2;
        this.traitMatching = traitMatching;
        card1.isBeingMatched = true;
        card2.isBeingMatched = true;
    }

    public void PlayMatch() {
        // TODO: add traits matching and probability calculation
        if (this.level == MatchLevel.Dating) {
            PlayDatingLevel();
        }
        else if (this.level == MatchLevel.InLove) {
            PlayInLoveLevel();
        }
        else if (this.level == MatchLevel.Married) {
            PlayMarriedLevel();
        }
        else if (this.level == MatchLevel.HappyEverAfter) {
            // do nothing, don't ruin it
        }
        else if (this.level == MatchLevel.Breakup){
            card1.isBeingMatched = false;
            card2.isBeingMatched = false;
        }
        else {
            Console.WriteLine("ERROR");
        }
    }

    public void PlayDatingLevel() {
        // this.card1.DecreaseDesparation(GameLogic.params_.DesparationDecreaseStep);
        // this.card2.DecreaseDesparation(GameLogic.params_.DesparationDecreaseStep);

        traitsScore = traitMatching.GetTraitMatchScore(card1.traits, card2.traits) + params_.DatingTraitBias;

        // double advanceProb;

        if (card1.desperation >= params_.DatingDesparationThreshold &&
            card2.desperation >= params_.DatingDesparationThreshold) {
            advanceProb = traitsScore + params_.DatingFullDesparationBias;
        }
        else if (card1.desperation >= params_.DatingDesparationThreshold ||
            card2.desperation >= params_.DatingDesparationThreshold) {
            advanceProb = traitsScore + params_.DatingSemiDesparationBias;
        }
        else {
            advanceProb = traitsScore + params_.DatingNoDesparationBias;
        }

        AdvanceToNextLevel(advanceProb, MatchLevel.InLove);
    }

    public void PlayInLoveLevel() {
        // this.card1.DecreaseDesparation(GameLogic.params_.DesparationDecreaseStep);
        // this.card2.DecreaseDesparation(GameLogic.params_.DesparationDecreaseStep);

        traitsScore = traitMatching.GetTraitMatchScore(card1.traits, card2.traits) + params_.InLoveTraitBias;

        // double advanceProb;

        if (card1.desperation >= params_.InLoveDesparationThreshold &&
            card2.desperation >= params_.InLoveDesparationThreshold) {
            advanceProb = traitsScore + params_.InLoveFullDesparationBias;
        }
        else if (card1.desperation >= params_.InLoveDesparationThreshold ||
            card2.desperation >= params_.InLoveDesparationThreshold) {
            advanceProb = traitsScore + params_.InLoveSemiDesparationBias;
        }
        else {
            advanceProb = traitsScore + params_.InLoveNoDesparationBias;
        }

        AdvanceToNextLevel(advanceProb, MatchLevel.Married);
    }

    public void PlayMarriedLevel() {
        this.card1.DecreaseDesparation(params_.DesparationDecreaseStep);
        this.card2.DecreaseDesparation(params_.DesparationDecreaseStep);

        traitsScore = traitMatching.GetTraitMatchScore(card1.traits, card2.traits) + params_.MarriedTraitBias;

        // double advanceProb;

        if (card1.desperation >= params_.MarriedDesparationThreshold &&
            card2.desperation >= params_.MarriedDesparationThreshold) {
            advanceProb = traitsScore + params_.MarriedFullDesparationBias;
        }
        else if (card1.desperation >= params_.MarriedDesparationThreshold ||
            card2.desperation >= params_.MarriedDesparationThreshold) {
            advanceProb = traitsScore + params_.MarriedSemiDesparationBias;
        }
        else {
            advanceProb = traitsScore + params_.MarriedNoDesparationBias;
        }

        AdvanceToNextLevel(advanceProb, MatchLevel.HappyEverAfter);
    }

    void AdvanceToNextLevel(double advanceProb, MatchLevel nextLevel) {
        // System.Random rand = new System.Random();
        // UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        advanceToss = UnityEngine.Random.value;

        if (advanceToss < advanceProb) {
            this.level = nextLevel;
        }
        else {
            this.ApplyBreakup();
        }
    }

    void ApplyBreakup() {
        Console.WriteLine("Breaking up");
        this.level = MatchLevel.Breakup;
        this.card1.IncreaseDesparation(params_.DesparationBreakupStep);
        // this.card1.isBeingMatched = false;
        this.card2.IncreaseDesparation(params_.DesparationBreakupStep);
        // this.card2.isBeingMatched = false;
    }
}

public class TraitMatching {
    int[,] table;

    public TraitMatching(string csvPath) {
        int numTraits = Trait.GetValues(typeof (Trait)).Length;
        this.table = new int[numTraits, numTraits];

        // Console.WriteLine("NumTraits " + numTraits);

        using(var reader = new StreamReader(csvPath))
        {
            int i = 0;
            int j = 0;
            string line = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] values = line.Split(',');

                bool firstValueInLine = true;

                foreach (string value in values) {
                    if (firstValueInLine) {
                        firstValueInLine = false;
                    }
                    else {
                        if (value.Length == 0) {
                            table[i, j] = 0;
                        }
                        else {
                            table[i, j] = Int32.Parse(value);
                        }

                        // Console.WriteLine("i " + i + " j " + j + " value " + table[i,j] + " string " + value);

                        j++;
                    }
                }

                i++;
                j = 0;
            }
        }

        for (int i = 0 ; i < numTraits ; i++) {
            for (int j = 0 ; j < numTraits ; j++) {
                if (i > j) {
                    table[i,j] = table[j,i];
                }
            }
        }
    }

    public double GetTraitMatchScore(List<Trait> traits1, List<Trait> traits2) {
        double score = 0;
        int numTraits = 0;

        foreach(Trait trait1 in traits1) {
            foreach(Trait trait2 in traits2) {
                int traitsMatch = this.table[(int)trait1, (int)trait2];
                if (traitsMatch != 0) {
                    score += (double)traitsMatch;
                    numTraits++;
                }
            }
        }

        if (numTraits == 0) {
            return 0.5;
        }
        else {
            return (score / numTraits + 1) / 2;
        }
    }

    public double GetTraitPairScore(Trait trait1, Trait trait2) {
        return this.table[(int)trait1, (int)trait2];
    }
}

public enum MatchLevel {
    Dating,
    InLove,
    Married,
    HappyEverAfter,
    Breakup
}

public enum Gender {
    Female,
    Male
}

public enum Trait {
    CatPerson,
    DogPerson,
    Vegan,
    Conservative,
    Feminist,
    MensRightsActivist,
    Liberal,
    Religious,
    Hunting,
    Fishing,
    TableTennis,
    Surfing,
    AmusementParks,
    WatchingMovies,
    SwimmingWithDolphins,
    PattyCake,
    BallroomDancing,
    RollerDisco,
    Karaoke,
    ListeningToMusic,
    LizardPerson,
    Atheist,
    ConspiracyTheorist,
    Cooking,
    Museums,
    MarioKart,
    Chess,
    TrivialPursuit,
    Bowling,
    Puns,
    HorsebackRiding
}

// etc...
