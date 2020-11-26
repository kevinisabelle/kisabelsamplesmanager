using KIsabelSampleLibrary.Entity;
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
        public App()
        {
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
