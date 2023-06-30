using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Singleton
{
	[SerializeField] PlayerData _data;
	[Space(5)]

	[SerializeField] Transform _groundCheckPoint;
	[SerializeField] Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]

	[SerializeField] Transform _frontWallCheckPoint;
	[SerializeField] Transform _backWallCheckPoint;
	[SerializeField] Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
	[Space(5)]

	Rigidbody2D _rb;
	BoxCollider2D _col;
	PlayerInput _input;
	PlayerAbilities _abilities;
	PlayerSprite _sprite;
	M_Camera _cam;

	bool _isFacingRight = true;
	bool _sliding, _jumpCutting, _wasGrounded, _wasWalled;
	[HideInInspector] public bool Jumping, JumpFalling, WallJumping;
	bool _lastWallJumpWasOut;

	float _lastOnGroundTime, _lastOnWallTime, _wallJumpStartTime, _lastPressedJumpTime;
	float _lastArrowKeyX = 1;

	int _lastWallTouched;

	public bool Grounded => _lastOnGroundTime > 0;
	public bool Walled => _lastOnWallTime > 0;
	public bool PressedJump => _lastPressedJumpTime > 0;

	int _movementDisablers;
	public bool MovementDisabled => _movementDisablers > 0;

	bool _sceneChangeTriggerActive;

	int _doubleJumpsDone;

    // To-do:
    // particles, change keybinds

    protected override void Awake()
    {
		DisableMovement();

        base.Awake();
    }

    private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_col = GetComponent<BoxCollider2D>();
		_abilities = GetComponent<PlayerAbilities>();
		_sprite = Get<PlayerSprite>();
		_input = Get<PlayerInput>();
		_cam = Get<M_Camera>();

		_rb.gravityScale = _data.gravityScale;

		ReEnableMovement();
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

		if (_input.Jump)
        {
			_lastPressedJumpTime = _data.jumpInputBufferTime;
		}

        if (_input.JumpUp)
        {
			bool canJumpCut = Jumping && _rb.velocity.y > 0;
			bool canWallJumpCut = WallJumping && _rb.velocity.y > 0 && (_data.jumpInputBufferTime - _lastPressedJumpTime > _data.wallMinimumCut);

			if (canJumpCut || canWallJumpCut)
				_jumpCutting = true;
		}
        #endregion

        #region COLLISION CHECKS
        if (!Jumping)
		{
			bool groundDetected = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, M_LayerMasks.Ground);
			if (groundDetected)
				_lastOnGroundTime = _data.coyoteTime;	
		}

		if (!Jumping)
        {
			Collider2D right = Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, M_LayerMasks.Ground);
			bool touchingRight = right && !right.gameObject.CheckTag("Slippy") && _isFacingRight;

			Collider2D left = Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, M_LayerMasks.Ground);
			bool touchingLeft = left && !left.gameObject.CheckTag("Slippy") && !_isFacingRight;

			if ((touchingLeft || touchingRight) && !WallJumping)
			{
				_lastOnWallTime = _data.coyoteTime;
				_lastWallTouched = touchingRight ? 1 : -1;
			}
		}
		#endregion

		JumpChecks();

		_rb.gravityScale = _data.gravityScale * GravityMult();

        #region HIT GROUND/WALL
        if (_lastOnGroundTime > 0 && !_wasGrounded)
			HitGround();

		if (_lastOnGroundTime <= 0 && _wasGrounded)
			LeaveGround();

		if (_lastOnWallTime > 0 && !_wasWalled)
			HitWall();

		_wasGrounded = _lastOnGroundTime > 0;
		_wasWalled = _lastOnWallTime > 0;
		#endregion

		if (_rb.velocity.y > 0 && !Walled)
			CornerCorrection();  
    }

	float GravityMult()
    {
		bool anyJumping = Jumping || WallJumping || JumpFalling;

		if (_sliding || MovementDisabled)
			return 0;

		bool fastFall = _rb.velocity.y < 0 && _input.ArrowKeys.y < 0 && _input.ArrowKeys.x == 0;
		if (fastFall)
		{
			_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFastFallSpeed));
			return _data.fastFallGravityMult;			
		}

		if (_jumpCutting)
		{
			_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFallSpeed));
			return _data.jumpCutGravityMult;			
		}

		if (anyJumping && Mathf.Abs(_rb.velocity.y) < _data.jumpHangTimeThreshold)
			return _data.jumpHangGravityMult;

		if (_rb.velocity.y < 0)
		{
			_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFallSpeed));
			return _data.fallGravityMult;			
		}

		return 1;
	}

    private void FixedUpdate()
	{
		if (MovementDisabled)
		{
			if (!_sceneChangeTriggerActive)
				_rb.velocity = Vector2.zero;
			return;
		}

		bool wasSliding = _sliding;
		_sliding = Walled && !Jumping && !WallJumping && !Grounded && _input.ArrowKeys.x != -_lastWallTouched;

		if (_sliding)
		{
			if (!wasSliding)
				_rb.velocity = new Vector2(_rb.velocity.x, -_data.slideInitialSpeed);

			Slide();
			return;
		}

		float runAmount = WallJumping ? _data.wallJumpRunLerp : 1;
		Run(runAmount);
    }

	void JumpChecks()
    {
		if (Jumping && _rb.velocity.y <= 0)
		{
			Jumping = false;

			if (!WallJumping)
				JumpFalling = true;
		}

		if (WallJumping && Time.time - _wallJumpStartTime <= _data.wallJumpChangeTime && !_lastWallJumpWasOut && _input.ArrowKeys.x == -_lastWallTouched)
        {
			_lastWallJumpWasOut = true;

			Vector2 force = new Vector2(4, 0);
			force.x *= -_lastWallTouched;

			if (Mathf.Sign(_rb.velocity.x) != Mathf.Sign(force.x))
				force.x -= _rb.velocity.x;

			_rb.AddForce(force, ForceMode2D.Impulse);
		}

		if (WallJumping && Time.time - _wallJumpStartTime > _data.wallJumpTime)
		{
			WallJumping = false;
		}

		if (WallJumping && Grounded)
			WallJumping = false;

		if (Grounded && !Jumping && !WallJumping)
		{
			_jumpCutting = false;

			if (!Jumping)
				JumpFalling = false;
		}

		if (MovementDisabled)
			return;

        #region POGOJUMP - REMOVED
        //bool withinPogoRange = M_Extensions.Ray(transform.position + new Vector3(0, -0.3105f), Vector2.down, _data.pogoJumpRange, M_LayerMasks.Ground).collider != null;
        //bool canPogoJump = _input.Attack && _input.ArrowKeys.y < 0 && withinPogoRange && !Grounded && _abilities.PogoOn && !Jumping;

        //if (canPogoJump)
        //{
        //	Jumping = true;

        //	WallJumping = false;
        //	_jumpCutting = false;
        //	JumpFalling = false;

        //	PogoJump();
        //	return;
        //}
        #endregion

        if (!PressedJump)
			return;

		bool canJump = Grounded && !Jumping;

		bool canWallJump = Walled && !Grounded && !WallJumping && _abilities.WallJumpOn;

		bool canDoubleJump = !Jumping && _doubleJumpsDone < _abilities.DoubleJumps;

		if (canJump)
		{
			Jumping = true;
			WallJumping = false;
			_jumpCutting = false;
			JumpFalling = false;

			Jump();
			return;
		}

		if (canWallJump)
		{
			WallJumping = true;
			Jumping = false;
			_jumpCutting = false;
			JumpFalling = false;
			_wallJumpStartTime = Time.time;

			WallJump(-_lastWallTouched);
			return;
		}	

		if (canDoubleJump)
        {
			Jumping = true;
			WallJumping = false;
			_jumpCutting = false;
			JumpFalling = false;

			_doubleJumpsDone++;

			_rb.velocity = new Vector2(_rb.velocity.x, 0);

			RaycastHit2D hit = M_Extensions.Ray(new Vector2(_col.bounds.center.x, _col.bounds.min.y), Vector2.up, M_LayerMasks.Ground, _data.doubleJumpRefundRange);
			if (hit.collider != null)
				_doubleJumpsDone--;

			Jump();
			return;
		}
	}

	#region RUN METHODS
	private void Run(float lerpAmount)
	{
		float targetSpeed = _input.ArrowKeys.x * _data.runMaxSpeed;

		if (_input.ArrowKeysUnRaw.x <= 1 && _input.ArrowKeysUnRaw.x >= -1 && _input.ArrowKeysUnRaw.x != 0)
			targetSpeed *= Mathf.Abs(_input.ArrowKeysUnRaw.x);

		targetSpeed = Mathf.Lerp(_rb.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		if (Grounded)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount : _data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount * _data.accelInAir : _data.runDeccelAmount * _data.deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		bool anyJumping = Jumping || WallJumping || JumpFalling;
		if (anyJumping && Mathf.Abs(_rb.velocity.y) < _data.jumpHangTimeThreshold)
		{
			accelRate *= _data.jumpHangAccelerationMult;
			targetSpeed *= _data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (_data.doConserveMomentum && Mathf.Abs(_rb.velocity.x) > Mathf.Abs(targetSpeed) 
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

		float force = _data.jumpForce;
		// In case of falling
		if (_rb.velocity.y < 0)
			force -= _rb.velocity.y;

		_rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
	}

	private void WallJump(int dir)
	{
		_lastPressedJumpTime = 0;
		_lastOnGroundTime = 0;

		_lastWallJumpWasOut = _input.ArrowKeys.x == -_lastWallTouched;

		Vector2 force = _lastWallJumpWasOut ? _data.wallJumpForceOut : _data.wallJumpForceUp;
		force.x *= dir;

		if (Mathf.Sign(_rb.velocity.x) != Mathf.Sign(force.x))
			force.x -= _rb.velocity.x;

		if (_rb.velocity.y < 0) 
			force.y -= _rb.velocity.y;

		_rb.AddForce(force, ForceMode2D.Impulse);
	}

	void PogoJump()
    {
		_lastPressedJumpTime = 0;
		_lastOnGroundTime = 0;

		Vector2 force = new Vector2(_data.pogoJumpForceX * _lastArrowKeyX, _data.jumpForce * _data.pogoJumpForceYMult);
		// In case of falling
		if (_rb.velocity.y < 0)
			force.y -= _rb.velocity.y;

		force.x -= _rb.velocity.x * 0.5f;

		_rb.AddForce(force, ForceMode2D.Impulse);
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide()
	{
		float speedDif = _data.slideInitialSpeed - _rb.velocity.y;
		float movement = speedDif * _data.slideAccel;
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        _rb.AddForce(-movement * Vector2.up);
	}
	#endregion

	#region GAMEFEEL
	void CornerCorrection()
	{
		float rayDis = 0.15f;

		RaycastHit2D hitLeft = M_Extensions.Ray(new Vector2(_col.bounds.min.x, _col.bounds.max.y), Vector2.up, M_LayerMasks.Ground, rayDis);

		if (hitLeft.collider != null)
		{
			for (float i = 0; i <= 0.15f; i += 0.025f)
			{
				RaycastHit2D hit = M_Extensions.Ray(new Vector2(_col.bounds.min.x + i, _col.bounds.max.y), Vector2.up, M_LayerMasks.Ground, rayDis);

				if (hit.collider != null)
					continue;

				transform.position = new Vector3(transform.position.x + 0.01f + i, transform.position.y);
				return;
			}
		}

		RaycastHit2D hitRight = M_Extensions.Ray(_col.bounds.max, Vector2.up, M_LayerMasks.Ground, rayDis);

		if (hitRight.collider != null)
		{
			for (float i = 0; i <= 0.15f; i += 0.025f)
			{
				RaycastHit2D hit = M_Extensions.Ray(new Vector2(_col.bounds.max.x - i, _col.bounds.max.y), Vector2.up, M_LayerMasks.Ground, rayDis);
				if (hit.collider != null)
					continue;

				transform.position = new Vector3(transform.position.x - 0.01f - i, transform.position.y);
				return;
			}
		}
	}
    #endregion

    void HitGround()
    {
		_doubleJumpsDone = 0;
		_sprite.Squash();

		if (_cam == null)
			return;

		_cam.ScreenShake(0.01f, 0.2f);
	}

	void LeaveGround()
    {
		_sprite.Stretch();
    }

	void HitWall()
    {
		_doubleJumpsDone = 0;
		Jumping = false;
    }

	public void Fling(Vector2 force, ForceMode2D mode = ForceMode2D.Force) => _rb.AddForce(force, mode);

    private void OnCollisionEnter2D(Collision2D collision)
    {
		//Debug.Log("Hit " + collision.gameObject.name);
    }

    public void DisableMovement() => _movementDisablers++;
	public void ReEnableMovement() => _movementDisablers--;

	public void ActivateSceneChange(Vector2 dir, float dur = 99999)
    {
		if (_sceneChangeTriggerActive)
			return;

		_sceneChangeTriggerActive = true;

		_rb.velocity = dir * _data.runMaxSpeed;

		_sprite.ForceMoveSceneChange = dir;

		StartCoroutine(C_WaitSceneChange(dur));	
    }

	IEnumerator C_WaitSceneChange(float dur)
    {
		yield return new WaitForSeconds(dur);

		_sceneChangeTriggerActive = false;

		_sprite.ForceMoveSceneChange = Vector2.zero;
    }

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
