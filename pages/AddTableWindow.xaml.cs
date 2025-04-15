﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace usbechka_app.pages
{
    public partial class AddTableWindow : Window
    {
        public AddTableWindow()
        {
            InitializeComponent();
        }

        private void ButtonConfirm(object sender, RoutedEventArgs e)
        {
            if (number.Text.Trim() == null)
            {
                CustomMessageBox.Show("Проверьте правильность введенных данных.");
                return;
            }

            if (!int.TryParse(number.Text.Trim(), out _))
            {
                CustomMessageBox.Show("Номер стола должен быть числом!");
                return;
            }

            int newNumber = int.Parse(number.Text.Trim());

            tables table = AppData.Db.tables.SingleOrDefault(x => x.number_table == newNumber);
            if (null != table)
            {
                CustomMessageBox.Show("Стол уже добавлен.");
                return;
            }
            
            tables newTable= new tables
            {
                id = Guid.NewGuid(),
                number_table = newNumber
            };
            AppData.Db.tables.Add(newTable);
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
