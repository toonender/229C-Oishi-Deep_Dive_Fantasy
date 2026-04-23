using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyFish : MonoBehaviour
{
    [Header("การเคลื่อนที่")]
    public float patrolSpeed = 2f;    // ความเร็วตอนลาดตระเวน (และตอนกลับจุดเกิด)
    public float chaseSpeed = 3.5f;   // ความเร็วตอนไล่ล่าผู้เล่น
    public float patrolRange = 3f;
    public float chaseRange = 5f;     // ระยะที่ปลาจะเริ่มไล่ล่า
    public float damageAmount = 25f;

    private Vector2 startPos;
    private int direction = 1;     // 1 = ขวา, -1 = ซ้าย
    private Transform playerTransform;

    void Start()
    {
        // บันทึกจุดเริ่มต้นเอาไว้ เพื่อให้มันรู้ว่าจะต้องว่ายกลับมาที่ไหน
        startPos = transform.position;
        GetComponent<Collider2D>().isTrigger = true;

        // หาผู้เล่นอัตโนมัติจาก Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // เช็คว่าผู้เล่นอยู่ในระยะไล่ล่าหรือไม่
        bool isPlayerInRange = playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= chaseRange;

        if (isPlayerInRange)
        {
            // ── โหมด 1: ไล่ล่าผู้เล่น (Chasing) ─────────────────────────
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);

            // หันหน้าหาผู้เล่น
            direction = playerTransform.position.x > transform.position.x ? 1 : -1;
        }
        else
        {
            // เช็คว่าตอนนี้อยู่ห่างจากจุดเกิด (startPos) มากกว่า 0.1 หน่วยหรือไม่
            if (Vector2.Distance(transform.position, startPos) > 0.1f)
            {
                // ── โหมด 2: ว่ายกลับจุดเดิม (Returning) ──────────────────
                transform.position = Vector2.MoveTowards(transform.position, startPos, patrolSpeed * Time.deltaTime);

                // หันหน้ากลับไปทางจุดเกิด
                direction = startPos.x > transform.position.x ? 1 : -1;
            }
            else
            {
                // ── โหมด 3: ลาดตระเวนปกติ (Patrolling) ──────────────────
                // ถึงจุดเกิดแล้ว เริ่มเดินซ้าย-ขวาตามเดิม
                transform.Translate(Vector2.right * direction * patrolSpeed * Time.deltaTime);

                float distFromStart = transform.position.x - startPos.x;
                if (distFromStart >= patrolRange) direction = -1;
                if (distFromStart <= -patrolRange) direction = 1;
            }
        }

        // หมุน sprite ให้หันตามทิศ (ซ้าย-ขวา)
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คว่าสิ่งที่เข้ามาชนมี Tag ว่า "Player" หรือไม่
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damageAmount); // สั่งลดเลือด Player
            Destroy(gameObject);             // ศัตรูทำลายตัวเองและหายไป
        }
    }
}