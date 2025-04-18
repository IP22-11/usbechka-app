using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace usbechka_app.pages
{
    public partial class TablesPage : Page
    {
        private Guid id;

        public TablesPage(Guid user_id)
        {
            InitializeComponent();
            id = user_id;
            LoadTables();
        }

        private void LoadTables()
        {
            tables.ItemsSource = AppData.Db.tables
                .Where(x => x.is_reserved == false)
                .ToList();
        }

        private void ButtonBook(object sender, RoutedEventArgs e)
        {
            tables selectedTable = tables.SelectedItem as tables;
            if (selectedTable == null)
            {
                CustomMessageBox.Show("Выберите стол из списка свободных!");
                return;
            }

            reserved_tables reserved = new reserved_tables
            {
                id = Guid.NewGuid(),
                user_id = id,
                table_id = selectedTable.id
            };

            AppData.Db.reserved_tables.Add(reserved);
            selectedTable.is_reserved = true;
            AppData.Db.SaveChanges();

            LoadTables();
        }
    }
}