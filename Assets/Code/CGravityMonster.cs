using UnityEngine;
using System.Collections;

public class CGravityMonster : CElement 
{
	CScriptGravityMonster m_ScriptGravityMonster;
	CScriptGravityMonsterZone m_ScriptGravityMonsterZone;
	CSpriteSheet m_SpriteSheet;
	CPlayer m_PlayerAttracted;
	float m_fTimerPrison;
	bool m_bPlayerInJail;


	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------	
	public CGravityMonster()
	{
		m_PlayerAttracted = null;
		m_fTimerPrison = 0.0f;
		m_bPlayerInJail = false;
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------	
	public override void Init()
	{	
		base.Init();
		m_ScriptGravityMonster = m_GameObject.GetComponent<CScriptGravityMonster>();
		m_ScriptGravityMonster.SetGravityMonsterElement(this);
		m_ScriptGravityMonsterZone = m_GameObject.GetComponentInChildren<CScriptGravityMonsterZone>();
		m_ScriptGravityMonsterZone.SetGravityMonsterElement(this);
		
		CAnimation anim = new CAnimation(m_ScriptGravityMonster.GetMaterial(), 3, 2, 1.0f);
		
		m_SpriteSheet = new CSpriteSheet(m_GameObject);
		m_Game = GameObject.Find("_Game").GetComponent<CGame>();
			
		m_SpriteSheet.Init();
		m_SpriteSheet.SetAnimation(anim);
		m_SpriteSheet.setEndCondition(CSpriteSheet.EEndCondition.e_Loop);
		m_SpriteSheet.AnimationStart();
	}

	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public override void Reset()
	{
		base.Reset();
		m_SpriteSheet.Reset();
		m_ScriptGravityMonster.Reset();
		m_ScriptGravityMonsterZone.Reset();
		m_fTimerPrison = 0.0f;
		m_bPlayerInJail = false;
	}
	

	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------	
	public override void Process(float fDeltatime)
	{
		base.Process(fDeltatime);
		m_SpriteSheet.Process();
		
		if(m_bPlayerInJail)
		{
			if(m_fTimerPrison < m_Game.m_fGravityTimerPrisonMax)
				m_fTimerPrison += fDeltatime;
			else
			{
				m_PlayerAttracted.SetParalyse(false);	
				DropPlayer(m_PlayerAttracted);
				m_bPlayerInJail = false;
			}
		}	
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public CSpriteSheet GetSpriteSheet()
	{
		return m_SpriteSheet;	
	}
	
	public void CatchPlayer(CPlayer player)
	{
		if(m_PlayerAttracted == null)
		{
			m_PlayerAttracted = player;
			m_PlayerAttracted.SetMagnetData(m_GameObject.transform.position, m_ScriptGravityMonster.m_fForceMagnet);	
		}
	}
	
	public void DropPlayer(CPlayer player)
	{
		if(m_PlayerAttracted == player)
		{
			m_PlayerAttracted.StopMagnet();
			m_PlayerAttracted = null;
		}
	}
	
	public void JailPlayer(CPlayer player)
	{
		DropPlayer(player);
		CatchPlayer(player);
		m_fTimerPrison = 0.0f;
		m_bPlayerInJail = true;
		m_PlayerAttracted.SetParalyse(true);
	}
}
