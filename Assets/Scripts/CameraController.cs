using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	Transform startMarker;
	Transform endMarker;

	public float speed = 15f;
	public int range = 80;
	public float bezierVariation = 10f;

	bool isRunning;
	int animationId;

	void Start() {
		performBezierMove ();
	}

	void Update() {
		if (!isRunning) {
			performBezierMove ();
		}
	}

	Vector3 generateRandomVector(Vector3 vector) {
		return vector + new Vector3(Random.Range (-range, range), Random.Range (-range, range), 0);
	}

	void performBezierMove() {
		Vector3 newPosition = generateRandomVector (transform.position);
		float distance = Vector3.Distance (transform.position, newPosition);
		isRunning = true;

		animationId = LeanTween.move (
			gameObject,
//		    new Vector3[] {
//				transform.position,
//				new Vector3(bezierVariation, bezierVariation, transform.position.z),
//				new Vector3(bezierVariation, bezierVariation, transform.position.z),
//				newPosition
//			},
			newPosition,
			distance / range * speed
		).setEase (LeanTweenType.easeOutQuad).setOnComplete(() => {
			isRunning = false;
		}).id;
	}

	public void pauseMovement() {
		LeanTween.pause (animationId);
	}

	public void resumeMovement() {
		LeanTween.resume (animationId);
	}
}
