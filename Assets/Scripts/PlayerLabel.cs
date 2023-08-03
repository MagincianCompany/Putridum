using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLabel : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI TotalScore;
    public TextMeshProUGUI RoundScore;
    public static Color Negative = new Color(0.9f,1,0.55f,1);
    public static Color Positive = new Color(0.740566f, 1, 0.8356655f, 1);
    public void setNegative() => RoundScore.color = Negative;
    public void setPositive() => RoundScore.color = Positive;
}
