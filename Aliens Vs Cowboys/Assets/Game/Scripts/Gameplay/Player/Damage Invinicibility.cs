using UnityEngine;

public class PlayerDamagedInvincibility : MonoBehaviour
{
    [SerializeField] private float invincibilityDuration = 3f;

    private InvincibilityController _invincibilityController;

    private void Awake()
    {
        _invincibilityController = GetComponent<InvincibilityController>();
    }

    public void StartInvincibility()
    {
        _invincibilityController.StartInvincibility(invincibilityDuration);
    }
}
