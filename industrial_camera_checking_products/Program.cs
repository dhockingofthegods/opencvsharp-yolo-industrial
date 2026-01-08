using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using industrial_camera_checking_products.Services;

namespace industrial_camera_checking_products
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            services.AddLogging(b =>
            {
                b.AddDebug();
                b.SetMinimumLevel(LogLevel.Information);
            });
            services.AddSingleton<ICameraService, CameraService>();
            services.AddSingleton<IYoloDetector, YoloDetector>();
            services.AddSingleton<Form1>();

            using var sp = services.BuildServiceProvider();
            var form = sp.GetRequiredService<Form1>();
            Application.Run(form);
        }
    }
}