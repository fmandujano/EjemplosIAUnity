using UnityEngine;
using System.Collections;

public class Estado<T>
{
	virtual public void Enter(T entity) { }
	virtual public void Execute(T entity) { }
	virtual public void Exit(T entity) { }
	virtual public void OnGUI(T entity) { }
}
