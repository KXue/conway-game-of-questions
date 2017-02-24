using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuEnable : MonoBehaviour {
	public GameObject m_Firstbutton;
	public Button[] m_Buttons;
	public EventSystem m_EventSystem;
	
	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	void OnEnable()
	{
		m_EventSystem.SetSelectedGameObject(m_Firstbutton);
	}

	/// <summary>
	/// This function is called when the behaviour becomes disabled or inactive.
	/// </summary>
	void OnDisable()
	{
		m_EventSystem.SetSelectedGameObject(null);
		foreach(GameObject child in transform){
			child.SetActive(false);
		}
	}
}
