using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SingletonPool : Pool{
	private static SingletonPool m_instance;
	private static int length = 10;
	private static GameObject prefab;
	public static SingletonPool Instance{
		get{
			if(m_instance == null){
				m_instance = new SingletonPool(prefab, length, true);
			}
			return m_instance;
		}
	}
	public static void SetLength(int newLength){
		length = newLength;
	}
	public static void SetPrefab(GameObject newPrefab){
		prefab = newPrefab;
	}
	public static void ForceInstantiate(GameObject forcedPrefab, int forcedLength, bool isGrowing = false){
		m_instance = new SingletonPool(forcedPrefab, forcedLength, isGrowing);
	}
	public SingletonPool(GameObject prefab, int length, bool isGrowing = false)
	: base(prefab, length, isGrowing){
	}
}
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
	public void DisableAll(){
		foreach(GameObject obj in m_Pool){
			obj.SetActive(false);
		}
	}
	public void ForEach(Action<GameObject> action){
		foreach(GameObject obj in m_Pool){
			action(obj);
		}
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
