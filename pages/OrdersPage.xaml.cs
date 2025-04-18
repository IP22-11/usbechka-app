using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace usbechka_app.pages
{
    public partial class OrdersPage : Page
    {
        private Guid id;

        public OrdersPage(Guid user_id)
        {
            InitializeComponent();
            id = user_id;
            LoadOrders();
        }

        private void LoadOrders()
        {
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
                LoadOrders();
                CustomMessageBox.Show("Заказ успешно отменен!");
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при отмене заказа: {ex.Message}");
            }
        }
    }
}