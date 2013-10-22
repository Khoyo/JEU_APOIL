using UnityEngine;
using System.Collections;

public class CElement
{
	protected GameObject m_GameObject;
	CGame m_Game;
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public CElement()
	{
		
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public virtual void Init()
	{	
		m_Game = GameObject.Find("_Game").GetComponent<CGame>();
	}
	
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public virtual void Reset()
	{
		
	}

	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public virtual void Process(float fDeltatime)
	{
		
	}
	
	public virtual void Destroy()
	{
		m_Game.getLevel().UnregisterElement(this);
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public GameObject getGameObject()
	{
		return m_GameObject;	
	}
	
	public void setGameObject(GameObject gameObject){
		m_GameObject = gameObject;
	}
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public void SetPosition2D(Vector2 pos2D)
	{
		Vector3 pos = m_GameObject.transform.position;
		pos.x = pos2D.x;
		pos.y = pos2D.y;
		pos.z = -2;
		m_GameObject.transform.position = pos;
	}

}
