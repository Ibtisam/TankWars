using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 0f;
    public Image hbImage;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        hbImage.fillAmount = currentHealth / maxHealth;
        if(currentHealth <= 0)
        {
            Destroy(this.gameObject, 1f);
        }
    }
}
