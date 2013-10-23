using UnityEngine;
using System.Collections;

public class CMachinePorte : MonoBehaviour, IMachineAction 
{	
	bool m_bIsOpen;
	bool m_bChangeState;
	float m_fTimer;
	const float m_fTimerMax = 3.0f;
	
	public void Activate(CPlayer player)
	{
		if(!m_bIsOpen)
			Open();
		else
			Close();
	}
	
	public void Init()
	{
		m_bIsOpen = false;
		m_bChangeState = false;
		gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().setEndCondition(CSpriteSheet.EEndCondition.e_Stop);
		gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().AnimationStop();
		m_fTimer = 0.0f;
	}
	
	public void Process()
	{
		if(gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().IsEnd() && m_bChangeState)
		{
			m_bIsOpen = !m_bIsOpen;	
			m_bChangeState = false;
		}
		
		gameObject.collider.isTrigger = m_bIsOpen;
		
		if(m_bIsOpen && !m_bChangeState)
		{
			if(m_fTimer < m_fTimerMax)
				m_fTimer += Time.deltaTime;
			else
				Close();
		}
			
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public void Open()
	{
		gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().Reset();	
		gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().SetDirection(true);
		m_bChangeState = true;
		gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().AnimationStart();	
		m_fTimer = 0.0f;
	}
	
	public void Close()
	{
		gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().ResetAtEnd();
		gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().SetDirection(false);
		m_bChangeState = true;
		gameObject.GetComponent<CScriptMachine>().GetMachine().GetSpriteSheet().AnimationStart();	
	}
}