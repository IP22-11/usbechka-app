using System.Windows;

namespace usbechka_app.pages
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            NavigateToTables(null, null);
        }

        private void UpdateNavButtons(string activePageTag)
        {
            TablesBtn.Style = activePageTag == "TablesPage" ?
                (Style)FindResource("NavButtonActive") : (Style)FindResource("NavButtonWithIcon");

            MenuBtn.Style = activePageTag == "MenuPage" ?
                (Style)FindResource("NavButtonActive") : (Style)FindResource("NavButtonWithIcon");
        }

        private void NavigateToTables(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AdminTablesPage());
            UpdateNavButtons("TablesPage");
        }

        private void NavigateToMenu(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AdminMenuPage());
            UpdateNavButtons("MenuPage");
        }

        private void ButtonExit(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}