using System;
using System.Windows.Forms;
using Capturador._01_Pantallas._00_Principal;

namespace Capturador;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new frmInicio_v2());
	}
}
