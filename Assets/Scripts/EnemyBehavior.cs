using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBehavior : MonoBehaviour {
	public static int m_LongestWord;
	public string m_BulletText;
	public float m_TransitionTime;
	public float m_BulletIntervalTime;
	public float m_Deviation;
	public Vector3 m_EndPosition;
	public CharacterControl m_TargetPlayer;
	private  Vector3 m_StartPosition;
	private float m_StartTime;
	private bool m_ReachedTarget = false;
	private bool m_ShotProjectiles = false;
	private int m_MessageIndex = 0;
	void OnEnable () {
		m_StartTime = Time.time;
		m_StartPosition = transform.position;
		m_ReachedTarget = false;
		m_ShotProjectiles = false;
		m_MessageIndex = 0;
	}
	void FixedUpdate () {
		UpdatePosition();
	}
	void UpdatePosition(){
		float timeFractionElapsed = (Time.time - m_StartTime) / m_TransitionTime;
		
		Vector3 TargetVector = m_TargetPlayer.transform.position - transform.position;
		float angle = Mathf.Atan2(TargetVector.y, TargetVector.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

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
			float quadTime = QuadraticLerp(1 - timeFractionElapsed);
			transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, quadTime);
		}
		else{
			Destroy();
		}
	}
	void Destroy(){
		gameObject.SetActive(false);
	}
	void ShootQuestion(){
		//Time to shoot
		
		if(Time.time - m_StartTime > m_BulletIntervalTime){
			Vector3 targetVector = m_TargetPlayer.transform.position - transform.position;
			GameObject bullet;
			if(SingletonPool.Instance.TryGetInactiveObject(out bullet)){
				bullet.transform.position = transform.position;
				bullet.transform.rotation = transform.rotation;

				Bullet bulletScript = bullet.GetComponent<Bullet>();
				Vector3 deviationVector = new Vector3 (UnityEngine.Random.Range(- m_Deviation, m_Deviation),
				UnityEngine.Random.Range(-m_Deviation, m_Deviation));
				bulletScript.m_DirectionVector = (deviationVector + targetVector).normalized;
				bulletScript.SetChar(m_BulletText[m_MessageIndex]);

				if(targetVector.x < 0){
					bullet.transform.Rotate(0f, 0f, 180f);
				}
				
				bullet.gameObject.SetActive(true);
				
				m_StartTime = Time.time;
				m_MessageIndex++;
		
				if(m_MessageIndex == m_BulletText.Length){
					m_ShotProjectiles = true;
				}
			}
		}
	}
	private float QuadraticLerp(float x){
		return 2 * x - x * x;
	}
}
