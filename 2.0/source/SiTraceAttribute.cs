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

namespace Gurock.SmartInspect.PostSharp
{
	[Serializable]
	[ProvideAspectRole(StandardRoles.Tracing)]
	[AspectTypeDependency(AspectDependencyAction.Commute,
		typeof(SiFieldAttribute))]
	public class SiTraceAttribute : SiBaseAttribute
	{
		private bool m_IncludeArguments;
		private bool m_IncludeReturnValue;
		private bool m_IncludeSpecialNames = true;
		private Level m_Level;

		public Level Level
		{
			get { return m_Level; }
			set { m_Level = value; }
		}

		public bool IncludeSpecialNames
		{
			get { return m_IncludeSpecialNames; }
			set { m_IncludeSpecialNames = value; }
		}

		public bool IncludeArguments
		{
			get { return m_IncludeArguments; }
			set { m_IncludeArguments = value; }
		}

		public bool IncludeReturnValue
		{
			get { return m_IncludeReturnValue; }
			set { m_IncludeReturnValue = value; }
		}

		public override void CompileTimeInitialize(MethodBase method,
			AspectInfo aspectInfo)
		{
			if (m_IncludeReturnValue)
			{
				if (method is MethodInfo)
				{
					MethodInfo info = (MethodInfo)method;
					m_IncludeReturnValue = info.ReturnType != typeof(void);
				}

				if (method.IsConstructor)
				{
					m_IncludeReturnValue = false;
				}
			}

			base.CompileTimeInitialize(method, aspectInfo);
		}

		protected string FormatEntry(MethodExecutionArgs eventArgs)
		{
			if (!m_IncludeArguments)
			{
				return MethodName;
			}

			object[] arguments = eventArgs.Arguments.ToArray();

			if (arguments == null)
			{
				return MethodName;
			}

			string args = FormatArguments(arguments);
			return String.Concat(MethodName, args);
		}

		protected virtual string FormatArguments(object[] arguments)
		{
			StringBuilder args = new StringBuilder();
			args.Append('(');

			bool first = true;
			foreach (object arg in arguments)
			{
				if (!first)
				{
					args.Append(", ");
				}

				if (arg is string)
				{
					args.Append('"');
					args.Append(arg.ToString().Replace("\"", "\\\""));
					args.Append('"');
				}
				else
				{
					args.Append(arg.ToString());
				}

				first = false;
			}

			args.Append(')');
			return args.ToString();
		}

		protected string FormatExit(MethodExecutionArgs eventArgs)
		{
			if (!m_IncludeReturnValue)
			{
				return MethodName;
			}

			string returnValue = FormatReturnValue(eventArgs.ReturnValue);
			return String.Concat(MethodName, " = ", returnValue);
		}

		protected string FormatReturnValue(object returnValue)
		{
			if (returnValue == null)
			{
				return "null";
			}

			string result = returnValue.ToString();

			if (returnValue is string)
			{
				result = result.Replace("\"", "\\\"");
				return "\"" + result + "\"";
			}
			else
			{
				return result;
			}
		}

		public override void OnEntry(MethodExecutionArgs eventArgs)
		{
			if (!m_IncludeSpecialNames && eventArgs.Method.IsSpecialName)
			{
				return;
			}
			
			Session session = GetSession();
			if (session.IsOn(m_Level))
			{
				session.EnterMethod(Level, FormatEntry(eventArgs));
			}
		}

		public override void OnExit(MethodExecutionArgs eventArgs)
		{
			if (!m_IncludeSpecialNames && eventArgs.Method.IsSpecialName)
			{
				return;
			}

			Session session = GetSession();
			if (session.IsOn(m_Level))
			{
				session.LeaveMethod(Level, FormatExit(eventArgs));
			}
		}
	}
}
