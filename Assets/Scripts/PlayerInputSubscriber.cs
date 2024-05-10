using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSubscriber : MonoBehaviour
{
    //serializeFields
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float jumpSpeed = 1f;
    [SerializeField] int maximalJumpsSinceLastTimeOnGround = 0;

    //variables
    Vector2 moveInput;
    int jumpsSinceLastTimeOnGround = 1;

    //references
    Rigidbody2D myRigidBody;
    Animator myAnimator;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        SwitchToStayOrRunAnimation();
        FlipToMovementDirection();
    }

    private void SwitchToStayOrRunAnimation()
    {
        if (moveInput.x != 0)
            myAnimator.SetBool("running", true);
        else
            myAnimator.SetBool("running", false);
    }

    private void FlipToMovementDirection()
    {
        if (moveInput.x > 0)
            transform.localScale = new Vector2(1, transform.localScale.y);
        else if (moveInput.x < 0)
            transform.localScale = new Vector2(-1, transform.localScale.y);
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsAllowedToJump())
        {
            Jump();
        }
    }

    private bool IsAllowedToJump()
    {
        return jumpsSinceLastTimeOnGround < maximalJumpsSinceLastTimeOnGround;
    }

    private void Jump()
    {
        myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpSpeed);
        myAnimator.SetBool("running", false);
        myAnimator.SetBool("jumping", true);
        jumpsSinceLastTimeOnGround++;
    }

    private void Update()
    {
        Run();
    }

    void Run()
    {
        myRigidBody.velocity = new Vector2(moveInput.x * moveSpeed, myRigidBody.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (JumpIsFinished())//is executed immediately after each jump -> disable (OnColliderExit?)
                             //also, running animation is not enabled when touching ground again when staying has been activated before
        {
            FinishJump();
        }
    }

    private void FinishJump()
    {
        myAnimator.SetBool("jumping", false);
        SwitchToStayOrRunAnimation();
        jumpsSinceLastTimeOnGround = 0;
    }

    private bool JumpIsFinished()
    {
        return myAnimator.GetBool("jumping") && myRigidBody.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }
}
