using System.Collections.Generic;
using System;
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
        deck = new Deck(params_);
        traitMatching = new TraitMatching(@"Assets/Resources/MatchmakingTraits.csv");
        Debug.Log(traitMatching);
    }

    public void CreateNewMatch(GameCard card1, GameCard card2) {
        matches.Add(new Match(card1, card2, traitMatching, params_));
    }

    public void PlayTurn() {
        Console.WriteLine ("Playing turn");
        foreach (Match match in matches) {
            match.PlayMatch();
        }

        matches.RemoveAll(match => match.level == MatchLevel.Breakup);

        deck.AgeCards();
        deck.DrawCards(params_.NumNewCardsInTurn);
        deck.RemoveDesparateCards();
    }

    public List<GameCard> GetHand() {
        return deck.GetHand();
    }

    public List<Match> GetMatches() {
        return matches;
    }

    public string GetDescription() {
        string desc = "";
        desc += "Hand: \n";

        foreach (GameCard card in this.GetHand()) {
            if (!card.isBeingMatched) {
                desc += card.GetDescription() + "\n";
            }
        }

        desc += "Matches: \n";

        foreach (Match match in this.matches) {
            desc += match.level + " ";
            desc += match.card1.GetDescription() + "\n";
            desc += match.card2.GetDescription() + "\n\n";
        }

        return desc;
    }
}

public class GameCard {
    public Gender gender;
    public Gender[] preferredGenders;
    public int attractiveness;
    public int desperation;
    public List<Trait> traits = new List<Trait>();
    public bool isBeingMatched;
    // System.Random _R = new System.Random();

    Params params_;

    public GameCard(Params params_) {
        this.params_ = params_;
        this.gender = this.RandomEnumValue<Gender>();

        this.preferredGenders = new Gender[1];
        this.preferredGenders[0] = this.RandomEnumValue<Gender>();

        // this.attractiveness = _R.Next(GameLogic.params_.InitialAttractivenessnMean - GameLogic.params_.InitialAttractivenessRange / 2,  GameLogic.params_.InitialAttractivenessnMean + GameLogic.params_.InitialAttractivenessRange / 2);
        this.desperation = UnityEngine.Random.Range(params_.InitialAttractivenessnMean - params_.InitialDesparationRange / 2, params_.InitialAttractivenessnMean + params_.InitialDesparationRange / 2);

        for (int i=0 ; i < params_.NumberOfTraits ; i++) {
            Trait newTrait = this.RandomEnumValue<Trait>();
            while (this.traits.Contains(newTrait)) {
                newTrait = this.RandomEnumValue<Trait>();
            }

            this.traits.Add(newTrait);
        }

        this.isBeingMatched = false;
    }

    T RandomEnumValue<T> ()
    {
        var v = Enum.GetValues (typeof (T));
        return (T) v.GetValue(UnityEngine.Random.Range(0, v.Length));
    }

    public string GetDescription() {
        return String.Format("G {0} Att {1} Desp {2} Traits {3}, {4}, {5}", gender, attractiveness, desperation, traits[0], traits[1], traits[2]);
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
    public List<GameCard> cards = new List<GameCard>();

    Params params_;

    public Deck(Params params_) {
        this.params_ = params_;
        this.DrawCards(params_.NumInitialCards);
    }

    public void DrawCards(int numCards) {
        // TODO: how many cards? how do me make sure there are enough possible matches, at least in the sense of preferred gender?
        for (int i = 0 ; i < numCards ; i++) {
            if (cards.Count >= params_.MaxCards) {
                return;
            }

            Console.WriteLine ("Adding new card");
            cards.Add(new GameCard(params_));
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
        this.cards.RemoveAll(card => card.desperation > params_.MaxDesparation);
    }
}

public class Match {
    public MatchLevel level;
    public GameCard card1, card2;

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
        else {
            Console.WriteLine("ERROR");
        }
    }

    public void PlayDatingLevel() {
        // this.card1.DecreaseDesparation(GameLogic.params_.DesparationDecreaseStep);
        // this.card2.DecreaseDesparation(GameLogic.params_.DesparationDecreaseStep);

        double traitsScore = traitMatching.GetTraitMatchScore(card1.traits, card2.traits);

        double advanceThreshold;

        if (card1.desperation >= params_.DatingDesparationThreshold &&
            card2.desperation >= params_.DatingDesparationThreshold) {
            advanceThreshold = traitsScore + params_.DatingFullDesparationBias;
        }
        else if (card1.desperation >= params_.DatingDesparationThreshold ||
            card2.desperation >= params_.DatingDesparationThreshold) {
            advanceThreshold = traitsScore + params_.DatingSemiDesparationBias;
        }
        else {
            advanceThreshold = traitsScore;
        }

        AdvanceToNextLevel(advanceThreshold, MatchLevel.InLove);
    }

    public void PlayInLoveLevel() {
        // this.card1.DecreaseDesparation(GameLogic.params_.DesparationDecreaseStep);
        // this.card2.DecreaseDesparation(GameLogic.params_.DesparationDecreaseStep);

        double traitsScore = traitMatching.GetTraitMatchScore(card1.traits, card2.traits);

        double advanceThreshold;

        if (card1.desperation >= params_.InLoveDesparationThreshold &&
            card2.desperation >= params_.InLoveDesparationThreshold) {
            advanceThreshold = traitsScore + params_.InLoveFullDesparationBias;
        }
        else if (card1.desperation >= params_.InLoveDesparationThreshold ||
            card2.desperation >= params_.InLoveDesparationThreshold) {
            advanceThreshold = traitsScore + params_.InLoveSemiDesparationBias;
        }
        else {
            advanceThreshold = traitsScore;
        }

        AdvanceToNextLevel(advanceThreshold, MatchLevel.Married);
    }

    public void PlayMarriedLevel() {
        this.card1.DecreaseDesparation(params_.DesparationDecreaseStep);
        this.card2.DecreaseDesparation(params_.DesparationDecreaseStep);

        double traitsScore = traitMatching.GetTraitMatchScore(card1.traits, card2.traits);

        double advanceThreshold;

        if (card1.desperation >= params_.MarriedDesparationThreshold &&
            card2.desperation >= params_.MarriedDesparationThreshold) {
            advanceThreshold = traitsScore + params_.MarriedFullDesparationBias;
        }
        else if (card1.desperation >= params_.MarriedDesparationThreshold ||
            card2.desperation >= params_.MarriedDesparationThreshold) {
            advanceThreshold = traitsScore + params_.MarriedSemiDesparationBias;
        }
        else {
            advanceThreshold = traitsScore;
        }

        AdvanceToNextLevel(advanceThreshold, MatchLevel.HappyEverAfter);
    }

    void AdvanceToNextLevel(double advanceThreshold, MatchLevel nextLevel) {
        // System.Random rand = new System.Random();
        double advanceProb = UnityEngine.Random.value;

        if (advanceProb > advanceThreshold) {
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
        this.card2.IncreaseDesparation(params_.DesparationBreakupStep);
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
            line = reader.ReadLine();
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

<<<<<<< HEAD
public enum Trait {
=======
enum Trait {
>>>>>>> c5a0a07 (Add all match level advance rules)
    CatPerson,
    DogPerson,
    Introvert,
    Extrovert,
    Clean,
    Messy,
    Competitive,
    LaidBack,
    Anxious,
    Optimist,
    Pessimist,
    Lazy,
    Workaholic,
    Vegan,
    AnimalRightsActivist,
    ConservativeCapitalist,
    Feminist,
    MensRightsActivist,
    SociallyLiberal,
    Religious,
    Anarchist,
    Authoritarian,
    Hunting,
    Fishing,
    TableTennis,
    BirdWatching,
    Kiting,
    Surfing,
    KiteSurfing,
    AmusementParks,
    Movies,
    LongWalksOnTheBeach,
    FeedingDucksInThePark,
    SwimmingWithDolphins,
    PettingZoos,
    PlayingPattyCake,
    BallroomDancing,
    RollerDisco,
    Karaoke,
    Music
}

// etc...
