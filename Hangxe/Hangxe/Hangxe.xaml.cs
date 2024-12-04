using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace QLHangxe
{
    public partial class Hangxe : Window
    {
        // Chuỗi kết nối đến cơ sở dữ liệu
        private readonly string connectionString = "Server=DESKTOP-OHOK4N1;Database=Hang_xe;Trusted_Connection=True;TrustServerCertificate=True;";

        public Hangxe()
        {
            InitializeComponent();
            LoadHangxe(); // Tải danh sách khi khởi động
        }

        // Phương thức tải danh sách 
        private void LoadHangxe()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM HangXe", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvHangxe.ItemsSource = dt.DefaultView;
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
                    string query = "SELECT * FROM HangXe WHERE 1=1";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(txtID.Text))
                    {
                        query += " AND ID = @ID";
                        cmd.Parameters.AddWithValue("@ID", txtID.Text);
                    }
                    if (!string.IsNullOrEmpty(txtTenHangXe.Text))
                    {
                        query += " AND TenHangXe LIKE @TenHangXe";
                        cmd.Parameters.AddWithValue("@TenHangXe", $"%{txtTenHangXe.Text}%");
                    }

                    cmd.CommandText = query;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvHangxe.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hiển thị chi tiết hãng xe khi chọn dòng trong DataGrid
        private void dgvHangxe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvHangxe.SelectedItem is DataRowView row)
            {
                txtIDDetail.Text = row["ID"].ToString();
                txtTenHangXeDetail.Text = row["TenHangXe"].ToString();
                txtSoDienThoai.Text = row["SoDienThoai"].ToString();

                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        // Thêm mới
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ClearDetails();
            txtIDDetail.IsReadOnly = true;
            btnSave.Tag = "Add"; // Gắn cờ thêm mới
        }

        // Sửa thông tin 
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgvHangxe.SelectedItem != null)
            {
                btnSave.Tag = "Edit"; // Gắn cờ sửa
            }
        }

        // Xoá 
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgvHangxe.SelectedItem is DataRowView row)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM HangXe WHERE ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", row["ID"]);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Xoá thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadHangxe();
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

                    // Thêm mới (xử lý ID tự động)
                    if (btnSave.Tag?.ToString() == "Add")
                    {
                        // Tắt IDENTITY_INSERT để cho phép chèn ID vào bảng
                        SqlCommand enableIdentityInsertCmd = new SqlCommand("SET IDENTITY_INSERT HangXe ON", conn);
                        enableIdentityInsertCmd.ExecuteNonQuery();

                        // Tìm ID bị thiếu
                        SqlCommand findMissingIDCmd = new SqlCommand(@"
                            SELECT MIN(T1.ID + 1) AS MissingID
                            FROM Hangxe T1
                            LEFT JOIN Hangxe T2 ON T1.ID + 1 = T2.ID
                            WHERE T2.ID IS NULL;", conn);
                        object missingID = findMissingIDCmd.ExecuteScalar();
                        int newID;

                        if (missingID != DBNull.Value && missingID != null)
                        {
                            newID = Convert.ToInt32(missingID); // Sử dụng ID bị thiếu
                        }
                        else
                        {
                            // Nếu không có ID bị thiếu, tìm ID lớn nhất + 1
                            SqlCommand getMaxIDCmd = new SqlCommand("SELECT ISNULL(MAX(ID), 0) + 1 FROM Hangxe", conn);
                            newID = Convert.ToInt32(getMaxIDCmd.ExecuteScalar());
                        }

                        // Chèn dữ liệu mới với ID tự tính
                        string query = "INSERT INTO HangXe (ID, TenHangXe, SoDienThoai) VALUES (@ID, @TenHangXe, @SoDienThoai)";
                        cmd.Parameters.AddWithValue("@ID", newID);
                        cmd.Parameters.AddWithValue("@TenHangXe", txtTenHangXeDetail.Text);
                        cmd.Parameters.AddWithValue("@SoDienThoai", txtSoDienThoai.Text);
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();

                        // Bật lại IDENTITY_INSERT
                        SqlCommand disableIdentityInsertCmd = new SqlCommand("SET IDENTITY_INSERT HangXe OFF", conn);
                        disableIdentityInsertCmd.ExecuteNonQuery();
                    }
                    // Sửa thông tin
                    else if (btnSave.Tag?.ToString() == "Edit")
                    {
                        string query = "UPDATE HangXe SET TenHangXe = @TenHangXe, SoDienThoai = @SoDienThoai WHERE ID = @ID";
                        cmd.Parameters.AddWithValue("@ID", txtIDDetail.Text);
                        cmd.Parameters.AddWithValue("@TenHangXe", txtTenHangXeDetail.Text);
                        cmd.Parameters.AddWithValue("@SoDienThoai", txtSoDienThoai.Text);
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Không xác định được hành động.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadHangxe();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Huỷ thao tác
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearDetails();
        }

        // Xoá thông tin chi tiết
        private void ClearDetails()
        {
            txtIDDetail.Clear();
            txtTenHangXeDetail.Clear();
            txtSoDienThoai.Clear();
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Menu MenuWindow = new Menu();
            MenuWindow.ShowDialog();
            this.Show();
        }
    }
}
