using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlash : MonoBehaviour
{

    #region FlashEffect

    [Tooltip("Material to switch to during the flash.")]
    [SerializeField] private Material flashMaterial;

    [Tooltip("Duration of the flash.")]
    [SerializeField] private float duration;

    [SerializeField] private int flashCounter = 2;


    // The SpriteRenderer that should flash.
    private SpriteRenderer spriteRenderer;

    // The material that was in use, when the script started.
    private Material originalMaterial;

    // The currently running coroutine.
    private Coroutine flashRoutine;



    #region Methods
    private void Awake()
    {
        // Get the SpriteRenderer to be used,
        // alternatively you could set it from the inspector.
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    private void OnEnable()
    {
        spriteRenderer.material = originalMaterial;
    }
    public void Flash()
    {
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    private IEnumerator FlashRoutine()
    {
        int _flashCounter = flashCounter;
        spriteRenderer.material = originalMaterial;
        while (_flashCounter > 0)
        {
            // Swap to the flashMaterial.
            spriteRenderer.material = flashMaterial;
            // Pause the execution of this function for "duration" seconds.
            yield return new WaitForSeconds(duration);
            // After the pause, swap back to the original material.
            spriteRenderer.material = originalMaterial;
            // Set the routine to null, signaling that it's finished.
            yield return new WaitForSeconds(duration);

            _flashCounter--;
        }
        flashRoutine = null;
    }
    #endregion

    #endregion
}
