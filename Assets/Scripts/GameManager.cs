using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {
	public CharacterControl m_Player;
	public EnemyFactory m_EnemyFactory;
	public Transform m_PlayerStart;
	public Text m_GameOverText;
	private bool m_PlayerHit = false;
	private bool m_Start = true;

	// Use this for initialization
	void Start () {
		m_Player.transform.position = m_PlayerStart.transform.position;
		StartCoroutine(GameLoop());
	}
	private void PlayerHit(){
		m_PlayerHit = true;
	}
	private IEnumerator GameLoop(){
		yield return StartCoroutine(GamePlaying());
		yield return StartCoroutine(GameOver());

		StartCoroutine (GameLoop ());
	}
	private IEnumerator GamePlaying(){
		EnableGame();
		m_GameOverText.text = "";
		while(!m_PlayerHit){
			yield return null;
		}
	}

	private IEnumerator GameOver(){
		DisableGame();
		m_GameOverText.text = "You've been hit. Press start to restart.";
		while(!m_Start){
			yield return null;
		}
	}
	void DisableGame(){
		m_EnemyFactory.enabled = false;
		m_Player.enabled = false;
		m_Player.m_PlayerHit -= PlayerHit;
		m_Start = false;
	}
	void EnableGame(){
		m_EnemyFactory.enabled = true;
		m_EnemyFactory.Reset();
		m_Player.enabled = true;
		m_Player.m_PlayerHit = PlayerHit;
		m_PlayerHit = false;
		m_Player.transform.position = m_PlayerStart.position;
	}
	// Update is called once per frame
	void Update () {
		if(!m_Start && Input.GetButtonDown("Start")){
			m_Start = true;
		}
	}
}
