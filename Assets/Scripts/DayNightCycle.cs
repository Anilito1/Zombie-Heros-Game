using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // Nouveau Input System (si installé)
#endif

[DisallowMultipleComponent]
[AddComponentMenu("TP3/Day-Night/Day Night Cycle (Script)")]
public class DayNightCycle : MonoBehaviour
{
    [Header("Sun (Directional Light)")]
    [SerializeField] private Light sun;              // Laisse vide : auto-prend le Light du GameObject
    [SerializeField, Range(0f, 360f)] private float sunYaw = 30f; // orientation est-ouest (Y)

    [Header("Time Settings")]
    [Tooltip("Durée d’un jour complet (24h virtuelles) en secondes réelles.")]
    [Min(1f)] public float cycleDurationSeconds = 120f; // 2 minutes pour 24h
    [Tooltip("Heure de départ (0..24).")]
    [Range(0f, 23.99f)] public float startHour = 8f;
    [Tooltip("Multiplicateur de vitesse (x1, x2, x4...). Modifiable en Play.")]
    [Min(0f)] public float timeMultiplier = 1f;
    [Tooltip("Activer les raccourcis clavier (1/2/3/4 pour x1/x2/x4/x8).")]
    public bool listenToKeyboard = true;

    [Header("Lighting")]
    [Min(0f)] public float dayIntensity = 1.2f;
    [Min(0f)] public float nightIntensity = 0.05f;
    public Color dayColor = new Color(1.0f, 0.956f, 0.84f); // légèrement chaud
    public Color nightColor = new Color(0.45f, 0.55f, 0.7f); // bleu nuit

    [Header("Ambient (optional)")]
    public bool controlAmbient = true;
    [Min(0f)] public float ambientDay = 1f;
    [Min(0f)] public float ambientNight = 0.1f;

    // 0..1 sur 24h (0 = minuit, 0.25 = 6h, 0.5 = midi, 0.75 = 18h)
    [Range(0f, 1f), SerializeField] private float time01;

    public float CurrentHour => time01 * 24f;

    void Reset()
    {
        sun = GetComponent<Light>();
        if (sun && RenderSettings.sun == null) RenderSettings.sun = sun;
        time01 = Mathf.Repeat(startHour / 24f, 1f);
    }

    void Start()
    {
        if (!sun) sun = GetComponent<Light>();
        if (sun && RenderSettings.sun == null) RenderSettings.sun = sun;
        time01 = Mathf.Repeat(startHour / 24f, 1f);
        ApplyLightingImmediate();
    }

    void Update()
    {
        // Avancement du temps 0..1
        float dayFracPerSec = 1f / Mathf.Max(1f, cycleDurationSeconds);
        time01 = Mathf.Repeat(time01 + Time.deltaTime * timeMultiplier * dayFracPerSec, 1f);

        // Clavier (optionnel, compatible old/new input via directives)
        if (listenToKeyboard)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.digit1Key.wasPressedThisFrame) timeMultiplier = 1f;
                if (Keyboard.current.digit2Key.wasPressedThisFrame) timeMultiplier = 2f;
                if (Keyboard.current.digit3Key.wasPressedThisFrame) timeMultiplier = 4f;
                if (Keyboard.current.digit4Key.wasPressedThisFrame) timeMultiplier = 8f;
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Alpha1)) timeMultiplier = 1f;
            if (Input.GetKeyDown(KeyCode.Alpha2)) timeMultiplier = 2f;
            if (Input.GetKeyDown(KeyCode.Alpha3)) timeMultiplier = 4f;
            if (Input.GetKeyDown(KeyCode.Alpha4)) timeMultiplier = 8f;
#endif
        }

        ApplyLightingImmediate();
    }

    private void ApplyLightingImmediate()
    {
        if (!sun) return;

        // 1) Orientation du soleil
        // 0..1 -> angle X de -90° (minuit) à +270° (retour minuit)
        float angleX = time01 * 360f - 90f;
        transform.rotation = Quaternion.Euler(angleX, sunYaw, 0f);

        // 2) Hauteur du soleil (−1 à 1) : 0 au lever/coucher, 1 à midi, −1 à minuit
        float height = Mathf.Sin(time01 * Mathf.PI * 2f - Mathf.PI / 2f);
        float daylight = Mathf.Clamp01(height); // 0 la nuit, 1 en plein jour

        // 3) Intensité & couleur
        sun.intensity = Mathf.Lerp(nightIntensity, dayIntensity, daylight);
        sun.color = Color.Lerp(nightColor, dayColor, daylight);

        // 4) Ambient (optionnel, si tu utilises la skybox standard)
        if (controlAmbient)
            RenderSettings.ambientIntensity = Mathf.Lerp(ambientNight, ambientDay, daylight);
    }

    // Exposé pour debug/UI
    public void SetHour(float hour)
    {
        time01 = Mathf.Repeat(hour / 24f, 1f);
        ApplyLightingImmediate();
    }
}
