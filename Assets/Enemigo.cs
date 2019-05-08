using UnityEngine;
using System.Collections;

public class Enemigo : MonoBehaviour
{
	Estado<Enemigo> CurrentState;
	Estado<Enemigo> PreviousState;

	public SteeringBehaviours CE;
	public MovingEntity ME;

	//cambiar estado
	public void ChangeState(Estado<Enemigo> NewState)
	{
		PreviousState = CurrentState;
 
		if (CurrentState != null)
			CurrentState.Exit(this);

		CurrentState = NewState;
 
		if (CurrentState != null)
			CurrentState.Enter(this);
	}

	// Use this for initialization
	void Start ()
	{
		CE = GetComponent<SteeringBehaviours>();
		ME = GetComponent<MovingEntity>();
		CurrentState = EEnemigoPatrullar.Instance;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (CurrentState != null) CurrentState.Execute(this);
	}

	void OnGUI()
	{
		if (CurrentState != null) CurrentState.OnGUI(this);
	}
}


//estado de patrullar
public class EEnemigoPatrullar : Estado<Enemigo>
{
	static readonly EEnemigoPatrullar instance = new EEnemigoPatrullar();
	public static EEnemigoPatrullar Instance { get { return instance; } }
	static EEnemigoPatrullar() { }
	protected EEnemigoPatrullar() { }

	public override void OnGUI(Enemigo enemigo)
	{
		GUI.contentColor = Color.red;
		GUI.Label(new Rect(10, 10, 200, 50), "PATRULLANDO");
	}


	public override void Execute(Enemigo entity)
	{
		entity.ME.Fuerza = entity.CE.Patrullar();

		//radio de escucha de ruidos 
		if( (entity.CE.posCursor - entity.transform.position).sqrMagnitude < 15  )
		{
			entity.ChangeState(EEnemigoAlertaAmarilla.Instance);
		}

	}
}

//estado alerta amarilla
public class EEnemigoAlertaAmarilla : Estado<Enemigo>
{
	static readonly EEnemigoAlertaAmarilla instance = new EEnemigoAlertaAmarilla();
	public static EEnemigoAlertaAmarilla Instance { get { return instance; } }
	static EEnemigoAlertaAmarilla() { }
	protected EEnemigoAlertaAmarilla() { }

	public override void Execute(Enemigo entity)
	{
		entity.ME.Fuerza = entity.CE.arrive(entity.CE.posCursor);

		//obtener el angulo con el jugador
		Vector3 AB =  Vector3.Normalize(  entity.CE.posCursor - entity.transform.position);
		Vector3 heading = entity.ME.Velocidad.normalized;
		
		//si esta dentro del cono de vision
		if ( Vector3.Dot(AB, heading) > 0.8)
		{
			entity.ChangeState( EEnemigoAlertaRoja.Instance);
		}	

	}
	public override void OnGUI(Enemigo enemigo)
	{
		GUI.contentColor = Color.red;
		GUI.Label(new Rect(10, 10, 200, 50), "EN ALERTA AMARILLA");
	}
}


public class EEnemigoAlertaRoja : Estado<Enemigo>
{
	static readonly EEnemigoAlertaRoja instance = new EEnemigoAlertaRoja();
	public static EEnemigoAlertaRoja Instance { get { return instance; } }
	static EEnemigoAlertaRoja() { }
	protected EEnemigoAlertaRoja() { }

	public override void OnGUI(Enemigo enemigo)
	{
		GUI.contentColor = Color.red;
		GUI.Label(new Rect(100, 100, 200, 50), "EN ALERTA ROJA MATAR O MORIR");
	}

	public override void Execute(Enemigo entity)
	{
		entity.ME.Fuerza = entity.CE.seek(entity.CE.posCursor);



		//obtener el angulo con el jugador
		Vector3 AB = Vector3.Normalize(entity.CE.posCursor - entity.transform.position);
		Vector3 heading = entity.ME.Velocidad.normalized;

	}
}
