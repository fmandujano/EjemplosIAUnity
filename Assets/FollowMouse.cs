using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour 
{
	MovingEntity entity;

	Vector3 posCursor;
	// Use this for initialization
	void Start ()
	{
		entity = transform.GetComponent<MovingEntity>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//aplicar fuerza al entity
		//calcular posicion del cursor en el plano xy del agente
		posCursor = Camera.main.ScreenToWorldPoint(
				new Vector3(Input.mousePosition.x , 
							Input.mousePosition.y, 
							20));

		Vector3 a = posCursor - entity.transform.position;
		a.z = 0;

		entity.Fuerza = (a - entity.Velocidad).normalized * entity.fuerzaMaxima;


	}

	void OnDrawGizmos()
	{
        Gizmos.color = Color.green;
		if (entity != null)
		{
			Gizmos.DrawSphere(posCursor, 0.3F);
			Gizmos.DrawLine(transform.position, transform.position + entity.Fuerza);
		}
	}
}
