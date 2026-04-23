using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyFish : MonoBehaviour
{
    [Header("การเคลื่อนที่")]
    public float patrolSpeed = 2f;    // ความเร็วตอนลาดตระเวน
    public float chaseSpeed = 3.5f;   // ความเร็วตอนไล่ล่าผู้เล่น
    public float patrolRange = 3f;
    public float chaseRange = 5f;     // ระยะที่ปลาจะเริ่มไล่ล่า
    public float damageAmount = 25f;

    private Vector2 startPos;
    private int direction = 1;     // 1 = ขวา, -1 = ซ้าย
    private Transform playerTransform;

    void Start()
    {
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
        if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= chaseRange)
        {
            // ไล่ล่าผู้เล่น
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);

            // หันหน้าหาผู้เล่น
            direction = directionToPlayer.x > 0 ? 1 : -1;
        }
        else
        {
            // เดินซ้าย-ขวา ลาดตระเวน
            transform.Translate(Vector2.right * direction * patrolSpeed * Time.deltaTime);

            float distFromStart = transform.position.x - startPos.x;
            if (distFromStart >= patrolRange) direction = -1;
            if (distFromStart <= -patrolRange) direction = 1;
        }

        // หมุน sprite ให้หันตามทิศ
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
            Destroy(gameObject);             // <-- เพิ่มบรรทัดนี้ ศัตรูจะทำลายตัวเองและหายไปทันที!
        }
    }
}