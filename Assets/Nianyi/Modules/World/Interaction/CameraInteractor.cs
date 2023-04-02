using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi {
	[RequireComponent(typeof(Camera))]
	public class CameraInteractor : Interactor {
		public enum AimPosition {
			CenterOfScreen,
			MousePosition,
		}

		#region Core fields
		Dictionary<Interactive, RaycastHit> raycastHits = new Dictionary<Interactive, RaycastHit>();
		#endregion

		#region Inspector fields
		[SerializeField] new Camera camera = null;
		public AimPosition aimPosition;
		public bool finiteInteractingDistance = true;
		[ShowIfBool("finiteInteractingDistance")] public float maxInteractingDistance = 10;
		public bool castThrough = false;
		#endregion

		#region Core methods
		float CastDistance => finiteInteractingDistance ? maxInteractingDistance : Mathf.Infinity;
		Ray CastRay {
			get {
				Vector3 point;
				switch(aimPosition) {
					case AimPosition.CenterOfScreen:
						point = new Vector3(Screen.width, Screen.height, 0) * .5f;
						break;
					case AimPosition.MousePosition:
						point = Input.mousePosition;
						break;
					default:
						point = Vector3.zero;
						break;
				}
				Ray ray = camera.ScreenPointToRay(point);
				return ray;
			}
		}
		#endregion

		#region Public interfaces
		public override IEnumerable<Interactive> Targets => raycastHits.Keys;
		public IEnumerable<RaycastHit> RaycastHits => raycastHits.Values;
		#endregion

		#region Life cycle
		protected void Start() {
			camera = GetComponent<Camera>();
		}

		protected void Update() {
			raycastHits.Clear();
			if(castThrough) {
				RaycastHit[] hits = Physics.RaycastAll(CastRay, CastDistance);
				foreach(var hit in hits) {
					var interactive = hit.transform?.GetComponent<Interactive>();
					if(interactive && interactive.isActiveAndEnabled)
						raycastHits[interactive] = hit;
				}
			}
			else {
				Physics.Raycast(CastRay, out RaycastHit hit, CastDistance);
				var interactive = hit.transform?.GetComponent<Interactive>();
				if(interactive && interactive.isActiveAndEnabled)
					raycastHits[interactive] = hit;
			}
		}

		protected new void OnDrawGizmos() {
			base.OnDrawGizmos();

			if(!Application.isPlaying)
				return;

			Ray ray = CastRay;
			Vector3 from = ray.origin, direction = ray.direction;
			float maxHitDistance = -Mathf.Infinity;

			Gizmos.color = new Color(1, 1, 0, .3f);
			foreach(var hit in raycastHits.Values) {
				Vector3 to = hit.point;
				Gizmos.DrawSphere(to, .05f);
				maxHitDistance = Mathf.Max(maxHitDistance, (to - from).magnitude);
			}

			float distance;
			if(maxHitDistance >= 0)
				distance = maxHitDistance;
			else
				distance = finiteInteractingDistance ? maxInteractingDistance : 1;
			direction *= distance;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(from, from + direction);
		}
		#endregion
	}
}