using System;
using System.Collections.Generic;
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
using Hotelbewertungsportal.Model;
using System.Data.SqlClient;

namespace Hotelbewertungsportal
{
    public partial class MainWindow : Window
    {
        public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\TFS\Hotelbewertungsportal_10\Hotelbewertungsportal\AppData\HotelBewertungDB.mdf;Integrated Security=True";
        SqlConnection connection;
        SqlCommand cmd;
        SqlDataReader reader;

        List<Hotel> Hotels = new List<Hotel>();

        public MainWindow()
        {
            InitializeComponent();

            //Hotels.Add(new Hotel()
            //{
            //    HotelId = 1,
            //    Hotelname = "Hotel Nummer 1",
            //    Adresse = "Muster Strasse 50",
            //    Email = "hotel1@gmail.com",
            //    Preis = 100,
            //    GesamtBewertungen = 0,
            //    AnzahlBewertungen = 0,
            //    Durchschnitt = 0
            //});
            //Hotels.Add(new Hotel()
            //{
            //    HotelId = 2,
            //    Hotelname = "Hotel Nummer 2",
            //    Adresse = "Muster Strasse 100",
            //    Email = "hotel2@gmail.com",
            //    Preis = 250,
            //    GesamtBewertungen = 0,
            //    AnzahlBewertungen = 0,
            //    Durchschnitt = 0
            //});
            //Hotels.Add(new Hotel()
            //{
            //    HotelId = 3,
            //    Hotelname = "Hotel Nummer 3",
            //    Adresse = "Muster Strasse 2",
            //    Email = "hotel3@gmail.com",
            //    Preis = 120,
            //    GesamtBewertungen = 0,
            //    AnzahlBewertungen = 0,
            //    Durchschnitt = 0
            //});
            //Hotels.Add(new Hotel()
            //{
            //    HotelId = 4,
            //    Hotelname = "Hotel Nummer 4",
            //    Adresse = "Muster Strasse 78",
            //    Email = "hotel4@gmail.com",
            //    Preis = 80,
            //    GesamtBewertungen = 0,
            //    AnzahlBewertungen = 0,
            //    Durchschnitt = 0
            //});

            //HotelnameBox.ItemsSource = Hotels;

            UpdateHotels();
        }

        void UpdateHotels()
        {
            try
            {
                Hotels = new List<Hotel>();
                //Verbindung wird erstellt
                connection = new SqlConnection(connectionString);
                connection.Open();

                //Daten werden hinzugefuegt 
                string query = "Select * from Hotel";
                cmd = new SqlCommand(query, connection);
                reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    Hotel hotel = new Hotel();
                    hotel.HotelId = Convert.ToInt32(reader["HotelId"]);
                    hotel.Email = reader["Email"].ToString();
                    hotel.Adresse = reader["Adresse"].ToString();
                    hotel.AnzahlBewertungen = Convert.ToInt32(reader["AnzahlBewertungen"]);
                    hotel.Durchschnitt = Convert.ToDouble(reader["Durchschnitt"]);
                    hotel.GesamtBewertungen = Convert.ToInt32(reader["GesamtBewertungen"]);
                    hotel.Hotelname = reader["Hotelname"].ToString();
                    hotel.Preis = Convert.ToInt32(reader["Preis"]);

                    Hotels.Add(hotel);
                }

                HotelnameBox.ItemsSource = Hotels;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    //Verbindung wird geschlossen
                    connection.Close();
                }
            }
        }
        private void SendenButton_Click_1(object sender, RoutedEventArgs e)
        {
           //eine Bewertung wird erstellt  
            HotelBewertung hotel = new HotelBewertung();

            //Username Input- aus der Textbox "Username"
            hotel.Username = Username.Text;

            //Hotelname Auswahl- aus der Combobox "Hotelname"
            hotel.Hotelname = (HotelnameBox.SelectedItem as Hotel).Hotelname;
            int HotelId = (HotelnameBox.SelectedItem as Hotel).HotelId;

            //Check In Datum- aus dem "DatePicker"
            hotel.Datum = DatePicker.SelectedDate.GetValueOrDefault();

            //Aufenthaltsdauer Aswahl- aus den Radio Buttons "Dauer"
            if (Dauer1.IsChecked == true)
                hotel.Dauer = Dauer1.Content.ToString(); //Dauer1 = 1-3 Tage

            if (Dauer2.IsChecked == true)
                hotel.Dauer = Dauer2.Content.ToString(); //Dauer2 = 3-5 Tage

            if (Dauer3.IsChecked == true)
                hotel.Dauer = Dauer3.Content.ToString(); //Dauer3 = 1-2 Wochen

            if (Dauer4.IsChecked == true)
                hotel.Dauer = Dauer4.Content.ToString(); //Dauer4 = Länger als 2 Wochen

            //Sternebewertung input- aus der Liste "Sternebewertung"
            ListBoxItem item = Sternebewertung.SelectedItem as ListBoxItem;

            if (item.Name == "EinStern")
                hotel.Bewertung = 1;
            if (item.Name == "ZweiSterne")
                hotel.Bewertung = 2;
            if (item.Name == "DreiSterne")
                hotel.Bewertung = 3;
            if (item.Name == "VierSterne")
                hotel.Bewertung = 4;
            if (item.Name == "FuenfSterne")
                hotel.Bewertung = 5;

            //Weiterempfehlung Auswahl- aus den Radio Buttons "Empfehlenswert" / "Nicht Empfehlenswert"
            if (Empfehlenswert.IsChecked == true)
                hotel.Empfehlung = true;
            else if (NichtEmpfehlenswert.IsChecked == true)
                hotel.Empfehlung = false;

            //die Bewertung wird in der DB gespeichert 
            bool gespeichert = SaveToDatabase(hotel);

            //wenn es gespeichert wurde
            if (gespeichert == true)
            {
                //Durchnitt wird berechnet und angezeigt 
                Hotel h = Hotels.First(x => x.HotelId == HotelId);
                h.GesamtBewertungen += hotel.Bewertung;
                h.AnzahlBewertungen++;
                h.Durchschnitt = Convert.ToDouble(h.GesamtBewertungen / h.AnzahlBewertungen);

                connection = new SqlConnection(connectionString);
                connection.Open();

                //Daten werden hinzugefuegt 
                string query = "Update Hotel Set GesamtBewertungen = "+h.GesamtBewertungen + ", AnzahlBewertungen= "+ h.AnzahlBewertungen + ",Durchschnitt="+ h.Durchschnitt + " Where HotelId = "+h.HotelId+"";
                cmd = new SqlCommand(query, connection);

                cmd.ExecuteNonQuery();

                UpdateHotels();

                MessageBox.Show("Vielen Dank für Ihre Bewertung! Das " + h.Hotelname + " hat eine durchschnittliche Bewertung von " + h.Durchschnitt);
            }
            else
                MessageBox.Show("Fehler!");
        }

        private bool SaveToDatabase(HotelBewertung hotel)
        {
            bool gespeichert = false;

            try
            {
                //Verbindung wird erstellt
                connection = new SqlConnection(connectionString);
                connection.Open();

                //Daten werden hinzugefuegt 
                string query = "Insert Into Bewertung (Username, Hotelname, Datum, Dauer, Bewertung, Empfehlung) Values ('" + hotel.Username + "','" + hotel.Hotelname + "','" + hotel.Datum + "','" + hotel.Dauer + "'," + hotel.Bewertung + ",'" + hotel.Empfehlung + "') ";
                cmd = new SqlCommand(query, connection);

                cmd.ExecuteNonQuery();
                gespeichert = true;
            }
            catch (Exception ex)
            {
                gespeichert = false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    //Verbindung wird geschlossen
                    connection.Close();
                }
            }
            return gespeichert;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Infos.IsSelected == true)
            {
                InfoHotels.ItemsSource = Hotels;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Username.Text = string.Empty;
            HotelnameBox.SelectedItem = null;
            DatePicker.SelectedDate = DateTime.Now;
            Dauer1.IsChecked = false;
            Dauer2.IsChecked = false;
            Dauer3.IsChecked = false;
            Dauer4.IsChecked = false;
            Empfehlenswert.IsChecked = false;
            NichtEmpfehlenswert.IsChecked = false;
            Sternebewertung.SelectedItem = null;
        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetVisibility();
        }

        private void HotelnameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetVisibility();
        }

        private void SetVisibility()
        {
            string Dauer = string.Empty;
            bool Empfehlung = false;
            if (Dauer1.IsChecked == true)
                Dauer = Dauer1.Content.ToString(); //Dauer1 = 1-3 Tage

            if (Dauer2.IsChecked == true)
                Dauer = Dauer2.Content.ToString(); //Dauer2 = 3-5 Tage

            if (Dauer3.IsChecked == true)
                Dauer = Dauer3.Content.ToString(); //Dauer3 = 1-2 Wochen

            if (Dauer4.IsChecked == true)
                Dauer = Dauer4.Content.ToString(); //Dauer4 = Länger als 2 Wochen

            if (Empfehlenswert.IsChecked == true)
                Empfehlung = true;
            else if (NichtEmpfehlenswert.IsChecked == true)
                Empfehlung = true;

            if (string.IsNullOrEmpty(Username.Text) || HotelnameBox.SelectedItem == null || DatePicker.SelectedDate == null
                || string.IsNullOrEmpty(Dauer) || Sternebewertung.SelectedItem == null || Empfehlung == false)
            {
                SendenButton.IsEnabled = false;
            }
            else
            {
                SendenButton.IsEnabled = true;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetVisibility();
        }

        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            SetVisibility();
        }

        private void Sternebewertung_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetVisibility();
        }

        private void InfoHotels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(InfoHotels.SelectedItem != null)
            {
                var hotel = InfoHotels.SelectedItem as Hotel;
                lblRatings.Content = "Avg. Rating : " + hotel.Durchschnitt;
            }
           
        }
    }

}