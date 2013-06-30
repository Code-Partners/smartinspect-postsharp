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
using Gurock.SmartInspect;
using Gurock.SmartInspect.PostSharp;

// We are using the SmartInspect aspect attributes to
// trace method execution, exceptions and field access.
// Please note that you can also specify these
// attributes for a specific class, method or field
[assembly:SiTrace(SessionPolicy = SessionPolicy.TypeName, 
	IncludeArguments = true,
	IncludeReturnValue = true)]
[assembly:SiException(SessionPolicy = SessionPolicy.TypeName)]
[assembly:SiField(SessionPolicy = SessionPolicy.TypeName)]

namespace Gurock.SmartInspect.PostSharp.Examples
{
	class Program
	{
		static void Main(string[] args)
		{
			// Enable SmartInspect logging
			SiAuto.Si.Enabled = true;

			// Create an instance of the Hello class. The
			// Hello class is using the SmartInspect PostSharp
			// aspects for method and exception tracing.
			Hello hello = new Hello();

			// Call The Hello methods and assign property values
			hello.SayHello("World");
			hello.SayGoodbye("World");
			hello.Count();
			hello.RaiseException("Hello World");
		}
	}
}
