using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiCircleScript : MonoBehaviour
{
	public enum SemiCircleOrientation
	{
		Agent,
		Objective
	}

	public SemiCircleOrientation orientation;

	public int nrRays;
	public float maxDistance;
	public AgentGridScript parentScript;

	private float delta;
	private const float minAngle = -90.0f;
	private const float maxAngle = 90.0f;
	private List<Vector3> directions;

	public void Initialize ()
	{
		float range = maxAngle - minAngle;
		delta = range / (float)(nrRays - 1);

		directions = new List<Vector3> (nrRays);
		for (float ang = minAngle; ang <= maxAngle; ang += delta) {
			float rad = ang * Mathf.PI / 180.0f;
			Vector3 newDirection;
			newDirection.x = transform.forward.x * Mathf.Cos (rad) + transform.forward.z * Mathf.Sin (rad);
			newDirection.y = transform.forward.y;
			newDirection.z = transform.forward.z * Mathf.Cos (rad) - transform.forward.x * Mathf.Sin (rad);

			directions.Add (newDirection);
		}

		for (int i = 0; i < nrRays; i++) {
			if (orientation == SemiCircleOrientation.Agent) {
				parentScript.agentRays [i] = maxDistance;
			} else if (orientation == SemiCircleOrientation.Objective) {
				parentScript.goalRays [i] = maxDistance;
			}
		}
	}

	// Update is called once per frame
	public void UpdateOutput ()
	{
		if (directions != null) {
			for (int i = 0; i < directions.Count; i++) {
				Vector3 dir = transform.TransformDirection (directions [i]);
				RaycastHit hit;
				if (Physics.Raycast (transform.position, dir, out hit, maxDistance)) {
					if (orientation == SemiCircleOrientation.Agent) {
						parentScript.agentRays [i] = hit.distance;
					} else if (orientation == SemiCircleOrientation.Objective) {
						parentScript.goalRays [i] = hit.distance;
					}
				} else {
					if (orientation == SemiCircleOrientation.Agent) {
						parentScript.agentRays [i] = maxDistance;
					} else if (orientation == SemiCircleOrientation.Objective) {
						parentScript.goalRays [i] = maxDistance;
					}
				}
				if (orientation == SemiCircleOrientation.Agent) {
					Debug.DrawRay (transform.position, dir * parentScript.agentRays [i]);
				} else if (orientation == SemiCircleOrientation.Objective) {
					Debug.DrawRay (transform.position, dir * parentScript.goalRays [i]);
				}
			}
		}
	}
}
