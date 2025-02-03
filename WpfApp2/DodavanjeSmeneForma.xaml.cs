using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for DodavanjeSmeneForma.xaml
    /// </summary>
    public partial class DodavanjeSmeneForma : Window
    {
        Osoblje admin = new Osoblje();
        RestoranDataContext DC = new RestoranDataContext();
        SqlConnection konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
        public DodavanjeSmeneForma(Osoblje admin)
        {
            InitializeComponent();
            this.admin = admin;
            prikaziSveSmene();
        }

        private void ocisti()
        {
            datePicker.SelectedDate = null;
            dataGridSmene.Items.Clear();
            tbPH.Text = string.Empty;
            tbPM.Text = string.Empty;
            tbKH.Text = string.Empty;
            tbKM.Text = string.Empty;
        }

        private void prikaziSveSmene()
        {
            dataGridSmene.Items.Clear();
            SqlCommand komanda = new SqlCommand();
            komanda.Connection = konekcija;
            komanda.CommandText = $"SELECT * FROM Smena S ORDER BY Datum";
            konekcija.Open();

            using (SqlDataReader reader = komanda.ExecuteReader())
            {
                while(reader.Read())
                {
                    Smena smena = new Smena();
                    smena.SmenaID = (int)reader["SmenaID"];
                    smena.Datum = (DateTime)reader["Datum"];
                    smena.PocetakSmene = (TimeSpan)reader["PocetakSmene"];
                    smena.KrajSmene = (TimeSpan)reader["KrajSmene"];
                    dataGridSmene.Items.Add(smena);
                }
            }
            konekcija.Close();

            if (datePicker.SelectedDate != null)
                btnPoDatumu.IsEnabled = true;
            btnSveSmene.IsEnabled = false;
        }

        private void prikaziPoDatumu()
        {
            dataGridSmene.Items.Clear();
            SqlCommand komanda = new SqlCommand();
            komanda.Connection = konekcija;
            komanda.CommandText = $"SELECT * FROM Smena S WHERE S.Datum = '{datePicker.SelectedDate.Value.Date.ToString("yyyy-MM-dd")}' ORDER BY Datum";
            konekcija.Open();
            using (SqlDataReader reader = komanda.ExecuteReader())
            {
                while (reader.Read())
                {
                    Smena smena = new Smena();
                    smena.SmenaID = (int)reader["SmenaID"];
                    smena.Datum = (DateTime)reader["Datum"];
                    smena.PocetakSmene = (TimeSpan)reader["PocetakSmene"];
                    smena.KrajSmene = (TimeSpan)reader["KrajSmene"];
                    dataGridSmene.Items.Add(smena);
                }
            }
            konekcija.Close();

            btnSveSmene.IsEnabled = true;
            btnPoDatumu.IsEnabled = false;
        }

        private void btnPoDatumu_Click(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate != null)
            {
                prikaziPoDatumu();
            }
            else
                MessageBox.Show("Morate izabrati neki datum.");
        }

        private void btnSveSmene_Click(object sender, RoutedEventArgs e)
        {
            prikaziSveSmene();
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datePicker.SelectedDate != null)
                prikaziPoDatumu();
        }

        private void btnKreirajSmenu_Click(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate != null && !String.IsNullOrEmpty(tbPH.Text) && !String.IsNullOrEmpty(tbPM.Text)
                && !String.IsNullOrEmpty(tbKH.Text) && !String.IsNullOrEmpty(tbKM.Text))
            {
                try
                {
                    Smena smena = new Smena();
                    smena.Datum = (DateTime)datePicker.SelectedDate;
                    smena.PocetakSmene = TimeSpan.Parse(tbPH.Text + ":" + tbPM.Text);
                    smena.KrajSmene = TimeSpan.Parse(tbKH.Text + ":" + tbKM.Text);

                    DC.Smenas.InsertOnSubmit(smena);
                    DC.SubmitChanges();

                    MessageBox.Show("Uspešno ste uneli smenu.");
                    ocisti();
                    prikaziSveSmene();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problem: " + ex.Message);
                }
            }
            else
                MessageBox.Show("Morate popuniti sva polja.");
        }

        private void messageBox(TextBox tb)
        {
            MessageBox.Show("Morate da ispravite vrednost.");
            tb.Text = string.Empty;
            tb.Focus();
        }

        private void proveriInput(TextBox tb)
        {
            if (!String.IsNullOrEmpty(tb.Text))
            {
                int broj;
                bool validan;
                validan = int.TryParse(tb.Text, NumberStyles.Integer, CultureInfo.GetCultureInfo("en-US"), out broj);
                if (!validan)
                {
                    messageBox(tb);
                }
                else
                {
                    if ((tb == tbPH || tb == tbKH) && (broj > 23 || broj < 0))
                    {
                        messageBox(tb);
                    }
                    else if ((tb == tbPM || tb == tbKM) && (broj > 59 || broj < 0))
                    {
                        messageBox(tb);
                    }
                }
            }
        }

        private void tbPH_TextChanged(object sender, TextChangedEventArgs e)
        {
            proveriInput(tbPH);
        }

        private void tbPM_TextChanged(object sender, TextChangedEventArgs e)
        {
            proveriInput(tbPM);
        }

        private void tbKH_TextChanged(object sender, TextChangedEventArgs e)
        {
            proveriInput(tbKH);
        }

        private void tbKM_TextChanged(object sender, TextChangedEventArgs e)
        {
            proveriInput(tbKM);
        }

        private void btnRaspoprediSmene_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();

            foreach (Window w in System.Windows.Application.Current.Windows)
                if (w != this)
                    if (w.Title == "Raspoređivanje smena")
                    {
                        window = w;
                    }

            if (window.Title == "Raspoređivanje smena")
            {
                window.WindowState = WindowState.Minimized;
                window.WindowState = WindowState.Normal;
                window.Focus();
            }
            else
            {
                RasporediSmeneForma rs = new RasporediSmeneForma(admin);
                rs.Show();
            }            
        }

        private void btnObrisiSmenu_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridSmene.SelectedValue != null)
            {
                if (MessageBox.Show("Da li ste sigurni da želite da obrišete smene?", "Brisanje smena", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (Smena s in dataGridSmene.SelectedItems)
                    {
                        SqlCommand komanda = new SqlCommand();
                        komanda.Connection = konekcija;
                        konekcija.Open();
                        komanda.CommandText = $"DELETE FROM Istorija WHERE SmenaID = '{s.SmenaID}'";
                        komanda.ExecuteNonQuery();
                        konekcija.Close();

                        konekcija.Open();
                        komanda.CommandText = $"DELETE FROM Smena WHERE SmenaID = '{s.SmenaID}'";
                        komanda.ExecuteNonQuery();
                        konekcija.Close();
                    }
                    ocisti();
                    btnPoDatumu.IsEnabled = false;
                    prikaziSveSmene();
                    MessageBox.Show("Uspešno ste izbrisali smene!");
                }
            }
            else
                MessageBox.Show("Morate izabrati neku stavku od ponuđenih.");
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            prikaziSveSmene();
        }
    }
}
