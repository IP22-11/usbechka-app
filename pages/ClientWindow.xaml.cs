using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace usbechka_app.pages
{
    public partial class ClientWindow : Window
    {
        private Guid id;
        public ClientWindow(Guid user_id)
        {
            InitializeComponent();
            id = user_id;
            NavigateToTables(null, null);
            UpdateNavButtons("Доступные столы");
        }

        private void UpdateNavButtons(string activePageTag)
        {
            TablesBtn.Style = activePageTag == "TablesPage" ?
                (Style)FindResource("NavButtonActive") : (Style)FindResource("NavButtonWithIcon");

            YourTablesBtn.Style = activePageTag == "YourTablesPage" ?
                (Style)FindResource("NavButtonActive") : (Style)FindResource("NavButtonWithIcon");

            OrdersBtn.Style = activePageTag == "OrdersPage" ?
                (Style)FindResource("NavButtonActive") : (Style)FindResource("NavButtonWithIcon");
        }

        private void NavigateToTables(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TablesPage(id));
            UpdateNavButtons("TablesPage");
        }

        private void NavigateToYourTables(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new YourTablesPage(id));
            UpdateNavButtons("YourTablesPage");
        }

        private void NavigateToOrders(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage(id));
            UpdateNavButtons("OrdersPage");
        }

        private void ButtonExit(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            GetWindow(this).Close();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.GetPosition(this).Y <= 30)
            {
                DragMove();
            }
        }
    }
}
