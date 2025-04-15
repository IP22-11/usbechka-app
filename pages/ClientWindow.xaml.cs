using System;
using System.Linq;
using System.Windows;
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

            tables.ItemsSource = AppData.Db.tables
                .Where(x => x.is_reserved == false)
                .ToList();

            orders.ItemsSource = AppData.Db.orders
                .Where(x => x.user_id == id)
                .AsEnumerable()
                .Select(o => new
                {
                    o.id,
                    delishes = GetDishNames(o.menu_ids),
                    time_end = DateTime.Today.Add(o.time_end).ToString("HH:mm"),
                    o.full_price,
                    o.payment_method,
                    o.table_id
                })
                .ToList();

            var reservedTableIds = AppData.Db.reserved_tables
                .Where(x => x.user_id == id)
                .Select(x => x.table_id)
                .ToList();

            your_tables.ItemsSource = AppData.Db.tables
                .Where(t => reservedTableIds.Contains(t.id))
                .ToList();
        }

        private static string GetDishNames(string menuIds)
        {
            if (string.IsNullOrEmpty(menuIds))
                return "Нет блюд";

            try
            {
                var items = menuIds.Split(' ')
                    .Select(x => x.Split(':'))
                    .Select(parts => new
                    {
                        Id = Guid.Parse(parts[0]),
                        Quantity = parts.Length > 1 ? int.Parse(parts[1]) : 1
                    });

                var dishNames = items.Select(item =>
                {
                    var dish = AppData.Db.menu.FirstOrDefault(m => m.id == item.Id);
                    return dish != null
                        ? $"{dish.title} x{item.Quantity}"
                        : "Неизвестное блюдо";
                });

                return string.Join(", ", dishNames);
            }
            catch
            {
                return "Ошибка формата данных";
            }
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

            tables.ItemsSource = AppData.Db.tables.Where(x => x.is_reserved == false).ToList();

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
            Close();
        }

        private void ButtonCancelOrder(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = orders.SelectedItem;
            if (selectedItem == null)
            {
                CustomMessageBox.Show("Выберите заказ!");
                return;
            }

            Guid orderId = selectedItem.id;

            orders selectedOrder = AppData.Db.orders.FirstOrDefault(o => o.id == orderId);
            if (selectedOrder == null)
            {
                CustomMessageBox.Show("Заказ не найден!");
                return;
            }

            try
            {
                AppData.Db.orders.Remove(selectedOrder);
                AppData.Db.SaveChanges();

                orders.ItemsSource = AppData.Db.orders
                    .Where(x => x.user_id == id)
                    .AsEnumerable()
                    .Select(o => new
                    {
                        o.id,
                        delishes = GetDishNames(o.menu_ids),
                        time_end = DateTime.Today.Add(o.time_end).ToString("HH:mm"),
                        o.full_price,
                        o.payment_method
                    })
                    .ToList();

                CustomMessageBox.Show("Заказ успешно отменен!");
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при отмене заказа: {ex.Message}");
            }
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
