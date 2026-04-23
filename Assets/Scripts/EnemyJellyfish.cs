using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyJellyfish : MonoBehaviour
{
    [Header("การเคลื่อนที่แบบ Sine Wave")]
    public float amplitude = 1.5f;
    public float frequency = 1.0f;
    public float damageAmount = 20f;

    private Vector2 startPos;
    private float timeOffset;

    void Start()
    {
        startPos = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);

        // บังคับเปิด isTrigger ป้องกันการลืมเซ็ตใน Editor
        GetComponent<Collider2D>().isTrigger = true;
    }

    void FixedUpdate() // ใช้ FixedUpdate เมื่อมีการเช็คการชน
    {
        float newY = startPos.y + amplitude * Mathf.Sin(2f * Mathf.PI * frequency * Time.fixedTime + timeOffset);
        transform.position = new Vector2(transform.position.x, newY);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null) player.TakeDamage(damageAmount);
    }
}