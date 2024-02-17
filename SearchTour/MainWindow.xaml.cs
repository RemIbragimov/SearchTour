using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace SearchTour
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=DESKTOP-OD7L183\\SQLEXPRESS;Initial Catalog=Tour;Integrated Security=True";

        public MainWindow()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            string query = "SELECT ClientID, FirstName, LastName, Email, PhoneNumber, OtherDetails FROM Client";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        clientsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void SaveClientButton_Click(object sender, RoutedEventArgs e)
        {
            string query = "INSERT INTO Client (FirstName, LastName, Email, PhoneNumber, OtherDetails) " +
                           "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @OtherDetails)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstNameTextBox.Text);
                    command.Parameters.AddWithValue("@LastName", lastNameTextBox.Text);
                    command.Parameters.AddWithValue("@Email", emailTextBox.Text);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneTextBox.Text);
                    command.Parameters.AddWithValue("@OtherDetails", otherDetailsTextBox.Text);
                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Информация о клиенте успешно сохранена");
                            LoadClients();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось сохранить информацию о клиенте");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка: " + ex.Message);
                    }
                }
            }
        }
        private void OpenDirectionsManagementWindow_Click(object sender, RoutedEventArgs e)
        {
            DirectionsManagementWindow directionsManagementWindow = new DirectionsManagementWindow();
            directionsManagementWindow.Show();
            this.Close();
        }
    }
}