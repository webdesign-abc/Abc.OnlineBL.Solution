using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using Abc.OnlineBL.Entities;
using System.Windows.Forms;
using System.Reflection;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new UI.FrmMain());			
		}
	}
}
