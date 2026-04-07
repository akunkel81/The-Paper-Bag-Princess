using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

    public Animator playerAnimator;

    [Header("Scene References")]
    public Unit playerUnit;
    public Unit enemyUnit;

    public TextMeshProUGUI dialogueText;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    [Header("Battle State")]
    public BattleState state;

    public bool skipEnemyTurn = false;
    public bool enemyHalfDamageNextTurn = false;

    [Header("Animator Flags")]
    public bool IsConfuse = false;
    public bool IsDistract = false;
    public bool IsCalculate = false;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerAnimator = playerUnit.GetComponent<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogError("BattleSystem: Player Animator not found on playerUnit object.");
}
        
        if (playerUnit == null || enemyUnit == null)
        {
            Debug.LogError("BattleSystem: Assign playerUnit and enemyUnit in the Inspector (scene objects).");
            yield break;
        }

        dialogueText.text = enemyUnit.unitName + " prepares to stop you!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    private int RollPlayerDamage()
    {
        return Random.Range(0, 5); // 0 to 4
    }

    private int RollEnemyDamage()
    {
        return Random.Range(0, 6); // 2 to 5
    }

    IEnumerator PlayerConfuse()
    {
        if (playerAnimator != null) playerAnimator.SetBool("IsConfuse", true);

        dialogueText.text = "You used Confuse on the " + enemyUnit.unitName + "!";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "Rolling for damage...";
        yield return new WaitForSeconds(2f);

        int rolledDamage = RollPlayerDamage();
        bool isEnemyDead = false;

        if (rolledDamage == 0)
        {
            dialogueText.text = "Your Confuse attack missed!";
        }
        else
        {
            isEnemyDead = enemyUnit.TakeDamage(rolledDamage);
            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = "You confused the " + enemyUnit.unitName + " and dealt " + rolledDamage + " damage!";
        }

        yield return new WaitForSeconds(2f);

        if (playerAnimator != null) playerAnimator.SetBool("IsConfuse", false);

        if (isEnemyDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerCalculate()
    {
        if (playerAnimator != null) playerAnimator.SetBool("IsCalculate", true);

        enemyHalfDamageNextTurn = true;

        dialogueText.text = "You used Calculate on the " + enemyUnit.unitName + "!";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "Rolling for damage...";
        yield return new WaitForSeconds(2f);

        int rolledDamage = RollPlayerDamage();
        bool isEnemyDead = false;

        if (rolledDamage == 0)
        {
            dialogueText.text = "Your Calculate attack missed! The enemy's next attack is still weakened.";
        }
        else
        {
            isEnemyDead = enemyUnit.TakeDamage(rolledDamage);
            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = "You calculated the move and dealt " + rolledDamage + " damage! The enemy's next attack is weakened.";
        }

        yield return new WaitForSeconds(2f);

        if (playerAnimator != null) playerAnimator.SetBool("IsCalculate", false);

        if (isEnemyDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerDistract()
    {
        if (playerAnimator != null) playerAnimator.SetBool("IsDistract", true);

        dialogueText.text = "You attempt to distract the " + enemyUnit.unitName + "!";
        yield return new WaitForSeconds(1f);

        dialogueText.text = "Rolling for damage...";
        yield return new WaitForSeconds(1f);

        int rolledDamage = RollPlayerDamage();
        bool isEnemyDead = false;

        // 50% chance to fail distraction
        bool distractionFails = Random.value < 0.5f;

        if (!distractionFails)
        {
            skipEnemyTurn = true;
        }

        if (rolledDamage == 0)
        {
            if (distractionFails)
                dialogueText.text = "Your attack missed and the distraction failed!";
            else
                dialogueText.text = "Your attack missed, but the enemy is distracted and will miss their next turn!";
        }
        else
        {
            isEnemyDead = enemyUnit.TakeDamage(rolledDamage);
            enemyHUD.SetHP(enemyUnit.currentHP);

            if (distractionFails)
                dialogueText.text = "You dealt " + rolledDamage + " damage, but failed to distract the enemy!";
            else
                dialogueText.text = "You dealt " + rolledDamage + " damage and distracted the enemy! They will miss their next attack.";
        }

        yield return new WaitForSeconds(2f);

        if (playerAnimator != null) playerAnimator.SetBool("IsDistract", false);

        if (isEnemyDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator EnemyTurn()
    {
        if (skipEnemyTurn)
        {
            skipEnemyTurn = false;

            dialogueText.text = enemyUnit.unitName + " misses!";
            yield return new WaitForSeconds(2f);

            state = BattleState.PLAYERTURN;
            PlayerTurn();
            yield break;
        }

        dialogueText.text = enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "Rolling for damage...";
        yield return new WaitForSeconds(2f);

        int damageToDeal = RollEnemyDamage();

        if (enemyHalfDamageNextTurn)
        {
            damageToDeal = Mathf.CeilToInt(damageToDeal * 0.5f);
            enemyHalfDamageNextTurn = false;
        }

        bool isPlayerDead = playerUnit.TakeDamage(damageToDeal);
        playerHUD.SetHP(playerUnit.currentHP);

        dialogueText.text = enemyUnit.unitName + " dealt " + damageToDeal + " damage!";
        yield return new WaitForSeconds(2f);

        if (isPlayerDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

void EndBattle()
{
    if (state == BattleState.WON)
    {
        dialogueText.text = "You defeated the " + enemyUnit.unitName + "!"; 
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextScene();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null. Add GameManager to the scene.");
        }
    }

    if (state == BattleState.LOST)
    {
        dialogueText.text = "You were defeated.";

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null. Add GameManager to the scene.");
        }
    }
}

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    public void OnConfuseButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerConfuse());
    }

    public void OnDistractButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerDistract());
    }

    public void OnCalculateButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerCalculate());
    }
}