using UnityEngine;
using System.Collections;

public class CLevel
{
	CPlayer m_Player;
	CPlayer m_Player2;
	CPlayer m_Player3;
	CMonster m_Monster;
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public CLevel()
	{
		Vector2 posInit = new Vector2(0.0f, 0.0f);
		Vector2 posInitM = new Vector2(100.0f, 0.0f);
		m_Player = new CPlayer(posInit, true);
		//m_Player2 =  new CPlayer();
		//m_Player3 =  new CPlayer();
		m_Monster = new CMonster(posInitM);
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public void Init()
	{	
		m_Player.Init();
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public void Reset()
	{
		m_Player.Reset();
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public void Process(float fDeltatime)
	{
		m_Player.Process(fDeltatime);
		
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public CPlayer getPlayer()
	{
		return m_Player;
	}
}
