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
using System.Windows.Shapes;

namespace SearchTour
{
    /// <summary>
    /// Логика взаимодействия для DirectionsManagementWindow.xaml
    /// </summary>
    public partial class DirectionsManagementWindow : Window
    {
        private string connectionString = "Data Source=DESKTOP-OD7L183\\SQLEXPRESS;Initial Catalog=Tour;Integrated Security=True";

        public DirectionsManagementWindow()
        {
            InitializeComponent();
            LoadTours();
            LoadClients();
        }
        private void LoadClients()
        {
            string query = "SELECT ClientID, FirstName, LastName FROM Client";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable clientTable = new DataTable();
                adapter.Fill(clientTable);
                clientComboBox.ItemsSource = clientTable.DefaultView;
                clientComboBox.DisplayMemberPath = "FirstName";
                clientComboBox.SelectedValuePath = "ClientID";
            }
        }

        private void LoadTours()
        {
            string query = "SELECT TourID, TourName, Destination, StartDate, Duration, Price, Description FROM Tour";

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
                        toursDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void AddTourButton_Click(object sender, RoutedEventArgs e)
        {
            string query = "INSERT INTO Tour (TourName, Destination, StartDate, Duration, Price, Description) " +
                          "VALUES (@TourName, @Destination, @StartDate, @Duration, @Price, @Description)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TourName", tourNameTextBox.Text);
                    command.Parameters.AddWithValue("@Destination", destinationTextBox.Text);
                    command.Parameters.AddWithValue("@StartDate", startDatePicker.SelectedDate);
                    command.Parameters.AddWithValue("@Duration", Int32.Parse(durationTextBox.Text));
                    command.Parameters.AddWithValue("@Price", Decimal.Parse(priceTextBox.Text));
                    command.Parameters.AddWithValue("@Description", descriptionTextBox.Text);
                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Тур успешно добавлен");
                            LoadTours();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить тур");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void RecommendToursForClient(int clientId)
        {
            // Я ПОКА НЕ РАЗБИРАЛСЯ С РЕКОМЕНДАЦИЯМИ ПОКА ТАК
            string query = "SELECT TOP 5 TourID, TourName, Destination, StartDate, Duration, Price, Description FROM Tour";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ClientID", clientId);
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable recommendedToursTable = new DataTable();
                    adapter.Fill(recommendedToursTable);
                    recommendedToursDataGrid.ItemsSource = recommendedToursTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при получении рекомендаций: " + ex.Message);
                }
            }
        }

        private void clientComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (clientComboBox.SelectedValue != null && int.TryParse(clientComboBox.SelectedValue.ToString(), out int clientId))
            {
                RecommendToursForClient(clientId);
            }
        }

        private void DeleteTourButton_Click_1(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)toursDataGrid.SelectedItem;
            int tourId = (int)selectedRow["TourID"];

            string query = "DELETE FROM Tour WHERE TourID = @TourID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TourID", tourId);
                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Тур успешно удален");
                            LoadTours();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить тур");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void OpenTourBookingWindow_Click(object sender, RoutedEventArgs e)
        {
            TourBookingWindow tourBookingWindow = new TourBookingWindow();
            tourBookingWindow.Show();
            this.Close();
        }
    }
}