using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBehavior : MonoBehaviour {
	public string m_BulletText;
	public Action<Transform, Vector3> m_MoveAction;
	public float m_TransitionTime;
	public Vector3 m_EndPosition;
	public CharacterControl m_TargetPlayer;
	private  Vector3 m_StartPosition;
	private float m_StartTime;
	private bool m_ReachedTarget = false;
	private bool m_ShotProjectiles = false;
	// Use this for initialization
	void Start () {
		m_StartTime = Time.time;
		m_StartPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePosition();
	}
	void UpdatePosition(){
		float timeFractionElapsed = (Time.time - m_StartTime) / m_TransitionTime;
		
		if(timeFractionElapsed <= 1 && timeFractionElapsed >=0){
			float quadTime = QuadraticLerp(timeFractionElapsed);
			transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, quadTime);
		}
		Vector3 TargetVector = m_TargetPlayer.transform.position - transform.position;
		float angle = Mathf.Atan2(TargetVector.y, TargetVector.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = q;
	}
	private float QuadraticLerp(float x){
		return 2 * x - x * x;
	}
}
