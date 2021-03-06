﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public CharacterControl m_Player;
	public EnemyFactory m_EnemyFactory;
	public Transform m_PlayerStart;
	public GameObject m_GameOverMenu;
	public GameObject m_ScoreContainer;
	public Text m_ScoreText;
	public Text m_HighScoreText;
	public float m_SlowMoTime;
	private bool m_PlayerHit = false;
	private bool m_Start = true;
	private float m_GameStartTime;
	private float m_Score = 0f;
	private float m_HighScore = 0f;
	private WaitForSeconds m_GameOverTransitionTime;
	// Use this for initialization
	void Start () {
		m_HighScore = LoadScore();
		m_Player.transform.position = m_PlayerStart.transform.position;
		m_GameOverTransitionTime = new WaitForSeconds(m_SlowMoTime);

		StartCoroutine(GameLoop());
	}
	private float LoadScore(){
		return PlayerPrefs.GetFloat("HighScore");
	}
	private void PlayerHit(){
		m_PlayerHit = true;
	}
	private void SaveScore(){
		PlayerPrefs.SetFloat("HighScore", m_HighScore);
	}	
	private void UpdateScore(){
		m_Score = (Time.time - m_GameStartTime);
		if(m_Score >= m_HighScore){
			m_HighScore = m_Score;
		}
	}
	private void UpdateScoreUI(){
		m_ScoreText.text = "Score: " + m_Score.ToString("F1");
		m_HighScoreText.text = "High Score: " + m_HighScore.ToString("F1");
	}
	private IEnumerator GameLoop(){
		DisableGameOverUI();
		yield return StartCoroutine(GamePlaying());
		yield return StartCoroutine(GameOver());

		StartCoroutine (GameLoop());
	}
	private IEnumerator GamePlaying(){
		EnableGame();
		m_GameStartTime = Time.time;
		m_Player.enabled = true;
		while(!m_PlayerHit){
			UpdateScore();
			UpdateScoreUI();
			yield return null;
		}
		m_Player.enabled = false;
		m_Player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		BeginSlowMo(m_SlowMoTime);
		yield return m_GameOverTransitionTime;
	}
	private void DisableGameOverUI(){
		m_GameOverMenu.SetActive(false);
	}
	private void EnableGameOverUI(){
		m_GameOverMenu.SetActive(true);
	}
	private IEnumerator GameOver(){
		DisableGame();
		EnableGameOverUI();
		while(!m_Start){
			yield return null;
		}
	}
	private void BeginSlowMo(float slowMoTime){
		Time.timeScale = 0.5f;
		Invoke("EndSlowMo", slowMoTime * Time.timeScale);
	}
	private void EndSlowMo(){
		Time.timeScale = 1f;
	}
	void DisableGame(){
		m_EnemyFactory.DisableChildren();
		m_EnemyFactory.gameObject.SetActive(false);
		
		m_Player.gameObject.SetActive(false);
		m_Player.m_PlayerHit -= PlayerHit;

		
		m_Start = false;
	}
	void EnableGame(){
		m_EnemyFactory.gameObject.SetActive(true);
		m_EnemyFactory.Reset();

		m_Player.gameObject.SetActive(true);
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
	public void RestartButtonPressed(){
		m_Start = true;
	}
	public void ExitButtonPressed(){
		SaveScore();
		SceneManager.LoadScene("Menu");
	}
}
