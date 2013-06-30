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

namespace Gurock.SmartInspect.PostSharp
{
	public enum SessionPolicy
	{
		Custom,
		TypeName,
		FullyQualifiedTypeName,
		Namespace,
	}
}
