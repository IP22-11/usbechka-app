using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace usbechka_app.pages
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();

            tables.ItemsSource = AppData.Db.tables.ToList();
            menu.ItemsSource = AppData.Db.menu.ToList();
        }

        private void ButtonDeleteTable(object sender, RoutedEventArgs e)
        {
            tables selectedTable = tables.SelectedItem as tables;
            if (selectedTable == null)
            {
                CustomMessageBox.Show("Выберите стол!");
                return;
            }

            try
            {
                var reservedRecords = AppData.Db.reserved_tables
                    .Where(rt => rt.table_id == selectedTable.id)
                    .ToList();

                AppData.Db.reserved_tables.RemoveRange(reservedRecords);

                var relatedOrders = AppData.Db.orders
                    .Where(o => o.table_id == selectedTable.id)
                    .ToList();

                AppData.Db.orders.RemoveRange(relatedOrders);

                AppData.Db.tables.Remove(selectedTable);

                AppData.Db.SaveChanges();

                tables.ItemsSource = AppData.Db.tables.ToList();

                CustomMessageBox.Show("Стол успешно удален!");
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при удалении стола: {ex.Message}");
            }
        }

        private void ButtonDeleteDelishes(object sender, RoutedEventArgs e)
        {
            menu selectedDelishes = menu.SelectedItem as menu;
            if (selectedDelishes == null)
            {
                CustomMessageBox.Show("Выберите блюдо!");
                return;
            }

            AppData.Db.menu.Remove(selectedDelishes);
            AppData.Db.SaveChanges();

            menu.ItemsSource = AppData.Db.menu.ToList();
        }

        private void ButtonAddTable(object sender, RoutedEventArgs e)
        {
            AddTableWindow add = new AddTableWindow();
            add.Show();
            Close();
        }

        private void ButtonAddDelishes(object sender, RoutedEventArgs e)
        {
            AddDelishesWindow add = new AddDelishesWindow();
            add.Show();
            Close();
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
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
