using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bullet : MonoBehaviour {
	public Sprite[] sprites;
	public float m_Speed;
	public float m_LifeTime;
	public Vector3 m_DirectionVector;
	public SpriteRenderer m_SpriteRenderer;
	void OnEnable()
	{
		Invoke("Destroy", m_LifeTime);
	} 
	void Destroy(){
		gameObject.SetActive(false);
	}
	void OnDisable()
	{
		CancelInvoke();
	}
	public void SetChar(char text){
		int alphabetIndex = char.ToUpper(text) - 'A';
		m_SpriteRenderer.sprite = sprites[alphabetIndex];
	}
	// Update is called once per frame
	void FixedUpdate () {
		transform.position += m_DirectionVector * m_Speed * Time.deltaTime;
	}
}
