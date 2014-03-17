﻿using UnityEngine;
using System.Collections;

public class CPlayer : MonoBehaviour {

	public GameObject objetRender;
	public GameObject objetTorchLight;

	public enum EIdPlayer 
	{
		e_IdPlayer_Player1,
		e_IdPlayer_Player2,
		e_IdPlayer_Player3,
		e_IdPlayer_Player4,
	}

	enum EStateMove
	{
		e_Attente,
		e_Marche,
		e_Cours
	}

	EIdPlayer m_eIdPlayer;
	EStateMove m_eStateMove;
	SPlayerInput m_PlayerInput;
	Vector2 m_PositionInit;
	Vector2 m_Move;
	Vector2 m_Direction;
	float m_fSpeed;
	float m_fAngleCone;
	int m_nEnergieTorchLight;
	bool m_bActiveTorchLight;

	//-------------------------------------------------------------------------------
	/// Unity
	//-------------------------------------------------------------------------------
	void Start () 
	{
		SetPlayerInput ();
		m_PositionInit = new Vector2 (0, 0);
		m_fSpeed = 1.0f;
		m_eStateMove = EStateMove.e_Attente;
		m_nEnergieTorchLight = CGame.ms_nEnergieTorchLightMax;
		m_bActiveTorchLight = true;
		m_fAngleCone = 0.0f;
		m_Direction = new Vector2 (1.0f, 0.0f);
	}
	
	//-------------------------------------------------------------------------------
	/// Unity
	//-------------------------------------------------------------------------------
	void Update () 
	{
		SetPlayerInput ();
		SetStateMove ();
		Move ();
		ProcessTorchLight ();
	}

	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	void Move()
	{
		if(gameObject.rigidbody2D != null)
		{
			m_Move = Vector2.zero;
			m_Move = new Vector2(m_PlayerInput.MoveHorizontal, m_PlayerInput.MoveVertical);

			if(Mathf.Abs(m_PlayerInput.DirectionHorizontal) > 0.25f || Mathf.Abs(m_PlayerInput.DirectionVertical) > 0.25f)
			{
				m_Direction = new Vector2(m_PlayerInput.DirectionHorizontal, m_PlayerInput.DirectionVertical);
				m_Direction.Normalize();
			}

			Debug.DrawRay(transform.position, 2 * new Vector3(m_Direction.x, m_Direction.y, 0));

			CalculateVelocity();

			gameObject.rigidbody2D.velocity = m_fSpeed * m_Move;
		}
	}

	void CalculateVelocity()
	{
		float fVelocityState = 1.0f;
		float fVelocityAttitude = 1.0f;
		float fCoeffDirection = 1.0f;

		switch(m_eStateMove)
		{
			case EStateMove.e_Attente:
			{
				fVelocityState = 0.0f;
				break;
			}
			case EStateMove.e_Marche:
			{
				fVelocityState = 1.0f;
				break;
			}
			case EStateMove.e_Cours:
			{
				fVelocityState = CGame.ms_fCoeffRun;
				break;
			}
		}

		fCoeffDirection = Vector2.Dot (m_Direction, m_Move);
		fCoeffDirection = CGame.ms_fCoeffReverseWalk + (1 - CGame.ms_fCoeffReverseWalk) * (fCoeffDirection + 1.0f) / 2.0f;

		m_fSpeed = CGame.ms_fVelocityPlayer * fVelocityState * fVelocityAttitude * fCoeffDirection;
	}

	void ProcessTorchLight()
	{
		if(m_bActiveTorchLight && m_nEnergieTorchLight > 0)
		{
			float fAngleOld = m_fAngleCone;

			if(!objetTorchLight.activeSelf)
				objetTorchLight.SetActive(true);


			m_fAngleCone = CApoilMath.ConvertCartesianToPolar(m_Direction).y;

			objetTorchLight.transform.RotateAround(new Vector3(0,0,1),  m_fAngleCone - fAngleOld);

			//m_nEnergieTorchLight--;
		}
		else
		{
			if(objetTorchLight.activeSelf)
				objetTorchLight.SetActive(false);
		}
	}

	//-------------------------------------------------------------------------------
	/// 
	//-------------------------------------------------------------------------------
	void SetPlayerInput()
	{
		int idPlayer = 0;
		switch(m_eIdPlayer)
		{
			case EIdPlayer.e_IdPlayer_Player1:
				idPlayer = 0;
				break;
			case EIdPlayer.e_IdPlayer_Player2:
				idPlayer = 1;
				break;
			case EIdPlayer.e_IdPlayer_Player3:
				idPlayer = 2;
				break;
			case EIdPlayer.e_IdPlayer_Player4:
				idPlayer = 3;
				break;
		}
		m_PlayerInput = CApoilInput.InputPlayer[idPlayer];
	}

	void SetStateMove()
	{
		if(Mathf.Abs(m_PlayerInput.MoveHorizontal) > 0.1 ||  Mathf.Abs(m_PlayerInput.MoveVertical) > 0.1)
		{
			m_eStateMove = EStateMove.e_Marche;
			if(m_PlayerInput.Run)
				m_eStateMove = EStateMove.e_Cours;
		}
		else
		{	
			m_eStateMove = EStateMove.e_Attente;
		}
	}

	//-------------------------------------------------------------------------------
	/// 
	//-------------------------------------------------------------------------------
	public void SetIdPlayer(int nId)
	{
		switch(nId)
		{
			case 0:
				m_eIdPlayer = EIdPlayer.e_IdPlayer_Player1;
				break;
			case 1:
				m_eIdPlayer = EIdPlayer.e_IdPlayer_Player2;
				break;
			case 2:
				m_eIdPlayer = EIdPlayer.e_IdPlayer_Player3;
				break;
			case 3:
				m_eIdPlayer = EIdPlayer.e_IdPlayer_Player4;
				break;
		}

		//ordonner en Z pour eviter les chevauchements
		objetRender.renderer.sortingOrder = 50+nId;
	}

	public EIdPlayer GetIdPlayer()
	{
		return m_eIdPlayer;
	}

	//-------------------------------------------------------------------------------
	/// 
	//-------------------------------------------------------------------------------
	public void SetPosition2D(Vector2 pos2D)
	{
		Vector3 pos3D = gameObject.transform.position;
		pos3D.x = pos2D.x;
		pos3D.y = pos2D.y;
		gameObject.transform.position = pos3D;
	}

	public void SetPositionInit(Vector2 pos2D)
	{
		m_PositionInit = pos2D;
	}
}
