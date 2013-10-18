using UnityEngine;
using System.Collections;

public struct SAnimationPlayer
{
	public CAnimation AnimRepos;
	public CAnimation AnimHorizontal;
	public CAnimation AnimVertical;
}

public class CPlayer : CCharacter {
	
	// a foutre dans le constructeur si on veut pouvoir le faire indifferement des jouerus
	int m_nResistance = 5;
	int m_nRadiusDiscrectionCircle = 10;
	CGame game;
	
	//CGame m_game = GameObject.Find("_Game").GetComponent<CGame>();
	float m_fSpeed;
	float m_fAngleCone;
	CSpriteSheet m_spriteSheet;
	SAnimationPlayer m_AnimPlayer;
	CConeVision m_ConeVision;
	GameObject m_Torche;
	
	Camera m_CameraCone;
	Vector2 m_DirectionRegard;
	Vector2 m_DirectionDeplacement;
	bool m_bMainCharacter;
	bool m_bHaveObject;
	bool m_bIsAlive;
	
	CCercleDiscretion m_CercleDiscretion;
	CTakeElement m_YounesSuceDesBites;
	SPlayerInput m_PlayerInput;
	
	public enum EMoveModState // mode de deplacement
	{
		e_MoveModState_attente,
		e_MoveModState_discret,
		e_MoveModState_marche,
		e_MoveModState_cours
	}
	
	public enum EState // etat de l'avatar
	{
		e_state_normal,
		e_state_enflamme,
		e_state_oxygeneManque,
		e_state_empoisonne,
		e_state_parasite,
		e_state_frigorifie,
		e_state_aveugle,
		
		e_state_nbState
	}
	
	public enum EIdPlayer // commentaire
	{
		e_IdPlayer_Player1,
		e_IdPlayer_Player2,
		e_IdPlayer_Player3,
		e_IdPlayer_Player4,
	}
	
	EMoveModState m_eMoveModState;
	EState m_eState;
	EIdPlayer m_eIdPlayer;
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public CPlayer(Vector2 posInit, EIdPlayer eIdPlayer, SAnimationPlayer AnimPlayer)
	{
		game = GameObject.Find("_Game").GetComponent<CGame>();
		GameObject prefab = game.prefabPlayer;
		m_GameObject = GameObject.Instantiate(prefab) as GameObject;
		SetPosition2D(posInit);
		
		m_ConeVision = m_GameObject.GetComponent<CConeVision>();
		m_CameraCone = game.m_CameraCone;
		
		m_fSpeed = game.m_fSpeedPlayer;
		m_spriteSheet = new CSpriteSheet(m_GameObject); //m_GameObject.GetComponent<CSpriteSheet>();	
				
		m_AnimPlayer = AnimPlayer;
		
		m_eMoveModState = EMoveModState.e_MoveModState_marche;
		m_eIdPlayer = eIdPlayer;
		
		SetPlayerInput();
		
		m_YounesSuceDesBites = null;
		m_bHaveObject = false;
		m_bIsAlive = true;
		
		m_Torche = m_GameObject.transform.FindChild("Torche").gameObject;
		
		m_CercleDiscretion = m_GameObject.transform.FindChild("CercleDiscretion").GetComponent<CCercleDiscretion>();
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------	
	public new void Init()
	{	
		base.Init();
		
		//Appel a la main des scripts du gameObject
		m_spriteSheet.Init();
		m_ConeVision.Init();
		//m_spriteSheet.SetAnimation(m_AnimRepos);
		m_CercleDiscretion.Init(this);
		
		game.getSoundEngine().setSwitch("Sol", "Metal02", m_GameObject);
	}

	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public new void Reset()
	{
		base.Reset();
	}

	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------	
	public new void Process(float fDeltatime)
	{
		if(m_bIsAlive)
		{
			base.Process(fDeltatime);
			SetPlayerInput();
			MovePlayer(fDeltatime);
			GestionTorche(fDeltatime);
			
			if(game.IsDebug())
			{
				if(Input.GetKeyDown(KeyCode.A))
				{
					m_eState = (m_eState + 1);
					if (m_eState >= EState.e_state_nbState)
						m_eState = EState.e_state_normal;
				}
			}
			
			//gestion de la lampe torche
			if(game.m_bLightIsOn == false)
			{
				m_Torche.SetActiveRecursively(true);
			}
			else
			{
				m_Torche.SetActiveRecursively(false);
			}
			
			//gestion si on tiens un objet
			if(m_bHaveObject)
			{
				m_YounesSuceDesBites.SetPosition2D(m_GameObject.transform.position);
			}
		
			//Appel a la main des scripts du gameObject
			m_spriteSheet.Process();
			if(m_bMainCharacter)
				m_ConeVision.Process();
			
			m_CercleDiscretion.Process();
		}
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------	
	void CalculateSpeed()
	{
		float fVitesseEtat = 1.0f;
		float fVitesseAttitude;
		switch(m_eMoveModState)
		{
			case EMoveModState.e_MoveModState_attente:
			{
				fVitesseAttitude = 0.0f;
				break;
			}
			case EMoveModState.e_MoveModState_discret:
			{
				fVitesseAttitude = game.m_fCoeffSlowWalk;
				break;
			}
			case EMoveModState.e_MoveModState_marche:
			{
				fVitesseAttitude = game.m_fCoeffNormalWalk;
				break;
			}
			case EMoveModState.e_MoveModState_cours	:
			{
				fVitesseAttitude = game.m_fCoeffRunWalk;
				break;
			}
			default: fVitesseAttitude = 1.0f; break;
		}
		
		float fCoeffDirection = Vector2.Dot(m_DirectionRegard, m_DirectionDeplacement);
		fCoeffDirection = game.m_fCoeffReverseWalk + (1.0f - game.m_fCoeffReverseWalk)*(fCoeffDirection + 1)/2;
		
		m_fSpeed = game.m_fSpeedPlayer * fVitesseEtat * fVitesseAttitude * fCoeffDirection;
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public void PickUpObject(CTakeElement obj)
	{
		m_YounesSuceDesBites = obj;
		m_bHaveObject = true;
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public void DropElement()
	{
		m_YounesSuceDesBites = null;
		m_bHaveObject = false;
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	void flipRight ()
	{
		if (m_GameObject.transform.localScale.x < 0)
		{
			Vector3 nTrans = m_GameObject.transform.localScale;
			nTrans.x *= -1;
			m_GameObject.transform.localScale = nTrans;
		}
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	void flipLeft ()
	{
		if (m_GameObject.transform.localScale.x > 0)
		{
			Vector3 nTrans = m_GameObject.transform.localScale;
			nTrans.x *= -1;
			m_GameObject.transform.localScale = nTrans;
		}
	}

	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	void MovePlayer(float fDeltatime)
	{	
		if (m_GameObject.rigidbody != null)
		{			
			Vector3 velocity = Vector3.zero;
			if (m_PlayerInput.MoveUp) 
			{ 
				velocity += new Vector3(0,1,0); 
				m_spriteSheet.SetAnimation(m_AnimPlayer.AnimVertical);
				m_spriteSheet.AnimationStart();
				m_eMoveModState = EMoveModState.e_MoveModState_marche;
			}
			if (m_PlayerInput.MoveDown) 
			{ 
				velocity += new Vector3(0,-1,0); 
				m_spriteSheet.SetAnimation(m_AnimPlayer.AnimVertical);
				m_spriteSheet.AnimationStart();
				m_eMoveModState = EMoveModState.e_MoveModState_marche;
			}
			if (m_PlayerInput.MoveLeft) 
			{
				velocity += new Vector3(-1,0,0); 
				m_spriteSheet.SetAnimation(m_AnimPlayer.AnimHorizontal);
				m_spriteSheet.AnimationStart();
				flipLeft();
				m_eMoveModState = EMoveModState.e_MoveModState_marche;
				
			}
			if (m_PlayerInput.MoveRight) 
			{ 
				velocity += new Vector3(1,0,0); 
				m_spriteSheet.SetAnimation(m_AnimPlayer.AnimHorizontal);
				m_spriteSheet.AnimationStart();
				flipRight();
				m_eMoveModState = EMoveModState.e_MoveModState_marche;
			}
			if(!m_PlayerInput.MoveUp && !m_PlayerInput.MoveDown && !m_PlayerInput.MoveLeft && !m_PlayerInput.MoveRight) 
			{
				m_spriteSheet.SetAnimation(m_AnimPlayer.AnimRepos);
				m_spriteSheet.AnimationStop();
				m_eMoveModState = EMoveModState.e_MoveModState_attente;
				m_GameObject.rigidbody.velocity = Vector3.zero;
			}
			
			velocity.Normalize();
			m_DirectionDeplacement = velocity;
			
			if(m_PlayerInput.WalkFast)
			{
				m_eMoveModState = EMoveModState.e_MoveModState_cours;
			}	
			if(m_PlayerInput.WalkSlow)
			{
				m_eMoveModState = EMoveModState.e_MoveModState_discret;	
			}
			
			CalculateSpeed();
			
			m_GameObject.transform.position += m_fSpeed * velocity * fDeltatime;
		}
		else
		{
			Debug.LogError("Pas de rigid body sur "+m_GameObject.name);
		}
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	void GestionTorche(float fDeltatime)
	{
		float fAngleOld = m_fAngleCone;
		
		if(game.IsPadXBoxMod())
		{
			float fPadY = m_PlayerInput.DirectionHorizontal;
			float fPadX = m_PlayerInput.DirectionVertical;
			float fTolerance = 0.05f;
			if(Mathf.Abs(fPadX) > fTolerance || Mathf.Abs(fPadY) > fTolerance)
				m_DirectionRegard = (new Vector2(fPadX, -fPadY)).normalized;
			
			m_fAngleCone = CApoilMath.ConvertCartesianToPolar(new Vector2(fPadY, fPadX)).y;
		}
		else
		{
			Vector3 posPlayerTmp = m_GameObject.transform.position;
			Vector2 posMouse = CApoilInput.MousePosition;
			Vector2 posPlayer = new Vector2(posPlayerTmp.x, posPlayerTmp.y);
			m_DirectionRegard = (posMouse - posPlayer).normalized;
			m_fAngleCone = Mathf.Acos(Vector2.Dot(m_DirectionRegard, new Vector2(1,0)));
			
			if(posMouse.y < posPlayer.y)
			{
				m_fAngleCone *=-1;
			}
			
			//float fAngleVise = -m_fAngleCone*180/3.14159f - 90 - 75/2;
			//m_ConeVision.setAngleVise(fAngleVise);  
			
			m_fAngleCone +=90.0f + 75.0f/2.0f;	
		}
		
		
		m_Torche.transform.RotateAround(new Vector3(0,0,1),  m_fAngleCone - fAngleOld);
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	void SetPlayerInput()
	{
		switch(m_eIdPlayer)
		{
			case EIdPlayer.e_IdPlayer_Player1:
				m_PlayerInput = CApoilInput.InputPlayer1;
				break;
			case EIdPlayer.e_IdPlayer_Player2:
				m_PlayerInput = CApoilInput.InputPlayer2;
				break;
			case EIdPlayer.e_IdPlayer_Player3:
				m_PlayerInput = CApoilInput.InputPlayer3;
				break;
			case EIdPlayer.e_IdPlayer_Player4:
				m_PlayerInput = CApoilInput.InputPlayer4;
				break;
		}
			
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public void Die()
	{
		m_bIsAlive = false;
		m_GameObject.active = false;
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public EState getState()
	{
		return m_eState;	
	}
	
	public Vector2 getDirectionDeplacement()
	{
		return m_DirectionDeplacement;	
	}
	
	public EMoveModState getMoveModState(){
		return m_eMoveModState;
	}
	
	public float getSpeed()
	{
		return m_fSpeed;
	}
	
	public bool HaveObject()
	{
		return m_bHaveObject;	
	}
	
}
