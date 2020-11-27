using KIsabelSampleLibrary.Entity;
using KIsabelSampleLibrary.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KIsabelSampleLibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServicesManager Services { get; set; }
     
        public App()
        {
            Services = new ServicesManager();
        }
    }
}
