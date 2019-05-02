using UnityEngine;

namespace extOSC.Components
{
	[AddComponentMenu("extOSC/Components/Misc/Receiver Skinned Mesh")]
	[RequireComponent(typeof(SkinnedMeshRenderer))]
	public class OSCReceiverSkinnedMesh : OSCReceiverComponent
	{
		#region Extensions

		public enum TargetType
		{
			// Take blend shape index form TargetIndex field.
			InComponent,

			// Take blend shape index from message.
			InMessage
		}

		#endregion

		#region Public Vars

		public TargetType Target = TargetType.InComponent;

		public int TargetIndex = 1;

		#endregion

		#region Private Vars

		private SkinnedMeshRenderer _skinnedMesh;

		#endregion

		#region Unity Methods

		protected void Awake()
		{
			_skinnedMesh = GetComponent<SkinnedMeshRenderer>();
		}

		#endregion

		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (Target == TargetType.InComponent)
			{
				float weight;

				// Take all messages with float value;
				if (message.ToFloat(out weight))
				{
					// Set skinned mesh blend shape weight.
					_skinnedMesh.SetBlendShapeWeight(TargetIndex, weight);
				}
			}
			else
			{
				int targetIndex;
				float weight;

				// Take all messages with int and float values.
				if (message.ToInt(out targetIndex) && 
				    message.ToFloat(out weight))
				{
					// Set skinned mesh blend shape weight.
					_skinnedMesh.SetBlendShapeWeight(targetIndex, weight);
				}
			}
		}

		#endregion
	}
}