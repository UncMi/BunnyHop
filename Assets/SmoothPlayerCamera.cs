using Psychonaut;
using UnityEngine;

public class SmoothPlayerCamera : MonoBehaviour
{
	[SerializeField]
	private float height = 0.5f;

	[SerializeField]
	private float moveSpeed = 100f;

	[SerializeField]
	private float turnSpeed = 100f;

	[SerializeField]
	private float distanceLimit = 1f;

	[SerializeField]
	private PlayerController playerMovement;

	private Vector3 oldPos;

	private Quaternion oldRot;

	private void Start()
	{
		oldPos = base.transform.position;
		oldRot = base.transform.rotation;
	}

	private void LateUpdate()
	{
		Vector3 vector = playerMovement.transform.position + new Vector3(0f, height, 0f);
		base.transform.position = Vector3.Lerp(oldPos, vector, moveSpeed * Time.deltaTime);
		base.transform.rotation = Quaternion.Lerp(oldRot, Quaternion.Euler(playerMovement.InputRot), turnSpeed * Time.deltaTime);
		if (Vector3.Distance(base.transform.position, vector) > distanceLimit)
		{
			base.transform.position = vector;
		}
		oldPos = base.transform.position;
		oldRot = base.transform.rotation;
	}
}
