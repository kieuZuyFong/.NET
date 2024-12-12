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
            this.Hide(); // Ẩn cửa sổ Menu hiện tại
            Hangxe hangxeWindow = new Hangxe();
            hangxeWindow.ShowDialog(); // Hiển thị cửa sổ Thêm Hãng xe
            this.Show(); // Hiển thị lại Menu sau khi đóng Hangxe
        }

        // Phương thức xử lý sự kiện nhấn nút "Quản lý Người dùng"
        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // Ẩn cửa sổ Menu hiện tại
            User userWindow = new User();
            userWindow.ShowDialog(); // Hiển thị cửa sổ Quản lý Người dùng
            this.Show(); // Hiển thị lại Menu sau khi đóng User
        }

        // Phương thức xử lý sự kiện nhấn nút "Thoát"
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Thoát ứng dụng
        }
    }
}
