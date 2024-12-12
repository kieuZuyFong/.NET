using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace QLHangxe
{
    public partial class User : Window
    {
        // Chuỗi kết nối đến cơ sở dữ liệu
        private readonly string connectionString = "Server=DESKTOP-OHOK4N1;Database=Hang_xe;Trusted_Connection=True;TrustServerCertificate=True;";

        public User()
        {
            InitializeComponent();
            LoadUsers(); // Tải danh sách khi khởi động
        }

        // Phương thức tải danh sách 
        private void LoadUsers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users1", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvUsers.ItemsSource = dt.DefaultView; // Đảm bảo DataGrid có tên khác
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức tìm kiếm 
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Users1 WHERE 1=1";
                    var queryBuilder = new System.Text.StringBuilder(query);
                    var cmd = new SqlCommand();

                    if (!string.IsNullOrEmpty(txtUserId.Text))
                    {
                        queryBuilder.Append(" AND UserId = @UserId");
                        cmd.Parameters.AddWithValue("@UserId", txtUserId.Text.Trim());
                    }
                    if (!string.IsNullOrEmpty(txtUserName.Text))
                    {
                        queryBuilder.Append(" AND UserName LIKE @UserName");
                        cmd.Parameters.AddWithValue("@UserName", $"%{txtUserName.Text.Trim()}%");
                    }

                    cmd.CommandText = queryBuilder.ToString();
                    cmd.Connection = conn;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvUsers.ItemsSource = dt.DefaultView; // Đảm bảo DataGrid có tên khác
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hiển thị chi tiết người dùng khi chọn dòng trong DataGrid
        private void dgvUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvUsers.SelectedItem is DataRowView row)
            {
                txtUserIdDetail.Text = row["UserId"].ToString();
                txtUserNameDetail.Text = row["UserName"].ToString();
                txtAge.Text = row["Age"].ToString();
                txtAddress.Text = row["Address"].ToString();
                txtPhone.Text = row["Phone"].ToString();

                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        // Thêm mới
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ClearDetails();
            txtUserIdDetail.IsReadOnly = true;
            btnSave.Tag = "Add";
        }

        // Sửa thông tin 
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgvUsers.SelectedItem != null)
            {
                btnSave.Tag = "Edit";
            }
        }

        // Xoá 
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgvUsers.SelectedItem is DataRowView row)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Users1 WHERE UserId = @UserId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@UserId", row["UserId"]);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Xoá thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xoá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Lưu thay đổi (thêm hoặc sửa)
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand { Connection = conn };

                    if (btnSave.Tag?.ToString() == "Add")
                    {
                        string query = "INSERT INTO Users1 (UserName, Age, Address, Phone) VALUES (@UserName, @Age, @Address, @Phone)";
                        cmd.Parameters.AddWithValue("@UserName", txtUserNameDetail.Text);
                        cmd.Parameters.AddWithValue("@Age", txtAge.Text);
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                    else if (btnSave.Tag?.ToString() == "Edit")
                    {
                        string query = "UPDATE Users1 SET UserName = @UserName, Age = @Age, Address = @Address, Phone = @Phone WHERE UserId = @UserId";
                        cmd.Parameters.AddWithValue("@UserId", txtUserIdDetail.Text);
                        cmd.Parameters.AddWithValue("@UserName", txtUserNameDetail.Text);
                        cmd.Parameters.AddWithValue("@Age", txtAge.Text);
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Không xác định được hành động.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearDetails();
        }

        private void ClearDetails()
        {
            txtUserIdDetail.Clear();
            txtUserNameDetail.Clear();
            txtAge.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Menu menuWindow = new Menu();
            menuWindow.ShowDialog();
            this.Show();
        }
    }
}
