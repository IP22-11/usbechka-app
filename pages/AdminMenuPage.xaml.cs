using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;

namespace usbechka_app.pages
{
    public partial class AdminMenuPage : Page
    {
        public AdminMenuPage()
        {
            InitializeComponent();
            LoadMenu();
        }

        private void LoadMenu()
        {
            menuGrid.ItemsSource = AppData.Db.menu.ToList();
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddDelishesWindow();
                dialog.ShowDialog();
                LoadMenu();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при добавлении блюда: {ex.Message}");
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (menuGrid.SelectedItem is menu selectedItem)
            {
                try
                {
                    var dish = AppData.Db.menu.Find(selectedItem.id);
                    if (dish != null)
                    {
                        if (AppData.Db.orders.Any(o => o.menu_ids.Contains(dish.id.ToString())))
                        {
                            CustomMessageBox.Show("Нельзя удалить блюдо, которое уже заказано");
                            return;
                        }

                        AppData.Db.menu.Remove(dish);
                        AppData.Db.SaveChanges();
                        LoadMenu();
                    }
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show($"Ошибка при удалении блюда: {ex.Message}");
                }
            }
            else
            {
                CustomMessageBox.Show("Выберите блюдо для удаления");
            }
        }
    }
}