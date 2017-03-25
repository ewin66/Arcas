using System;
using System.Windows.Forms;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ArcasMain());

            //System.Windows.Application app = new System.Windows.Application();
            //App app = new App();
            //app.MainWindow = new Window1();
            //app.MainWindow.Show();
            //app.Run();
        }
    }
}
