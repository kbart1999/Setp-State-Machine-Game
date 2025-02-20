using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerStats : CharacterStats
    {

        public HealthBar healthBar;
        //public StaminaBar staminaBar;

        private void Awake()
        {
            healthBar = FindObjectOfType<HealthBar>();
            //staminaBar = FindObjectOfType<StaminaBar>();

        }
        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth = currentHealth - damage;
            Debug.Log("Player has been attacked");

            healthBar.SetCurrentHealth(currentHealth);

            if(currentHealth <= 0)
            {
                currentHealth = 0;
                SceneManager.LoadScene("GUI");
        }
        }
    }



