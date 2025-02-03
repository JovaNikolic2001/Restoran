using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Globalization;
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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for OsobljeForma.xaml
    /// </summary>
    public partial class OsobljeForma : Window
    {
        SqlConnection konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
        SqlConnection konekcija2 = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
        RestoranDataContext DC = new RestoranDataContext();
        Osoblje adminOS = new Osoblje();

        public OsobljeForma(Osoblje admin)
        {
            InitializeComponent();
            napuniComboBox();
            this.adminOS = admin;
        }

        private void napuniComboBox()
        {
            konekcija2.Open();
            SqlCommand komanda2 = new SqlCommand();
            komanda2.Connection = konekcija2;
            komanda2.CommandText = $"SELECT * FROM Osoblje";

            using (SqlDataReader reader = komanda2.ExecuteReader())
            {
                while (reader.Read())
                {
                    Osoblje o = new Osoblje();
                    o.OsobljeID = (int)reader["OsobljeID"];
                    o.Ime = reader["Ime"].ToString();
                    o.Prezime = reader["Prezime"].ToString();
                    o.Uloga = reader["Uloga"].ToString();
                    o.Kontakt = reader["Kontakt"].ToString();
                    o.Plata = (decimal)reader["Plata"];
                    o.Username = reader["Username"].ToString();
                    o.Password = reader["Password"].ToString();
                    o.Admin = (bool)reader["Admin"];
                    cbOsoblje.Items.Add(o);
                }
            }
            konekcija2.Close();
        } 

        private void ocistiSve()
        {
            cbOsoblje.SelectedItem = null;
            tbIme.Text = string.Empty;
            tbPrezime.Text = string.Empty;
            tbUloga.Text = string.Empty;
            tbKontakt.Text = string.Empty;
            tbPlata.Text = string.Empty;
            rbAdminDa.IsChecked = false;
            rbAdminNe.IsChecked = false;
            tbUsername.Text = string.Empty;
            passBox.Password = string.Empty;
            tbPassword.Text = string.Empty;
            btnPogled.IsChecked = true;
            btnIzmeni.IsEnabled = false;
            passBox.Visibility = Visibility.Hidden;
            tbPassword.Visibility = Visibility.Visible;
            btnPogled.Visibility = Visibility.Hidden;
        }

        private void btnOcisti_Click(object sender, RoutedEventArgs e)
        {
            ocistiSve();
        }

        private void cbOsoblje_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbOsoblje.SelectedItem != null && adminOS.Admin == true)
            {
                Osoblje o = cbOsoblje.SelectedItem as Osoblje;

                tbIme.Text = o.Ime;
                tbPrezime.Text = o.Prezime;
                tbUloga.Text = o.Uloga;
                tbKontakt.Text = o.Kontakt;
                tbPlata.Text = decimal.Truncate(o.Plata).ToString();
                if (o.Admin)
                    rbAdminDa.IsChecked = true;
                else
                    rbAdminNe.IsChecked = true;
                tbUsername.Text = o.Username;
                tbPassword.Visibility = Visibility.Hidden;
                passBox.Visibility = Visibility.Visible;
                passBox.Password = o.Password;
                btnPogled.IsChecked = false;
                btnIzmeni.IsEnabled = true;
                btnPogled.Visibility = Visibility.Visible;
            }
        }

        private void btnPogled_Checked(object sender, RoutedEventArgs e)
        {
            Osoblje o = cbOsoblje.SelectedItem as Osoblje;
            if (o != null)
            {
                if (MessageBox.Show("Da li zelite da izmenite šifru?", "Šifra?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    tbPassword.Visibility = Visibility.Visible;
                    passBox.Visibility = Visibility.Hidden;
                    tbPassword.Text = o.Password;
                }
                else
                    btnPogled.IsChecked = false;
            }
            else if (String.IsNullOrEmpty(passBox.Password) && String.IsNullOrEmpty(tbPassword.Text))
            {
                tbPassword.Visibility= Visibility.Visible;
                passBox.Visibility = Visibility.Hidden;
            }
            else
            {
                if (MessageBox.Show("Da li zelite da izmenite šifru?", "Šifra?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    tbPassword.Visibility = Visibility.Visible;
                    passBox.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnPogled_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cbOsoblje.SelectedItem == null && String.IsNullOrEmpty(tbPassword.Text))
            {
                MessageBox.Show("Morate uneti nesto u polje za šifru da bi kreirali korisnika.");
                btnPogled.IsChecked = true;
                tbPassword.Focus();
            }
            else if (cbOsoblje.SelectedItem != null && passBox.Visibility == Visibility.Hidden)
            {
                if (MessageBox.Show("Da li želite da odustanete od izmene šifre?", "Pitanje u vezi šifre!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    tbPassword.Visibility = Visibility.Hidden;
                    passBox.Visibility = Visibility.Visible;
                    passBox.Password = tbPassword.Text;
                    tbPassword.Text = String.Empty;
                }
                else
                    btnPogled.IsChecked = true;
            }
        }

        private bool admin()
        {
            if (rbAdminDa.IsChecked == true || rbAdminNe.IsChecked == true)
                return true;
            else return false;
        }

        private bool isAdmin()
        {
            if (rbAdminDa.IsChecked == true)
                return true;
            else return false;
        }

        private void btnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            if (cbOsoblje.SelectedItem != null)
            {
                if (!String.IsNullOrEmpty(tbIme.Text) && !String.IsNullOrEmpty(tbPrezime.Text) && !String.IsNullOrEmpty(tbUloga.Text) && 
                    !String.IsNullOrEmpty(tbKontakt.Text) && !String.IsNullOrEmpty(tbPlata.Text) && !String.IsNullOrEmpty(tbUsername.Text) || !String.IsNullOrEmpty(passBox.Password) && admin())
                {
                    Osoblje s = (Osoblje)cbOsoblje.SelectedItem;

                    decimal plata;
                    var validan = decimal.TryParse(tbPlata.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out plata);
                    decimal kontakt;
                    var validan2 = decimal.TryParse(tbKontakt.Text, NumberStyles.Integer, CultureInfo.GetCultureInfo("en-US"), out kontakt);
                    if (validan && validan2)
                    {
                        if (MessageBox.Show("Da li ste sigurni da želite da izmenite podatke?", "Izmena podataka", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            string sifra = s.Password;
                            if (!String.IsNullOrEmpty(tbPassword.Text))
                                sifra = tbPassword.Text;

                            konekcija.Open();
                            try
                            {
                                SqlCommand komanda2 = new SqlCommand();
                                komanda2.Connection = konekcija;
                                komanda2.CommandText = $"UPDATE Osoblje SET Ime = '{tbIme.Text}', Prezime = '{tbPrezime.Text}', Uloga = '{tbUloga.Text}', Kontakt = '{tbKontakt.Text}', " +
                                                        $"Plata = '{plata}', Admin = '{isAdmin()}', Username = '{tbUsername.Text}', Password = '{sifra}' WHERE OsobljeID = '{s.OsobljeID}'";
                                komanda2.ExecuteNonQuery();
                                MessageBox.Show("Uspešna izmena podataka!");
                                cbOsoblje.Items.Clear();
                                
                                if (adminOS.OsobljeID == s.OsobljeID)
                                {
                                    adminOS.Ime = tbIme.Text;
                                    adminOS.Prezime = tbPrezime.Text;
                                    adminOS.Uloga = tbUloga.Text;
                                    adminOS.Kontakt = tbKontakt.Text;
                                    if (rbAdminDa.IsChecked == true && rbAdminNe.IsChecked == false)
                                        adminOS.Admin = true;
                                    else
                                        adminOS.Admin = false;
                                    adminOS.Plata = plata;
                                    adminOS.Username = tbUsername.Text;
                                    adminOS.Password = sifra;
                                }
                                ocistiSve();
                                napuniComboBox();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Neuspešna izmena podataka! - " + ex.StackTrace);
                            }
                            finally
                            {
                                konekcija.Close();
                            }
                        }
                    }
                    else
                        MessageBox.Show("Plata i kontakt moraju biti isključivo brojevi.");
                }
                else
                    MessageBox.Show("Morate upisati nešto u sva polja.");
            }
        }

        private void btnKreiraj_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(tbIme.Text) && !String.IsNullOrEmpty(tbPrezime.Text) && !String.IsNullOrEmpty(tbUloga.Text) &&
                    !String.IsNullOrEmpty(tbKontakt.Text) && !String.IsNullOrEmpty(tbPlata.Text) && !String.IsNullOrEmpty(tbUsername.Text) || !String.IsNullOrEmpty(passBox.Password) && admin())
            {

                decimal plata;
                var validan = decimal.TryParse(tbPlata.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out plata);
                if (validan)
                {
                    if (MessageBox.Show("Da li ste sigurni da želite da kreirate korisnika?", "Izmena podataka", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        string sifra = passBox.Password;
                        if (!String.IsNullOrEmpty(tbPassword.Text))
                            sifra = tbPassword.Text;

                        konekcija.Open();
                        try
                        {
                            SqlCommand komanda = new SqlCommand();
                            komanda.Connection = konekcija;
                            komanda.CommandText = $"INSERT INTO Osoblje VALUES ('{tbIme.Text}', '{tbPrezime.Text}', '{tbUloga.Text}', '{tbKontakt.Text}', " +
                                                  $"'{plata}', '{isAdmin()}', '{tbUsername.Text}', '{sifra}')";
                            komanda.ExecuteNonQuery();
                            MessageBox.Show("Uspešno uneto u tabelu!");

                            cbOsoblje.Items.Clear();
                            ocistiSve();
                            napuniComboBox();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Neuspešna unosenje podataka! - " + ex.StackTrace);
                        }
                        finally
                        {
                            konekcija.Close();
                        }
                    }
                }
                else
                    MessageBox.Show("Plata mora biti upisana isključivo brojevima.");
            }
            else
                MessageBox.Show("Morate upisati nešto u sva polja.");
            
        }
    }
}