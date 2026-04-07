using UnityEngine;

public enum BattlePortrait
{
    Enemy = 0,
    PlayerNeutral = 1,
    PlayerConfuse = 2,
    PlayerDistract = 3,
    PlayerCalculate = 4
}

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapper : MonoBehaviour
{
    [Tooltip("Index mapping: 0 Enemy, 1 PlayerNeutral, 2 PlayerConfuse, 3 PlayerDistract, 4 PlayerCalculate")]
    public Sprite[] portraits;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPortrait(BattlePortrait portrait)
    {
        int index = (int)portrait;

        if (portraits == null || portraits.Length == 0)
        {
            Debug.LogWarning("BattlePortraitController: No portraits assigned.");
            spriteRenderer.enabled = false;
            return;
        }

        if (index < 0 || index >= portraits.Length)
        {
            Debug.LogWarning("BattlePortraitController: Invalid portrait index " + index + ".");
            return;
        }

        Sprite s = portraits[index];
        spriteRenderer.sprite = s;
        spriteRenderer.enabled = (s != null);
    }
}