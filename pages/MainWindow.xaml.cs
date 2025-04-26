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

        private void Phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !e.Text.All(char.IsDigit);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataObject.AddPastingHandler(phone, Phone_Pasting);
        }

        private void Phone_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (phone.SelectionStart < 2)
            {
                e.CancelCommand();
            }
        }

        private void Phone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            if (e.Key == Key.Back)
            {
                if (textBox.SelectionStart <= 2 && textBox.SelectionLength == 0)
                {
                    e.Handled = true;
                }
                else if (textBox.SelectionStart <= 2 && textBox.SelectionLength > 0)
                {
                    if (textBox.SelectionStart + textBox.SelectionLength > 2)
                    {
                        textBox.SelectionStart = 2;
                        textBox.SelectionLength = textBox.Text.Length - 2;
                    }
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (textBox.SelectionStart < 2)
                {
                    e.Handled = true;
                }
            }
        }


        private void Phone_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text) || !textBox.Text.StartsWith("+7"))
            {
                textBox.Text = "+7";
                textBox.SelectionStart = textBox.Text.Length;
                return;
            }

            int cursorPos = textBox.SelectionStart;

            string digits = new string(textBox.Text.Where(char.IsDigit).ToArray());
            if (digits.Length < 2) return;

            string formatted = "+7";
            if (digits.Length > 1)
            {
                formatted += $" ({digits.Substring(1, Math.Min(3, digits.Length - 1))}";
            }
            if (digits.Length > 4)
            {
                formatted += $") {digits.Substring(4, Math.Min(3, digits.Length - 4))}";
            }
            if (digits.Length > 7)
            {
                formatted += $" {digits.Substring(7, Math.Min(2, digits.Length - 7))}";
            }
            if (digits.Length > 9)
            {
                formatted += $"-{digits.Substring(9, Math.Min(2, digits.Length - 9))}";
            }

            if (textBox.Text != formatted)
            {
                int diff = formatted.Length - textBox.Text.Length;
                cursorPos += diff;

                textBox.Text = formatted;

                cursorPos = Math.Max(4, Math.Min(cursorPos, formatted.Length));

                textBox.SelectionStart = cursorPos;
            }
        }

        private bool ValidatePhone(string phoneNumber)
        {
            string digits = phoneNumber.Length > 0 && phoneNumber[0] == '+'
                ? "+" + new string(phoneNumber.Where(char.IsDigit).ToArray())
                : new string(phoneNumber.Where(char.IsDigit).ToArray());

            return digits.Length == 12 && digits.StartsWith("+7");
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

        private void TopPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
