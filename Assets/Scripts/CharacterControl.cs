using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerDirection : sbyte{LEFT = -1, RIGHT = 1};
public delegate void PlayerHitDelegate();
public class CharacterControl : MonoBehaviour {
	public float m_MaxHorizontalSpeed;
	public float m_MaxVerticalSpeed;
	public float m_MaxWallSlideVelocity;
	public float m_JumpVelocity;
	public float m_WallKickVelocity;
	public float m_MoveForce;
	public float m_DashSpeed;
	public float m_BlinkDistance;
	public float m_StopThreshold;
	public float m_AirborneForceFactor;
	public float m_ReverseAirborneForceFactor;
	public float m_DashTime;
	public float m_SqrDeadZone;
	public float m_CollisionDeadZone;
	public bool m_IsInvincible;
	public bool m_Mario;
	public bool m_OmniDash;
	public byte m_TotalNumJumps;
	public byte  m_TotalNumDashes;
	public byte m_TotalNumBlinks;
	public LayerMask m_PlatformLayer;
	public SpriteRenderer m_SpriteRenderer;
	public Transform m_CursorTransform;
	public PlayerHitDelegate m_PlayerHit;
	private Rigidbody2D m_RigidBody;
	private Animator m_Anim;
	private byte m_NumJumps = 0;
	private byte m_NumDashes = 0;
	private byte m_NumBlinks = 0;
	//Sprite better be facing right by deafult
	private PlayerDirection m_Direction = PlayerDirection.RIGHT;
	private bool m_IsGrounded = false;
	// private bool m_DashTriggerPressed = false;
	// private bool m_DashTriggerUp = false;
	// private bool m_DashTriggerDown = false;
	private bool m_IsWalled;
	private bool m_IsUsingController = false;
	private bool m_IsCrouching = false;
	private bool m_DashButtonPressed = false;
	private bool m_JumpButtonPressed = false;
	private bool m_CanJump = true;
	private bool m_CanBlink = true;
	private bool m_PhysicsActive = true;
	private bool m_BlinkButtonPressed = false;
	private float m_Epsilon = 0.05f;
	private float m_UserHAxisValue;
	private Vector2 m_DashDirection;
	private Vector2 m_BlinkVector;
	private Vector2 m_StoredVelocity;
	private float m_UserRightHAxisValue;
	private float m_UserRightVAxisValue;
	private float m_StartDashTime = 0f;
	private float m_DashDeadZone = 0.1f;
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		m_DashDirection = new Vector2(0, 1);
		m_NumJumps = m_TotalNumJumps;
		m_NumDashes = m_TotalNumDashes;
		m_NumBlinks = m_TotalNumBlinks;
	}
	// Use this for initialization
	void Start () {
		m_RigidBody = GetComponent<Rigidbody2D>();
		m_Anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		Crouch();
		GetBlinkVector();
		UpdateCursor();
		m_UserHAxisValue = Input.GetAxis("Horizontal");
		m_BlinkButtonPressed = Input.GetButtonDown("Blink");
		GetJumpInput();
		GetDashInput();
	}
	private void GetBlinkVector(){
		float blinkDistance = m_BlinkDistance;
		Vector2 blinkDirection = new Vector2(Input.GetAxis("RHorizontal"), Input.GetAxis("RVertical"));
		if(blinkDirection.sqrMagnitude > m_SqrDeadZone){
			m_IsUsingController = true;
		}
		else if(Mathf.Abs(Input.GetAxis("Mouse X")) > m_Epsilon &&  Mathf.Abs(Input.GetAxis("Mouse Y")) > m_Epsilon){
			m_IsUsingController = false;
		}
		//&& blinkDirection.sqrMagnitude < m_SqrDeadZone
		if(!m_IsUsingController){
			Vector3 mousePosition = Input.mousePosition;
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			mousePosition = mouseRay.GetPoint(Mathf.Abs(mouseRay.origin.z / mouseRay.direction.z));
			blinkDirection = mousePosition - transform.position;
			if(blinkDirection.magnitude < m_BlinkDistance){
				blinkDistance = blinkDirection.magnitude;
			}
			blinkDirection.Normalize();

		}
		else{
			if(blinkDirection.sqrMagnitude < m_SqrDeadZone){
				blinkDirection = new Vector2((float)m_Direction, 0f);
			}
			else{
				blinkDirection = blinkDirection.normalized;
			}
		}
		m_BlinkVector = blinkDirection * blinkDistance;
	}
	private void UpdateCursor(){
		float xOffset = 0;
		float yOffset = 0;
		Vector3 halfSize = m_SpriteRenderer.bounds.extents;
		//right of player
		if(m_BlinkVector.x > 0){
			xOffset = halfSize.x;
		}
		else{
			xOffset = -halfSize.x;
		}
		//above player
		if(m_BlinkVector.y > 0){
			yOffset = halfSize.y;
		}
		else{
			yOffset = -halfSize.y;
		}
		Vector3 offsetVector = new Vector3(xOffset, yOffset);
		Vector3 blinkLocation = transform.position + offsetVector + (Vector3)(m_BlinkVector);
		RaycastHit2D hit = Physics2D.Linecast(transform.position, blinkLocation, m_PlatformLayer);
		if(hit){
			m_CursorTransform.position = (Vector3)hit.point - offsetVector;
		}
		else{
			m_CursorTransform.position = blinkLocation - offsetVector;
		}
		SpriteRenderer cursorRenderer = m_CursorTransform.GetComponent<SpriteRenderer>();
		if(m_NumBlinks >= m_TotalNumBlinks){
			cursorRenderer.color = new Color(0f, 0f, 0f, 0f);
		}
		else{
			cursorRenderer.color = new Color(0.133f, 0.886f, 1f,1f);
		}
	}
	private void GetJumpInput(){
		m_JumpButtonPressed = Input.GetButton("Jump");
		if(Input.GetButtonUp("Jump")){
			m_CanJump = true;
		}
	}
	private void GetDashInput(){
		// m_DashTriggerDown = false;
		// m_DashTriggerUp = false;
		// if(!m_DashTriggerPressed && Input.GetAxis("Dash") > m_DashDeadZone){
		// 	m_DashDeadZone = -0.9f;
		// 	m_DashTriggerPressed = true;
		// 	m_DashTriggerDown = true;
		// }
		// else if(m_DashTriggerPressed && Input.GetAxis("Dash") <= m_DashDeadZone){
		// 	m_DashTriggerPressed = false;
		// 	m_DashTriggerUp = true;
		// }

		if(!m_DashButtonPressed && m_NumDashes < m_TotalNumDashes && (Input.GetButtonDown("Dash"))){
			m_StartDashTime = Time.time;
			m_DashButtonPressed = true;
			m_Anim.SetBool("Dash", true);
			
			if(m_OmniDash){
				float userVAxisValue = Input.GetAxis("Vertical");
				Vector2 dashDirection = new Vector2(m_UserHAxisValue, userVAxisValue);
				if(Vector2.SqrMagnitude(dashDirection) > m_SqrDeadZone){
					m_DashDirection = dashDirection.normalized;
				}
				else{
					m_DashDirection = new Vector2((float)m_Direction, 0);
				}
			}
			else{
				m_DashDirection = new Vector2((float)m_Direction, 0);
			}
		}
		else if(m_DashButtonPressed && (Input.GetButtonUp("Dash") || Time.time > (m_StartDashTime + m_DashTime))){
			m_DashButtonPressed = false;
			m_Anim.SetBool("Dash", false);
			m_NumDashes++;
		}
	}
	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		CheckGrounded();
		CheckWalled();
		if(m_PhysicsActive){
			MoveHorizontal();
			Jump();
			Dash();
			Blink();
		}
	}
	//Getting Ugly
	void Blink(){
		if(m_BlinkButtonPressed && m_CanBlink && m_NumBlinks < m_TotalNumBlinks){
			m_CanBlink = false;
			m_PhysicsActive = false;
			m_StoredVelocity = m_RigidBody.velocity;
			m_RigidBody.velocity = Vector2.zero;
			m_RigidBody.isKinematic = true; 
			m_Anim.SetTrigger("Blink");
			Invoke("BlinkInvoke", 0.125f);
			Invoke("BlinkDoneInvoke", 0.25f);
		}
	}
	void BlinkInvoke(){
		m_RigidBody.MovePosition(m_CursorTransform.position);
		m_NumBlinks++;
	}
	void BlinkDoneInvoke(){
		m_CanBlink = true;
		m_RigidBody.velocity = m_StoredVelocity;
		m_RigidBody.isKinematic = false; 
		m_PhysicsActive = true;
	}
	void MoveHorizontal(){
		if(!m_IsCrouching){
			if(m_Mario){
				float moveForce = m_UserHAxisValue * m_MoveForce;
				if(!m_IsGrounded){
					if((m_UserHAxisValue * (sbyte)m_Direction < 0)){
						moveForce *= m_ReverseAirborneForceFactor;
					}
					else{
						moveForce *= m_AirborneForceFactor;
					}
				}
				m_RigidBody.AddForce(new Vector2(moveForce, 0f));
				if(Mathf.Abs(m_RigidBody.velocity.x) > m_MaxHorizontalSpeed){
					m_RigidBody.velocity = new Vector2(Mathf.Clamp(m_RigidBody.velocity.x, -m_MaxHorizontalSpeed, m_MaxHorizontalSpeed), m_RigidBody.velocity.y);
				}
			}
			else{
				if(Mathf.Abs(m_UserHAxisValue) > m_SqrDeadZone){
					m_RigidBody.velocity = new Vector2(m_UserHAxisValue, 0f) * m_MaxHorizontalSpeed + new Vector2(0f, m_RigidBody.velocity.y);
				}
				else{
					m_RigidBody.velocity = new Vector2(0f, m_RigidBody.velocity.y);
				}
			}
		}
		if((Mathf.Abs(m_UserHAxisValue) > m_Epsilon) && (m_UserHAxisValue * (sbyte)m_Direction < 0f)){
			Flip();
		}
		m_Anim.SetFloat("Speed", Mathf.Abs(m_RigidBody.velocity.x));
	}
	private void Jump(){
		if(m_JumpButtonPressed && m_CanJump && m_NumJumps < m_TotalNumJumps){
			float xVelocity = m_RigidBody.velocity.x;
			if(m_IsWalled){
				xVelocity = -(float)m_Direction * m_WallKickVelocity;
			}
			m_RigidBody.velocity = new Vector2(xVelocity, m_JumpVelocity);
			m_NumJumps++;
			m_CanJump = false;
		}
		else if(!m_JumpButtonPressed && m_RigidBody.velocity.y > 0f){
			m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0f);
		}
	}
	private void Dash(){
		if(m_DashButtonPressed){
			m_RigidBody.velocity = m_DashDirection * m_DashSpeed;
		}
	}
	private void CheckGrounded(){
		Vector3 bottomLeft = m_SpriteRenderer.bounds.min;
		float left = bottomLeft.x + m_CollisionDeadZone;
		float right = left + m_SpriteRenderer.bounds.size.x - 2 * m_CollisionDeadZone;
		float bottom = bottomLeft.y - m_Epsilon;
		
		bool grounded = Physics2D.Linecast(new Vector2(left, bottom), new Vector2(right, bottom), m_PlatformLayer);
		if(grounded != m_IsGrounded){
			m_IsGrounded = grounded;
			m_Anim.SetBool("Ground", grounded);
		}
		if(m_IsGrounded){
			m_NumJumps = 0;
			m_NumDashes = 0;
			m_NumBlinks = 0;
		}
	}
	private void CheckWalled(){
		Vector3 max = m_SpriteRenderer.bounds.max;
		Vector3 size = m_SpriteRenderer.bounds.size;
		Vector2 topRight = new Vector2(max.x + m_Epsilon, max.y - m_CollisionDeadZone);
		Vector2 bottomLeft = new Vector2(max.x - size.x - m_Epsilon, max.y - size.y + m_CollisionDeadZone);
		RaycastHit2D hit = Physics2D.Linecast(topRight, bottomLeft, m_PlatformLayer);
		bool walled = false;
		if(hit){
			float direction = hit.point.x - m_SpriteRenderer.bounds.center.x;
			walled = m_UserHAxisValue * direction > 0;
		}
		if(walled != m_IsWalled){
			m_IsWalled = walled;
			m_Anim.SetBool("Wall", walled);
		}
		if(m_IsWalled){
			m_NumJumps = 0;
			m_NumDashes = 0;
			if(m_RigidBody.velocity.y < 0 && !m_DashButtonPressed){
				m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, -m_MaxWallSlideVelocity);
			}
		}
	}
	private void Crouch(){
		if(Input.GetAxis("Vertical") < -m_SqrDeadZone && Mathf.Abs(m_RigidBody.velocity.x) < m_Epsilon){
			m_IsCrouching = true;
		}
		else{
			m_IsCrouching = false;
		}
		m_Anim.SetBool("Crouch", m_IsCrouching);
	}
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_Direction = (PlayerDirection)((sbyte)m_Direction * -1);

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		if (!m_IsInvincible && other.gameObject.CompareTag ("Projectile") && m_PlayerHit != null)
        {
            m_PlayerHit();
        }
	}
}