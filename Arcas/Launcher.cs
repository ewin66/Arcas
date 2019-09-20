using System;

namespace Arcas
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new ArcasMain());

            //System.Windows.Application app = new System.Windows.Application();
            //app.Run(new Arcas.WPF.View.MainWindow());

        }
    }
}
