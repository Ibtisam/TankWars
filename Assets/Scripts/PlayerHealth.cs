using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth = 0f;
    public Image hbImage;
    public ParticleSystem destructionFX;

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
            destructionFX.Play();
            Destroy(this.gameObject, 1f);
        }
    }
}
