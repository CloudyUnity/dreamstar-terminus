using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")] 
public class PlayerData : ScriptableObject
{
	[Header("Gravity")]
	public float fallGravityMult; 
	public float maxFallSpeed;
	[Space(5)]
	public float fastFallGravityMult; 
	public float maxFastFallSpeed;
	[HideInInspector] public float gravityScale;

	[Space(20)]

	[Header("Run")]
	public float runMaxSpeed;
	public float runAcceleration;
	[HideInInspector] public float runAccelAmount;
	public float runDecceleration;
	[HideInInspector] public float runDeccelAmount;
	[Space(5)]
	[Range(0f, 1)] public float accelInAir;
	[Range(0f, 1)] public float deccelInAir;
	[Space(5)]
	public bool doConserveMomentum = true;

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; //Height of the player's jump
	public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
	[HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.	

	[Header("Both Jumps")]
	public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
	[Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
	public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
	[Space(0.5f)]
	public float jumpHangAccelerationMult;
	public float jumpHangMaxSpeedMult;

	[Header("Wall Jump")]
	public Vector2 wallJumpForceUp; 
	public Vector2 wallJumpForceOut; 
	[Space(5)]
	[Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
	[Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
	[SerializeField] public float wallMinimumCut;
	[SerializeField] public float wallJumpChangeTime;

	[Space(20)]

	[Header("Slide")]
	public float slideInitialSpeed;
	public float slideAccel;
	public float slideFallMult;
	public float slideForceDownSpeed;
	public float slideUnstickForce;

	[Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.
	public float doubleJumpRefundRange;

    [Space(20)]

	public float pogoJumpMult;
	public float pogoJumpRange;

	[Space(20)]

	public int lightAttackDamage;


	//Unity Callback, called when the inspector updates
	private void OnValidate()
	{
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		float gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		#region Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
		#endregion
	}
}
