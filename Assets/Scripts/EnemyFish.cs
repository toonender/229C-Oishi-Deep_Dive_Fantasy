using UnityEngine;

public class EnemyFish : MonoBehaviour
{
    [Header("การเคลื่อนที่")]
    public float speed = 2f;   // ความเร็วซ้าย-ขวา
    public float patrolRange = 3f;   // ระยะ patrol จาก spawn point
    public float damageAmount = 25f;  // ความเสียหายเมื่อชน

    private Vector2 startPos;
    private int direction = 1;     // 1 = ขวา, -1 = ซ้าย

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // เดินซ้าย-ขวา
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // กลับทิศเมื่อถึงขอบ patrol
        float distFromStart = transform.position.x - startPos.x;
        if (distFromStart >= patrolRange) direction = -1;
        if (distFromStart <= -patrolRange) direction = 1;

        // หมุน sprite ให้หันตามทิศ
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null) player.TakeDamage(damageAmount);
    }
}