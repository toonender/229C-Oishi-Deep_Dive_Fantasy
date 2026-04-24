using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CurrentZone : MonoBehaviour
{
    [Header("ทิศทางและความแรงกระแสน้ำ")]
    [Tooltip("ใส่ค่าบวก (เช่น 10) = พัดไปขวา \nใส่ค่าติดลบ (เช่น -10) = พัดไปซ้าย")]
    public float currentStrength = 5f;

    void Start()
    {
        // บังคับให้เป็น Trigger เสมอ
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            // บวกค่าแรงน้ำเข้าไป (ถ้าเป็นลบ มันก็จะกลายเป็นการผลักไปทางซ้ายเองโดยอัตโนมัติ)
            if (player != null) player.externalForceX += currentStrength;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            // ลบค่าแรงน้ำออกเมื่อหลุดโซน
            if (player != null) player.externalForceX -= currentStrength;
        }
    }
}