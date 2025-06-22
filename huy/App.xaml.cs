using BusinessObjects;
using System.IO;
using System.Windows;

namespace huy
{
    public partial class App : Application
    {
        public static int CurrentCustomerID { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Copy appsettings.json to the output directory if it doesn't exist
            string sourceFile = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            string targetFile = Path.Combine(Directory.GetCurrentDirectory(), "bin", "Debug", "net6.0-windows", "appsettings.json");
            
            if (File.Exists(sourceFile) && !File.Exists(targetFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                File.Copy(sourceFile, targetFile);
            }
            
            // Initialize database and seed data
            using (var context = new HotelDbContext())
            {
                DbSeeder.Seed(context);
            }
        }
    }
} 