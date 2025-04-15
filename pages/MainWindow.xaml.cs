using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace usbechka_app.pages
{
    public partial class MainWindow : Window
    {
        public const string ROLE_CLIENT = "Client";
        public const string ROLE_ADMIN = "Admin";
        public static List<string> s_roles = new List<string> { ROLE_CLIENT, ROLE_ADMIN };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonAuth(object sender, RoutedEventArgs e)
        {
            string phoneInput = phone.Text.Trim();
            string passwordInput = password.Password.Trim();

            if (!ValidatePhone(phoneInput))
            {
                CustomMessageBox.Show("Проверьте правильность введенных данных. Телефон должен содержать 12 символов, начинаться с '+' и содержать только цифры после '+'");
                return;
            }

            if (!Validate(passwordInput))
            {
                CustomMessageBox.Show("Пароль должен содержать хотя бы 3 символа!");
                return;
            }

            var user = AppData.Db.users.SingleOrDefault(x => x.phone == phoneInput);
            if (user == null)
            {
                CustomMessageBox.Show("Пользователь не найден.");
                return;
            }

            if (passwordInput != user.password)
            {
                CustomMessageBox.Show("Неверный логин или пароль.");
                return;
            }

            Window window;
            if (ROLE_ADMIN == user.role)
            {
                window = new AdminWindow();
            }
            else
            {
                window = new ClientWindow(user.id);
            }

            window.Show();
            Close();
        }

        private void ButtonReg(object sender, RoutedEventArgs e)
        {
            string phone1 = phone.Text.Trim();
            string password1 = password.Password.Trim();
            if (!ValidatePhone(phone1))
            {
                CustomMessageBox.Show("Проверьте правильность введенных данных. Телефон должен содержать 12 символов, начинаться с '+' и содержать только цифры после '+'");
                return;
            }

            if (!Validate(password1))
            {
                CustomMessageBox.Show("Пароль должен содержать хотя бы 3 символа!");
                return;
            }

            if (null != AppData.Db.users.SingleOrDefault(x => x.phone == phone1))
            {
                CustomMessageBox.Show("Такой пользователь уже существует.");
                return;
            }

            users user = new users
            {
                id = Guid.NewGuid(),
                phone = phone1,
                password = password1,
                join_date = DateTime.Now,
                role = ROLE_CLIENT
            };
            AppData.Db.users.Add(user);
            AppData.Db.SaveChanges();

            new ClientWindow(user.id).Show();
            Close();
        }
        private bool ValidatePhone(string phoneNumber)
        {
            if (phoneNumber.Length != 12)
                return false;

            if (phoneNumber[0] != '+')
                return false;

            for (int i = 1; i < phoneNumber.Length; i++)
            {
                if (!char.IsDigit(phoneNumber[i]))
                    return false;
            }

            return true;
        }

        public static bool Validate(string value)
        {
            if (3 > value.Length || 20 < value.Length)
            {
                return false;
            }

            return true;
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
