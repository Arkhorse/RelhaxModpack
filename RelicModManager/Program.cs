﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

namespace RelicModManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {

                String resourceName = "AssemblyLoadingAndReflection." +

                   new AssemblyName(args.Name).Name + ".dll";

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RelicModManager.Ionic.Zip.dll"))
                {

                    Byte[] assemblyData = new Byte[stream.Length];

                    stream.Read(assemblyData, 0, assemblyData.Length);

                    return Assembly.Load(assemblyData);

                }

            };
            //Application.Run(new MainWindow());
            Application.Run(new MainWindow());
        }

       /* static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EmbedAssembly.Ionic.Zip.dll"))
            {
                string dllName = args.Name.Contains(',') ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");

                dllName = dllName.Replace(".", "_");

                if (dllName.EndsWith("_resources")) return null;

                System.Resources.ResourceManager rm = new System.Resources.ResourceManager(GetType().Namespace + "RelicModManager.Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());

                byte[] bytes = (byte[])rm.GetObject("Ionic.Zip.dll");

                return System.Reflection.Assembly.Load(bytes);
            }
        }*/
    }
}
