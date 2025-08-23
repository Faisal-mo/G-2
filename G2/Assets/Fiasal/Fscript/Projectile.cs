using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public AudioClip impactSound;
    public GameObject impactEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower") || other.CompareTag("Projectile"))
            return;

        FEnemy enemy = other.GetComponent<FEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            if (impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }

            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}