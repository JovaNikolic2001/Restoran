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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for RasporediSmeneForma.xaml
    /// </summary>
    public partial class RasporediSmeneForma : Window
    {
        Osoblje admin = new Osoblje();
        SqlConnection konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
        Dictionary<int, Tuple<int, int>> pronadjeno = new Dictionary<int, Tuple<int, int>>();
        Dictionary<int, Tuple<int, int>> prosli = new Dictionary<int, Tuple<int, int>>();
        RestoranDataContext DC = new RestoranDataContext();
        string poslednjiReport;
        public RasporediSmeneForma(Osoblje admin)
        {
            InitializeComponent();
            this.admin = admin;
            napuniDataGridove();
            izbor();
        }

        private void izbor()
        {
            if (!btnJJ.IsEnabled || !btnJV.IsEnabled)
            {
                rbSve.Visibility = Visibility.Visible;
                rbDodeljene.Visibility = Visibility.Visible;
                rbNedodeljene.Visibility = Visibility.Visible;
                rbSve.IsChecked = true;
                dataGridSmene.Height = 230;
            }
            else
            {
                rbSve.Visibility = Visibility.Hidden;
                rbDodeljene.Visibility = Visibility.Hidden;
                rbNedodeljene.Visibility = Visibility.Hidden;
                dataGridSmene.Visibility = Visibility.Visible;
                dataGridSmeneAssigned.Visibility = Visibility.Hidden;
                dataGridSmeneUnassigned.Visibility = Visibility.Hidden;
                dataGridSmene.Height = 250;
            }
        }

        private void napuniDataGridove()
        {
            dataGridOsoblje.Items.Clear();
            dataGridSmene.Items.Clear();
            dataGridSmeneAssigned.Items.Clear();
            dataGridSmeneUnassigned.Items.Clear();
            dataGridOsoblje.SelectionMode = DataGridSelectionMode.Single;
            dataGridSmene.SelectionMode = DataGridSelectionMode.Single;
            dataGridSmeneUnassigned.SelectionMode = DataGridSelectionMode.Single;

            SqlCommand komanda = new SqlCommand();
            komanda.Connection = konekcija;

            konekcija.Open();
            komanda.CommandText = "SELECT * FROM Osoblje";
            using (SqlDataReader reader = komanda.ExecuteReader())
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
                    dataGridOsoblje.Items.Add(o);
                }
            }
            konekcija.Close();

            konekcija.Open();
            komanda.CommandText = "SELECT * FROM Smena ORDER BY Datum";
            using (SqlDataReader reader = komanda.ExecuteReader())
            {
                while (reader.Read())
                {
                    Smena s = new Smena();
                    s.SmenaID = (int)reader["SmenaID"];
                    s.Datum = (DateTime)reader["Datum"];
                    s.PocetakSmene = (TimeSpan)reader["PocetakSmene"];
                    s.KrajSmene = (TimeSpan)reader["KrajSmene"];
                    dataGridSmene.Items.Add(s);
                }
            }
            konekcija.Close();
        }

        private void ocisti()
        {
            dataGridOsoblje.Items.Clear();
            dataGridSmene.Items.Clear();
            dataGridSmeneAssigned.Items.Clear();
            dataGridSmeneUnassigned.Items.Clear();
            btnJJ.IsEnabled = false;
            btnJV.IsEnabled = true;
            btnVJ.IsEnabled = true;
            btnVV.IsEnabled = true;
            btnDodeliSmene.IsEnabled = false;
            btnReport.IsEnabled = false;
            izbor();
        }

        private void btnJJ_Click(object sender, RoutedEventArgs e)
        {
            labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete JEDNOG člana osoblja i dodelite mu JEDNU smenu";
            btnDodeliSmene.Content = "Dodeli smenu";
            dataGridOsoblje.SelectionMode = DataGridSelectionMode.Single;
            dataGridSmene.SelectionMode = DataGridSelectionMode.Single;
            dataGridSmeneAssigned.SelectionMode = DataGridSelectionMode.Single;
            dataGridSmeneUnassigned.SelectionMode = DataGridSelectionMode.Single;
            btnJJ.IsEnabled = false;
            btnJV.IsEnabled = true;
            btnVJ.IsEnabled = true;
            btnVV.IsEnabled = true;
            dataGridOsoblje.SelectedItem = null;
            dataGridSmene.SelectedItem = null;
            btnDodeliSmene.IsEnabled = false;
            izbor();
            btnSkiniSmene.Visibility = Visibility.Hidden;
            btnDodeliSmene.Visibility = Visibility.Visible;
        }

        private void btnJV_Click(object sender, RoutedEventArgs e)
        {
            labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete JEDNOG člana osoblja i dodelite mu VIŠE smena";
            btnDodeliSmene.Content = "Dodeli smene";
            dataGridOsoblje.SelectionMode = DataGridSelectionMode.Single;
            dataGridSmene.SelectionMode = DataGridSelectionMode.Extended;
            dataGridSmeneAssigned.SelectionMode = DataGridSelectionMode.Extended;
            dataGridSmeneUnassigned.SelectionMode = DataGridSelectionMode.Extended;
            btnJJ.IsEnabled = true;
            btnJV.IsEnabled = false;
            btnVJ.IsEnabled = true;
            btnVV.IsEnabled = true;
            dataGridOsoblje.SelectedItem = null;
            dataGridSmene.SelectedItem = null;
            btnDodeliSmene.IsEnabled = false;
            izbor();
            btnSkiniSmene.Visibility = Visibility.Hidden;
            btnDodeliSmene.Visibility = Visibility.Visible;
        }

        private void btnVJ_Click(object sender, RoutedEventArgs e)
        {
            labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete VIŠE članova osoblja i dodelite im JEDNU smenu";
            btnDodeliSmene.Content = "Dodeli smenu";
            dataGridOsoblje.SelectionMode = DataGridSelectionMode.Extended;
            dataGridSmene.SelectionMode = DataGridSelectionMode.Single;
            btnJJ.IsEnabled = true;
            btnJV.IsEnabled = true;
            btnVJ.IsEnabled = false;
            btnVV.IsEnabled = true;
            dataGridOsoblje.SelectedItem = null;
            dataGridSmene.SelectedItem = null;
            btnDodeliSmene.IsEnabled = false;
            izbor();
            btnSkiniSmene.Visibility = Visibility.Hidden;
            btnDodeliSmene.Visibility = Visibility.Visible;
        }

        private void btnVV_Click(object sender, RoutedEventArgs e)
        {
            labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete VIŠE članova osoblja i dodelite im VIŠE smena";
            btnDodeliSmene.Content = "Dodeli smene";
            dataGridOsoblje.SelectionMode = DataGridSelectionMode.Extended;
            dataGridSmene.SelectionMode = DataGridSelectionMode.Extended;
            btnJJ.IsEnabled = true;
            btnJV.IsEnabled = true;
            btnVJ.IsEnabled = true;
            btnVV.IsEnabled = false;
            dataGridOsoblje.SelectedItem = null;
            dataGridSmene.SelectedItem = null;
            btnDodeliSmene.IsEnabled = false;
            izbor(); 
            btnSkiniSmene.Visibility = Visibility.Hidden;
            btnDodeliSmene.Visibility = Visibility.Visible;
        }

        private void btnDodeliSmene_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridOsoblje.SelectedItem != null && dataGridSmene.SelectedItem != null || dataGridSmeneUnassigned.SelectedItem != null)
            {
                List<Osoblje> o = new List<Osoblje>();
                List<Smena> s = new List<Smena>();

                foreach (Osoblje select in dataGridOsoblje.SelectedItems)
                {
                    o.Add(select);
                }

                if (dataGridSmene.Visibility == Visibility.Hidden)
                    foreach (Smena select in dataGridSmeneUnassigned.SelectedItems)
                    {
                        s.Add(select);
                    }
                else
                    foreach (Smena select in dataGridSmene.SelectedItems)
                    {
                        s.Add(select);
                    }

                SqlCommand komanda1 = new SqlCommand();
                komanda1.Connection = konekcija;
                konekcija.Open();
                komanda1.CommandText = "SELECT * FROM Istorija";
                List<Istorija> listaIstorija = new List<Istorija>();
                using (SqlDataReader reader = komanda1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Istorija ist = new Istorija();
                        ist.OsobljeID = (int)reader["OsobljeID"];
                        ist.SmenaID = (int)reader["SmenaID"];
                        listaIstorija.Add(ist);
                    }
                }
                konekcija.Close();

                pronadjeno.Clear();
                prosli.Clear();
                int k = 0;
                int p = 0;
                for (int i = 0; i < o.Count; i++)
                    for (int j = 0; j < s.Count; j++)
                    {
                        bool proslo = true;
                        foreach (Istorija isto in listaIstorija)
                            if (isto.OsobljeID == o[i].OsobljeID && isto.SmenaID == s[j].SmenaID)
                            {
                                pronadjeno.Add(k++, new Tuple<int, int>(o[i].OsobljeID, s[j].SmenaID));
                                proslo = false;
                                break;
                            }
                        if (proslo)
                        {
                            SqlCommand komanda2 = new SqlCommand();
                            komanda2.Connection = konekcija;
                            konekcija.Open();
                            komanda2.CommandText = $"INSERT INTO Istorija VALUES ('{o[i].OsobljeID}','{s[j].SmenaID}')";
                            komanda2.ExecuteNonQuery();
                            konekcija.Close();
                            prosli.Add(p++, new Tuple<int, int>(o[i].OsobljeID, s[j].SmenaID));
                        }
                    }
                ocisti();
                napuniDataGridove();
                btnReport.IsEnabled = true;
                report();
            }
            else
                MessageBox.Show("Morate izabrati neku smenu.");
        }

        private void dataGridOsoblje_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataGridSmeneAssigned.Items.Clear();
            dataGridSmeneUnassigned.Items.Clear();

            if (dataGridOsoblje.SelectedValue != null && dataGridSmene.SelectedValue != null || dataGridSmeneUnassigned.SelectedValue != null)
                btnDodeliSmene.IsEnabled = true;

            if (dataGridOsoblje.SelectionMode == DataGridSelectionMode.Single && dataGridOsoblje.SelectedItem != null)
            {
                Osoblje o = (Osoblje)dataGridOsoblje.SelectedValue;
                SqlCommand komanda = new SqlCommand();
                komanda.Connection = konekcija;
                konekcija.Open();
                komanda.CommandText = $"SELECT * FROM Istorija WHERE OsobljeID = '{o.OsobljeID}'";
                List<Istorija> listaIstorija = new List<Istorija>();
                using (SqlDataReader reader = komanda.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Istorija ist = new Istorija();
                        ist.OsobljeID = (int)reader["OsobljeID"];
                        ist.SmenaID = (int)reader["SmenaID"];
                        listaIstorija.Add(ist);
                    }
                }
                konekcija.Close();

                konekcija.Open();
                komanda.CommandText = "SELECT * FROM Smena ORDER BY Datum";
                using (SqlDataReader reader = komanda.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Smena s = new Smena();
                        s.SmenaID = (int)reader["SmenaID"];
                        s.Datum = (DateTime)reader["Datum"];
                        s.PocetakSmene = (TimeSpan)reader["PocetakSmene"];
                        s.KrajSmene = (TimeSpan)reader["KrajSmene"];
                        int i = 1;
                        if (listaIstorija.Count > 0)
                            foreach (Istorija isto in listaIstorija)
                            {
                                if (isto.OsobljeID == o.OsobljeID && isto.SmenaID == s.SmenaID)
                                {
                                    dataGridSmeneAssigned.Items.Add(s);
                                    break;
                                }
                                else if (i == listaIstorija.Count)
                                {
                                    dataGridSmeneUnassigned.Items.Add(s);
                                    i = 1;
                                }
                                else
                                    i++;
                            }
                        else
                            dataGridSmeneUnassigned.Items.Add(s);

                    }
                }
                konekcija.Close();
            }
        }

        private void dataGridSmene_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridOsoblje.SelectedValue != null && dataGridSmene.SelectedValue != null)
            {
                btnSkiniSmene.Visibility = Visibility.Hidden;
                btnDodeliSmene.Visibility = Visibility.Visible;
                btnDodeliSmene.IsEnabled = true;
                dataGridSmeneAssigned.SelectedValue = null;
                dataGridSmeneUnassigned.SelectedValue = null;
            }
                
        }

        private void rbSve_Checked(object sender, RoutedEventArgs e)
        {
            if (!btnJJ.IsEnabled)
                labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete JEDNOG člana osoblja i dodelite mu JEDNU smenu";
            else
                labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete JEDNOG člana osoblja i dodelite mu VIŠE smena";
            dataGridSmene.Visibility = Visibility.Visible;
            dataGridSmeneAssigned.Visibility = Visibility.Hidden;
            dataGridSmeneUnassigned.Visibility = Visibility.Hidden;
            dataGridSmene.SelectedItem = null;
            dataGridSmeneUnassigned.SelectedItem = null;
            btnDodeliSmene.IsEnabled = false;
        }

        private void rbDodeljene_Checked(object sender, RoutedEventArgs e)
        {
            if (!btnJJ.IsEnabled)
                labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete JEDNOG člana osoblja i IZBRIŠETE mu JEDNU smenu";
            else
                labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete JEDNOG člana osoblja i IZBRIŠETE mu VIŠE smena";
            dataGridSmene.Visibility = Visibility.Hidden;
            dataGridSmeneAssigned.Visibility = Visibility.Visible;
            dataGridSmeneUnassigned.Visibility = Visibility.Hidden;
            dataGridSmene.SelectedItem = null;
            dataGridSmeneUnassigned.SelectedItem = null;
            btnDodeliSmene.IsEnabled = false;
        }

        private void rbNedodeljene_Checked(object sender, RoutedEventArgs e)
        {
            if (!btnJJ.IsEnabled)
                labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete JEDNOG člana osoblja i dodelite mu JEDNU smenu";
            else
                labelIspis.Content = "Trenutni mod Vam dozvoljava da izaberete JEDNOG člana osoblja i dodelite mu VIŠE smena";
            dataGridSmene.Visibility = Visibility.Hidden;
            dataGridSmeneAssigned.Visibility = Visibility.Hidden;
            dataGridSmeneUnassigned.Visibility = Visibility.Visible;
            dataGridSmene.SelectedItem = null;
            dataGridSmeneUnassigned.SelectedItem = null;
            btnDodeliSmene.IsEnabled = false;
        }

        private void dataGridSmeneUnassigned_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridOsoblje.SelectedValue != null && dataGridSmene.SelectedValue != null || dataGridSmeneUnassigned.SelectedValue != null)
            {
                btnDodeliSmene.IsEnabled = true;
                dataGridSmeneAssigned.SelectedValue = null;
                dataGridSmene.SelectedValue = null;
                btnSkiniSmene.Visibility = Visibility.Hidden;
                btnDodeliSmene.Visibility = Visibility.Visible;
            }
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            //napuniDataGridove();
            //izbor();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(poslednjiReport);
        }

        private void report()
        {
            string report = string.Empty;
            if (btnSkiniSmene.Visibility == Visibility.Hidden)
            {
                if (pronadjeno.Count > 0)
                {
                    report = "                 NEUSPESNO OBAVLJEN UNOS         \nRb.  Ime              Datum          Pocetak        Kraj smene      \n";

                    List<Istorija> greska = new List<Istorija>();
                    for (int i = 0; i < pronadjeno.Count; i++)
                    {
                        Istorija rez = (from isto in DC.Istorijas
                                        where isto.OsobljeID == pronadjeno[i].Item1 && isto.SmenaID == pronadjeno[i].Item2
                                        select isto).Single();
                        greska.Add(rez);
                    }

                    int ic = 1;
                    foreach (Istorija i in greska)
                    {
                        var rez = (from ist in DC.Istorijas
                                   join o in DC.Osobljes on ist.OsobljeID equals o.OsobljeID
                                   join s in DC.Smenas on ist.SmenaID equals s.SmenaID
                                   where ist.OsobljeID == i.OsobljeID && ist.SmenaID == i.SmenaID
                                   select new { o.Ime, Datumi = s.Datum.Day + "." + s.Datum.Month + "." + s.Datum.Year, s.PocetakSmene, s.KrajSmene }).Single();

                        report += ic++ + "     " + rez.Ime + "       " + rez.Datumi + "       " + rez.PocetakSmene + "        " + rez.KrajSmene + "\n";
                    }
                    report += "\n ----------------------------------------------------- \n";
                }

                if (prosli.Count > 0)
                {
                    report += "                 USPESNO OBAVLJEN UNOS       \nRb.  Ime             Datum         Pocetak         Kraj smene      \n";

                    List<Istorija> uspesno = new List<Istorija>();
                    for (int i = 0; i < prosli.Count; i++)
                    {
                        Istorija rez = (from isto in DC.Istorijas
                                        where isto.OsobljeID == prosli[i].Item1 && isto.SmenaID == prosli[i].Item2
                                        select isto).Single();
                        uspesno.Add(rez);
                    }

                    int c = 1;
                    foreach (Istorija i in uspesno)
                    {
                        var rez = (from ist in DC.Istorijas
                                   join o in DC.Osobljes on ist.OsobljeID equals o.OsobljeID
                                   join s in DC.Smenas on ist.SmenaID equals s.SmenaID
                                   where ist.OsobljeID == i.OsobljeID && ist.SmenaID == i.SmenaID
                                   select new { o.Ime, Datumi = s.Datum.Day + "." + s.Datum.Month + "." + s.Datum.Year, s.PocetakSmene, s.KrajSmene }).Single();

                        report += c++ + "     " + rez.Ime + "       " + rez.Datumi + "       " + rez.PocetakSmene + "        " + rez.KrajSmene + "\n";
                    }
                }

            }
            else
            {
                int b = 1;
                report += "                 USPEŠNO IZBRISANI RASPOREDI         \nRb.  Ime              Datum          Početak        Kraj smene      \n";
                foreach (Osoblje os in dataGridOsoblje.SelectedItems)
                    foreach (Smena sm in dataGridSmeneAssigned.SelectedItems)
                    {
                        

                        var rez = (from i in DC.Istorijas
                                   join o in DC.Osobljes on i.OsobljeID equals o.OsobljeID
                                   join s in DC.Smenas on i.SmenaID equals s.SmenaID
                                   where i.OsobljeID == os.OsobljeID && i.SmenaID == sm.SmenaID
                                   select new { o.Ime, Datumi = s.Datum.Day + "." + s.Datum.Month + "." + s.Datum.Year, s.PocetakSmene, s.KrajSmene }).Single();
                        
                        report += b++ + "     " + rez.Ime + "       " + rez.Datumi + "       " + rez.PocetakSmene + "        " + rez.KrajSmene + "\n";

                        SqlCommand komanda = new SqlCommand();
                        komanda.Connection = konekcija;
                        konekcija.Open();
                        komanda.CommandText = $"DELETE FROM Istorija WHERE OsobljeID = '{os.OsobljeID}' AND SmenaID = '{sm.SmenaID}'";
                        komanda.ExecuteNonQuery();
                        konekcija.Close();
                    }
            }
            poslednjiReport = report;
            MessageBox.Show(report);
        }

        private void dataGridSmeneAssigned_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridOsoblje.SelectedValue != null && dataGridSmeneAssigned.SelectedValue != null)
            {
                btnSkiniSmene.Visibility = Visibility.Visible;
                btnDodeliSmene.Visibility = Visibility.Hidden;
                dataGridSmeneUnassigned.SelectedValue = null;
                dataGridSmene.SelectedValue = null;
                btnDodeliSmene.IsEnabled = false;
            }
        }

        private void btnSkiniSmene_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridOsoblje.SelectedValue != null && dataGridSmeneAssigned.SelectedValue != null)
            {
                if (MessageBox.Show("Da li ste sigurni da želite da uklonite smene?", "Uklanjanje smena", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    report();
                    ocisti();
                    napuniDataGridove();
                    btnReport.IsEnabled = true;
                }
            }
            else
                MessageBox.Show("Morate izabrati nekog iz osoblja i iz dodeljenih smena.");
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            napuniDataGridove();
            izbor();
        }
    }
}
