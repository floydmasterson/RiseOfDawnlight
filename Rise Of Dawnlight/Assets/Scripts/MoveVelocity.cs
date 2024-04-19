using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVelocity : MonoBehaviour, IMoveVelocity
{
	[SerializeField] private float moveSpeed;

	private Vector3 velocityVector;
	private Rigidbody _rigidbody;

	private void Awake()
	{
		_rigidbody= GetComponent<Rigidbody>();
	}


	public void SetVelocity(Vector3 velocityVector)
	{
		this.velocityVector = velocityVector;
	}

	private void FixedUpdate()
	{
		_rigidbody.velocity = velocityVector;
	}
}
