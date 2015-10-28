using UnityEngine;
using System.Collections;

public class SteeringBehaviours : MonoBehaviour
{
	MovingEntity entity;
	Vector3 posCursor;
	Vector3 posCursorAnt;

	public float VagarDistancia;
	public float VagarRadio;
	public float VagarJitter;
	private float vagarAngulo;

	Vector3[] antenas;
	public float magAntena = 10;

	void Start()
	{
		entity = transform.GetComponent<MovingEntity>();
		//entity.Fuerza = Vector3.left;

		antenas = new Vector3[3]
		{
			//antena hacia el frente
			new Vector3(1,0,0), 
			//antena hacia la izquierda
			new Vector3(0.7071f, 0.7071f, 0 ),
			//antena hacia la derecha de la nave
			new Vector3(0.7071f, -0.7071f, 0)
		};
	}

	// Update is called once per frame
	void Update ()
	{
		//calcular posicion del cursor en el plano xy del agente
		posCursor = Camera.main.ScreenToWorldPoint(
				new Vector3(Input.mousePosition.x,
							Input.mousePosition.y,
							20));

		//calcular velocidad del cursor
		Vector3 velCursor = (posCursorAnt - posCursor) * Time.deltaTime;

		//promedio de fuerzas
		//entity.Fuerza = (WallAvoidance() + wander() + flee(posCursor)) / 3;

		//suma prioritizada
		//entity.Fuerza = WallAvoidance() * 0.1f + 
		//					wander() * 0.2f +
		//					flee(posCursor) * 0.7f;

		//tramado prioritizado , prioritized dithering
		float dado = Random.value;
		if(dado<0.4f)
		{
			entity.Fuerza =WallAvoidance();
		}
		else if(dado >= 0.4f  && dado < 0.7f)
		{
			entity.Fuerza =wander();
		}
		else
		{
			entity.Fuerza =flee(posCursor);
		}
		/*
		if (Input.GetKey(KeyCode.A))
			entity.Fuerza = seek(posCursor);
		else if (Input.GetKey(KeyCode.S))
			entity.Fuerza = arrive(posCursor);
		else if (Input.GetKey(KeyCode.D))
			entity.Fuerza = flee(posCursor);
		else if (Input.GetKey(KeyCode.F))
			entity.Fuerza = intercept(posCursor, velCursor);
		else
			entity.Fuerza = Vector3.zero;
		*/


		posCursorAnt = posCursor;
	}
	

	/// <summary>
	/// Comportamiento de buscar
	/// </summary>
	/// <param name="objetivo"></param>
	/// <returns></returns>
	Vector3 seek(Vector3 objetivo)
	{
		Vector3 a = objetivo - entity.transform.position;
		a.z = 0;
		return (a - entity.Velocidad).normalized * entity.fuerzaMaxima;
	}

	/// <summary>
	/// Comportamiento de arribar
	/// </summary>
	/// <param name="objetivo"></param>
	/// <returns></returns>
	Vector3 arrive(Vector3 objetivo)
	{
		Vector3 a = objetivo - entity.transform.position;
		a.z = 0;

		float dist = a.sqrMagnitude;
		Vector3 fres = (a - entity.Velocidad).normalized * entity.fuerzaMaxima;
		//moderar la fuerza resultante si la distancia al objetivo es menor a cierto umbral
		if (dist < 50)
		{
			fres *= (dist / 50);
		}
		return fres;
	}

	Vector3 flee(Vector3 objetivo)
	{
		Vector3 a = objetivo - entity.transform.position;
		a.z = 0;
		return (a - entity.Velocidad).normalized * entity.fuerzaMaxima *-1;
	}

	/// <summary>
	/// comportamiento de perseguir o interceptar
	/// </summary>
	/// <param name="objetivo"></param>
	/// <returns></returns>
	Vector3 intercept(Vector3 objetivo, Vector3 velocidadObjetivo)
	{
		//estimar el tiempo de llegada en base a nuestra velocidad
		float distObj = (objetivo - transform.position).sqrMagnitude ;
		float tLlegada = distObj / entity.Velocidad.sqrMagnitude;
		//estimar posicion futura en base a la velocidad del objetivo
		Vector3 objEst = objetivo + velocidadObjetivo * tLlegada;


		Vector3 a = objEst - entity.transform.position;
		a.z = 0;

		float dist = a.sqrMagnitude;
		Vector3 fres = (a - entity.Velocidad).normalized * entity.fuerzaMaxima;
		//moderar la fuerza resultante si la distancia al objetivo es menor a cierto umbral
		if (dist < 50)
		{
			fres *= (dist / 50);
		}
		return fres;
	}

	Vector3 wander()
	{
		//calcular centro de la circunferencia
		Vector3 centro = transform.position + (transform.rotation * Vector3.right) * VagarDistancia;
		Debug.DrawLine(entity.transform.position, centro, Color.red);
		//calcular punto sobre la circunferencia
		vagarAngulo +=  Random.Range(-1,1) * VagarJitter;
		float objx = VagarRadio * Mathf.Cos(vagarAngulo) + centro.x;
		float objy = VagarRadio * Mathf.Sin(vagarAngulo) + centro.y;

		Debug.DrawLine(centro, new Vector3(objx, objy, 0), Color.yellow);

		return seek(new Vector3(objx, objy, 0));
	}

	Vector3 WallAvoidance()
	{
		Vector3 fuerza = Vector3.zero;
		RaycastHit hitInfo;
		float distMasCorta = 100000;
		float distActual = distMasCorta;
		Vector3 normal = Vector3.zero;

		//para cada antena, trazar un rayo y ver si choca con alguna pared
		for (int i = 0; i < antenas.Length; i++)
		{

			//rotar la antena
			Vector3 antRot = transform.rotation * antenas[i];
			Vector3 antena = transform.position +(antRot*magAntena);
			//antena = transform.rotation * antena;
			//Vector3 antRot = transform.rotation * antenas[i];
			Debug.DrawLine(transform.position, antena, Color.magenta, 0.05f);
			if (Physics.Raycast(transform.position, antena, out hitInfo, magAntena))
			{
				//si el collider que toco es de un objeto etiquetado como pared
				if (hitInfo.collider.gameObject.tag == "pared")
				{
					Debug.DrawLine(transform.position, hitInfo.point, Color.white);
					//calcular la distancia al punto de impacto
					distActual = (hitInfo.point - transform.position).magnitude;
					//almacenar la distancia mas corta
					if (distActual < distMasCorta)
					{
						distMasCorta = distActual;
						normal = hitInfo.normal;
					}
				}
				//si encontramos una pared, la normal es diferente de cero
				if (normal != Vector3.zero)
				{
					fuerza = normal * (magAntena - distMasCorta);
					//ignorar el componente Y de la fuerza
					fuerza.z = 0;
				}
				return fuerza;
			}
		}
		return Vector3.zero;
	}
			//trazar un rayo de distancia magAntena 
			/*
			
			//si encontramos una pared, la normal es diferente de cero
			if (normal != Vector3.zero)
			{
				fuerza = normal * (magAntena - distMasCorta) * 10;
				//ignorar el componente Y de la fuerza
				fuerza.z = 0;
			}
			return fuerza;
		}
		return Vector3.zero;
	}
	
			*/

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if (entity != null)
		{
			Gizmos.DrawSphere(posCursor, 0.3F);
			Gizmos.DrawLine(transform.position, transform.position + entity.Fuerza);

			//Vector3 centro = transform.position + (transform.rotation * Vector3.right);
			//Gizmos.DrawLine(transform.position, centro * 100);
		}
	}
}
