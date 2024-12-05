using UnityEditor;

namespace Nianyi.UnityToolkit
{
	[CustomPropertyDrawer(typeof(ShowWhenAttribute))]
	public class ShowWhenAttributeDrawer : ConditionalShowingAttributeDrawer
	{
	}
}