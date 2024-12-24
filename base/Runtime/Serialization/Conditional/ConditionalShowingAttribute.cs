using System;
using UnityEngine;

namespace Nianyi.UnityToolkit
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public abstract class ConditionalShowingAttribute : PropertyAttribute
	{
	}
}
