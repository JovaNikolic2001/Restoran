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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
        public MainWindow()
        {
            InitializeComponent();
            tbUsername.Focus();
        }
        private void tbUsername_KeyUp(object sender, KeyEventArgs e)
        {
            okidac(e);
        }
        private void tbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            okidac(e);
        }
        public void okidac(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(tbUsername.Text) && !string.IsNullOrEmpty(tbPassword.Password))
            {
                prijavljivanje();
            }
        }
        private void btnPrijavi_Click(object sender, RoutedEventArgs e)
        {
            prijavljivanje();
        }
        public void prijavljivanje()
        {
            if (!string.IsNullOrEmpty(tbUsername.Text) && !string.IsNullOrEmpty(tbPassword.Password))
            {
                
                SqlCommand komanda2 = new SqlCommand();
                komanda2.Connection = konekcija;
                komanda2.CommandText = $"SELECT * FROM Osoblje WHERE Username = '{tbUsername.Text}' AND Password = '{tbPassword.Password}'";
                konekcija.Open();
                Osoblje o = new Osoblje();
                using (SqlDataReader reader = komanda2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        o.OsobljeID = (int)reader["OsobljeID"];
                        o.Ime = reader["Ime"].ToString();
                        o.Prezime = reader["Prezime"].ToString();
                        o.Uloga = reader["Uloga"].ToString();
                        o.Kontakt = reader["Kontakt"].ToString();
                        o.Plata = (decimal)reader["Plata"];
                        o.Username = reader["Username"].ToString();
                        o.Password = reader["Password"].ToString();
                        o.Admin = (bool)reader["Admin"];
                    }
                }
                konekcija.Close();

                this.Hide();
                if (o.Admin == true && o.Username == tbUsername.Text && o.Password == tbPassword.Password)
                {
                    Administrator admin = new Administrator(o);
                    admin.ShowDialog();

                    admin = null;
                }
                else if (o.Admin == false && o.Username == tbUsername.Text && o.Password == tbPassword.Password)
                {
                    OceanEdgeResort oer = new OceanEdgeResort(o);
                    oer.ShowDialog();

                    oer = null;
                }
                else
                {
                    MessageBox.Show("Pogrešno ste ukucali Username ili Password!");
                }
                tbUsername.Text = "";
                tbPassword.Password = "";
                this.Show();
                tbUsername.Focus();
            }
            else
            {
                MessageBox.Show("Morate uneti podatke u oba polja!");
            }
        }
    }
}
