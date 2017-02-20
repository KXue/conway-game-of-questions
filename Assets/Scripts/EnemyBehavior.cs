﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBehavior : MonoBehaviour {
	public string m_BulletText;
	public float m_TransitionTime;
	public float m_BulletIntervalTime;
	public Vector3 m_EndPosition;
	public CharacterControl m_TargetPlayer;
	public EnemyFactory m_EnemyFactory;
	public Bullet m_BulletPrefab;
	private  Vector3 m_StartPosition;
	private float m_StartTime;
	private bool m_ReachedTarget = false;
	private bool m_ShotProjectiles = false;
	private int m_MessageIndex = 0;
	// Use this for initialization
	void Start () {
		Debug.Log(m_EnemyFactory);
		m_StartTime = Time.time;
		m_StartPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePosition();
	}
	void UpdatePosition(){
		float timeFractionElapsed = (Time.time - m_StartTime) / m_TransitionTime;
		
		Vector3 TargetVector = m_TargetPlayer.transform.position - transform.position;
		float angle = Mathf.Atan2(TargetVector.y, TargetVector.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = q;

		//Holy Hell I'm sleep deprived
		if(!m_ReachedTarget && timeFractionElapsed <= 1 && timeFractionElapsed >=0){
			float quadTime = QuadraticLerp(timeFractionElapsed);
			transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, quadTime);
		}
		else if(!m_ReachedTarget && timeFractionElapsed > 1){
			m_ReachedTarget = true;
			m_StartTime = Time.time;
		}
		else if(m_ReachedTarget && !m_ShotProjectiles){
			ShootQuestion();
		}
		else if(m_ShotProjectiles && timeFractionElapsed <= 1 && timeFractionElapsed >=0){
			float quadTime = QuadraticLerp(timeFractionElapsed);
			transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, 1 - quadTime);
		}
		else{
			Destroy(gameObject);
		}
	}
	void ShootQuestion(){
		//Time to shoot
		if(Time.time - m_StartTime > m_BulletIntervalTime){
			Vector3 TargetVector = m_TargetPlayer.transform.position - transform.position;
			Bullet bullet = Instantiate(m_BulletPrefab, transform.position + 0.8f * TargetVector.normalized, transform.rotation);
			
			bullet.m_DirectionVector = TargetVector.normalized;
			bullet.m_EnemyFactory = m_EnemyFactory;
			bullet.SetChar(m_BulletText[m_MessageIndex]);

			if(TargetVector.x < 0){
				bullet.transform.Rotate(0f, 0f, 180f);
			}
			
			m_StartTime = Time.time;
			m_MessageIndex++;
			if(m_MessageIndex == m_BulletText.Length){
				m_ShotProjectiles = true;
			}
		}
	}
	private float QuadraticLerp(float x){
		return 2 * x - x * x;
	}
}