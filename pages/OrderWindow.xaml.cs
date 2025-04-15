using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace usbechka_app.pages
{
    public partial class OrderWindow : Window
    {
        private Guid _userId;
        private Guid _tableId;
        private List<menu> _menuItems;
        private Dictionary<Guid, int> _selectedItems = new Dictionary<Guid, int>();

        public OrderWindow(Guid userId, Guid tableId)
        {
            InitializeComponent();
            _userId = userId;
            _tableId = tableId;
            LoadMenuItems();
            UpdateTotalPrice();
        }

        private void LoadMenuItems()
        {
            _menuItems = AppData.Db.menu.ToList();

            MenuStackPanel.Children.Clear();

            foreach (var item in _menuItems)
            {
                var border = new Border
                {
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    Background = new SolidColorBrush(Color.FromRgb(54, 54, 54)),
                    CornerRadius = new CornerRadius(5),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(85, 85, 85)),
                    BorderThickness = new Thickness(1)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                var checkBox = new CheckBox
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 10, 0),
                    Tag = item.id,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 122, 204))
                };
                checkBox.Checked += CheckBox_Checked;
                checkBox.Unchecked += CheckBox_Unchecked;
                Grid.SetColumn(checkBox, 0);

                var infoStack = new StackPanel();
                infoStack.Children.Add(new TextBlock
                {
                    Text = item.title,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White
                });
                infoStack.Children.Add(new TextBlock
                {
                    Text = item.description,
                    FontStyle = FontStyles.Italic,
                    Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200))
                });
                infoStack.Children.Add(new TextBlock
                {
                    Text = $"{item.price} руб.",
                    Foreground = new SolidColorBrush(Color.FromRgb(100, 200, 100)),
                    Margin = new Thickness(0, 5, 0, 0)
                });
                Grid.SetColumn(infoStack, 1);

                var quantityPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Visibility = Visibility.Collapsed,
                    Tag = item.id
                };

                var minusButton = new Button
                {
                    Content = "-",
                    Width = 25,
                    Margin = new Thickness(2),
                    Tag = quantityPanel,
                    Background = new SolidColorBrush(Color.FromRgb(69, 69, 69)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0)
                };
                minusButton.Click += QuantityDown_Click;

                var quantityText = new TextBlock
                {
                    Text = "1",
                    Width = 30,
                    TextAlignment = TextAlignment.Center,
                    Tag = item.id,
                    Foreground = Brushes.White
                };

                var plusButton = new Button
                {
                    Content = "+",
                    Width = 25,
                    Margin = new Thickness(2),
                    Tag = quantityPanel,
                    Background = new SolidColorBrush(Color.FromRgb(69, 69, 69)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0)
                };
                plusButton.Click += QuantityUp_Click;

                quantityPanel.Children.Add(minusButton);
                quantityPanel.Children.Add(quantityText);
                quantityPanel.Children.Add(plusButton);
                Grid.SetColumn(quantityPanel, 2);

                grid.Children.Add(checkBox);
                grid.Children.Add(infoStack);
                grid.Children.Add(quantityPanel);

                border.Child = grid;

                MenuStackPanel.Children.Add(border);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox) || checkBox.Tag == null) return;
            if (!(checkBox.Tag is Guid menuId)) return;

            _selectedItems[menuId] = 1;

            SetQuantityPanelVisibility(menuId, Visibility.Visible);

            UpdateTotalPrice();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox) || checkBox.Tag == null) return;
            if (!(checkBox.Tag is Guid menuId)) return;

            _selectedItems.Remove(menuId);

            SetQuantityPanelVisibility(menuId, Visibility.Collapsed);

            UpdateTotalPrice();
        }

        private void SetQuantityPanelVisibility(Guid menuId, Visibility visibility)
        {
            foreach (var border in MenuStackPanel.Children.OfType<Border>())
            {
                if (!(border.Child is Grid grid) || grid.Children.Count <= 2) continue;
                if (!(grid.Children[2] is StackPanel panel)) continue;
                if (panel.Tag == null || !(panel.Tag is Guid panelMenuId)) continue;

                if (panelMenuId == menuId)
                {
                    panel.Visibility = visibility;
                    break;
                }
            }
        }

        private void QuantityUp_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var panel = (StackPanel)button.Tag;
            var menuId = (Guid)panel.Tag;

            var quantityText = panel.Children.OfType<TextBlock>().First();
            int quantity = int.Parse(quantityText.Text) + 1;
            quantityText.Text = quantity.ToString();

            _selectedItems[menuId] = quantity;
            UpdateTotalPrice();
        }

        private void QuantityDown_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var panel = (StackPanel)button.Tag;
            var menuId = (Guid)panel.Tag;

            var quantityText = panel.Children.OfType<TextBlock>().First();
            int quantity = int.Parse(quantityText.Text);

            if (quantity > 1)
            {
                quantity--;
                quantityText.Text = quantity.ToString();
                _selectedItems[menuId] = quantity;
                UpdateTotalPrice();
            }
        }

        private void PaymentMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            decimal totalRub = 0;
            foreach (var item in _selectedItems)
            {
                var menuItem = _menuItems.FirstOrDefault(m => m.id == item.Key);
                if (menuItem != null)
                {
                    totalRub += menuItem.price * item.Value;
                }
            }

            if (PaymentMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string paymentMethod = selectedItem.Content.ToString();
                if (paymentMethod == "Trump Coin")
                {
                    decimal totalTrump = totalRub / 718;
                    TotalPriceTextBlock.Text = $"Итого: {totalTrump:F2} TRUMP";
                }
                else
                {
                    TotalPriceTextBlock.Text = $"Итого: {totalRub} руб.";
                }
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedItems.Count == 0)
            {
                CustomMessageBox.Show("Выберите хотя бы одно блюдо!");
                return;
            }

            if (PaymentMethodComboBox.SelectedItem == null)
            {
                CustomMessageBox.Show("Выберите способ оплаты!");
                return;
            }

            var paymentMethod = "";
            decimal totalAmount = 0;

            if (PaymentMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                paymentMethod = selectedItem.Content.ToString();

                totalAmount = _menuItems
                    .Where(m => _selectedItems.ContainsKey(m.id))
                    .Sum(m => m.price * _selectedItems[m.id]);

                if (paymentMethod == "Trump Coin")
                {
                    totalAmount /= 718;
                }
            }

            var menuIdsString = string.Join(" ", _selectedItems.Select(item => $"{item.Key}:{item.Value}"));
            var newOrder = new orders
            {
                id = Guid.NewGuid(),
                user_id = _userId,
                table_id = _tableId,
                time_end = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(2)),
                menu_ids = menuIdsString,
                full_price = totalAmount,
                payment_date = DateTime.Now,
                payment_method = paymentMethod
            };

            try
            {
                AppData.Db.orders.Add(newOrder);
                AppData.Db.SaveChanges();
                CustomMessageBox.Show("Заказ успешно оформлен!");
                new ClientWindow(_userId).Show();
                Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new ClientWindow(_userId).Show();
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
