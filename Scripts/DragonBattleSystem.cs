using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public enum DragonBattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public enum DragonAttackType { Bite, TailSlam, FireBreath }

public class DragonBattleSystem : MonoBehaviour
{
    public Animator playerAnimator;

    [Header("Scene References")]
    public Unit playerUnit;
    public Unit enemyUnit;

    public TextMeshProUGUI dialogueText;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    [Header("Battle State")]
    public DragonBattleState state;

    [Header("Player Defense Effects")]
    public bool reduceNextDamage = false;
    public bool skipNextDragonAttack = false;
    public bool reflectBonusDamage = false;
    public bool enemyHalfDamageNextTurn = false;

    private DragonAttackType nextDragonAttack;
    public enum PlayerMove { None, Confuse, Distract, Calculate }
    private PlayerMove lastPlayerMove = PlayerMove.None;
    

    void Start()
    {
        state = DragonBattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        if (playerUnit == null || enemyUnit == null)
        {
            Debug.LogError("DragonBattleSystem: Assign playerUnit and enemyUnit in the Inspector.");
            yield break;
        }

        playerAnimator = playerUnit.GetComponent<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogWarning("DragonBattleSystem: No Animator found on playerUnit.");
        }

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        dialogueText.text = enemyUnit.unitName + " descends before you!";
        yield return new WaitForSeconds(2f);

        PickNextDragonAttack();
        dialogueText.text = "The dragon watches you carefully...";
        yield return new WaitForSeconds(2f);

        state = DragonBattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PickNextDragonAttack()
    {
        nextDragonAttack = (DragonAttackType)Random.Range(0, 3);
    }

    string GetHintForNextAttack()
    {
        switch (nextDragonAttack)
        {
            case DragonAttackType.FireBreath:
                return "You see it take a big inhale...";
            case DragonAttackType.TailSlam:
                return "Its tail twitches and coils behind it...";
            case DragonAttackType.Bite:
                return "It lowers its head and bares its teeth...";
            default:
                return "It stares at you intensely...";
        }
    }

    int RollPlayerDamage()
    {
        return Random.Range(1, 6); // 1 to 5
    }

    int RollDragonDamage(DragonAttackType attackType)
    {
        switch (attackType)
        {
            case DragonAttackType.Bite:
                return Random.Range(2, 5); // 2 to 4
            case DragonAttackType.TailSlam:
                return Random.Range(3, 6); // 3 to 5
            case DragonAttackType.FireBreath:
                return Random.Range(4, 7); // 4 to 6
            default:
                return 2;
        }
    }

    IEnumerator PlayerConfuse()
    {
        if (playerAnimator != null) playerAnimator.SetBool("IsConfuse", true);

        dialogueText.text = "You attempt to confuse the dragon!";
        yield return new WaitForSeconds(1.5f);

        int rolledDamage = RollPlayerDamage();
        bool isEnemyDead = false;

        if (rolledDamage > 0)
        {
            isEnemyDead = enemyUnit.TakeDamage(rolledDamage);
            enemyHUD.SetHP(enemyUnit.currentHP);
        }

        // Best against Tail Slam
        if (nextDragonAttack == DragonAttackType.TailSlam)
        {
            lastPlayerMove = PlayerMove.Confuse;
            reflectBonusDamage = true;
            dialogueText.text = "You used the correct strategy and dealt " + rolledDamage + " damage to throw off the dragon's balance.";
        }
        else
        {
            dialogueText.text = "You dealt " + rolledDamage + " damage, but the dragon does not seem fully disoriented.";
        }

        yield return new WaitForSeconds(3f);

        if (playerAnimator != null) playerAnimator.SetBool("IsConfuse", false);

        if (isEnemyDead)
        {
            state = DragonBattleState.WON;
            EndBattle();
        }
        else
        {
            state = DragonBattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerDistract()
    {
        if (playerAnimator != null) playerAnimator.SetBool("IsDistract", true);

        dialogueText.text = "You try to distract the dragon!";
        yield return new WaitForSeconds(3f);

        int rolledDamage = RollPlayerDamage();
        bool isEnemyDead = false;

        if (rolledDamage > 0)
        {
            isEnemyDead = enemyUnit.TakeDamage(rolledDamage);
            enemyHUD.SetHP(enemyUnit.currentHP);
        }

        // Best against Bite
        if (nextDragonAttack == DragonAttackType.Bite)
        {
            reduceNextDamage = true;
            lastPlayerMove = PlayerMove.Distract;
            dialogueText.text = "You used the correct strategy and dealt " + rolledDamage + " damage and pulled the dragon off target! Its next attack will be weakened.";
        }
        else
        {
            dialogueText.text = "You dealt " + rolledDamage + " damage, but the dragon quickly regains focus.";
        }

        yield return new WaitForSeconds(3f);

        if (playerAnimator != null) playerAnimator.SetBool("IsDistract", false);

        if (isEnemyDead)
        {
            state = DragonBattleState.WON;
            EndBattle();
        }
        else
        {
            state = DragonBattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerCalculate()
    {
        if (playerAnimator != null) playerAnimator.SetBool("IsCalculate", true);

        dialogueText.text = "You study the dragon's movements carefully...";
        yield return new WaitForSeconds(3f);

        int rolledDamage = RollPlayerDamage();
        bool isEnemyDead = false;

        if (rolledDamage > 0)
        {
            isEnemyDead = enemyUnit.TakeDamage(rolledDamage);
            enemyHUD.SetHP(enemyUnit.currentHP);
        }

        // Best against Fire Breath
        if (nextDragonAttack == DragonAttackType.FireBreath)
        {
            enemyHalfDamageNextTurn = true;
            lastPlayerMove = PlayerMove.Calculate;
            dialogueText.text = "You used the correct strategy and dealt " + rolledDamage + " damage!Its next attack will be turned against it.";
        }
        else
        {
            dialogueText.text = "You dealt " + rolledDamage + " damage, but learned little from the dragon's stance.";
        }

        yield return new WaitForSeconds(3f);

        if (playerAnimator != null) playerAnimator.SetBool("IsCalculate", false);

        if (isEnemyDead)
        {
            state = DragonBattleState.WON;
            EndBattle();
        }
        else
        {
            state = DragonBattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        DragonAttackType currentAttack = nextDragonAttack;
        if (skipNextDragonAttack)
        {
            skipNextDragonAttack = false;

            dialogueText.text = "The dragon tries to attack, but loses its balance and misses completely!";
            yield return new WaitForSeconds(2f);

            PickNextDragonAttack();
            dialogueText.text = GetHintForNextAttack();
            yield return new WaitForSeconds(2f);

            state = DragonBattleState.PLAYERTURN;
            PlayerTurn();
            yield break;
        }

        int damageToDeal = RollDragonDamage(currentAttack);
        string attackName = "";

        switch (currentAttack)
        {
            case DragonAttackType.Bite:
                attackName = "Bite";
                break;
            case DragonAttackType.TailSlam:
                attackName = "Tail Slam";
                break;
            case DragonAttackType.FireBreath:
                attackName = "Fire Breath";
                break;
        }
        
        bool dodged = false;

        if (currentAttack == DragonAttackType.TailSlam && lastPlayerMove == PlayerMove.Confuse)
            dodged = true;

        if (currentAttack == DragonAttackType.Bite && lastPlayerMove == PlayerMove.Distract)
            dodged = true;

        if (currentAttack == DragonAttackType.FireBreath && lastPlayerMove == PlayerMove.Calculate)
            dodged = true;

        if (dodged)
        {
            dialogueText.text = "You predicted the dragon's move and dodged the attack completely!";
            damageToDeal = 0;
        }

        dialogueText.text = "The dragon uses " + attackName + "!";
        yield return new WaitForSeconds(2f);

        if (reduceNextDamage)
        {
            damageToDeal = Mathf.Max(1, damageToDeal - 2);
            reduceNextDamage = false;
        }

        if (enemyHalfDamageNextTurn)
        {
            damageToDeal = Mathf.Max(1, damageToDeal / 2);
            enemyHalfDamageNextTurn = false;
        }

        bool isPlayerDead = playerUnit.TakeDamage(damageToDeal);
        playerHUD.SetHP(playerUnit.currentHP);

        dialogueText.text = "The dragon hits you for " + damageToDeal + " damage!";
        yield return new WaitForSeconds(2f);

        if (reflectBonusDamage && currentAttack == DragonAttackType.FireBreath)
        {
            reflectBonusDamage = false;

            bool isEnemyDead = enemyUnit.TakeDamage(3);
            enemyHUD.SetHP(enemyUnit.currentHP);

            dialogueText.text = "You anticipated the flames and turned the attack against the dragon for 3 damage!";
            yield return new WaitForSeconds(2f);

            if (isEnemyDead)
            {
                state = DragonBattleState.WON;
                EndBattle();
                yield break;
            }
        }
        else
        {
            reflectBonusDamage = false;
        }

        if (isPlayerDead)
        {
            state = DragonBattleState.LOST;
            EndBattle();
            yield break;
        }

        PickNextDragonAttack();
        dialogueText.text = GetHintForNextAttack();
        yield return new WaitForSeconds(2f);

        state = DragonBattleState.PLAYERTURN;
        PlayerTurn();
    }

    void EndBattle()
    {
        if (state == DragonBattleState.WON)
        {
            dialogueText.text = "You outsmarted the " + enemyUnit.unitName + "!";
            
            SceneManager.LoadScene("Scene4");
        }

        if (state == DragonBattleState.LOST)
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
        dialogueText.text = "Choose your move carefully...";
    }

    public void OnConfuseButton()
    {
        if (state != DragonBattleState.PLAYERTURN) return;
        StartCoroutine(PlayerConfuse());
    }

    public void OnDistractButton()
    {
        if (state != DragonBattleState.PLAYERTURN) return;
        StartCoroutine(PlayerDistract());
    }

    public void OnCalculateButton()
    {
        if (state != DragonBattleState.PLAYERTURN) return;
        StartCoroutine(PlayerCalculate());
    }
}