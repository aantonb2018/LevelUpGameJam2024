using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private float dodgeSpeeed = 250.0f;
    private float playerSpeed = 15.0f;

    private CharacterController controller;
    private PlayerInput pInput;

    private Vector2 direction;
    private Vector3 lastDirection;

    public ItemScriptableObject so;
    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        pInput = this.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = pInput.actions["Move"].ReadValue<Vector2>();
        Vector3 movement = new Vector3(direction.x, 0.0f, direction.y);
        controller.Move(movement * Time.deltaTime * playerSpeed);
        if(direction != Vector2.zero)
        {
            lastDirection = movement;
            transform.rotation = Quaternion.LookRotation(lastDirection);
        }
        
    }

    public void Dodge(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            direction = pInput.actions["Move"].ReadValue<Vector2>();
            Vector3 movement = new Vector3(direction.x, 0.0f, direction.y);
            if (movement != Vector3.zero)
            {
                controller.Move(new Vector3(direction.x, 0.0f, direction.y) * Time.deltaTime * dodgeSpeeed);
            }
            else
            {
                controller.Move(-lastDirection * Time.deltaTime * dodgeSpeeed);
            }
            //velocity = new Vector3(0, jumpForce, 0);
        }
    }

    public void LightAttack(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("LightAttack: " + so.ItemName + " | " + so.ItemDef);
    }

    public void HeavyAttack(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("HeavyAttack");
    }

    public void ChangeItemMin(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("ChangeItemMin");
    }

    public void ChangeIteMax(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("ChangeItemMax");
    }

    public void UseItem(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("UseItem");
    }
}
