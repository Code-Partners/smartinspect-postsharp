/**
 * Copyright Gurock Software GmbH. All rights reserved.
 * http://www.gurock.com - contact@gurock.com
 *
 * Learn more about the SmartInspect for PostSharp aspects on
 * the following project website:
 *
 * http://code.gurock.com/p/smartinspect-postsharp/
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Reflection;

namespace Gurock.SmartInspect.PostSharp
{
	[Serializable]
	[ProvideAspectRole(StandardRoles.Tracing)]
	[AspectTypeDependency(AspectDependencyAction.Commute,
		typeof(SiTraceAttribute))]
	public class SiFieldAttribute: LocationInterceptionAspect
	{
		private string m_SessionName;
		private SessionPolicy m_SessionPolicy;
		private string m_FormattedSessionName;
		private Level m_Level;
		private bool m_Trace;

		private string m_Name;
		private WatchType m_WatchType;
		private bool m_Log = true;
		private bool m_Watch = true;
		private bool m_IncludeType;
		private bool m_IncludeInstance;

		public Level Level
		{
			get { return m_Level; }
			set { m_Level = value; }
		}

		public bool Watch 
		{
			get { return m_Watch; }
			set { m_Watch = value; }
		}

		public bool Log
		{
			get { return m_Log; }
			set { m_Log = value; }
		}

		public bool IncludeType
		{
			get { return m_IncludeType; }
			set { m_IncludeType = value; }
		}

		public bool IncludeInstance
		{
			get { return m_IncludeInstance; }
			set { m_IncludeInstance = value; }
		}

		public override void CompileTimeInitialize(LocationInfo targetLocation,
			AspectInfo aspectInfo)
		{
			m_Trace = false;
			if (targetLocation.LocationKind == LocationKind.Field)
			{
				m_Trace = !targetLocation.FieldInfo.IsSpecialName;
			}
			else if (targetLocation.LocationKind == LocationKind.Property)
			{
				m_Trace = true;
			}

			if (m_Trace)
			{
				if (m_IncludeType)
				{
					m_Name = targetLocation.DeclaringType.Name + "." + targetLocation.Name;
				}
				else
				{
					m_Name = targetLocation.Name;
				}

				m_FormattedSessionName = FormatSessionName(targetLocation);
				m_WatchType = GetWatchType(targetLocation);

				if (m_IncludeInstance)
				{
					m_IncludeInstance = !targetLocation.IsStatic;
				}
			}

			base.CompileTimeInitialize(targetLocation, aspectInfo);
		}

		private WatchType GetWatchType(LocationInfo targetLocation)
		{
			Type type = targetLocation.LocationType;

			if (type != null)
			{
				if (type == typeof(System.String))
				{
					return WatchType.String;
				}
				else if (type == typeof(System.Char))
				{
					return WatchType.Char;
				}
				else if (type == typeof(System.Byte) ||
					type == typeof(System.Int16) ||
					type == typeof(System.Int32) ||
					type == typeof(System.Int64))
				{
					return WatchType.Integer;
				}
				else if (type == typeof(System.Single) ||
					type == typeof(System.Double) ||
					type == typeof(System.Decimal))
				{
					return WatchType.Float;
				}
				else if (type == typeof(System.DateTime))
				{
					return WatchType.Timestamp;
				}
				else if (type == typeof(System.Boolean))
				{
					return WatchType.Boolean;
				}
			}

			return WatchType.String;
		}

		protected virtual string FormatSessionName(LocationInfo targetLocation)
		{
			return SessionHelper.GetName(m_SessionPolicy,
				targetLocation.DeclaringType.Name, targetLocation.DeclaringType.Namespace, 
				m_SessionName);
		}

		public string SessionName
		{
			get { return m_SessionName; }
			set { m_SessionName = value; }
		}

		public SessionPolicy SessionPolicy
		{
			get { return m_SessionPolicy; }
			set { m_SessionPolicy = value; }
		}

		protected virtual string FormatName(string name, object instance)
		{
			if (m_IncludeInstance)
			{
				string o = FormatInstance(instance);
				return String.Format("{0} ({1})", name, instance);
			}
			else
			{
				return name;
			}
		}

		protected virtual string FormatInstance(object instance)
		{
			if (instance == null)
			{
				return ""; // Shouldn't happen
			}
			else
			{
				return instance.ToString();
			}
		}

		protected virtual string FormatValue(object value)
		{
			if (value == null)
			{
				return "null";
			}

			string result = value.ToString();

			if (value is string)
			{
				result = result.Replace("\"", "\\\"");
				return "\"" + result + "\"";
			}
			else
			{
				return result;
			}
		}

		protected virtual string FormatMessage(string name, string newValue,
			string oldValue)
		{
			return String.Format("{0} = {1} [was: {2}]", name, newValue,
				oldValue);
		}

		private Session GetSession()
		{
			return SessionHelper.GetSession(m_FormattedSessionName);
		}

		public override void OnSetValue(LocationInterceptionArgs eventArgs)
		{
			Session session = GetSession();
			bool isOn = session.IsOn(m_Level) && (m_Log || m_Watch);

			if (isOn)
			{
				string name = FormatName(m_Name, eventArgs.Instance);
				string newValue = FormatValue(eventArgs.Value);
				string oldValue = FormatValue(eventArgs.GetCurrentValue());

				if (Log)
				{
					string message = FormatMessage(name, newValue, oldValue);
					session.SendCustomLogEntry(message, LogEntryType.VariableValue,
						ViewerId.Title, null);
				}

				if (Watch)
				{
					session.SendCustomWatch(m_Level, name, newValue, m_WatchType);
				}
			}

			base.OnSetValue(eventArgs);
		}
	}
}
