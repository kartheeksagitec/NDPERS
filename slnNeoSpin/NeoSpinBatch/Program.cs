#region Using directives

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace NeoSpinBatch
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main(string[] args)
		{

            int lintStartStep = 0;
            int lintStopStep = 0;
            bool lblnExecuteAll = false;

            //Argument- 2 is not specified
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "all")
                    lblnExecuteAll = true;
                else
                {
                    lintStartStep = Convert.ToInt32(args[0]);
                    if (args.Length > 1)
                        lintStopStep = Convert.ToInt32(args[1]);
                }
            }
            Application.EnableVisualStyles();
            //Application.Run(new frmNeoSpinBatch());

            //Framework Upgrade - 6.0.7.0 - Code will ensure framework will create an object of Solution side class while its loading SystemSettings.
            NeoSpin.Common.ApplicationSettings.MapSettingsObject();

            Application.Run(new frmNeoSpinBatch(lintStartStep, lintStopStep, lblnExecuteAll));

        }
	}
}