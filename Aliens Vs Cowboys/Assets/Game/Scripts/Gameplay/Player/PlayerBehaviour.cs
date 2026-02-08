using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class PlayerBehaviour : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;

    private InputSystem_Actions _actions;

    void Awake()
    {
        _actions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _actions.Player.Enable();
        _actions.Player.Attack.performed += OnAttackPerformed;
    }

    void OnDisable()
    {
        _actions.Player.Disable();
        _actions.Player.Attack.performed -= OnAttackPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        Shoot();
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        float3 upDir = (float3)firePoint.up;
        float3 forceVec = upDir * bulletForce;

        rb.AddForce(new Vector2(forceVec.x, forceVec.y), ForceMode2D.Impulse);
    }
}