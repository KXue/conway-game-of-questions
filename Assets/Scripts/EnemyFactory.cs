using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour {
	public Sprite[] sprites;
	public CharacterControl m_Player;
	public float m_InitialEnemySpawnInterval;
	public float m_EnemySpawnIntervalDecayFactor;
	public float m_MinEnemySpawnInterval;
	public EnemyBehavior m_EnemyPrefab;
	public LayerMask m_PlatformLayer;
	public string[] m_Words;
	private Transform m_SpawnLocation;
	private float m_StartTime;
	private float m_EnemySpawnInterval;
	// Use this for initialization
	void Awake () {
		m_SpawnLocation = transform.FindChild("EnemySpawnPoint");
		Reset();
	}
	public void Reset(){
		m_StartTime = Time.time;
		m_EnemySpawnInterval = m_InitialEnemySpawnInterval;
	}
	// Update is called once per frame
	void Update () {
		if(Time.time - m_StartTime > m_EnemySpawnInterval){
			EnemyBehavior enemy = Instantiate(m_EnemyPrefab, m_SpawnLocation);
			enemy.m_BulletText = PickAWord();
			enemy.m_EndPosition = PickASpot();
			enemy.m_TargetPlayer = m_Player;
			enemy.m_EnemyFactory = this;
			m_StartTime = Time.time;
			Debug.Log(m_EnemySpawnInterval);
			if(m_EnemySpawnInterval > m_MinEnemySpawnInterval){
				m_EnemySpawnInterval *= m_EnemySpawnIntervalDecayFactor;
			}
		}
	}

	private string PickAWord(){
		return m_Words[(int)Random.Range(0, m_Words.Length)];
	}
	private Vector3 PickASpot(){
		float angle = Random.Range(0f, Mathf.PI);
		Vector2 directionVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		float offsetValue = 0.5f;
		Vector2 offsetVector = new Vector2(0f, offsetValue);
		
		if(angle < Mathf.PI/2f){
			offsetVector.x += offsetValue;
		}
		else{
			offsetVector.x -= offsetValue;
		}
		
		RaycastHit2D hit = Physics2D.Raycast(Vector2.zero, directionVector, Mathf.Infinity, m_PlatformLayer);

		Vector3 hitPoint = hit.point;
		
		if(Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.y)){
			hitPoint.x -= offsetVector.x;
		}
		else{
			hitPoint.y -= offsetVector.y;
		}
		
		return hitPoint;
	}
}
