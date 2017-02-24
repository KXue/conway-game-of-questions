using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	public Canvas m_MainCanvas;
	public Canvas m_CreditsCanvas;
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		m_MainCanvas.enabled = true;
		m_CreditsCanvas.enabled = false;
	}
	public void CreditsOn(){
		m_MainCanvas.enabled = false;
		m_CreditsCanvas.enabled = true;
	}
	public void MainOn(){
		m_CreditsCanvas.enabled = false;
		m_MainCanvas.enabled = true;
	}
	public void LoadLevel(){
		SceneManager.LoadScene("Main");
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
