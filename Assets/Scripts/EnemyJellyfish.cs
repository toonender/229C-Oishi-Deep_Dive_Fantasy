using UnityEngine;

public class EnemyJellyfish : MonoBehaviour
{
    [Header("การเคลื่อนที่แบบ Sine Wave")]
    public float amplitude = 1.5f;  // ขนาดการแกว่ง (หน่วย Unity)
    public float frequency = 1.0f;  // ความเร็วการแกว่ง
    public float damageAmount = 20f;

    private Vector2 startPos;
    private float timeOffset;

    void Start()
    {
        startPos = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2f); // แต่ละตัวแกว่งไม่พร้อมกัน
    }

    void Update()
    {
        // y = A × sin(2πft + offset)  → oscillation จริง
        float newY = startPos.y + amplitude * Mathf.Sin(2f * Mathf.PI * frequency * Time.time + timeOffset);
        transform.position = new Vector2(transform.position.x, newY);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null) player.TakeDamage(damageAmount);
    }
}