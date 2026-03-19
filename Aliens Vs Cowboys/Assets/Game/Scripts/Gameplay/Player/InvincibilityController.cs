using System.Collections;
using UnityEngine;

public class InvincibilityController : MonoBehaviour
{
    private PlayerHealth _healthController;

    private void Awake()
    {
        _healthController = GetComponent<PlayerHealth>();
    }

    public void StartInvincibility(float duration)
    {
        StartCoroutine(InvincibilityCoroutine(duration));
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        _healthController.isInvincible = true;
        yield return new WaitForSeconds(duration);
        _healthController.isInvincible = false;
    }
}