using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bullet : MonoBehaviour {
	public float m_Speed;
	public float m_LifeTime;
	public Vector3 m_DirectionVector;
	public EnemyFactory m_EnemyFactory;
	public SpriteRenderer m_SpriteRenderer;
	// Use this for initialization
	void Start () {
		Destroy(gameObject, m_LifeTime);
	}
	public void SetChar(char text){
		int alphabetIndex = char.ToUpper(text) - 'A';
		m_SpriteRenderer.sprite = m_EnemyFactory.sprites[alphabetIndex];
	}
	// Update is called once per frame
	void FixedUpdate () {
		transform.position += m_DirectionVector * m_Speed * Time.deltaTime;
	}
}
