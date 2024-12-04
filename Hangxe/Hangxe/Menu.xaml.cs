using System.Windows;

namespace QLHangxe
{
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        // Phương thức xử lý sự kiện nhấn nút "Thêm Hãng xe"
        private void BtnAddCarBrand_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // Đóng cửa sổ Menu hiện tại
            Hangxe hangxeWindow = new Hangxe(); // Mở cửa sổ Hangxe
            hangxeWindow.ShowDialog();
            this.Show();
        }

        // Phương thức xử lý sự kiện nhấn nút "Thoát"
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Thoát ứng dụng
        }
    }
}
