using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi {
	/// <summary>
	/// A system where elements that can be selected and interacted with.
	/// </summary>
	public partial class InteractionSystem : MonoBehaviour {
		#region Serialized fields
		[SerializeField] private List<Interaction> selectedInteractions;
		#endregion

		#region Life cycle
		protected void Start() {
			previouslySelectedInteractions = selectedInteractions;
		}

		protected void Update() {
			selectedInteractions ??= new List<Interaction>();
			previouslySelectedInteractions ??= new List<Interaction>(selectedInteractions);
			UpdateSelectedInteractions();
		}
		#endregion

		#region Message handlers
		protected void OnInteract() => Interact();
		#endregion

		#region Properties
		public IEnumerable<Interaction> SelectedInteractions {
			get => previouslySelectedInteractions;
			set => selectedInteractions = value.ToList();
		}
		#endregion

		#region Interfaces
		public void Select(Interaction i) {
			selectedInteractions.Add(i);
			UpdateSelectedInteractions();
		}

		public void Deselect(Interaction i) {
			selectedInteractions.Remove(i);
			UpdateSelectedInteractions();
		}

		public void Interact() {
			foreach(var i in selectedInteractions) {
				if(i != null)
					i.SendMessage("OnInteract");
			}
		}
		#endregion

		#region Fields
		private IEnumerable<Interaction> previouslySelectedInteractions;
		#endregion

		#region Functions
		private List<Interaction> RecifyInteractionList(IEnumerable<Interaction> list) {
			var result = new List<Interaction>();
			if(list == null)
				return result;
			foreach(var i in list) {
				if(i == null)
					continue;
				if(result.Contains(i))
					continue;
				result.Add(i);
			}
			return result;
		}

		private struct SelectionDiff {
			public List<Interaction> toBeDeselected, toBeSelected, updated;
		}
		private SelectionDiff CalculateSelectionDiff(IEnumerable<Interaction> previous, IEnumerable<Interaction> next) {
			previous = RecifyInteractionList(previous);
			next = RecifyInteractionList(next);
			var diff = new SelectionDiff {
				toBeDeselected = new List<Interaction>(),
				toBeSelected = new List<Interaction>(),
				updated = new List<Interaction>(),
			};
			foreach(var i in previous) {
				if(!next.Contains(i))
					diff.toBeDeselected.Add(i);
				else
					diff.updated.Add(i);
			}
			foreach(var i in next) {
				if(!previous.Contains(i)) {
					diff.toBeSelected.Add(i);
					diff.updated.Add(i);
				}
			}
			return diff;
		}

		/// <summary>
		/// Calculate the diffs between previously and newly selected interactions
		/// and send messages to interactions whose selection state has changed.
		/// </summary>
		/// <remarks>
		/// Invalidated interactions will be cleared out.
		/// </remarks>
		private void UpdateSelectedInteractions() {
			// Calculate the diffs and send messages.
			var diff = CalculateSelectionDiff(previouslySelectedInteractions, selectedInteractions);
			foreach(var i in diff.toBeDeselected)
				i.SendMessage("OnDeselect");
			foreach(var i in diff.toBeSelected)
				i.SendMessage("OnSelect");

			// Update maintained list.
			selectedInteractions = diff.updated;
			// We don't want the two lists to share reference.
			previouslySelectedInteractions = new List<Interaction>(selectedInteractions);
		}
		#endregion
	}
}