using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Slider hpSlider;
    public TextMeshProUGUI levelText;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        levelText.text = "Lvl " + unit.unitLevel;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}