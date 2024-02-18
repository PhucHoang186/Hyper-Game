
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AnimationName
{
    StartJump,
    Land,
}

public class PlayerController : MonoBehaviour, ITakeDamage
{
    [SerializeField] Camera mainCam;
    [SerializeField] Transform model;
    [SerializeField] Transform attackRangeGfx;
    [SerializeField] Transform landPoint;
    [SerializeField] SpriteRenderer diceFace;
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
    private bool isattacking;
    private float moveTime;
    private Transform minBorder;
    private Transform maxBorder;

    void Start()
    {
        anim = GetComponent<Animator>();
        inputData = new();
        SetRadiusRange();
        Land();
    }

    public void SetBorders(Transform minPoint, Transform maxPoint)
    {
        this.minBorder = minPoint;
        this.maxBorder = maxPoint;
    }

    void Update()
    {
        GetInput();
        Attack();
        Move();
        Rotate();
    }

    private void Attack()
    {
        if (inputData.isattack && !isattacking)
        {
            Startattack();
        }

        if (isattacking)
        {
            if (moveTime <= attackTime)
                Attacking();
            else
                Land();
        }
    }

    private void Startattack()
    {
        boxCollider.enabled = false;
        isattacking = true;
        moveTime = 0f;
        startPos = transform.position;
        desPos = CalculateDesPosition(mainCam.ScreenToWorldPoint(Input.mousePosition), attackRadius);
        landPoint.position = desPos;
        ToggleLandPoint(true);
        ToggleAttackRange(false);
        PLayAnim(AnimationName.StartJump, 0.1f);
    }

    private void Land()
    {
  
        CalculateNewRange();
        SetRadiusRange();
        ToggleLandPoint(false);
        ToggleAttackRange(true);
        CheckAttackRange();
        PLayAnim(AnimationName.Land, 0.1f);
        CameraController.Instance?.ShakeCamera();
        boxCollider.enabled = true;
        isattacking = false;
    }

    private void CheckAttackRange()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRange);
        if (cols.Length > 0)
        {
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
        if (isattacking)
            return;

        float angle = Mathf.Atan2(inputData.rotateVec.y, inputData.rotateVec.x) * Mathf.Rad2Deg - 90f;
        model.rotation = Quaternion.Lerp(model.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotateSpeed);
    }

    private void Move()
    {
        if (isattacking)
            return;
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
        Debug.LogError("Hit");
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy") && !isattacking)
        {
            TakeDamage();
        }
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