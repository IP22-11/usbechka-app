using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace usbechka_app.pages
{
    public partial class AddDelishesWindow : Window
    {
        public AddDelishesWindow()
        {
            InitializeComponent();
        }

        private void ButtonConfirm(object sender, RoutedEventArgs e)
        {
            if (title.Text.Trim() == null || description.Text.Trim() == null || price.Text.Trim() == null)
            {
                CustomMessageBox.Show("Проверьте правильность введенных данных.");
                return;
            }

            if (!int.TryParse(price.Text.Trim(), out _))
            {
                CustomMessageBox.Show("Цена должна быть числом!");
                return;
            }

            menu delishes = AppData.Db.menu.SingleOrDefault(x => x.title == title.Text);
            if (null != delishes)
            {
                CustomMessageBox.Show("Стол уже добавлен.");
                return;
            }

            int newPrice = int.Parse(price.Text.Trim());

            menu newDelishes = new menu
            {
                id = Guid.NewGuid(),
                title = title.Text,
                description = description.Text,
                price = newPrice,
            };
            AppData.Db.menu.Add(newDelishes);
            AppData.Db.SaveChanges();

            new AdminWindow().Show();
            Close();
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            new AdminWindow().Show();
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
