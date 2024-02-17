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
    /// Логика взаимодействия для TourBookingWindow.xaml
    /// </summary>
    public partial class TourBookingWindow : Window
    {
        private string connectionString = "Data Source=DESKTOP-OD7L183\\SQLEXPRESS;Initial Catalog=Tour;Integrated Security=True";

        public TourBookingWindow()
        {
            InitializeComponent();
            LoadTours();
            LoadClients();
            LoadBookings();
        }

        private void LoadTours()
        {
            string query = "SELECT TourID, TourName FROM Tour";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable tourTable = new DataTable();
                adapter.Fill(tourTable);
                tourComboBox.ItemsSource = tourTable.DefaultView;
                tourComboBox.DisplayMemberPath = "TourName";
                tourComboBox.SelectedValuePath = "TourID";
            }
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

        private void LoadBookings()
        {
            string query = "SELECT BookingID, BookingDate, OtherOptions, TourName, FirstName, LastName FROM Booking JOIN Client ON Booking.ClientID = Client.ClientID JOIN Tour ON Booking.TourID = Tour.TourID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable bookingTable = new DataTable();
                adapter.Fill(bookingTable);
                bookingsDataGrid.ItemsSource = bookingTable.DefaultView;
            }
        }

        private void bookTourButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedClientID = (int)clientComboBox.SelectedValue;
            int selectedTourID = (int)tourComboBox.SelectedValue;
            DateTime bookingDate = bookingDatePicker.SelectedDate ?? DateTime.Now;
            string otherOptions = otherOptionsTextBox.Text;

            string query = "INSERT INTO Booking (ClientID, TourID, BookingDate, OtherOptions) VALUES (@ClientID, @TourID, @BookingDate, @OtherOptions)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ClientID", selectedClientID);
                command.Parameters.AddWithValue("@TourID", selectedTourID);
                command.Parameters.AddWithValue("@BookingDate", bookingDate);
                command.Parameters.AddWithValue("@OtherOptions", otherOptions);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Тур успешно забронирован");
                        LoadBookings();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось забронировать тур");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
            }
        }
    }
}