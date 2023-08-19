using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    public List<Player> PlayerList;
    public List<string> PlayerListNames => PlayerList.Select(x=> x.Name).ToList();
    public List<Player> PlayerListByScore=> PlayerList.OrderBy(x => x.GetTotalScore()).Reverse().ToList();

    public int HandCards;
    public int multyplier;
    public int RoundIndex;
    public int maxRounds;
    public CardType cardType;
    public SceneManager sceneManager;
    // Start is called before the first frame update
    bool indexIsShowing;
    bool handsIsShowing;
    bool selectingButtonIsClicked;
    public void AskForPlayerAmount()
    {
        PlayerList = new List<Player>();
        multyplier = 1;
        HandCards = 1;
        sceneManager.selectingPlayerCanvas();
    }

    public void BackToMainMenu()
    { 
        PlayerList=new List<Player>();
        sceneManager.mainMenuCanvas();
    }
    public void SetRounds()
    {
        sceneManager.SelectingMin= 1;
        sceneManager.SelectingMax = Mathf.FloorToInt((int)cardType / PlayerList.Count) ;
        sceneManager.SelectingAmount = sceneManager.SelectingMax;
        sceneManager.SelectingTitle = "Rounds";
        sceneManager.SelectingAmountInformation.text = $"";
        sceneManager.SelectingAmountButton.onClick.RemoveAllListeners();
        sceneManager.SelectingAmountButton.onClick.AddListener(() =>{
                maxRounds = sceneManager.SelectingAmount;
                Round();});

        sceneManager.SelectingAmountBackButton.onClick.RemoveAllListeners();
        sceneManager.SelectingAmountBackButton.onClick.AddListener(BackToPlayerSelection);
        sceneManager.selectingAmountCanvas();
    }
    public void BackToPlayerSelection()
    {
        sceneManager.selectingPlayerCanvas();
    }
    public void ChangeDirection()
    {
        HandCards -= multyplier;
        multyplier *= -1;
        Round();
    }

    public void Round()
    {
        rotateList();

        if (HandCards > maxRounds)
        {
            HandCards -= multyplier;
            multyplier *= -1;
        }

        if (HandCards <= 0)
        {
            sceneManager.playerListCanvas();
        }
        else
        {
            StartRound();
            StartCoroutine(RoundCoroutine());
        }
    }

    public void StartRound() => StartCoroutine(StartRoundTitleAnimationCoroutine());

    public void rotateList()
    {
        var temp = PlayerList[0];
        PlayerList.RemoveAt(0);
        PlayerList.Add(temp);

    }
    public void antiRotateList()
    {
        var temp = PlayerList[PlayerList.Count-1];
        PlayerList.RemoveAt(PlayerList.Count - 1);
        var tempList = new List<Player>() { temp };
        tempList.AddRange(PlayerList);
        PlayerList= tempList;
    }
    //Hace la predicción de cada jugador
    IEnumerator RoundCoroutine(int init = 0)
    {
        bool cancel = false;

        int w = 0;

        for (int i = 0; i < init; i++)
        {
            w += PlayerList[i].Predictions[RoundIndex];
        }
        
        for (int i=init;i<PlayerList.Count;i++) 
        {
            if (i < 0)
            {
                cancel = true;
                break;
            }

            var player = PlayerList[i];

            sceneManager.SelectingMin = 0;
            sceneManager.SelectingAmount = PlayerList[i].Predictions.Count > RoundIndex ? PlayerList[i].Predictions[RoundIndex] : 0;
            sceneManager.SelectingMax = HandCards;
            sceneManager.SelectingTitle = $"Prediction {player.Name}";

            if (player == PlayerList[PlayerList.Count - 1] && HandCards - w>=0)
            {

                sceneManager.SelectingForbiddenList = new List<int> { HandCards - w };
                sceneManager.SelectingAmountInformation.text = $"Forbidden: {HandCards - w}";

                while(sceneManager.SelectingForbiddenList.Contains(sceneManager.SelectingMin))
                {
                    sceneManager.SelectingMin++;
                }
                sceneManager.SelectingAmount = sceneManager.SelectingMin;

                while (sceneManager.SelectingForbiddenList.Contains(sceneManager.SelectingMax))
                {
                    sceneManager.SelectingMax--;
                }


            }
            else sceneManager.SelectingAmountInformation.text = $"";

            sceneManager.SelectingAmountButton.onClick.RemoveAllListeners();
            sceneManager.SelectingAmountButton.onClick.AddListener(()=>selectingButtonIsClicked=true);

            sceneManager.SelectingAmountBackButton.onClick.RemoveAllListeners();
            sceneManager.SelectingAmountBackButton.onClick.AddListener(() =>{
                print(i);
                selectingButtonIsClicked = true;
                i -=2;
            });


                sceneManager.selectingAmountCanvas();
            yield return new WaitUntil(()=> selectingButtonIsClicked);

            selectingButtonIsClicked = false;
            var x = int.Parse(sceneManager.SelectingAmountText.text);
            w += x;
            if (player.Predictions.Count > RoundIndex)
                player.Predictions[RoundIndex] = x;
            else player.AddPrediction(x);


            Debug.Log("Predicción agregada");
        }

        if (!cancel)
        {
            sceneManager.chargingCanvas();
            sceneManager.ChargingButton.onClick.RemoveAllListeners();
            sceneManager.ChargingButton.onClick.AddListener(() => StartCoroutine(EndRoundCoroutine()));
            sceneManager.ChargingBackButton.onClick.RemoveAllListeners();
            sceneManager.ChargingBackButton.onClick.AddListener(() =>{
                StartCoroutine(RoundCoroutine(PlayerList.Count-1));
            });
        }
        else
        {
            HandCards -= multyplier;
            RoundIndex--;
            antiRotateList();

            if (RoundIndex >= 0) StartCoroutine(EndRoundCoroutine(PlayerList.Count - 1)); 
            else SetRounds();
        }
    }
    IEnumerator EndRoundCoroutine(int init = 0)
    {
        bool cancel = false;
        for (int i = init; i < PlayerList.Count; i++)
        {
            if (i < 0)
            {
                cancel = true;
                break;
            }

            var player = PlayerList[i];
            sceneManager.SelectingMin = 0;
            sceneManager.SelectingAmount = PlayerList[i].Wins.Count > RoundIndex ? PlayerList[i].Wins[RoundIndex] : 0;
            sceneManager.SelectingMax = HandCards;
            sceneManager.SelectingTitle = $"Wins {player.Name}" ;
            sceneManager.SelectingForbiddenList=new List<int>();
            sceneManager.SelectingAmountInformation.text = $"";
            sceneManager.SelectingAmountButton.onClick.RemoveAllListeners();
            sceneManager.SelectingAmountButton.onClick.AddListener(() => selectingButtonIsClicked = true);

            sceneManager.SelectingAmountBackButton.onClick.RemoveAllListeners();
            sceneManager.SelectingAmountBackButton.onClick.AddListener(() => {
                print(i);
                selectingButtonIsClicked = true;
                i -= 2;
            });

            sceneManager.selectingAmountCanvas();
            yield return new WaitUntil(() => selectingButtonIsClicked);

            selectingButtonIsClicked = false;
            var x = int.Parse(sceneManager.SelectingAmountText.text);

            if (player.Predictions.Count > RoundIndex)
                player.AddWins(x);
            else player.AddPrediction(x);

            
            Debug.Log("Win agregada");
        }

        if (!cancel)
        {   
            sceneManager.setPlayersLabels();
            sceneManager.playerListCanvas();
            RoundIndex++;
            HandCards += multyplier;
        }
        else 
        {
            StartCoroutine(RoundCoroutine(PlayerList.Count - 1));
        }


    }
    

    IEnumerator StartRoundTitleAnimationCoroutine()
    {
        sceneManager.ShowPlayerListCommon.gameObject.SetActive(false);
        sceneManager.StartRoundIndex.text = "" + (RoundIndex+1);
        sceneManager.StartRoundTitleShadow.text = sceneManager.StartRoundTitle.text;
        sceneManager.StartRoundIndexShadow.text = sceneManager.StartRoundIndex.text;

        sceneManager.StartRoundHand.text = $"( {HandCards} cards )";
        sceneManager.StartRoundHandShadow.text = $"( {HandCards} cards )";

        sceneManager.StartRoundCanvas.SetActive(true);

        var c = sceneManager.StartRoundTitle.color;
        var d = sceneManager.StartRoundIndex.color;
        var h = sceneManager.StartRoundHand.color;
        var sc = sceneManager.StartRoundTitleShadow.color;
        var sd = sceneManager.StartRoundIndexShadow.color;
        var sh = sceneManager.StartRoundHandShadow.color;

        sceneManager.StartRoundTitle.color = new(c.r, c.g, c.b, 0);
        sceneManager.StartRoundIndex.color = new(d.r, d.g, d.b, 0);
        sceneManager.StartRoundHand.color = new(h.r, h.g, h.b, 0);
        sceneManager.StartRoundTitleShadow.color = new(sc.r, sc.g, sc.b, 0);
        sceneManager.StartRoundIndexShadow.color = new(sd.r, sd.g, sd.b, 0);
        sceneManager.StartRoundHandShadow.color = new(sh.r, sh.g, sh.b, 0);

        while (sceneManager.StartRoundTitle.color.a <= 1)
        {
            sceneManager.ShowPlayerListCommon.gameObject.SetActive(false);

            c = sceneManager.StartRoundTitle.color;
            sceneManager.StartRoundTitle.color = new(c.r,c.g,c.b,c.a+0.02f);
            sceneManager.StartRoundTitleShadow.color = new(sc.r,sc.g,sc.b,sc.a);
            print(c.a);
            if(c.a >= 0.5f && !indexIsShowing) StartCoroutine(StartRoundIndexAnimationCoroutine());

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }


    IEnumerator StartRoundIndexAnimationCoroutine()
    {
        indexIsShowing=true;
        while (sceneManager.StartRoundIndex.color.a <= 1)
        {
            var c = sceneManager.StartRoundIndex.color;
            var s = sceneManager.StartRoundIndexShadow.color;
            sceneManager.StartRoundIndex.color = new(c.r, c.g, c.b, c.a + 0.04f);
            sceneManager.StartRoundIndexShadow.color = new(s.r, s.g, s.b, c.a);

            if (c.a >= 0.5f && !handsIsShowing) StartCoroutine(StartRoundMoreInfoAnimationCoroutine());

            yield return new WaitForSecondsRealtime(0.01f);
        }
        indexIsShowing = false;

        
        yield return new WaitForSecondsRealtime(1f);

    }
    IEnumerator StartRoundMoreInfoAnimationCoroutine()
    {
        handsIsShowing = true;
        while (sceneManager.StartRoundHand.color.a <= 1)
        {
            var c = sceneManager.StartRoundHand.color;
            var s = sceneManager.StartRoundHandShadow.color;
            sceneManager.StartRoundHand.color = new(c.r, c.g, c.b, c.a + 0.04f);
            sceneManager.StartRoundHandShadow.color = new(s.r, s.g, s.b, c.a);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        handsIsShowing = false;

        yield return new WaitForSecondsRealtime(1f);
        sceneManager.ShowPlayerListCommon.gameObject.SetActive(RoundIndex>0);
        sceneManager.StartRoundCanvas.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }


}

[System.Serializable]
public class Player
{
    public string Name;
    public List<int> Scores;
    public List<int> TotalScores()
    { 
        var res = new List<int>();

        for(int i = 0; i<Scores.Count;i++)
        {
            var x = 0;
            for (int j = 0; j <= i ; j++)
            {
                x += Scores[j];
            }
            res.Add(x);
        }
        return res;
    }
    public List<int> Predictions;
    public List<int> Wins;

    public Player(string name)
    { 
        Name = name;
        Scores = new List<int>();
        Predictions = new List<int>();
        Wins = new List<int>();
    }

    public void AddPrediction(int predict) => Predictions.Add(predict);
    public void AddWins(int predict) => Wins.Add(predict);
    public int GetScoreInRound(int round) 
    {
        if (Wins[round] == Predictions[round]) return 10 + Wins[round] * 5;
        return - 5 * Mathf.Abs(Wins[round] - Predictions[round]);
    }
    
    public void AddScore() => Scores.Add(GetScoreInRound(Scores.Count));

    public int GetTotalScore()
    {
        var res = 0;

        foreach (var score in Scores)
        { 
            res+= score;
        }

        return res;
    }

}

public enum CardType
{
    ES52=50 ,
    ES48=48
}



//POPU PUTO