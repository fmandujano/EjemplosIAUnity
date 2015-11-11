using UnityEngine;
using System.Collections;

public class Enemigo : MonoBehaviour
{
	Estado<Enemigo> CurrentState;
	Estado<Enemigo> PreviousState;

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
		if(Input.GetKeyUp(KeyCode.Space ))
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

	public override void OnGUI(Enemigo enemigo)
	{
		GUI.contentColor = Color.red;
		GUI.Label(new Rect(10, 10, 200, 50), "EN ALERTA AMARILLA");
	}
}


