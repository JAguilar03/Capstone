using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public float pulseSpeed = 2f;  // Speed of the pulse (higher is faster)
    public float pulseAmount = 0.2f;  // Amount to scale the object (larger values for a bigger pulse)
    
    private Vector3 originalScale;

    void Start()
    {

        originalScale = transform.localScale;
    }

    void Update()
    {
        // Calculate the scale factor based on sine wave for a smooth pulsing effect
        float scaleMultiplier = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        // Apply the new scale
        transform.localScale = originalScale * scaleMultiplier;
    }
}
