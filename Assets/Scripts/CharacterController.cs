using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    
    [Header("Movement Variables")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float runSpeed = 10f;
    public float originalMoveSpeed = 5f;
    public float mouseSensitivity = 100f;
    
    [Header("Ground Variables")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Running Variables")]
    private Vector3 _velocity;
    public bool _isGrounded;
    public bool _isSprinting;
    
    [Header("Misc")]
    private bool canMove = true;
    public bool itemNearby = false;

    void Start()
    { 

    }
    void Update()
    {
        Jump();
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        float verticalInput = Input.GetAxis("Vertical");
        float HorizontalInput = Input.GetAxis("Horizontal");

        Vector3 move = transform.right * HorizontalInput + transform.forward * verticalInput;

        if (Input.GetKey(KeyCode.LeftShift) && _isGrounded)
        {
           _isSprinting = true;
           controller.Move(move * runSpeed * Time.deltaTime);
           
        }
        else
        {
            _isSprinting = false;
            controller.Move(move * moveSpeed * Time.deltaTime);
            
        }
        
        controller.Move(move * moveSpeed * Time.deltaTime);
        
        _velocity.y += gravity * Time.deltaTime;
        
        controller.Move(_velocity * Time.deltaTime);
    }
    
    private void Jump()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    /*private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E) && itemNearby)
        {
            itemRef.eButton.SetActive(false);
            itemNearby = false;
            inventory.Add(itemRef.ReturnPickUpItem());
            Destroy(itemRef.gameObject);
            _flashlight.flashlight.SetActive(true);
        }
    }*/
}