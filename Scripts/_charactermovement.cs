using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float runMultiplier = 2f;

    public float jumpAmount = 2f;
    public float jumpCooldownSeconds = 0.5f;

    private float nextJumpTime;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isMoving = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.dialogueRunning)
        {
            ForceIdle();
            return;
        }

        MovePlayer();
        UpdateAnimation();
    }

    void MovePlayer()
    {
        Vector2 curPos = transform.position;

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= runMultiplier;

        isMoving = false;

        if (Input.GetKey(KeyCode.D))
        {
            curPos.x += Time.deltaTime * currentSpeed;
            isMoving = true;

            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
        }

        if (Input.GetKey(KeyCode.A))
        {
            curPos.x -= Time.deltaTime * currentSpeed;
            isMoving = true;

            if (spriteRenderer != null)
                spriteRenderer.flipX = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time >= nextJumpTime)
            {
                curPos.y += jumpAmount;
                nextJumpTime = Time.time + jumpCooldownSeconds;
            }
        }

        transform.position = curPos;
    }

    void UpdateAnimation()
    {
        if (animator == null)
            return;

        animator.SetBool("isWalking", isMoving);
    }

    public void ForceIdle()
    {
        isMoving = false;

        if (animator != null)
            animator.SetBool("isWalking", false);
    }
}