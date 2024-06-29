using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private float jumpForce = 250.0f;
    private float lateralForce = 15.0f;

    private Rigidbody rb;
    private PlayerInput pInput;

    private Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        pInput = this.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = pInput.actions["Move"].ReadValue<Vector2>();
        rb.AddForce(new Vector3(direction.x, 0.0f, direction.y) * lateralForce);
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.performed)
            rb.AddForce(Vector3.up * jumpForce);//velocity = new Vector3(0, jumpForce, 0);
        //Debug.Log("Juuuuump!");
        //Debug.Log(callbackContext.phase);
    }
}
