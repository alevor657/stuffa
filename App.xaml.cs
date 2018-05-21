using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.HockeyApp;


namespace DesignPrototype
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override async void OnStartup(StartupEventArgs e)
        {
            HockeyClient.Current.Configure("4a22a1a0433f4b8fa0ff57c69c6db259");


            await HockeyClient.Current.SendCrashesAsync();



        }

    }


}
