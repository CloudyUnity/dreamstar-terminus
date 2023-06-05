using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Singleton
{
	[SerializeField] PlayerData Data;
	[Space(5)]

	[SerializeField] Transform _groundCheckPoint;
	[SerializeField] Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]

	[SerializeField] Transform _frontWallCheckPoint;
	[SerializeField] Transform _backWallCheckPoint;
	[SerializeField] Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
	[Space(5)]

	[SerializeField] LayerMask _groundLayer;

	Rigidbody2D _rb;
	PlayerInput _input;
	SpriteRenderer _rend;

	bool _isFacingRight = true;
	bool _jumping, _wallJumping, _sliding, _jumpCutting, _jumpFalling;

	float _lastOnGroundTime, _lastOnWallTime, _wallJumpStartTime, _lastPressedJumpTime;
	float _lastArrowKeyX = 1;

	int _lastWallTouched;

	public bool Grounded => _lastOnGroundTime > 0;
	public bool Walled => _lastOnWallTime > 0;
	public bool PressedJump => _lastPressedJumpTime > 0;

	protected override void Awake()
	{
		base.Awake();

		_rb = GetComponent<Rigidbody2D>();
		_rend = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		_input = Get<PlayerInput>();

		_rb.gravityScale = Data.gravityScale;
	}

	private void Update()
	{
		#region TIMERS
		_lastOnGroundTime -= Time.deltaTime;
		_lastOnWallTime -= Time.deltaTime;
		_lastPressedJumpTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER
		if (_input.ArrowKeys.x != 0)
			_lastArrowKeyX = _input.ArrowKeys.x;

		_isFacingRight = _lastArrowKeyX == 1;
		_rend.flipX = _lastArrowKeyX == -1;

		if (_input.Jump)
        {
			_lastPressedJumpTime = Data.jumpInputBufferTime;
		}

        if (_input.JumpUp)
        {
			bool canJumpCut = _jumping && _rb.velocity.y > 0;
			bool canWallJumpCut = _wallJumping && _rb.velocity.y > 0;

			if (canJumpCut || canWallJumpCut)
				_jumpCutting = true;
		}
        #endregion

        #region COLLISION CHECKS
        if (!_jumping)
		{
			bool groundDetected = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);
			if (groundDetected && !_jumping)
				_lastOnGroundTime = Data.coyoteTime;

			bool touchingRight = Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && _isFacingRight;
			bool touchingLeft = Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !_isFacingRight;

			if ((touchingLeft || touchingRight) && !_wallJumping)
            {
                _lastOnWallTime = Data.coyoteTime;
				_lastWallTouched = touchingRight ? 1 : -1;
            }
        }
		#endregion

		#region JUMP CHECKS
		if (_jumping && _rb.velocity.y < 0)
		{
			_jumping = false;

			if (!_wallJumping)
				_jumpFalling = true;
		}

		if (_wallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
		{
			_wallJumping = false;
		}

		if (Grounded && !_jumping && !_wallJumping)
		{
			_jumpCutting = false;

			if (!_jumping)
				_jumpFalling = false;
		}

		bool canJump = Grounded && !_jumping;

		bool canWallJump = Walled && !Grounded && !_wallJumping;

		if (canJump && PressedJump)
		{
			_jumping = true;
			_wallJumping = false;
			_jumpCutting = false;
			_jumpFalling = false;
			Jump();
		}
		else if (canWallJump && PressedJump)
		{
			_wallJumping = true;
			_jumping = false;
			_jumpCutting = false;
			_jumpFalling = false;
			_wallJumpStartTime = Time.time;

			WallJump(-_lastWallTouched);
		}
		#endregion

		#region GRAVITY
		float gravMult = Data.gravityScale;
		bool anyJumping = _jumping || _wallJumping || _jumpFalling;

		// Sliding
		if (_sliding)
		{
			gravMult *= 0;
		}
		// Fast Fall
		else if (_rb.velocity.y < 0 && _input.ArrowKeys.y < 0)
		{
			gravMult *= Data.fastFallGravityMult;
			_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -Data.maxFastFallSpeed));
		}
		// Early Jump Cut
		else if (_jumpCutting)
		{
			gravMult *= Data.jumpCutGravityMult;
			_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -Data.maxFallSpeed));
		}
		// Jumping
		else if (anyJumping && Mathf.Abs(_rb.velocity.y) < Data.jumpHangTimeThreshold)
		{
			gravMult *= Data.jumpHangGravityMult;
		}
		// Falling
		else if (_rb.velocity.y < 0)
		{
			gravMult *= Data.fallGravityMult;
			_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -Data.maxFallSpeed));
		}

		_rb.gravityScale = gravMult;
		#endregion
	}

	private void FixedUpdate()
	{
		_sliding = Walled && !_jumping && !_wallJumping && !Grounded;
		if (_sliding)
		{
			Slide();
			return;
		}

		float runAmount = _wallJumping ? Data.wallJumpRunLerp : 1;
		Run(runAmount);
    }

	#region RUN METHODS
	private void Run(float lerpAmount)
	{
		float targetSpeed = _input.ArrowKeys.x * Data.runMaxSpeed;
		targetSpeed = Mathf.Lerp(_rb.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		if (Grounded)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		bool anyJumping = _jumping || _wallJumping || _jumpFalling;
		if (anyJumping && Mathf.Abs(_rb.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (Data.doConserveMomentum && Mathf.Abs(_rb.velocity.x) > Mathf.Abs(targetSpeed) 
			&& Mathf.Sign(_rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && _lastOnGroundTime < 0)
		{
			accelRate = 0;
		}
		#endregion

		float speedDif = targetSpeed - _rb.velocity.x;
		float movement = speedDif * accelRate;

		_rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}
	#endregion

	#region JUMP METHODS
	private void Jump()
	{
		_lastPressedJumpTime = 0;
		_lastOnGroundTime = 0;

		#region Perform Jump
		float force = Data.jumpForce;
		// In case of falling
		if (_rb.velocity.y < 0)
			force -= _rb.velocity.y;

		_rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	private void WallJump(int dir)
	{
		_lastPressedJumpTime = 0;
		_lastOnGroundTime = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
		force.x *= dir;

		if (Mathf.Sign(_rb.velocity.x) != Mathf.Sign(force.x))
			force.x -= _rb.velocity.x;

		if (_rb.velocity.y < 0) 
			force.y -= _rb.velocity.y;

		_rb.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide()
	{
		float speedDif = Data.slideInitialSpeed - _rb.velocity.y;
		float movement = speedDif * Data.slideAccel;
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		if (_input.ArrowKeys.x == 0)
			movement *= Data.slideFallMult;

		_rb.AddForce(-movement * Vector2.up);
	}
	#endregion


	#region EDITOR METHODS
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	}
	#endregion
}
