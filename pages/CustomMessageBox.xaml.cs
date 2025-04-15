using System.Windows;

namespace usbechka_app.pages
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public static bool? Show(string message)
        {
            var msgBox = new CustomMessageBox(message);
            return msgBox.ShowDialog();
        }
    }
}
