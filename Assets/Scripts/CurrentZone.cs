using UnityEngine;

// ติด Script นี้กับ GameObject ที่มี Collider2D (IsTrigger = true)
// แล้ววาง Zone ให้ทับกับบริเวณที่มีกระแสน้ำ
[RequireComponent(typeof(Collider2D))]
public class CurrentZone : MonoBehaviour
{
    [Header("กระแสน้ำ")]
    [Tooltip("บวก = ขวา, ลบ = ซ้าย")]
    public float currentStrength = 5f;  // ความแรงกระแสน้ำ (m/s²)

    void Start()
    {
        // ตรวจสอบว่า Collider เป็น Trigger
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // F_current = strength × mass  (F = ma → a = strength)
        float mass = player.playerMass;
        float fCurrent = currentStrength * mass;

        // สะสมแรงใน PlayerController (ถูก AddForce ใน FixedUpdate)
        player.externalForceX += fCurrent;
    }
}