using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace usbechka_app.pages
{
    public partial class AdminTablesPage : Page
    {
        public AdminTablesPage()
        {
            InitializeComponent();
            LoadTables();
        }

        private void LoadTables()
        {
            var tables = AppData.Db.tables
                .Select(t => new
                {
                    Table = t,
                    Reservation = AppData.Db.reserved_tables
                        .FirstOrDefault(rt => rt.table_id == t.id),
                    User = AppData.Db.reserved_tables
                        .Where(rt => rt.table_id == t.id)
                        .Join(AppData.Db.users,
                            rt => rt.user_id,
                            u => u.id,
                            (rt, u) => u)
                        .FirstOrDefault()
                })
                .AsEnumerable()
                .Select(x => new TableViewModel
                {
                    Id = x.Table.id,
                    Number = x.Table.number_table,
                    IsReserved = x.Reservation != null,
                    ClientName = x.User != null ? x.User.phone : null
                })
                .OrderBy(t => t.Number)
                .ToList();

            tablesGrid.ItemsSource = tables;
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            var selected = tablesGrid.SelectedItem as TableViewModel;
        }

        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddTableWindow();
                dialog.ShowDialog();
                LoadTables();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при добавлении стола: {ex.Message}");
            }
        }

        private void DeleteTable_Click(object sender, RoutedEventArgs e)
        {
            var selected = tablesGrid.SelectedItem as TableViewModel;
            if (selected != null)
            {
                try
                {
                    var table = AppData.Db.tables.Find(selected.Id);
                    if (table != null)
                    {
                        if (AppData.Db.reserved_tables.Any(rt => rt.table_id == table.id))
                        {
                            CustomMessageBox.Show("Нельзя удалить стол с активной бронью");
                            return;
                        }

                        if (AppData.Db.orders.Any(o => o.table_id == table.id))
                        {
                            CustomMessageBox.Show("Нельзя удалить стол с активными заказами");
                            return;
                        }

                        AppData.Db.tables.Remove(table);
                        AppData.Db.SaveChanges();
                        LoadTables();
                    }
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show($"Ошибка при удалении стола: {ex.Message}");
                }
            }
            else
            {
                CustomMessageBox.Show("Выберите стол для удаления");
            }
        }

        private void tablesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonsState();
        }
    }

    public class TableViewModel
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public bool IsReserved { get; set; }
        public string ClientName { get; set; }
    }
}