using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Physics — ปรับค่าได้ใน Inspector")]
    public float playerMass = 70f;
    public float gravity = 9.81f;
    public float baseBuoyancy = 0.0f;
    public float breathBuoyancy = 1.4f;
    public float dragCoefficient = 3.0f;
    public float maxVerticalSpeed = 5f;

    [Header("Horizontal Movement")]
    public float horizontalForce = 200f;
    public float maxHorizontalSpeed = 4f;

    [Header("Oxygen System")]
    public float maxOxygen = 100f;
    public float oxygenDrainRate = 25f;
    public float oxygenRegenRate = 15f;
    public Slider oxygenSlider;
    public Text oxygenText;
    [SerializeField] private float oxygen;

    [Header("UI References")]
    public Slider hpSlider;
    public Text hpText;
    public Text depthText;
    public Text forceText;

    [Header("State (Read-only)")]
    [SerializeField] private float hp = 100f;
    [SerializeField] private bool isBreathing;
    [SerializeField] private float netForce;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;
    private float invincibleTimer;
    private float horizontalInput;

    [HideInInspector] public float externalForceX = 0f; // เอาไว้รับแรงจาก CurrentZone

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        oxygen = maxOxygen;
        if (hpSlider) hpSlider.maxValue = 100f;
        if (oxygenSlider) oxygenSlider.maxValue = maxOxygen;
    }

    void Update()
    {
        if (!isAlive) return;

        // ── Oxygen + Breathing ─────────────────────────────────
        bool holdingSpace = Input.GetKey(KeyCode.Space);

        if (holdingSpace && oxygen > 0f)
        {
            isBreathing = true;
            oxygen -= oxygenDrainRate * Time.deltaTime;
        }
        else
        {
            isBreathing = false;
            oxygen += oxygenRegenRate * Time.deltaTime;
        }
        oxygen = Mathf.Clamp(oxygen, 0f, maxOxygen);

        // ── รับ Input แนวนอน A/D หรือ Arrow ──────────────────
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // ── ระบบ Flip หันซ้าย-ขวา ─────────────────────────────
        if (horizontalInput != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(horizontalInput);
            transform.localScale = scale;
        }

        if (invincibleTimer > 0f) invincibleTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        ApplyPhysics();
        UpdateUI();
    }

    void ApplyPhysics()
    {
        float fWeight = playerMass * gravity;
        float mult = isBreathing ? breathBuoyancy : baseBuoyancy;
        float fBuoyancy = playerMass * gravity * mult;
        float fDrag = -dragCoefficient * rb.linearVelocity.y;

        netForce = fBuoyancy - fWeight + fDrag;
        rb.AddForce(new Vector2(0f, netForce));

        // ── แรงแนวนอน A/D ─────────────────────────────────────
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            float fHorizontal = horizontalInput * horizontalForce;
            rb.AddForce(new Vector2(fHorizontal, 0f));
        }

        // ── แรงกระแสน้ำจาก CurrentZone (ส่วนที่เพิ่มเข้ามาใหม่) ────────
        if (Mathf.Abs(externalForceX) > 0.01f)
        {
            // F = m * a
            rb.AddForce(new Vector2(externalForceX * playerMass, 0f));
        }

        float dragX = -dragCoefficient * rb.linearVelocity.x;
        rb.AddForce(new Vector2(dragX, 0f));

        // ── Clamp ความเร็ว ─────────────────────────────────────
        float clampedVY = Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed);

        // ขยายขีดจำกัดความเร็วเมื่อโดนกระแสน้ำพัด
        float currentMaxSpeedX = maxHorizontalSpeed + Mathf.Abs(externalForceX * 0.5f);
        float clampedVX = Mathf.Clamp(rb.linearVelocity.x, -currentMaxSpeedX, currentMaxSpeedX);

        rb.linearVelocity = new Vector2(clampedVX, clampedVY);
    }

    void UpdateUI()
    {
        if (hpSlider) hpSlider.value = hp;
        if (hpText) hpText.text = "HP: " + Mathf.RoundToInt(hp);
        if (oxygenSlider) oxygenSlider.value = oxygen;
        if (oxygenText) oxygenText.text = "Stamina: " + Mathf.RoundToInt(oxygen);

        float depth = Mathf.Max(0f, -transform.position.y);
        if (depthText) depthText.text = "Depth: " + Mathf.RoundToInt(depth) + " m";
        if (forceText) forceText.text = "Net Force: " + Mathf.RoundToInt(netForce) + " N";
    }

    public void TakeDamage(float amount)
    {
        if (invincibleTimer > 0f) return;

        hp -= amount;
        invincibleTimer = 1.5f;

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRedRoutine());
        }

        if (hp <= 0f)
        {
            hp = 0f;
            isAlive = false;
            FindObjectOfType<GameManager>().TriggerGameOver();
        }
    }

    private IEnumerator FlashRedRoutine()
    {
        float blinkDuration = 1.5f;
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }

        spriteRenderer.color = Color.white;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
            FindObjectOfType<GameManager>().TriggerWin();
    }
}