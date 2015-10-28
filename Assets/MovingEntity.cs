using UnityEngine;
using System.Collections;

public class MovingEntity : MonoBehaviour 
{
	Vector3 velocidad;
	Vector3 aceleracion;

	Quaternion rotacionDeseada;


	public Vector3 Velocidad
	{
		get { return velocidad; }
	}


	public float rapidezMaxima = 100;
	public float fuerzaMaxima = 100; //magnitud
	public float masa = 1;
	public float friccion = 0.1f; 
	public Vector3 Fuerza;

	public float rotacion;

	// Use this for initialization
	void Start () 
	{
		//condiciones iniciales
		velocidad = Vector3.zero;
		aceleracion = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{
		aceleracion = Fuerza / masa;
		velocidad += (aceleracion * Time.deltaTime) - (friccion * velocidad) ;
		transform.position += velocidad * Time.deltaTime;
		
		//calcular angulo de rotacion en base a la velocidad
		rotacion = Mathf.Atan2( velocidad.y, velocidad.x) * Mathf.Rad2Deg;
		rotacionDeseada = Quaternion.Euler(new Vector3(0,0,rotacion));

		transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * 5f);

		pantallaToroidal();
	}

	void pantallaToroidal()
	{
		// 20 y -20
		//12 y -12
		Vector3 pos = transform.position;
		if (pos.x < -20)
			pos.x = 20;
		if (pos.x > 20)
			pos.x = -20;
		if (pos.y < -12)
			pos.y = 12;
		if (pos.y > 12)
			pos.y = -12;
		transform.position = pos;
	}
}



















