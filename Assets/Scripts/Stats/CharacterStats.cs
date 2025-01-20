using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace gameracers.Stats
{
    public class CharacterStats : MonoBehaviour
    {
        public int maxHP = 100;
        public int currentHP { get; private set; }
        [SerializeField] Image healthBar;

        // More interactable stats
        public Stat attackPwr;
        public Stat speed;
        public Stat defense;

        // Obvious Stats
        public Stat experience;

        [SerializeField] float variance = .1f;

        private void Start()
        {
            currentHP = maxHP;
            if (healthBar != null)
                healthBar.fillAmount = currentHP / maxHP;
        }

        public void TakeDamage(int damage)
        {
            // Using Dragon Quest as inspiration
            currentHP -= (damage - (defense.GetValue() / 2)) / 2;

            currentHP = Mathf.Min(currentHP, maxHP);
            currentHP = Mathf.Max(0, currentHP);

            // Adjust HP Bar
            if (healthBar != null)
                healthBar.fillAmount = currentHP / maxHP;

            if (currentHP <= 0)
                Die();
        }

        public int CalcDamage()
        {
            return (int)(attackPwr.GetValue() * variance);
        }

        public void Heal(int amount)
        {
            currentHP += amount;
            currentHP = Mathf.Min(currentHP, maxHP);
            if (healthBar != null)
                healthBar.fillAmount = currentHP / maxHP;
        }

        public virtual void Die()
        {
            // TODO
            // This method is meant to be overwritten
            Debug.Log(transform.name + " has died.");
        }

        public bool GetIsDead()
        {
            if (currentHP <= 0)
                return true;
            return false;
        }
    }
}