using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationName
{
    Idle,
    StartJump,
    Land,
    Hit,
    Destroy,
}

public enum PlayerState
{
    Idle,
    Attack,
    Move,
    Hit,
    Lose,
}

public class PlayerController : MonoBehaviour, ITakeDamage
{
    [SerializeField] Camera mainCam;
    [SerializeField] Transform model;
    [SerializeField] Transform attackRangeGfx;
    [SerializeField] Transform landPoint;
    [SerializeField] SpriteRenderer diceFace;
    [SerializeField] PlayerHeartHandle heartHandle;
    [SerializeField] float maxSize;
    [SerializeField] float attackTime;
    [SerializeField] float attackRange;
    [SerializeField] float rotateSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] Sprite[] sprites;
    [SerializeField] Collider2D boxCollider;
    private float attackRadius;
    private Animator anim;
    private Vector2 desPos;
    private Vector2 startPos;
    private InputData inputData;
    private Transform minBorder;
    private Transform maxBorder;
    private PlayerState currentPlayerState;
    private float moveTime;
    private float currentInvincibleTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        inputData = new();
        Land();
    }

    public void SetBorders(Transform minPoint, Transform maxPoint)
    {
        this.minBorder = minPoint;
        this.maxBorder = maxPoint;
    }

    private void OnChangeState(PlayerState newState)
    {
        if (currentPlayerState == newState)
            return;
        currentPlayerState = newState;

        switch (newState)
        {
            case PlayerState.Idle:
                PLayAnim(AnimationName.Idle);
                break;
            case PlayerState.Move:
            case PlayerState.Attack:
                break;
            case PlayerState.Hit:
                PlayHitAnim();
                break;
            case PlayerState.Lose:
                OnPlayerDestroy();
                break;
        }
    }

    private bool CheckState(PlayerState state)
    {
        return currentPlayerState == state;
    }

    void Update()
    {
        if (CheckState(PlayerState.Lose))
            return;

        GetInput();
        UpdateInvincibleTime();
        Attack();

        if (IsAttackState())
            return;

        Move();
        Rotate();
    }

    private void UpdateInvincibleTime()
    {
        if (currentInvincibleTime > 0)
            currentInvincibleTime -= Time.deltaTime;
    }

    private bool CheckInvincible()
    {
        return currentInvincibleTime > 0;
    }

    private void SetInvincible(float invincibleTime)
    {
        currentInvincibleTime = invincibleTime;
    }

    private void Attack()
    {
        bool isattack = IsAttackState();

        if (inputData.isattack && !isattack)
            StartAttack();

        if (!isattack)
            return;

        if (moveTime <= attackTime)
            Attacking();
        else
            Land();
    }

    private void StartAttack()
    {
        moveTime = 0f;
        OnChangeState(PlayerState.Attack);
        SetUpAttackPositions();
        ToggleBoxCollider(false);
        ToggleAttack(false);
        PLayAnim(AnimationName.StartJump, 0.1f);
    }

    private void SetUpAttackPositions()
    {
        startPos = transform.position;
        desPos = CalculateDesPosition(mainCam.ScreenToWorldPoint(Input.mousePosition), attackRadius);
        landPoint.position = desPos;
    }

    private void Land()
    {
        ToggleBoxCollider(true);
        CalculateNewRange();
        SetRadiusRange();
        ToggleAttack(true);
        CheckAttackRange();
        PLayAnim(AnimationName.Land, 0.1f);
        CameraController.Instance?.ShakeCamera();
        OnChangeState(PlayerState.Idle);
        SetInvincible(0.5f);
    }

    private void ToggleAttack(bool isActive)
    {
        ToggleAttackRange(isActive);
        ToggleLandPoint(!isActive);
    }

    private void CheckAttackRange()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRange);
        if (cols == null || cols.Length == 0)
            return;

        foreach (var col in cols)
        {
            if (col.gameObject == this.gameObject)
                continue;

            if (col.TryGetComponent<ITakeDamage>(out var enemy))
            {
                enemy.TakeDamage();
            }
        }
    }

    private void Attacking()
    {
        float t = moveTime / attackTime;
        transform.position = Vector2.Lerp(startPos, desPos, t);
        moveTime += Time.deltaTime;

        float s = Mathf.Sin(t * Mathf.PI);
        Vector3 desScale = Vector2.one * maxSize;
        model.localScale = Vector2.Lerp(Vector2.one, desScale, s);
    }

    private void SetRadiusRange()
    {
        attackRangeGfx.localScale = Vector2.one * (attackRadius * 2);
    }

    private void Rotate()
    {
        float angle = Mathf.Atan2(inputData.rotateVec.y, inputData.rotateVec.x) * Mathf.Rad2Deg - 90f;
        model.rotation = Quaternion.Lerp(model.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotateSpeed);
    }

    private void Move()
    {
        OnChangeState(PlayerState.Move);
        transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + inputData.moveVec * moveSpeed, Time.deltaTime);
        transform.position = ClampMovement(transform.position);
    }

    private void CalculateNewRange()
    {
        int dice = Random.Range(0, sprites.Length);
        diceFace.sprite = sprites[dice];
        attackRadius = dice + 1.5f;
    }

    private void GetInput()
    {
        inputData.GetInput(mainCam, transform);
    }

    protected void PLayAnim(AnimationName animName, float transitionTime = 0.1f)
    {
        anim?.CrossFade(animName.ToString(), transitionTime);
    }

    private Vector2 CalculateDesPosition(Vector2 mousePos, float radius)
    {
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        Ray ray = new Ray(transform.position, dir);
        Vector2 finalPoint = ClampMovement(ray.GetPoint(radius));
        return finalPoint;
    }

    private void ToggleLandPoint(bool isActive)
    {
        landPoint.SetParent(isActive ? null : transform);
        landPoint.gameObject.SetActive(isActive);
    }

    private void ToggleAttackRange(bool isActive)
    {
        attackRangeGfx.gameObject.SetActive(isActive);
    }

    public Vector2 ClampMovement(Vector2 playerPosition)
    {
        Vector2 newPosition = playerPosition;
        newPosition.x = Mathf.Clamp(newPosition.x, minBorder.position.x, maxBorder.position.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBorder.position.y, maxBorder.position.y);
        return newPosition;
    }

    public void TakeDamage()
    {
        if (CheckInvincible())
            return;
        OnChangeState(heartHandle.TakeDamage() ? PlayerState.Lose : PlayerState.Hit);
    }

    private void PlayHitAnim()
    {
        StartCoroutine(CorPlayHitAnim());
    }

    private IEnumerator CorPlayHitAnim()
    {
        if (CheckInvincible())
            yield break;
        CameraController.Instance.ShakeCamera();
        SetInvincible(2f);
        PLayAnim(AnimationName.Hit, 0f);
        yield return new WaitForSeconds(2f);
        OnChangeState(PlayerState.Idle);
    }

    private bool IsAttackState()
    {
        return CheckState(PlayerState.Attack);
    }

    public int GetMaxHeart()
    {
        return heartHandle.maxHeart;
    }

    private void OnPlayerDestroy()
    {
        ToggleAttackRange(false);
        ToggleBoxCollider(false);
        PLayAnim(AnimationName.Destroy);
    }

    private void ToggleBoxCollider(bool isActive)
    {
        boxCollider.enabled = isActive;
    }
}

public class InputData
{
    public Vector2 moveVec;
    public Vector3 rotateVec;
    public bool isattack;

    public void GetInput(Camera mainCam, Transform transform)
    {
        // movement
        isattack = Input.GetMouseButtonDown(0);
        moveVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        rotateVec = (mouseWorldPos - transform.position).normalized;
    }
}

public interface ITakeDamage
{
    public void TakeDamage();
}