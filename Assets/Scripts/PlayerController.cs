using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Physics — ปรับค่าได้ใน Inspector")]
    public float playerMass = 70f;
    public float gravity = 9.81f;
    public float baseBuoyancy = 0.0f;   // <-- ลดลง ให้จมตอนไม่กด
    public float breathBuoyancy = 1.4f;    // <-- เพิ่มขึ้น ให้ลอยชัดตอนกด
    public float dragCoefficient = 3.0f;    // <-- เพิ่ม drag ให้ movement นุ่มขึ้น
    public float maxVerticalSpeed = 5f;

    [Header("Horizontal Movement")]
    public float horizontalForce = 200f;    // แรงกด A/D
    public float maxHorizontalSpeed = 4f;    // จำกัดความเร็วแนวนอน

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
    private bool isAlive = true;
    private float invincibleTimer;
    private float horizontalInput;

    [HideInInspector] public float externalForceX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        horizontalInput = Input.GetAxisRaw("Horizontal"); // -1, 0, 1

        if (invincibleTimer > 0f) invincibleTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        ApplyPhysics();
        UpdateUI();
        externalForceX = 0f;
    }

    void ApplyPhysics()
    {
        // ── แรงโน้มถ่วง (ลง) ──────────────────────────────────
        // F_weight = m × g
        float fWeight = playerMass * gravity;

        // ── แรงลอยตัว Buoyancy (ขึ้น) ─────────────────────────
        // F_buoyancy = m × g × multiplier
        // baseBuoyancy < 1.0 → จมสุทธิ / breathBuoyancy > 1.0 → ลอยสุทธิ
        float mult = isBreathing ? breathBuoyancy : baseBuoyancy;
        float fBuoyancy = playerMass * gravity * mult;

        // ── แรงต้านน้ำ Drag แนวตั้ง (ต้านทิศเคลื่อนที่) ───────
        // F_drag = -k × v
        float fDrag = -dragCoefficient * rb.linearVelocity.y;

        // ── Net Force แนวตั้ง ──────────────────────────────────
        // ลบ Time.fixedDeltaTime ออก — Unity คิดให้อยู่แล้วใน ForceMode2D.Force
        netForce = fBuoyancy - fWeight + fDrag;
        rb.AddForce(new Vector2(0f, netForce));

        // ── แรงแนวนอน A/D ─────────────────────────────────────
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            float fHorizontal = horizontalInput * horizontalForce;
            rb.AddForce(new Vector2(fHorizontal, 0f));
        }

        // ── แรงกระแสน้ำ (มาจาก CurrentZone) ──────────────────
        if (Mathf.Abs(externalForceX) > 0.01f)
            rb.AddForce(new Vector2(externalForceX, 0f));

        // ── Drag แนวนอน (หน่วงเมื่อไม่กด) ───────────────────
        // ทำให้ไม่ไถลไปเรื่อยๆ
        float dragX = -dragCoefficient * rb.linearVelocity.x;
        rb.AddForce(new Vector2(dragX, 0f));

        // ── Clamp ความเร็ว ─────────────────────────────────────
        float clampedVY = Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed);
        float clampedVX = Mathf.Clamp(rb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);
        rb.linearVelocity = new Vector2(clampedVX, clampedVY);
    }

    void UpdateUI()
    {
        if (hpSlider) hpSlider.value = hp;
        if (hpText) hpText.text = "HP: " + Mathf.RoundToInt(hp);
        if (oxygenSlider) oxygenSlider.value = oxygen;
        if (oxygenText) oxygenText.text = "O2: " + Mathf.RoundToInt(oxygen);

        float depth = Mathf.Max(0f, -transform.position.y);
        if (depthText) depthText.text = "Depth: " + Mathf.RoundToInt(depth) + " m";
        if (forceText) forceText.text = "Net Force: " + Mathf.RoundToInt(netForce) + " N";
    }

    public void TakeDamage(float amount)
    {
        if (invincibleTimer > 0f) return;
        hp -= amount;
        invincibleTimer = 1.5f;
        if (hp <= 0f)
        {
            hp = 0f;
            isAlive = false;
            FindObjectOfType<GameManager>().TriggerGameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
            FindObjectOfType<GameManager>().TriggerWin();
    }
}