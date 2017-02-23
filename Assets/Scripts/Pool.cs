using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool{
	private List<GameObject> m_Pool;
	private bool m_IsGrowing;
	private GameObject m_Prefab;
	
	public Pool(GameObject prefab, int length, bool isGrowing = false){
		m_Pool = new List<GameObject>();
		m_IsGrowing = isGrowing;
		m_Prefab = prefab;
		for(int i = 0; i < length; i++){
			GameObject obj = SpawnObject();
			m_Pool.Add(obj);
		}
	}
	private GameObject SpawnObject(){
		GameObject obj = GameObject.Instantiate(m_Prefab);
		obj.SetActive(false);
		return obj;
	}
	public bool TryGetInactiveObject(out GameObject retObj){
		bool foundObject = false;
		retObj = null;
		
		for(int i = 0; i < m_Pool.Count; i++){
			if(!m_Pool[i].activeInHierarchy){
				retObj = m_Pool[i];
				foundObject = true;
				break;
			}
		}

		if(!foundObject && m_IsGrowing){
			retObj = SpawnObject();
			m_Pool.Add(retObj);
			foundObject = true;
		}

		return foundObject;
	}
}
