using System;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [Header("HP & Damage")]
    public int MaxHP = 100;
    public int AttackPower = 10;

    [Header("UI")]
    public Slider HpSlider;

    public int CurrentHP { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<Stats> OnDeath;   // abonnés: Respawn

    Animator _anim;

    void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        CurrentHP = MaxHP;
        if (HpSlider)
        {
            HpSlider.maxValue = MaxHP;
            HpSlider.value = MaxHP;
        }
    }

    public void HealFull()
    {
        CurrentHP = MaxHP;
        if (HpSlider) HpSlider.value = CurrentHP;
    }

    public void Revive()
    {
        IsDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHP = Mathf.Max(0, CurrentHP - Mathf.Max(0, amount));
        if (HpSlider) HpSlider.value = CurrentHP;

        if (CurrentHP <= 0)
        {
            IsDead = true;
            if (_anim) _anim.SetTrigger("Die");
            OnDeath?.Invoke(this);
        }
    }
}
