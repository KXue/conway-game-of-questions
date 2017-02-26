using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour {
	public CharacterControl m_Player;
	public int m_MaxEnemies;
	public float m_InitialEnemyCount;
	public int m_EnemyCountDeviation;
	public int m_EnemyCountCap;
	public float m_EnemyCountGrowthRate;
	public float m_InitialEnemySpawnInterval;
	public float m_EnemySpawnIntervalDecayFactor;
	public float m_MinEnemySpawnInterval;
	public GameObject m_EnemyPrefab;
	public GameObject m_BulletPrefab;
	public LayerMask m_PlatformLayer;
	public Transform m_SpawnLocation;
	public string[] m_Words;
	private float m_StartTime;
	private float m_EnemySpawnInterval;
	private float m_EnemyCount;
	private Pool m_EnemyPool;
	// Use this for initialization
	void Awake () {
		m_EnemyPool = new Pool(m_EnemyPrefab, m_MaxEnemies, true);
		m_EnemyPool.ForEach(SetTargetPlayer);
		m_EnemyPool.ForEach(MakeChildOfEnemy);

		int longestWord = GetLongestWord();
		EnemyBehavior.m_LongestWord = longestWord;
		
		SingletonPool.ForceInstantiate(m_BulletPrefab, longestWord * 2 * m_EnemyCountCap, true);
		SingletonPool.Instance.ForEach(MakeChildOfEnemy);
		
		Reset();
		
	}
	private void MakeChildOfEnemy(GameObject obj){
		obj.transform.parent = this.transform;
	}
	private void SetTargetPlayer(GameObject obj){
		EnemyBehavior behavior = obj.GetComponent<EnemyBehavior>();
		behavior.m_TargetPlayer = m_Player;
	}
	public void DisableChildren(){
		m_EnemyPool.DisableAll();
		SingletonPool.Instance.DisableAll();
	}
	int GetLongestWord(){
		int longestWord = 0;
		for(int i = 0; i < m_Words.Length; i++){
			if(m_Words[i].Length > longestWord){
				longestWord = m_Words[i].Length;
			}
		}
		return longestWord;
	}
	public void Reset(){
		m_EnemyCount = m_InitialEnemyCount;
		m_StartTime = Time.time - m_EnemySpawnInterval;
		m_EnemySpawnInterval = m_InitialEnemySpawnInterval;
	}
	// Update is called once per frame
	void Update () {
		if(Time.time - m_StartTime > m_EnemySpawnInterval){
			int enemyCount = Mathf.Min(m_EnemyCountCap, (int)Random.Range(m_EnemyCount - m_EnemyCountDeviation, m_EnemyCount + m_EnemyCountDeviation));
			for(int i = 0; i < enemyCount; i++){
				GameObject enemy;
				
				if(m_EnemyPool.TryGetInactiveObject(out enemy)){
					enemy.transform.position = m_SpawnLocation.transform.position;
					enemy.transform.rotation = m_SpawnLocation.transform.rotation;

					EnemyBehavior enemyScript = enemy.GetComponent<EnemyBehavior>();
					enemyScript.m_BulletText = PickAWord();
					enemyScript.m_EndPosition = PickASpot();

					m_StartTime = Time.time;
					
					enemy.SetActive(true);

					if(m_EnemySpawnInterval > m_MinEnemySpawnInterval){
						m_EnemySpawnInterval *= m_EnemySpawnIntervalDecayFactor;
					}
				}
			}
			if(m_EnemyCount < m_EnemyCountCap){
				m_EnemyCount *= m_EnemyCountGrowthRate;
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
