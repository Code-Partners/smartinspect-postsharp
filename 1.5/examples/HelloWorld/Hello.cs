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
using System.Threading;
using Gurock.SmartInspect;

namespace Gurock.SmartInspect.PostSharp.Examples
{
	class Hello
	{
		public int m_Count;

		public void SayHello(string name)
		{
			SiAuto.Main.LogMessage("Hello, {0}.", name);
		}

		public string SayGoodbye(string name)
		{
			string msg = String.Format("Goodbye, {0}.", name);
			SiAuto.Main.LogMessage(msg);
			return msg;
		}

		public void Count()
		{
			Random r = new Random();
			for (int i = 0; i < 10; i++)
			{
				m_Count = r.Next(100);
				Thread.Sleep(1000);
			}			
		}

		public void RaiseException(string name)
		{
			SiAuto.Main.LogMessage("Hello, {0}.", name);
			throw new Exception("This is a test exception");
		}
	}
}
