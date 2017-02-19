using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ProjectileBehavior : MonoBehaviour {
	public Transform m_PlayerTransform;
	public Vector3 m_DirectionVector;
	public EnemyFactory m_EnemyFactory;
	public Action<Transform, Vector3> m_Move;
	// Use this for initialization
	void Start () {
		
	}
	void SetChar(char text){
		int alphabetIndex = char.ToUpper(text) - 'A';
		GetComponent<SpriteRenderer>().sprite = m_EnemyFactory.sprites[alphabetIndex];
	}
	// Update is called once per frame
	void Update () {
		m_Move(m_PlayerTransform, m_DirectionVector);
	}
}
