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

namespace Gurock.SmartInspect.PostSharp
{
	static class SessionHelper
	{
		public static Session GetSession(string name)
		{
			Session session = SiAuto.Si.GetSession(name);

			if (session == null)
			{
				session = SiAuto.Si.AddSession(name, true);
			}

			return session;
		}

		public static string GetName(SessionPolicy policy, string typeName,
			string typeNamespace, string customName)
		{
			switch (policy)
			{
				case SessionPolicy.Custom:
					if (customName != null)
					{
						return customName;
					}
					else 
					{
						return SiAuto.Main.Name;
					}

				case SessionPolicy.TypeName:
					return typeName;

				case SessionPolicy.FullyQualifiedTypeName:
					return typeNamespace + "." + typeName;

				case SessionPolicy.Namespace:
					return typeNamespace;
			}

			return SiAuto.Main.Name;
		}
	}
}
