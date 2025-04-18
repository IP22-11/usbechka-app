using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace usbechka_app.pages
{
    public partial class YourTablesPage : Page
    {
        private Guid id;

        public YourTablesPage(Guid user_id)
        {
            InitializeComponent();
            id = user_id;
            LoadYourTables();
        }

        private void LoadYourTables()
        {
            var reservedTableIds = AppData.Db.reserved_tables
                .Where(x => x.user_id == id)
                .Select(x => x.table_id)
                .ToList();

            your_tables.ItemsSource = AppData.Db.tables
                .Where(t => reservedTableIds.Contains(t.id))
                .ToList();
        }

        private void ButtonOrder(object sender, RoutedEventArgs e)
        {
            tables selectedTable = your_tables.SelectedItem as tables;
            if (selectedTable == null)
            {
                CustomMessageBox.Show("Выберите стол из списка зарезервированных!");
                return;
            }

            new OrderWindow(id, selectedTable.id).Show();
            Window.GetWindow(this).Close();
        }
    }
}