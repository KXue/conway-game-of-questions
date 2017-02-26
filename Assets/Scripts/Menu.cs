using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	public GameObject m_MainMenu;
	public GameObject m_CreditsMenu;
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		MainOn();
	}
	public void CreditsOn(){
		m_MainMenu.SetActive(false);
		m_CreditsMenu.SetActive(true);
	}
	public void MainOn(){
		m_CreditsMenu.SetActive(false);
		m_MainMenu.SetActive(true);
	}
	public void LoadLevel(){
		SceneManager.LoadScene("Main");
	}
	void Update(){
		if(m_CreditsMenu.activeInHierarchy && Input.GetButtonUp("Cancel")){
			MainOn();
		}
	}
}
