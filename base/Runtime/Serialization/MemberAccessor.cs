using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace Nianyi.UnityToolkit
{
	public partial class MemberAccessor
	{
		struct MatchPattern
		{
			public Regex loose, strict;

			public MatchPattern(string pattern)
			{
				loose = new Regex($"({pattern}).*");
				strict = new Regex($"{pattern}");
			}
		}
		static MatchPattern
			memberPattern = new(@"\.([_a-zA-Z][_a-zA-Z\d]*)"),
			indexPattern = new(@"\[(0|(?:[1-9]\d*))\]");
		static List<string> PathFromString(string path)
		{
			var paths = new List<string>();
			for(int i = 0; i < path.Length;)
			{
				var memberMatch = memberPattern.loose.Match(path, i);
				if(memberMatch.Success)
				{
					string member = memberMatch.Groups[1].Value;
					i += member.Length;
					paths.Add(member);
					continue;
				}
				var indexMatch = indexPattern.loose.Match(path, i);
				if(indexMatch.Success)
				{
					string index = indexMatch.Groups[1].Value;
					i += index.Length;
					paths.Add(index);
					continue;
				}
				throw new FormatException($"Malformed accessor path: {path}");
			}
			return paths;
		}

		public struct Step
		{
			public enum Type { None, Member, Index }
			public Type type;
			public string member;
			public int index;

			void Init(string step)
			{
				var indexMatch = indexPattern.strict.Match(step);
				if(indexMatch.Success)
				{
					type = Type.Index;
					index = Convert.ToInt32(indexMatch.Groups[1].Value);
					return;
				}
				var memberMatch = memberPattern.strict.Match(step);
				if(memberMatch.Success)
				{
					type = Type.Member;
					member = memberMatch.Groups[1].Value;
					return;
				}
				throw new FormatException($"Malformed accessor path step: {step}");
			}

			public Step(string step)
			{
				type = Type.None;
				member = null;
				index = -1;
				Init(step);
			}

			const BindingFlags bindingFlagsDontCare = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
			private static MemberInfo FindMember(System.Type type, string name)
			{
				if(type == null)
					return null;

				FieldInfo field = type.GetField(name, bindingFlagsDontCare);
				if(field != null)
					return field;
				PropertyInfo property = type.GetProperty(name, bindingFlagsDontCare);
				if(property != null)
					return property;

				return FindMember(type.BaseType, name);
			}

			public readonly T Get<T>(object from)
			{
				switch(type)
				{
					case Type.Index:
						return (T)((IList)from)[index];
					case Type.Member:
						var type = from.GetType();
						var memberInfo = FindMember(type, member);
						if(memberInfo == null)
							throw new MemberAccessException($"Cannot find member {member} in {type.Name}");
						switch(memberInfo)
						{
							case FieldInfo field:
								return (T)field.GetValue(from);
							case PropertyInfo property:
								return (T)property.GetValue(from);
						};
						break;
				}
				throw new MemberAccessException($"Invalid accessor path step: {member}");
			}
			public readonly void Set<T>(object from, T value)
			{
				switch(type)
				{
					case Type.Index:
						((IList)from)[index] = value;
						return;
					case Type.Member:
						var type = from.GetType();
						var memberInfo = FindMember(type, member);
						if(memberInfo == null)
							throw new MemberAccessException($"Cannot find member {member} in {type.Name}");
						switch(memberInfo)
						{
							case FieldInfo field:
								field.SetValue(from, value);
								break;
							case PropertyInfo property:
								property.SetValue(from, value);
								break;
						};
						throw new MemberAccessException($"Cannot find member {member} in {type.Name}");
				}
			}

			public override readonly string ToString()
			{
				return type switch
				{
					Type.Index => $"[{index}]",
					Type.Member => $".{member}",
					_ => string.Empty,
				};
			}
		}
		public readonly object root;
		public readonly Step[] path;

		MemberAccessor(object root, IEnumerable<Step> path)
		{
			this.root = root;
			this.path = path?.ToArray();
			if(this.path.Length == 0)
				throw new IndexOutOfRangeException("Path of a member accessor must be non-empty");
		}
		public MemberAccessor(object root, IEnumerable<string> path) :
			this(root, path?.Select(step => new Step(step)).ToArray())
		{ }
		public MemberAccessor(object root, string path) :
			this(root, PathFromString(path))
		{ }

		public override string ToString()
			=> string.Join("", path.Select(step => step.ToString()));

		object Last()
		{
			object target = root;
			for(int i = 0; i < path.Length - 1; ++i)
				target = path[i].Get<object>(target);
			return target;
		}
		public T Get<T>()
			=> path.Last().Get<T>(Last());
		public void Set<T>(T value)
			=> path.Last().Set<T>(Last(), value);

		public MemberAccessor Navigate(string forward) => Navigate(0, forward);
		public MemberAccessor Navigate(int backard, string forward = "")
		{
			var newPath = path
				.Take(path.Length - backard)
				.Concat(PathFromString(forward).Select(step => new Step(step)));
			return new MemberAccessor(root, newPath);
		}

		public MemberAccessor Simplify()
		{
			return new(Last(), path[^1].ToString());
		}

#if UNITY_EDITOR
		static string PropertyPathToPath(string path)
			=> ("." + path).Replace(".Array.data", "");

		public SerializedProperty ToSerializedProperty()
		{
			if(path.Length == 0 || path[0].type != Step.Type.Member)
				return null;
			SerializedObject so = new((UnityEngine.Object)root);
			SerializedProperty sp = so.FindProperty(path[0].member);
			for(int i = 1; i < path.Length; ++i)
			{
				if(sp == null)
					return null;
				switch(path[i].type)
				{
					case Step.Type.Index:
						sp = sp.GetArrayElementAtIndex(path[i].index);
						continue;
					case Step.Type.Member:
						sp = sp.FindPropertyRelative(path[i].member);
						continue;
					default:
						return null;
				}
			}
			return sp;
		}

		public MemberAccessor(SerializedProperty property) :
			this(property.serializedObject.targetObject, PropertyPathToPath(property.propertyPath))
		{ }
		public static implicit operator SerializedProperty(MemberAccessor accessor)
			=> accessor.ToSerializedProperty();
#endif
	}
}
