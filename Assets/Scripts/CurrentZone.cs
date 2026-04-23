using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CurrentZone : MonoBehaviour
{
    [Header("กระแสน้ำ")]
    [Tooltip("บวก = ขวา, ลบ = ซ้าย")]
    public float currentStrength = 5f;  // ความแรงกระแสน้ำ

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    // เมื่อผู้เล่นว่ายเข้ามาในโซน ให้รับแรงกระแสน้ำไป
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) player.externalForceX += currentStrength;
        }
    }

    // เมื่อผู้เล่นว่ายออกจากโซน ให้หักล้างแรงกระแสน้ำออก
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) player.externalForceX -= currentStrength;
        }
    }
}