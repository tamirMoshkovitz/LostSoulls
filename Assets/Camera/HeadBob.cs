using PlayerMovement;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public PlayerController controller;
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;

    private float defaultY;
    private float timer;

    void Start()
    {
        defaultY = transform.localPosition.y;
    }

    void Update()
    {
        if (controller.Velocity.magnitude > 0.1f && controller.IsGrounded)
        {
            timer += Time.deltaTime * bobSpeed;
            float newY = defaultY + Mathf.Sin(timer) * bobAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            timer = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, defaultY, transform.localPosition.z), Time.deltaTime * bobSpeed);
        }
    }
}