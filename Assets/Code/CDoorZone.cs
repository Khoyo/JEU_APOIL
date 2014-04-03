﻿using UnityEngine;
using System.Collections;

public class CDoorZone : MonoBehaviour {

	public GameObject objetDoor;
	
	CDoor m_Door;
	float m_fTimerStopBlockClose;

	// Use this for initialization
	void Start () 
	{
		m_Door = objetDoor.GetComponent<CDoor> ();
		m_fTimerStopBlockClose = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_fTimerStopBlockClose += Time.deltaTime;
		if(m_fTimerStopBlockClose > 0.2f)
		{
			m_Door.SetBlockClose(false);
			m_fTimerStopBlockClose = 0.0f;
		}
	}

	//-------------------------------------------------------------------------------
	/// Unity
	//-------------------------------------------------------------------------------
	void OnTriggerStay2D(Collider2D other) 
	{
		if(other.CompareTag("Player"))
		{
			m_Door.Open();
			m_Door.SetBlockClose(true);
		}
	}
}
