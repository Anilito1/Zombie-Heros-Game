using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleDeathRespawn : MonoBehaviour
{
    [Header("Refs")]
    public Slider hpSlider;           // barre de CE perso
    public Animator animator;         // Animator (souvent sur l'enfant mesh)
    public Transform spawnPoint;      // où réapparaître

    [Header("Animator")]
    public string dieTrigger = "Die";
    public string idleStateName = "Idle";  // ex: "Idle_Battle"

    [Header("Timing")]
    public float respawnDelay = 2f;

    bool isDying;
    bool initialized;
    Stats stats;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        stats = GetComponent<Stats>();
    }

    IEnumerator Start()
    {
        // Laisser Stats/Sliders s'initialiser un frame
        yield return null;

        // Forcer HP full au premier spawn
        if (stats != null)
        {
            stats.HealFull();
            stats.Revive();
        }
        if (hpSlider)
        {
            if (hpSlider.maxValue <= 0f)
                hpSlider.maxValue = stats ? stats.MaxHP : 100;
            if (hpSlider.value <= 0f)
                hpSlider.value = hpSlider.maxValue;
        }

        // Forcer l'état Idle au démarrage
        if (animator && !string.IsNullOrEmpty(idleStateName))
            animator.Play(idleStateName, 0, 0f);

        initialized = true;
    }

    void Update()
    {
        if (!initialized || isDying) return;
        if (hpSlider && hpSlider.value <= 0f)
            StartCoroutine(DieAndRespawn());
    }

    IEnumerator DieAndRespawn()
    {
        isDying = true;

        if (animator && !string.IsNullOrEmpty(dieTrigger))
            animator.SetTrigger(dieTrigger);

        yield return new WaitForSeconds(respawnDelay);

        if (spawnPoint) transform.position = spawnPoint.position;

        if (animator && !string.IsNullOrEmpty(idleStateName))
            animator.Play(idleStateName, 0, 0f);

        if (stats != null)
        {
            stats.HealFull();
            stats.Revive();
        }
        else if (hpSlider)
        {
            hpSlider.value = hpSlider.maxValue;
        }

        isDying = false;
    }
}
