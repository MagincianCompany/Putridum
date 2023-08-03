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

    public void SetRounds()
    {
        sceneManager.SelectingMin= 1;
        sceneManager.SelectingMax = Mathf.FloorToInt((int)cardType / PlayerList.Count) ;
        sceneManager.SelectingAmount = sceneManager.SelectingMax;
        sceneManager.SelectingTitle = "Rounds";
        sceneManager.SelectingAmountButton.onClick.RemoveAllListeners();
        sceneManager.SelectingAmountButton.onClick.AddListener(
            () =>{
                maxRounds = sceneManager.SelectingAmount;
                Round();
            }
            
            );
        sceneManager.selectingAmountCanvas();
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
    IEnumerator RoundCoroutine()
    {
        int w = 0;
        foreach (var player in PlayerList) 
        {
            sceneManager.SelectingMin = 0;
            sceneManager.SelectingAmount = 0;
            sceneManager.SelectingMax = HandCards;
            sceneManager.SelectingTitle = $"Prediction {player.Name}";

            if (player == PlayerList[PlayerList.Count - 1])
            {
                sceneManager.SelectingForbiddenList = new List<int> { HandCards - w };

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
            sceneManager.SelectingAmountButton.onClick.RemoveAllListeners();
            sceneManager.SelectingAmountButton.onClick.AddListener(()=>selectingButtonIsClicked=true);

            sceneManager.selectingAmountCanvas();
            yield return new WaitUntil(()=> selectingButtonIsClicked);

            selectingButtonIsClicked = false;
            var x = int.Parse(sceneManager.SelectingAmountText.text);
            w += x;
            player.AddPrediction(x);

            Debug.Log("Predicción agregada");
        }
        sceneManager.chargingCanvas();
        sceneManager.ChargingButton.onClick.RemoveAllListeners();
        sceneManager.ChargingButton.onClick.AddListener(()=>StartCoroutine(EndRoundCoroutine()));
    }
    IEnumerator EndRoundCoroutine()
    {
        foreach (var player in PlayerList)
        {
            sceneManager.SelectingMin = 0;
            sceneManager.SelectingAmount = 0;
            sceneManager.SelectingMax = HandCards;
            sceneManager.SelectingTitle = $"Wins {player.Name}" ;
            sceneManager.SelectingForbiddenList=new List<int>();
            sceneManager.SelectingAmountButton.onClick.RemoveAllListeners();
            sceneManager.SelectingAmountButton.onClick.AddListener(() => selectingButtonIsClicked = true);

            sceneManager.selectingAmountCanvas();
            yield return new WaitUntil(() => selectingButtonIsClicked);

            selectingButtonIsClicked = false;
            player.AddWins(int.Parse(sceneManager.SelectingAmountText.text));
            Debug.Log("Predicción agregada");
        }

        //TODO cambiar para que salgan las puntuaciones
        sceneManager.setPlayersLabels();
        sceneManager.playerListCanvas();
        RoundIndex++;
        HandCards+= multyplier;

    }
    

    IEnumerator StartRoundTitleAnimationCoroutine()
    {
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
        sceneManager.StartRoundCanvas.SetActive(false);

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

        yield return new WaitForSecondsRealtime(1.5f);
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
    ES52=52 ,
    ES48=48
}