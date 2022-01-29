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
    public  int InitialAttractivenessRange = 70;
    public  int AttractivenessDecreaseStep = 2;

    public  int MaxDesparation = 100;
    public  int InitialDesparationMean = 50;
    public  int InitialDesparationRange = 70;
    public  int DesparationIncreaseStep = 10;
    public  int DesparationDecreaseStep = 0;
    public  int DesparationBreakupStep = 20;

    public  int NumberOfTraits = 3;

    public  int MaxCards = 30;

    public  double DatingDesparationThreshold = 70;
    public  double DatingFullDesparationBias = 1;
    public  double DatingSemiDesparationBias = 0.4;
    public  double DatingNoDesparationBias = 0.2;
    public  double DatingTraitBias = 0;

    public  double InLoveDesparationThreshold = 70;
    public  double InLoveFullDesparationBias = 1;
    public  double InLoveSemiDesparationBias = 0.2;
    public  double InLoveNoDesparationBias = 0.1;
    public  double InLoveTraitBias = 0;

    public  double MarriedDesparationThreshold = 70;
    public  double MarriedFullDesparationBias = 1;
    public  double MarriedSemiDesparationBias = 0.2;
    public  double MarriedNoDesparationBias = 0;
    public  double MarriedTraitBias = 0;

    public  int AgeMean = 35;
    public  int AgeRange = 20;
}

public class State : MonoBehaviour
{

    public Card selectedCard;

    public Params params_;

    public GameLogic logic;
    public Hand hand;
    public MatchArea matchArea;

    public LifeBar lifeBar;

    public bool gameOver = false;

    public Dictionary<int, Card> cards = new Dictionary<int, Card>();

    [Serializable]
    public struct Sentence {
        public Trait trait;
        public string sentence;

        public Sentence(Trait t, string s) {
            trait = t;
            sentence = s;
        }
    }

    public Sentence[] sentences_ = new Sentence[] {
        new Sentence(Trait.CatPerson, "I LOVE MY CAT! I wish it loved me back"),
        new Sentence(Trait.CatPerson, "My cat brought me a dead bird. Not a great present, but it's the thought that counts."),
        new Sentence(Trait.CatPerson, "I'd let you touch my pussycat, but he's a scratcher!"),
        new Sentence(Trait.DogPerson, "My dog is my best friend in the world. Steve from work is second best."),
        new Sentence(Trait.DogPerson, "I don't deserve my dog's love, but I'll take it anyway."),
        new Sentence(Trait.DogPerson, "I wish I was ever as happy as my dog is every time I come home from work."),
        new Sentence(Trait.Vegan, "Meat is murder!"),
        new Sentence(Trait.Vegan, "If you're not Vegan you're part of the problem."),
        new Sentence(Trait.Vegan, "I can't even tell the difference between meat and the plant based alternatives I eat."),
        new Sentence(Trait.Conservative, "Make America Great Again, Again!"),
        new Sentence(Trait.Conservative, "I rather not discuss politics, because it might lead to change."),
        new Sentence(Trait.Conservative, "I'll pay for dinner, but don't make me pay taxes!"),
        new Sentence(Trait.Feminist, "F___ the patriarchy!"),
        new Sentence(Trait.Feminist, "Virginia Wolfe, Oprah, Beyonce! Nuff said!"),
        new Sentence(Trait.Feminist, "Women should have more representation in the media! Not including porn."),
        new Sentence(Trait.MensRightsActivist, "The true marginalised minority is straight white men!"),
        new Sentence(Trait.MensRightsActivist, "Women, stop complaining! Men wish they could stay in the kitchen!"),
        new Sentence(Trait.MensRightsActivist, "Men die younger and spend on average two years of their life holding doors for women. Tell me again who the oppressed ones are?"),
        new Sentence(Trait.Liberal, "Free healthcare for all!"),
        new Sentence(Trait.Liberal, "I don't like guns except in socially conscious historical war movies."),
        new Sentence(Trait.Liberal, "I believe in affirmative action, so if you're a minority, let's hook up!"),
        new Sentence(Trait.Religious, "The real holy trinity is Jesus, you and me."),
        new Sentence(Trait.Religious, "Jesus is in my heart, but there's room for you too."),
        new Sentence(Trait.Religious, "I'll pray for your soul, whether you want me to or not."),
        new Sentence(Trait.Hunting, "I hunt for sport. Deer are sore losers."),
        new Sentence(Trait.Hunting, "This is my hunting rifle. There are many like it, but this one is mine."),
        new Sentence(Trait.Hunting, "If you don't see me I didn't stand you up, I'm just wearing my camouflage hunting gear."),
        new Sentence(Trait.Fishing, "I sometimes release the fish I catch so they can tell their friends how cool I am."),
        new Sentence(Trait.Fishing, "Fishing is very calming. For me, not so much for the fish."),
        new Sentence(Trait.Fishing, "There are plenty of fish in the sea and I'm a master fisherman."),
        new Sentence(Trait.TableTennis, "Try and stop my balls! BTW, I'm talking about ping pong."),
        new Sentence(Trait.TableTennis, "I'll happily serve you… my baddest ping pong serve!!!"),
        new Sentence(Trait.TableTennis, "You can come over to my place, I have a ping pong table."),
        new Sentence(Trait.Surfing, "My surfboard is with me at all times! It's a bit of a hassle actually."),
        new Sentence(Trait.Surfing, "I can teach you how to surf, but the love of the sea must come from within."),
        new Sentence(Trait.Surfing, "I love surfing. Any time, anywhere. Actually not anywhere, only on the beach."),
        new Sentence(Trait.AmusementParks, "Let's ride a rollercoaster! A real one, not an emotional one."),
        new Sentence(Trait.AmusementParks, "Nothing's more romantic than riding a ferris wheel together."),
        new Sentence(Trait.AmusementParks, "Let's go to the carnival, i'll win as many plush toys for you as you'd like."),
        new Sentence(Trait.WatchingMovies, "Have you seen the new Spider Man Movie? Can't wait to watch it."),
        new Sentence(Trait.WatchingMovies, "I like movies! I don't have many hobbies."),
        new Sentence(Trait.WatchingMovies, "My favorite food? The popcorn at the movies!"),
        new Sentence(Trait.SwimmingWithDolphins, "If you haven't swum with Dolphins you haven't experienced true joy."),
        new Sentence(Trait.SwimmingWithDolphins, "Let's go swimming with Dolphins! I promise I won't leave you for one."),
        new Sentence(Trait.SwimmingWithDolphins, "If dolphins could walk, would walking with people be as amazing for them as swimming with dolphins is for us?"),
        new Sentence(Trait.PattyCake, "Patty Cake Patty Cake let's go on a date!"),
        new Sentence(Trait.PattyCake, "You never truly know someone until you play Patty Cake with them."),
        new Sentence(Trait.PattyCake, "People who say Patty Cake is a game for kids never played Extreme Patty Cake!"),
        new Sentence(Trait.BallroomDancing, "Let me lead and I'll waltz you through a wonderful date!"),
        new Sentence(Trait.BallroomDancing, "It might take TWO to Tango, but it takes TEN years of practice to Tango well."),
        new Sentence(Trait.BallroomDancing, "When I hear Salsa, I think of dancing! Not spicy sauce."),
        new Sentence(Trait.RollerDisco, "I got the Saturday Night Fever and roller disco is the only cure!"),
        new Sentence(Trait.RollerDisco, "Dancing without skates is like walking without hands. Possible, but a little sad."),
        new Sentence(Trait.RollerDisco, "Ì learned to roller disco before I learned to walk!"),
        new Sentence(Trait.Karaoke, "Let's sing karaoke! I mean, I'll sing, you listen."),
        new Sentence(Trait.Karaoke, "I only let loose in karaoke!"),
        new Sentence(Trait.Karaoke, "What's your go-to karaoke song? You better have one ready."),
        new Sentence(Trait.ListeningToMusic, "I'd take you to see my favorite band in concert, but they broke up in the 80s."),
        new Sentence(Trait.ListeningToMusic, "If you capture my heart I'll make you a mixtape."),
        new Sentence(Trait.ListeningToMusic, "I'll let you listen to my favorite band, and all the solo projects every band member ever did."),
        new Sentence(Trait.LizardPerson, "No one is allergic to lizards!"),
        new Sentence(Trait.LizardPerson, "I have a warm relationship with my lizard. Not literally, because it's cold blooded."),
        new Sentence(Trait.LizardPerson, "My lizard may not be cuddly, but at least it doesn't bark or meow."),
        new Sentence(Trait.Atheist, "If god existed I would have stopped being single by now."),
        new Sentence(Trait.Atheist, "I don't believe in god, I believe in love!"),
        new Sentence(Trait.Atheist, "What does god and good cholesterol have in common? They're both made up!"),
        new Sentence(Trait.ConspiracyTheorist, "Covid 19 was synthesised in a lab in China! Also, it doesn't really exist!"),
        new Sentence(Trait.ConspiracyTheorist, "The 5G signal is used to brainwash us into believing the moon landing was real."),
        new Sentence(Trait.ConspiracyTheorist, "The only way to tell if someone is an alien: you yell \"zorlop!\" and see if they turn."),
        new Sentence(Trait.Cooking, "I'll cook my way into your heart."),
        new Sentence(Trait.Cooking, "I love the smell of cooking stew!"),
        new Sentence(Trait.Cooking, "I'll cook, you eat! You can also help me cook, though."),
        new Sentence(Trait.Museums, "I heard the new neo-cubist exhibition in the modern art museum is exquisite."),
        new Sentence(Trait.Museums, "I know a lot about art. Let's go to a museum, I'll show you."),
        new Sentence(Trait.Museums, "I love contemporary art. I'd tell you my favorite artist, but you probably never heard of them."),
        new Sentence(Trait.MarioKart, "Let's play Mario Kart. I'll let you pick whoever you want. Just not Bowser!"),
        new Sentence(Trait.MarioKart, "I'll take the Rainbow Road into your heart!"),
        new Sentence(Trait.MarioKart, "Sitting on the couch playing Mario Kart? BEST. DATE. EVER!"),
        new Sentence(Trait.Chess, "If you leave yourself open I'll checkmate your heart!"),
        new Sentence(Trait.Chess, "I don't care if you're white or black, I'll let you make the first move!"),
        new Sentence(Trait.Chess, "If dating was like chess I could tell how you're going to dump me the first time we meet."),
        new Sentence(Trait.TrivialPursuit, "I love trivia, did you know that WW2 ended more than 30 years ago?"),
        new Sentence(Trait.TrivialPursuit, "I'm a trivia champion! Sharks can't breathe if they don't swim. See?"),
        new Sentence(Trait.TrivialPursuit, "Try and test me on trivia, I never fail. No math questions!"),
        new Sentence(Trait.Bowling, "I'll kick your ass at bowling! But in a fun daty sort of way."),
        new Sentence(Trait.Bowling, "Do you have your own bowling shoes? I do!"),
        new Sentence(Trait.Bowling, "Let's play bowling, I promise I'll go easy on you."),
        new Sentence(Trait.Puns, "My favorite thing in the world are clever puns… I can't think of an example."),
        new Sentence(Trait.Puns, "Knock Knock. Who's there? Common. Common who? Common a date with me and you'll find out."),
        new Sentence(Trait.Puns, "Puns are fun, hon! Also fun - rhymes."),
        new Sentence(Trait.HorsebackRiding, "I love HorseBACK riding. It's my favourite way to ride a horse."),
        new Sentence(Trait.HorsebackRiding, "I won a medal for horseback riding, but actually the horse did most of the work."),
        new Sentence(Trait.HorsebackRiding, "Seeing a horse gallop is majestic! Seeing it poop, not so much.")
    };

    [Serializable]
    public struct FirstName {
        public Gender gender;
        public string firstName;

        public FirstName(Gender gender, string firstName){
            this.gender = gender;
            this.firstName = firstName;
        }
    }

    public FirstName[] firstNames_ = new FirstName[] {
        new FirstName(Gender.Male, "Alex"),
        new FirstName(Gender.Male, "Yuval"),
        new FirstName(Gender.Male, "Yoav"),
        new FirstName(Gender.Male, "Alon"),
        new FirstName(Gender.Male, "Rom"),
        new FirstName(Gender.Male, "Ran"),
        new FirstName(Gender.Male, "Daniel"),
        new FirstName(Gender.Male, "Liam"),
        new FirstName(Gender.Female, "Olivia"),
        new FirstName(Gender.Male, "Noah"),
        new FirstName(Gender.Female, "Emma"),
        new FirstName(Gender.Male, "Oliver"),
        new FirstName(Gender.Female, "Ava"),
        new FirstName(Gender.Male, "Elijah"),
        new FirstName(Gender.Female, "Charlotte"),
        new FirstName(Gender.Male, "William"),
        new FirstName(Gender.Female, "Sophia"),
        new FirstName(Gender.Male, "James"),
        new FirstName(Gender.Female, "Amelia"),
        new FirstName(Gender.Male, "Benjamin"),
        new FirstName(Gender.Female, "Isabella"),
        new FirstName(Gender.Male, "Lucas"),
        new FirstName(Gender.Female, "Mia"),
        new FirstName(Gender.Male, "Henry"),
        new FirstName(Gender.Female, "Evelyn"),
        new FirstName(Gender.Male, "Alexander"),
        new FirstName(Gender.Female, "Harper"),
        new FirstName(Gender.Male, "Mason"),
        new FirstName(Gender.Female, "Camila"),
        new FirstName(Gender.Male, "Michael"),
        new FirstName(Gender.Female, "Gianna"),
        new FirstName(Gender.Male, "Ethan"),
        new FirstName(Gender.Female, "Abigail"),
        new FirstName(Gender.Male, "Daniel"),
        new FirstName(Gender.Female, "Luna"),
        new FirstName(Gender.Male, "Jacob"),
        new FirstName(Gender.Female, "Ella"),
        new FirstName(Gender.Male, "Logan"),
        new FirstName(Gender.Female, "Elizabeth"),
        new FirstName(Gender.Male, "Jackson"),
        new FirstName(Gender.Female, "Sofia"),
        new FirstName(Gender.Male, "Levi"),
        new FirstName(Gender.Female, "Emily"),
        new FirstName(Gender.Male, "Sebastian"),
        new FirstName(Gender.Female, "Avery"),
        new FirstName(Gender.Male, "Mateo"),
        new FirstName(Gender.Female, "Mila"),
        new FirstName(Gender.Male, "Jack"),
        new FirstName(Gender.Female, "Scarlett"),
        new FirstName(Gender.Male, "Owen"),
        new FirstName(Gender.Female, "Eleanor"),
        new FirstName(Gender.Male, "Theodore"),
        new FirstName(Gender.Female, "Madison"),
        new FirstName(Gender.Male, "Aiden"),
        new FirstName(Gender.Female, "Layla"),
        new FirstName(Gender.Male, "Samuel"),
        new FirstName(Gender.Female, "Penelope"),
        new FirstName(Gender.Male, "Joseph"),
        new FirstName(Gender.Female, "Aria"),
        new FirstName(Gender.Male, "John"),
        new FirstName(Gender.Female, "Chloe"),
        new FirstName(Gender.Male, "David"),
        new FirstName(Gender.Female, "Grace"),
        new FirstName(Gender.Male, "Wyatt"),
        new FirstName(Gender.Female, "Ellie"),
        new FirstName(Gender.Male, "Matthew"),
        new FirstName(Gender.Female, "Nora"),
        new FirstName(Gender.Male, "Luke"),
        new FirstName(Gender.Female, "Hazel"),
        new FirstName(Gender.Male, "Asher"),
        new FirstName(Gender.Female, "Zoey"),
        new FirstName(Gender.Male, "Carter"),
        new FirstName(Gender.Female, "Riley"),
        new FirstName(Gender.Male, "Julian"),
        new FirstName(Gender.Female, "Victoria"),
        new FirstName(Gender.Male, "Grayson"),
        new FirstName(Gender.Female, "Lily"),
        new FirstName(Gender.Male, "Leo"),
        new FirstName(Gender.Female, "Aurora"),
        new FirstName(Gender.Male, "Jayden"),
        new FirstName(Gender.Female, "Violet"),
        new FirstName(Gender.Male, "Gabriel"),
        new FirstName(Gender.Female, "Nova"),
        new FirstName(Gender.Male, "Isaac"),
        new FirstName(Gender.Female, "Hannah"),
        new FirstName(Gender.Male, "Lincoln"),
        new FirstName(Gender.Female, "Emilia"),
        new FirstName(Gender.Male, "Anthony"),
        new FirstName(Gender.Female, "Zoe"),
        new FirstName(Gender.Male, "Hudson"),
        new FirstName(Gender.Female, "Stella"),
        new FirstName(Gender.Male, "Dylan"),
        new FirstName(Gender.Female, "Everly"),
        new FirstName(Gender.Male, "Ezra"),
        new FirstName(Gender.Female, "Isla"),
        new FirstName(Gender.Male, "Thomas"),
        new FirstName(Gender.Female, "Leah"),
        new FirstName(Gender.Male, "Charles"),
        new FirstName(Gender.Female, "Lillian"),
        new FirstName(Gender.Male, "Christopher"),
        new FirstName(Gender.Female, "Addison"),
        new FirstName(Gender.Male, "Jaxon"),
        new FirstName(Gender.Female, "Willow"),
        new FirstName(Gender.Male, "Maverick"),
        new FirstName(Gender.Female, "Lucy"),
        new FirstName(Gender.Male, "Josiah"),
        new FirstName(Gender.Female, "Paisley")
    };

    public string[] desparationSentences = new string[] {
        "Literally anyone would do",
        "If you have a heartbeat I want to date you",
        "Not looking for a prince charming",
        "Of course I'll give you a chance!",
        "I want a date, don't care with whom",
        "One more disappointment and I’m adopting 20 cats",
        "Don’t make me give up on love",
        "My heart wouldn’t stand another blow",
        "Who needs love, anyway…"
    };

    void Start()
    {
        logic = new GameLogic(params_);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayTurn(){
        logic.PlayTurn();

        foreach (var kvp in cards){
            kvp.Value.speechBubble.SetActive(false);
            kvp.Value.gameObject.SetActive(false);
        }

        var almostDesperate = logic.GetAlmostDesparateCards();
        foreach (var card in almostDesperate){
            StartCoroutine(cards[card.id].Say(desparationSentences[UnityEngine.Random.Range(0, desparationSentences.Length)]));
        }

        var desperateCards = logic.GetDesperateCards();
        foreach (GameCard gc in desperateCards){
            cards[gc.id].MakeDesperate();
        }
        lifeBar.SetLife(5 - desperateCards.Count);
        if (lifeBar.life <= 0){
            gameOver = true;
        }

        matchArea.UpdateMatches();
        hand.NewTurn();
    }
    private GUIStyle guiStyle = new GUIStyle(); 
 
         
    public void OnGUI(){
        if (gameOver){
            guiStyle.fontSize = 100;
            GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 200, 100), "Game Over", guiStyle);
        }
    }
}
