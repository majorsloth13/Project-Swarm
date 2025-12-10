using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    private HealthBase health;

    private Vector3 originalScale;

    void Awake()
    {
        // store the original scale of the healthbar
        originalScale = transform.localScale;
    }

    void LateUpdate()
    {
        // counteract parent flipping
        Vector3 parentScale = transform.parent.localScale;
        transform.localScale = new Vector3(
            Mathf.Abs(originalScale.x) * Mathf.Sign(parentScale.x),
            originalScale.y,
            originalScale.z
        );

        // reset rotation just in case
        transform.rotation = Quaternion.identity;
    }

    public void Initialize(HealthBase linkedHealth)
    {
        health = linkedHealth;

        slider.maxValue = health.maxHealth;
        slider.value = health.currentHealth;

        slider.fillRect.GetComponent<Image>().color = HexToColor("AC0000");

        gameObject.SetActive(false);
    }

    public void UpdateHealth(float current)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        slider.value = current;
    }

    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color c))
            return c;
        return Color.red;
    }
}
