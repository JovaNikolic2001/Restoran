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
using System.Data.Sql;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Security.Principal;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Administrator.xaml
    /// </summary>
    public partial class NovaPorudzbina : Window
    {
        RestoranDataContext DC = new RestoranDataContext();
        private int broj = 0;
        Osoblje admin = new Osoblje();
        Button sto;
        int porID = -1;
        int k = 0;
        Dictionary<int, Tuple<int, int, string>> listaUsluga = new Dictionary<int, Tuple<int, int, string>>();

        public NovaPorudzbina(Osoblje admin, object e)
        {
            this.admin = admin;
            sto = e as Button;
            InitializeComponent();
            this.Title = "Nova porudžbina za sto broj " + sto.Content.ToString();
            lUlogovan.Content += admin.Ime + " " + admin.Prezime;
            napuniListuStavki();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stoOtvoren();
        }

        public int Broj
        {
            get { return broj; }
            set
            {
                broj = value;
                tbBroj.Text = value.ToString();
            }
        }

        private void btnGore_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxStavka.SelectedItem != null || listBoxPorudzbina.SelectedItem != null)
            {
                if (Broj < 15)
                    Broj++;
            }
        }

        private void btnDole_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxStavka.SelectedItem != null || listBoxPorudzbina.SelectedItem != null)
            {
                if (Broj > 1)
                    Broj--;
            }
        }

        private void tbBroj_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbBroj == null)
            {
                return;
            }

            if (!int.TryParse(tbBroj.Text, out broj))
                tbBroj.Text = broj.ToString();
        }

        private void stoOtvoren()
        {
            Osoblje osoblje = (from p in DC.Porudzbinas
                               join o in DC.Osobljes on p.OsobljeID equals o.OsobljeID
                               where p.StoID == Int32.Parse(sto.Content.ToString()) && p.Zavrseno == false
                               select o).DefaultIfEmpty().SingleOrDefault();

            if(osoblje != null)
                if (osoblje.OsobljeID != admin.OsobljeID)
                {
                    string ip = osoblje.Ime + " " + osoblje.Prezime;
                    MessageBox.Show("Sto je vec zauzet od strane: " + ip);
                    lZauzet.Visibility = Visibility.Visible;
                    lZauzetoOd.Visibility = Visibility.Visible;
                    lZauzetoOd.Content = ip;
                    if (!admin.Admin)
                        this.Hide();
                }
        }

        public void napuniListuStavki()
        {
            var rez = (from s in DC.Stavkas
                       select s).ToList();

            listBoxStavka.Items.Clear();
            foreach (Stavka s in rez)
            {
                listBoxStavka.Items.Add(s);
            }

            var rez2 = (from p in DC.Porudzbinas
                       where p.StoID == Int32.Parse(sto.Content.ToString()) && p.Zavrseno == false
                       select p).DefaultIfEmpty().Single();

            dataGridPorIzBaze.ItemsSource = null;
            if (rez2 != null)
            {
                dataGridPorIzBaze.ItemsSource = null;
                dataGridPorIzBaze.ItemsSource = (from u in DC.Uslugas
                                                join s in DC.Stavkas on u.StavkaID equals s.StavkaID
                                                where u.PorudzbinaID == rez2.PorudzbinaID
                                                select new { s.Naziv, u.Količina }).ToList();
                if (dataGridPorIzBaze.Items.Count > 7)
                {
                    dataGridPorIzBaze.Width = 325;
                    this.Width = 817;
                }
                else
                {
                    dataGridPorIzBaze.Width = 308;
                    this.Width = 800;
                }                    

                porID = rez2.PorudzbinaID;
            }

            if (dataGridPorIzBaze.Items.Count > 0)
                btnZatvori.IsEnabled = true;
            else
                btnZatvori.IsEnabled = false;
        }

        private void listBoxStavka_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxStavka.SelectedItem != null)
            {
                Stavka s = new Stavka();
                s = listBoxStavka.SelectedItem as Stavka;
                lIme.Content = s.Naziv;
                lCena.Content = (int)s.Cena;
                lKategorija.Content = s.Kategorija;
                tbBroj.Text = 1.ToString();
                tbNapomena.Text = string.Empty;
                btnIzmena.Visibility = Visibility.Hidden;
                btnIzaberi.Visibility = Visibility.Visible;
                listBoxPorudzbina.UnselectAll();
            }
        }

        private void btnIzaberi_Click(object sender, RoutedEventArgs e)
        {
            if (listaUsluga.Count > 0)
                k = listaUsluga.Keys.Max() + 1;
            else k = 0;
            if (listBoxStavka.SelectedItem != null)
            {
                Stavka s = new Stavka();
                s = listBoxStavka.SelectedItem as Stavka;
                try
                {
                    int brojac = 0;
                    if (listBoxPorudzbina.Items.Count > 0)
                        foreach (var item in listaUsluga)
                        {
                            if (item.Value.Item1 == s.StavkaID)
                            {
                                listaUsluga.Remove(item.Key);

                                string nap;
                                if (String.IsNullOrEmpty(tbNapomena.Text))
                                    nap = item.Value.Item3;
                                else
                                {
                                    if (String.IsNullOrEmpty(item.Value.Item3))
                                        nap = tbNapomena.Text;
                                    else
                                        nap = item.Value.Item3 + "\n" + tbNapomena.Text;
                                }

                                listaUsluga.Add(item.Key, new Tuple<int, int, string>(item.Value.Item1, item.Value.Item2 + Broj, nap));
                                break;
                            }
                            else
                            {
                                brojac++;
                                if (brojac == listBoxPorudzbina.Items.Count)
                                {
                                    listBoxPorudzbina.Items.Add(s);
                                    listaUsluga.Add(k++, new Tuple<int, int, string>(s.StavkaID, Broj, tbNapomena.Text));
                                    break;
                                }
                            }
                        }
                    else
                    {
                        listBoxPorudzbina.Items.Add(s);
                        listaUsluga.Add(k++, new Tuple<int, int, string>(s.StavkaID, Broj, tbNapomena.Text));
                    }
                }
                catch(Exception ex) { MessageBox.Show(ex.Message); }
            }
            else
                MessageBox.Show("Morate izabrati neku stavku iz liste.");
            listBoxStavka.SelectedItem = null;
            lIme.Content = null;
            lKategorija.Content = null;
            lCena.Content = null;
            tbBroj.Text = 1.ToString();
            tbNapomena.Text = null;
        }

        private void btnIzbrisi_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxPorudzbina.SelectedItem == null)
                MessageBox.Show("Morate prvo izabrati stavku za brisanje.");
            else
                if (MessageBox.Show("Da li ste sigurni da želite da izbrišete stavku?", "Brisanje stavke?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    List<int> koli = new List<int>();
                    List<string> strings = new List<string>();
                    foreach (var i in listaUsluga)
                    {
                        koli.Add(i.Value.Item2);
                        strings.Add(i.Value.Item3);
                    }
                    listaUsluga.Clear();
                    k = 0;
                    koli.RemoveAt(listBoxPorudzbina.SelectedIndex);
                    listBoxPorudzbina.Items.Remove(listBoxPorudzbina.SelectedItem);
                    foreach (Stavka s in listBoxPorudzbina.Items)
                    {
                        listaUsluga.Add(k++, new Tuple<int, int, string>(s.StavkaID, koli[0], strings[0]));
                        koli.RemoveAt(0);
                        strings.RemoveAt(0);
                    }
            }
        }

        private void btnPoruci_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxPorudzbina.Items.Count > 0)
            {                
                bool nepostoji = false;
                var konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
                string poruka = String.Empty;

                if (porID == -1)
                {
                    nepostoji = true;

                    using (var komanda = konekcija.CreateCommand())
                    {
                        try
                        {
                            konekcija.Open();
                            komanda.CommandText = "NovaPorudzbina";
                            komanda.CommandType = CommandType.StoredProcedure;
                            komanda.Parameters.AddWithValue("@StoID", sto.Content);
                            komanda.Parameters.AddWithValue("@OsobljeID", admin.OsobljeID);
                            komanda.Parameters.AddWithValue("@VremePorudzbine", DateTime.Now);
                            komanda.Parameters.AddWithValue("@Zavrseno", false);
                            komanda.ExecuteNonQuery();
                            poruka += "Uspešno napravljena porudžbina!\n";
                        }
                        catch (Exception ex) { MessageBox.Show("Problem sa PORUDŽBINOM! - " + ex.Message); }
                        finally { konekcija.Close(); }
                    }
                    porID = (from p in DC.Porudzbinas
                             orderby p.PorudzbinaID descending
                             where p.StoID == Int32.Parse(sto.Content.ToString()) && p.Zavrseno == false
                             select p.PorudzbinaID).FirstOrDefault();
                }

                List<Usluga> usluge = (from u in DC.Uslugas
                                        where u.PorudzbinaID == porID
                                        select u).ToList();

                if (nepostoji || usluge.Count() == 0)
                {
                    try
                    {
                        konekcija.Open();
                        foreach (var u in listaUsluga)
                        {
                            SqlCommand komanda2 = new SqlCommand();
                            komanda2.Connection = konekcija;
                            var cena = (from s in DC.Stavkas
                                        where s.StavkaID == u.Value.Item1
                                        select s.Cena).SingleOrDefault();

                            komanda2.CommandText = $"INSERT INTO Usluga VALUES ('{porID}','{u.Value.Item1}','{u.Value.Item2}','{(int)(cena * u.Value.Item2)}', '{u.Value.Item3}')";
                            komanda2.ExecuteNonQuery();

                            usluge.Remove((from usl in DC.Uslugas
                                            where usl.PorudzbinaID == porID && usl.StavkaID == u.Value.Item1
                                            select usl).SingleOrDefault());

                            poruka += "Uspešno uneta stavka!\n";
                        }
                        listaUsluga.Clear();
                        k = 0;
                        listBoxPorudzbina.Items.Clear();
                    }
                    catch (Exception ex) { MessageBox.Show("Problem sa USLUGOM! -  " + ex.Message); }
                    finally { konekcija.Close(); }
                }
                else
                {
                    List<int> listaZaIzbac = new List<int>();

                    foreach (var l in listaUsluga)
                    {
                        int brojac = 0;
                        foreach (Usluga u in usluge)
                        {
                            if (u.StavkaID == l.Value.Item1)
                            {
                                Stavka st = (from s in DC.Stavkas
                                                where s.StavkaID == l.Value.Item1
                                                select s).SingleOrDefault();

                                MessageBox.Show("\t\t\tNaziv: " + st.Naziv + "\nTrenutna količina: " + u.Količina + "\tNova količina: " + l.Value.Item2 + "\t\tUkupno: " + (u.Količina + l.Value.Item2) + "\nNapomena: " + l.Value.Item3);

                                konekcija.Open();
                                try
                                {
                                    SqlCommand komanda = new SqlCommand();
                                    komanda.Connection = konekcija;
                                    u.Količina += l.Value.Item2;
                                    if (!String.IsNullOrEmpty(tbNapomena.Text))
                                        u.Napomena += "\n\n" + l.Value.Item3;
                                    komanda.CommandText = $"UPDATE Usluga SET Količina = '{u.Količina}', Ukupno = '{(int)st.Cena * u.Količina}', Napomena = '{u.Napomena}' WHERE StavkaID = '{u.StavkaID}' AND PorudzbinaID = '{u.PorudzbinaID}'";
                                    komanda.ExecuteNonQuery();
                                    Usluga usluga = (from usl in DC.Uslugas
                                                        where usl.PorudzbinaID == porID && usl.StavkaID == u.StavkaID
                                                        select usl).SingleOrDefault();
                                    usluge.Remove(usluga);
                                    listaZaIzbac.Add(l.Key);
                                    poruka += "Uspesno ažurirana količina!\n";
                                }
                                catch (Exception ex) { MessageBox.Show(ex.Message); }
                                finally { konekcija.Close(); }
                            }
                            else
                            {
                                brojac++;
                                if (usluge.Count() == brojac)
                                {
                                    konekcija.Open();
                                    try
                                    {
                                        SqlCommand komanda2 = new SqlCommand();
                                        komanda2.Connection = konekcija;
                                        var cena = (from s in DC.Stavkas
                                                    where s.StavkaID == l.Value.Item1
                                                    select s.Cena).SingleOrDefault();
                                        string nap;
                                        if (String.IsNullOrEmpty(tbNapomena.Text))
                                            nap = "";
                                        else
                                            nap = tbNapomena.Text;

                                        komanda2.CommandText = $"INSERT INTO Usluga VALUES ('{porID}','{l.Value.Item1}','{l.Value.Item2}','{(int)(cena * l.Value.Item2)}', '{nap}')";
                                        komanda2.ExecuteNonQuery();

                                        listaZaIzbac.Add(l.Key);
                                        poruka += "Uspešno uneta stavka!\n";
                                    }
                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                    finally { konekcija.Close(); }
                                }
                            }
                        }
                    }
                    tbNapomena.Text = string.Empty;
                    listBoxPorudzbina.Items.Clear();

                    foreach (int i in listaZaIzbac)
                        listaUsluga.Remove(i);
                    if (listaUsluga.Count > 0)
                    {
                        Dictionary<int, Tuple<int, int, string>> change = new Dictionary<int, Tuple<int, int, string>>();
                        foreach (var c in listaUsluga)
                            change.Add(c.Key, new Tuple<int, int, string>(c.Value.Item1, c.Value.Item2, c.Value.Item3));
                        listaUsluga.Clear();
                        k = 0;
                        foreach (var c in change)
                            listaUsluga.Add(k++, new Tuple<int, int, string>(c.Value.Item1, c.Value.Item2, c.Value.Item3));
                    }
                    foreach (var ls in listaUsluga)
                    {
                        Stavka s = (from st in DC.Stavkas
                                    where st.StavkaID == ls.Value.Item1
                                    select st).SingleOrDefault();
                        listBoxPorudzbina.Items.Add(s);
                    }
                }

                MessageBox.Show(poruka);
                btnIzaberi.Visibility = Visibility.Visible;
                btnIzmena.Visibility = Visibility.Hidden;
                tbNapomena.Text = String.Empty;
                if (dataGridPorIzBaze.Items.Count > 0 )
                    btnZatvori.IsEnabled = true;
                else
                    btnZatvori.IsEnabled = false;
                napuniListuStavki();
            }
            else
                MessageBox.Show("Morate uneti nešto u listu za porudžbinu!");
        }

        private void btnIzmena_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxPorudzbina.SelectedValue != null)
            {
                int i1t = listaUsluga[listBoxPorudzbina.SelectedIndex].Item1;
                int i2t = Int32.Parse(tbBroj.Text);
                string i3t = tbNapomena.Text;
                listaUsluga.Remove(listBoxPorudzbina.SelectedIndex);
                listaUsluga.Add(listBoxPorudzbina.SelectedIndex, new Tuple<int, int, string>(i1t, i2t, i3t));                
                MessageBox.Show("Uspešno ste napravili izmenu.");
            }
        }

        private void listBoxPorudzbina_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Stavka s = new Stavka();
            s = listBoxPorudzbina.SelectedItem as Stavka;
            if (s != null)
            {
                lIme.Content = s.Naziv;
                lCena.Content = (int)s.Cena;
                lKategorija.Content = s.Kategorija;
                tbBroj.Text = listaUsluga[listBoxPorudzbina.SelectedIndex].Item2.ToString();
                tbNapomena.Text = listaUsluga[listBoxPorudzbina.SelectedIndex].Item3.ToString();
                btnIzaberi.Visibility = Visibility.Hidden;
                btnIzmena.Visibility = Visibility.Visible;
            }
        }

        private void PrikaziNapomenu(object sender, RoutedEventArgs e)
        {
            var naziv = TypeDescriptor.GetProperties(((Button)sender).DataContext)["Naziv"].GetValue(((Button)sender).DataContext);
            var kolicina = TypeDescriptor.GetProperties(((Button)sender).DataContext)["Količina"].GetValue(((Button)sender).DataContext);

            if (porID == -1)
            {
                porID = (from p in DC.Porudzbinas
                         where p.StoID == Int32.Parse(sto.Content.ToString()) && p.Zavrseno == false
                         select p.PorudzbinaID).Single();
            }

            string rez = (from u in DC.Uslugas
                          join s in DC.Stavkas on u.StavkaID equals s.StavkaID
                          where s.Naziv == naziv.ToString() && u.Količina == (int)kolicina && u.PorudzbinaID == porID
                          select u.Napomena).FirstOrDefault();

            if (String.IsNullOrEmpty(rez))
                MessageBox.Show("Nema napomena za ovu stavku.");
            else
                MessageBox.Show(rez);
        }

        private void IzbrisiStavku(object sender, RoutedEventArgs e)
        {
            var naziv = TypeDescriptor.GetProperties(((Button)sender).DataContext)["Naziv"].GetValue(((Button)sender).DataContext);
            var kolicina = TypeDescriptor.GetProperties(((Button)sender).DataContext)["Količina"].GetValue(((Button)sender).DataContext);
            var konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);

            if (MessageBox.Show("Naziv: " + naziv + "\t\tTrenutna količina: " + kolicina
                + "\nDa li ste sigurni da želite da izbrišete uslugu?", "Brisanje usluge?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                konekcija.Open();
                try
                {
                    Usluga usluga = (from u in DC.Uslugas
                                     join s in DC.Stavkas on u.StavkaID equals s.StavkaID
                                     where u.PorudzbinaID == porID && u.Količina == (int)kolicina && s.Naziv == naziv.ToString()
                                     select u).SingleOrDefault();

                    SqlCommand komanda2 = new SqlCommand();
                    komanda2.Connection = konekcija;
                    komanda2.CommandText = $"DELETE FROM Usluga WHERE PorudzbinaID = '{porID}' AND StavkaID = '{usluga.StavkaID}' AND Količina = '{(int)kolicina}'";
                    komanda2.ExecuteNonQuery();

                    napuniListuStavki();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { konekcija.Close(); }

                MessageBox.Show("Uspešno izbrisana stavka!");
            }

            var rezultat = (from u in DC.Uslugas
                            join p in DC.Porudzbinas on u.PorudzbinaID equals p.PorudzbinaID
                            where p.Zavrseno == false && p.StoID == Int32.Parse(sto.Content.ToString())
                            select u).ToList();

            if (rezultat.Count == 0)
            {
                try
                {
                    DC.Porudzbinas.DeleteOnSubmit((from p in DC.Porudzbinas
                                                   where p.PorudzbinaID == porID
                                                   select p).SingleOrDefault());

                    DC.SubmitChanges();
                    MessageBox.Show("Porudžbina izbrisana zato što nema poručenih stavki.");
                    porID = -1;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void btnZatvori_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridPorIzBaze.Items.Count > 0)
            {
                if (MessageBox.Show("Da li ste sigurni da želite da zatvorite porudžbinu?", "Zatvaranje porudžbine?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Porudzbina porudzbina = (from p in DC.Porudzbinas
                                             where p.PorudzbinaID == porID
                                             select p).SingleOrDefault();

                    porudzbina.Zavrseno = true;
                    DC.SubmitChanges();
                    napuniListuStavki();
                    porID = -1;
                }
            }
        }
    }
}
