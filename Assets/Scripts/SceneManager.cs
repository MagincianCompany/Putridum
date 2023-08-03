using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

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

    [Header("PlayerList")]
    public GameObject PlayerListCanvas;
    public GameObject Content;
    public Object playerLabel;

    [Header("Other")]
    public TextMeshProUGUI version;
    public TextMeshProUGUI info_resolution;
    public GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        mainMenuCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void moreInfoCanvas()
    {
        OnChangeMenu();

        MainCanvas.SetActive(false);
        MoreInfoCanvas.SetActive(true);
        SelectingAmountCanvas.SetActive(false);
        SelectingPlayerCanvas.SetActive(false);
        ChargingCanvas.SetActive(false);
        PlayerListCanvas.SetActive(false);

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


    public void OnChangeMenu()
    {
        version.text = Application.version;
        info_resolution.text = $"{Screen.width}x{Screen.height}";
    }
}
