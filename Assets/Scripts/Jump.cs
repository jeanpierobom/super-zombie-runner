using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour {

    public float jumpSpeed = 240f;
    public float doubleJumpSpeed = 120f;
    public float forwardSpeed = 20f;

    private Rigidbody2D body2d;
    private InputState inputState;

    private AudioSource audioJump;

    private bool allowDoubleJump = false;

    void Awake()
    {
        body2d = GetComponent<Rigidbody2D>();    
        inputState = GetComponent<InputState>();
        audioJump = GameObject.Find("AudioJump").GetComponent<AudioSource>();
    }

	// Update is called once per frame
	void Update () {

        /*        if (inputState.standing)
                {
                    if (inputState.actionButton)
                    {
                        body2d.velocity = new Vector2(transform.position.x < 0 ? forwardSpeed : 0, jumpSpeed);
                        audioJump.Play();
                    }
                }*/

        if (inputState.actionButton)
        {
            if (inputState.standing) {
                goJump(jumpSpeed);
                allowDoubleJump = true;
            } else if (allowDoubleJump) {
                goJump(doubleJumpSpeed);
                allowDoubleJump = false;
            }
        }
    }

    void goJump(float jumpSpeed)
    {
        body2d.velocity = new Vector2(transform.position.x < 0 ? forwardSpeed : 0, jumpSpeed);
        audioJump.Play();
    }

}
