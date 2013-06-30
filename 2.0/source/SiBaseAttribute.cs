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

namespace Gurock.SmartInspect.PostSharp
{
	[Serializable]
	public class SiBaseAttribute: OnMethodBoundaryAspect
	{
		private string m_SessionName;
		private SessionPolicy m_SessionPolicy;

		private string m_FormattedSessionName;
		private string m_MethodName;

		public override void CompileTimeInitialize(MethodBase method,
			AspectInfo aspectInfo)
		{
			m_FormattedSessionName = FormatSessionName(method);
			m_MethodName = FormatMethodName(method);
			base.CompileTimeInitialize(method, aspectInfo);
		}

		protected virtual string FormatSessionName(MethodBase method)
		{
			return SessionHelper.GetName(m_SessionPolicy,
				method.DeclaringType.Name, method.DeclaringType.Namespace,
				m_SessionName);
		}

		protected virtual string FormatMethodName(MethodBase method)
		{
			return String.Format("{0}.{1}", method.DeclaringType.Name, 
				method.Name);
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

		protected string MethodName		
		{
			get { return m_MethodName; }
		}

		protected Session GetSession()
		{
			return SessionHelper.GetSession(m_FormattedSessionName);
		}
	}
}
