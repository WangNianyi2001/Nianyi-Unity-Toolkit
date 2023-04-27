using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Nianyi {
	using IInteractionList = IEnumerable<Interaction>;
	using InteractionList = List<Interaction>;

	/// <summary>
	/// A system where elements that can be selected and interacted with.
	/// </summary>
	[ExecuteAlways]
	public partial class InteractionSystem : BehaviourBase {
		#region Internal fields
		private IInteractionList previouslySelectedInteractions;
		#endregion

		#region Serialized fields
		[SerializeField] private InteractionList selectedInteractions;
		#endregion

		#region Internal functions
		private struct SelectionDiff {
			public InteractionList toBeDeselected, toBeSelected, updated;
		}
		private SelectionDiff CalculateSelectionDiff(IInteractionList previous, IInteractionList next) {
			var diff = new SelectionDiff {
				toBeDeselected = new InteractionList(),
				toBeSelected = new InteractionList(),
				updated = new InteractionList(),
			};
			foreach(var i in previous) {
				if(i == null || i.Invalidated)
					continue;
				if(!i.isActiveAndEnabled || !next.Contains(i))
					diff.toBeDeselected.Add(i);
				else
					diff.updated.Add(i);
			}
			foreach(var i in next) {
				if(i == null || i.Invalidated)
					continue;
				if(i.isActiveAndEnabled && !previous.Contains(i)) {
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
			previouslySelectedInteractions = new InteractionList(selectedInteractions);
		}
		#endregion

		#region Message handlers
		protected void OnGameStart() {
			previouslySelectedInteractions = selectedInteractions;
		}
		protected void OnGameUpdate() {
			if(selectedInteractions == null)
				selectedInteractions = new InteractionList();
			if(previouslySelectedInteractions == null)
				previouslySelectedInteractions = new InteractionList(selectedInteractions);
			UpdateSelectedInteractions();
		}
		#endregion
	}
}