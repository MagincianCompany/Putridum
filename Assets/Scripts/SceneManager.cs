using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class SceneManager : MonoBehaviour
{
    public GameObject MainCanvas;
    public GameObject MoreInfoCanvas;
    [Header("Selecting Amount")]
    public GameObject SelectingAmountCanvas;
    public TextMeshProUGUI SelectingAmountTitle;
    public TMP_InputField SelectingAmountText;
    public TextMeshProUGUI SelectingAmountTextShadow;
    public TextMeshProUGUI SelectingAmountInformation;
    public string SelectingTitle;
    public int SelectingMin = 0;
    public int SelectingAmount = 0;
    public int SelectingMax = 1000;
    public List<int> SelectingForbiddenList = new List<int>();
    public GameObject SelectingAmountMinus;
    public GameObject SelectingAmountPlus;
    public Button SelectingAmountButton;
    public Button SelectingAmountBackButton;
    public TextMeshProUGUI SelectingAmountButtonText;

    [Header("Selecting Player")]
    public GameObject SelectingPlayerCanvas;
    public TMP_InputField SelectingPlayerNameField;
    public GameObject SelectingPlayerMinus;
    public GameObject SelectingPlayerRemove;
    public GameObject SelectingPlayerPlus;
    public GameObject SelectingPlayerMore;
    public Button SelectingPlayerButton;
    public int PlayerIndex;

    [Header("StartRound")]
    public GameObject StartRoundCanvas;
    public TextMeshProUGUI StartRoundTitle;
    public TextMeshProUGUI StartRoundIndex;
    public TextMeshProUGUI StartRoundTitleShadow;
    public TextMeshProUGUI StartRoundIndexShadow;
    public TextMeshProUGUI StartRoundHand;
    public TextMeshProUGUI StartRoundHandShadow;

    [Header("Charging")]
    public GameObject ChargingCanvas;
    public TextMeshProUGUI ChargingTitle;
    public TextMeshProUGUI ChargingTitleShadow;
    public Button ChargingButton;
    public Button ChargingBackButton;

    [Header("PlayerList")]
    public GameObject PlayerListCanvas;
    public GameObject Content;
    public Button Continue;
    public Button Reload;
    public Object playerLabel;

    [Header("Graph")]
    public GameObject GraphCanvas;
    public GameObject conteinerGraph;

    public Button ShowGraphButton;
    public Button HideGraphButton;
    public Object line;
    public Object point;
    public Object conteiner;
    public int subdivision=2;
    public GraphType graphType;

    [Header("Common")]
    public Button ShowPlayerListCommon;
    public Button HidePlayerListCommon;


    [Header("Other")]
    public TextMeshProUGUI version;
    public TextMeshProUGUI info_resolution;
    public GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        mainMenuCanvas();
    }

    
    public void setGrid()
    {
        // Al modificar el tamaño de las lineas se debe modificar siempre el eje X, aunque luego tenga que rotarse
        // esto es para mantener el eje centrado

        int xh = gameManager.PlayerList.Max(x => x.TotalScores().Max());
        int mh = gameManager.PlayerList.Min(x => x.TotalScores().Min());
        mh = mh>=0 ? 0 : mh;
        print(mh);
        var x = conteinerGraph.GetComponent<RectTransform>().sizeDelta.x;
        var y = conteinerGraph.GetComponent<RectTransform>().sizeDelta.y;
        int w = gameManager.RoundIndex;
        if (gameManager.RoundIndex>0)
        {
            List<GameObject> conteiners = new List<GameObject>();
            foreach (var _ in gameManager.PlayerList)
            {
                conteiners.Add((GameObject)Instantiate(conteiner,conteinerGraph.transform));
            }
            for (int p = 0; p< gameManager.PlayerList.Count;p++)
            {
                var player = gameManager.PlayerList[p];
                print(p);
                var c = Color.HSVToRGB((float)p/ (float)gameManager.PlayerList.Count,1,1);
                
                float sc = 0;
                for (int i = 0; i < player.Scores.Count;i++)
                {
                    var score = player.Scores[i];
                    sc += score;

                    var pt = (GameObject)Instantiate(point, Vector3.zero,
                        Quaternion.identity, conteinerGraph.transform);

                    pt.transform.localPosition = new Vector3(i * x / (w-1),
                        (sc - mh) * y / (xh - mh), 0);
                    
                    if(graphType==GraphType.THERMOMETER)
                    pt.GetComponent<RectTransform>().sizeDelta = 
                        34f * (gameManager.PlayerList.Count - p) / gameManager.PlayerList.Count 
                        * Vector2.one;

                    pt.GetComponent<Image>().color = c;

                    if (i + 1 < player.Scores.Count)
                    {
                        if (graphType == GraphType.SLICEDBAR)
                        {
                            var yscore = player.Scores[i + 1] * y / (xh - mh);
                            var xscore = x / (w - 1);
                            var a = Mathf.Atan2(yscore, xscore)
                                * 180 / Mathf.PI;

                            var Rfull = Mathf.Sqrt(yscore * yscore + xscore * xscore);
                            var origin = new Vector3(
                                    i * x / (w - 1),
                                    (sc - mh) * y / (xh - mh), 0);
                            var N = Mathf.Pow(gameManager.PlayerList.Count, subdivision);
                            for (int z = 0; z < N; z++)
                            {
                                var ln = (GameObject)Instantiate(line, Vector3.zero,
                                Quaternion.identity, conteiners[(p + z) % gameManager.PlayerList.Count].transform);

                                ln.transform.localPosition = origin + new Vector3(
                                    xscore, yscore) * z / N;
                                ln.GetComponent<RectTransform>().sizeDelta = new Vector2(
                                Rfull / N, 10);

                                ln.transform.rotation = Quaternion.Euler(0, 0, a);
                                ln.GetComponent<Image>().color = c;

                            }
                        }
                        else
                        {
                            var ln = (GameObject)Instantiate(line, Vector3.zero,
                                Quaternion.identity, conteinerGraph.transform);

                            ln.transform.localPosition = new Vector3(i * x / (w - 1),
                                (sc - mh) * y / (xh - mh), 0);
                            var yscore = player.Scores[i + 1] * y / (xh - mh);
                            var xscore = x / (w - 1);
                            var a = Mathf.Atan2(yscore, xscore)
                                * 180 / Mathf.PI;

                            ln.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Sqrt(
                                yscore * yscore + xscore * xscore),
                                graphType == GraphType.THERMOMETER ? 17f * (gameManager.PlayerList.Count - p) / gameManager.PlayerList.Count
                                : 10);

                            ln.transform.rotation = Quaternion.Euler(0, 0, a);
                            ln.GetComponent<Image>().color = c;
                        }

            
                    }

                }
            }
        }

    }

    public void setPlayersLabels()
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Destroy(Content.transform.GetChild(i).gameObject);
        }

        foreach (var p in gameManager.PlayerList)
        {
            p.AddScore();
        }
        foreach (var p in gameManager.PlayerListByScore)
        {
            var go = (GameObject)Instantiate(playerLabel, Content.transform.position, Quaternion.identity,Content.transform);
            var label = go.GetComponent<PlayerLabel>();
            label.Name.text = p.Name;

            var ind = p.GetScoreInRound(gameManager.RoundIndex) >= 0 ? "+" : "";

            label.RoundScore.color = p.GetScoreInRound(gameManager.RoundIndex) >= 0 ? PlayerLabel.Positive : PlayerLabel.Negative;
            label.RoundScore.text = ind + p.GetScoreInRound(gameManager.RoundIndex).ToString();

            label.TotalScore.text = p.GetTotalScore().ToString();
        }
    }

    #region CanvasActivation

    public void moreInfoCanvas()
    {
        OnChangeMenu();

        MainCanvas.SetActive(false);
        MoreInfoCanvas.SetActive(true);
        SelectingAmountCanvas.SetActive(false);
        SelectingPlayerCanvas.SetActive(false);
        ChargingCanvas.SetActive(false);
        PlayerListCanvas.SetActive(false);
        GraphCanvas.SetActive(false);
        ShowPlayerListCommon.gameObject.SetActive(false);
        
    }
    public void mainMenuCanvas()
    {
        OnChangeMenu();

        MainCanvas.SetActive(true);
        MoreInfoCanvas.SetActive(false);
        SelectingAmountCanvas.SetActive(false);
        SelectingPlayerCanvas.SetActive(false);
        ChargingCanvas.SetActive(false);
        PlayerListCanvas.SetActive(false);
        GraphCanvas.SetActive(false);
        ShowPlayerListCommon.gameObject.SetActive(false);
    }
    public void chargingCanvas()
    {
        OnChangeMenu();

        MainCanvas.SetActive(false);
        MoreInfoCanvas.SetActive(false);
        SelectingAmountCanvas.SetActive(false);
        SelectingPlayerCanvas.SetActive(false);
        ChargingCanvas.SetActive(true); 
        PlayerListCanvas.SetActive(false);
        GraphCanvas.SetActive(false);
        ShowPlayerListCommon.gameObject.SetActive(gameManager.RoundIndex > 0);
        ChargingTitleShadow.text = ChargingTitle.text;
    }
    public void selectingAmountCanvas()
    {
        OnChangeMenu();
        
        MainCanvas.SetActive(false);
        MoreInfoCanvas.SetActive(false);
        SelectingAmountCanvas.SetActive(true);
        SelectingPlayerCanvas.SetActive(false);
        SelectingAmountTitle.text = SelectingTitle;
        
        SelectingAmountTextShadow.text = SelectingTitle;
        ChargingCanvas.SetActive(false);
        PlayerListCanvas.SetActive(false);
        GraphCanvas.SetActive(false);
        print($"sh: {gameManager.RoundIndex > 0}");
        ShowPlayerListCommon.gameObject.SetActive(gameManager.RoundIndex > 0);
        selectingAmountChange();
    }
    public void selectingPlayerCanvas()
    {
        OnChangeMenu();

        MainCanvas.SetActive(false);
        MoreInfoCanvas.SetActive(false);
        SelectingAmountCanvas.SetActive(false);
        SelectingPlayerCanvas.SetActive(true);
        ChargingCanvas.SetActive(false);
        PlayerListCanvas.SetActive(false); 
        ShowPlayerListCommon.gameObject.SetActive(false);

        PlayerIndex = 0;

        SelectingPlayerNameField.text = "";

        if(gameManager.PlayerList.Count<=0) gameManager.PlayerList.Add(new(""));

        setPlayerName();

        selectingPlayerChange();
    }
    public void playerListCanvas()
    {
        OnChangeMenu();

        MainCanvas.SetActive(false);
        MoreInfoCanvas.SetActive(false);
        SelectingAmountCanvas.SetActive(false);
        SelectingAmountMinus.SetActive(false);
        SelectingAmountPlus.SetActive(false);
        SelectingPlayerCanvas.SetActive(true);
        ChargingCanvas.SetActive(false);
        PlayerListCanvas.SetActive(true);
        GraphCanvas.SetActive(false);
        ShowPlayerListCommon.gameObject.SetActive(false);
    }

    #endregion
    public void ShowPlayerListSuper()
    {
        PlayerListCanvas.SetActive(true);
        ShowPlayerListCommon.gameObject.SetActive(false);
        HidePlayerListCommon.gameObject.SetActive(true);
        Continue.gameObject.SetActive(false);
        Reload.gameObject.SetActive(false);

    }
    public void HidePlayerListSuper()
    {
        PlayerListCanvas.SetActive(false);
        ShowPlayerListCommon.gameObject.SetActive(true);
        HidePlayerListCommon.gameObject.SetActive(false);
        Continue.gameObject.SetActive(true);
        Reload.gameObject.SetActive(true);
    }

    public void selectingPlayerAdd()
    {
        if (gameManager.PlayerList[gameManager.PlayerList.Count - 1] != null 
            && gameManager.PlayerList[gameManager.PlayerList.Count - 1].Name != "")
        {
            PlayerIndex++;
            gameManager.PlayerList.Add(new(""));
            DesactivateButtonPlayerSelect();
        }
        selectingPlayerChange();
    }
    public void selectingPlayerPlus()
    {
        PlayerIndex++;
        selectingPlayerChange();
    }

    public void selectingPlayerMinus()
    {
        PlayerIndex--;
        selectingPlayerChange();
    }
    public void selectingPlayerRemove()
    {
        gameManager.PlayerList.RemoveAt(PlayerIndex);
        if (PlayerIndex >= gameManager.PlayerList.Count) PlayerIndex--;
        selectingPlayerChange();
    }
    public void selectingPlayerChange()
    {
        SelectingPlayerNameField.text = gameManager.PlayerList[PlayerIndex] != null ? gameManager.PlayerList[PlayerIndex].Name : "";

        SelectingPlayerMore.SetActive(PlayerIndex >= gameManager.PlayerList.Count-1);
        SelectingPlayerPlus.SetActive(!(PlayerIndex >= gameManager.PlayerList.Count-1));

        SelectingPlayerMinus.SetActive(PlayerIndex > 0);
        SelectingPlayerRemove.SetActive(PlayerIndex > 0);
    }

    public void selectingAmountMinus()
    {
       
        var y = SelectingAmount;
        SelectingAmount--;
        while (SelectingForbiddenList.Contains(SelectingAmount)) 
        {
            SelectingAmount--;
        }
        if (SelectingAmount < SelectingMin) SelectingAmount = y;


        selectingAmountChange();
    }
    public void selectingAmountPlus()
    {
        
        var y = SelectingAmount;

        SelectingAmount++;

        while (SelectingForbiddenList.Contains(SelectingAmount))
        {
            SelectingAmount++;
        }
        if (SelectingAmount > SelectingMax) SelectingAmount = y;

        selectingAmountChange();
    }
    public void selectingAmountChange()
    {
        SelectingAmountText.text = SelectingAmount.ToString();
        SelectingAmountPlus.SetActive(SelectingAmount != SelectingMax);
        SelectingAmountMinus.SetActive(SelectingAmount != SelectingMin);
    }

    public void setSelectingAmountByInputField() => SelectingAmount = int.Parse(SelectingAmountText.text);

    public void setPlayerName()
    {
        var list = gameManager.PlayerListNames;
        list.RemoveAt(PlayerIndex);

        print(SelectingPlayerNameField.text.Trim());
        if (SelectingPlayerNameField.text.Trim() != string.Empty 
            && !list.Contains(SelectingPlayerNameField.text))
        {
            ActivateButtonPlayerSelect();
        }
        else
        {
            DesactivateButtonPlayerSelect();
        }
    }
    public void ActivateButtonPlayerSelect()
    {
        gameManager.PlayerList[PlayerIndex] = new(SelectingPlayerNameField.text);
        SelectingPlayerButton.interactable = true;
        SelectingAmountButtonText.color = new Color(SelectingAmountButtonText.color.r,
            SelectingAmountButtonText.color.g,
            SelectingAmountButtonText.color.b,
            1);
    }
    public void DesactivateButtonPlayerSelect() 
    {

        SelectingPlayerButton.interactable = false;
        SelectingAmountButtonText.color = new Color(SelectingAmountButtonText.color.r,
            SelectingAmountButtonText.color.g,
            SelectingAmountButtonText.color.b,
            0.1568f);

        //esta línea va a haber que mejorarla por que si se quita, tenemos un problema, si se pone tenemos otro
        //gameManager.PlayerList[PlayerIndex] = new("");
    }

    public void ShowGraph()
    { 
        GraphCanvas.SetActive(true);
        HideGraphButton.gameObject.SetActive(true);
        ShowGraphButton.gameObject.SetActive(false);
        setGrid();
    }
    public void HideGraph()
    {
        GraphCanvas.SetActive(false);
        HideGraphButton.gameObject.SetActive(false);
        ShowGraphButton.gameObject.SetActive(true);

        for (int i = 0; i < conteinerGraph.transform.childCount; i++)
        {
            Destroy(conteinerGraph.transform.GetChild(i).gameObject);
        }
    }

    public void OnChangeMenu()
    {
        version.text = Application.version;
        info_resolution.text = $"{Screen.width}x{Screen.height}";
    }
}

public enum GraphType{
    BASIC,
    THERMOMETER,
    SLICEDBAR
}