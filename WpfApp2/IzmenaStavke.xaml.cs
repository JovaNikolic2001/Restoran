using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp2.Properties;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for IzmenaStavke.xaml
    /// </summary>
    public partial class IzmenaStavke : Window
    {
        RestoranDataContext DC = new RestoranDataContext();

        public IzmenaStavke()
        {
            InitializeComponent();
            btnIzmeni.Visibility = Visibility.Hidden;
            napuniListuStavki();
        }

        private void napuniListuStavki()
        {
            listBoxStavka.ItemsSource = null;

            var rez = (from s in DC.Stavkas
                       select s).ToList();

            listBoxStavka.ItemsSource = rez;
        }

        private void reset()
        {
            tbNaziv.Text = "";
            tbCena.Text = "";
            rbHrana.IsChecked = false;
            rbPice.IsChecked = false;
            rbDodatak.IsChecked = false;
            rbOstalo.IsChecked = false;
            btnIzmeni.Visibility = Visibility.Hidden;
            btnKreiraj.Visibility = Visibility.Visible;
        }

        private string cekiraniRB()
        {
            string kontent = null;

            List<RadioButton> rbLista = new List<RadioButton>();
            rbLista.Add(rbHrana);
            rbLista.Add(rbPice);
            rbLista.Add(rbDodatak);
            rbLista.Add(rbOstalo);

            foreach (RadioButton rbO in rbLista)
            {
                if (rbO.IsChecked == true)
                    kontent = rbO.Content.ToString();
            }

            return kontent;
        }

        private void btnKreiraj_Click(object sender, RoutedEventArgs e)
        {
            Stavka s = new Stavka();
            s.Kategorija = cekiraniRB();

            if (String.IsNullOrEmpty(s.Kategorija))
            {
                MessageBox.Show("Morate izabrati neku od kategorija.");
            }
            else
            {
                if (!String.IsNullOrEmpty(tbCena.Text) && !String.IsNullOrEmpty(tbNaziv.Text))
                {
                    Stavka st = (from sta in DC.Stavkas
                                 where sta.Naziv == tbNaziv.Text
                                 select sta).DefaultIfEmpty().SingleOrDefault();
                    if (st != null)
                    {
                        tbNaziv.Focus();
                        MessageBox.Show("Stavka sa istim nazivom već postoji.\nMolim Vas promenite naziv da biste nastavili dalje.", "Stavka već postoji", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        float broj;
                        bool validan;
                        validan = float.TryParse(tbCena.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out broj);
                        if (validan)
                        {
                            s.Naziv = tbNaziv.Text;
                            s.Cena = (decimal)broj;

                            if (MessageBox.Show("Naziv: " + s.Naziv + ", Kategorija: " + s.Kategorija + ", Cena: " + s.Cena + "RSD.", "Da li ste sigurni?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                DC.Stavkas.InsertOnSubmit(s);
                                try
                                {
                                    DC.SubmitChanges();
                                    MessageBox.Show("Uspešno kreiranje stavke!");
                                    napuniListuStavki();
                                    reset();
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Neuspešno kreiranje stavke!");
                                }
                            }
                        }
                        else
                            MessageBox.Show("Cena mora biti upisana isključivo brojevima.");
                    }
                }
                else
                    MessageBox.Show("Morate upisati nešto u sva polja.");
            }
        }

        private void btnOcisti_Click(object sender, RoutedEventArgs e)
        {
            reset();
            napuniListuStavki();
            btnIzbrisi.IsEnabled = false;
        }

        private void btnIzbrisi_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Da li ste sigurni da želite da izbrišete stavku?", "Pitanje", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SqlConnection konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
                konekcija.Open();
                Stavka s = listBoxStavka.SelectedItem as Stavka;
                string poruka = String.Empty;
                try
                {
                    var rez = (from u in DC.Uslugas
                               where u.StavkaID == s.StavkaID
                               select u).DefaultIfEmpty().ToList();

                    if (rez[0] == null || MessageBox.Show("Postoje usluge vezane za ovu stavku. \nDa li ste sigurni da želite da ih obrišete?", "Pitanje", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        SqlCommand komanda = new SqlCommand();
                        komanda.Connection = konekcija;
                        komanda.CommandText = $"DELETE FROM Usluga WHERE StavkaID = '{s.StavkaID}'";
                        komanda.ExecuteNonQuery();
                        DC.Stavkas.DeleteOnSubmit(s);
                        DC.SubmitChanges();
                        poruka += "Uspešno brisanje stavke " + s.Naziv + "!";

                        foreach (var r in rez)
                        {
                            var rez2 = (from u in DC.Uslugas
                                        where u.PorudzbinaID == r.PorudzbinaID
                                        select u).DefaultIfEmpty().ToList();
                            if (rez2[0] == null)
                            {
                                Porudzbina porudzbina = (from p in DC.Porudzbinas
                                                         where p.PorudzbinaID == r.PorudzbinaID
                                                         select p).SingleOrDefault();

                                DC.Porudzbinas.DeleteOnSubmit(porudzbina);
                                DC.SubmitChanges();
                                if (!porudzbina.Zavrseno)
                                    poruka += "\nIzbrisana porudžbina broj " + r.PorudzbinaID + ".";
                            }
                        }
                        MessageBox.Show(poruka);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Neuspešno brisanje stavke: " + ex.Message); }
                finally { konekcija.Close(); }
            }
            reset();
            napuniListuStavki();
        }
        private void popuni(Stavka s)
        {
            btnKreiraj.Visibility = Visibility.Hidden;
            btnIzmeni.Visibility = Visibility.Visible;
            tbNaziv.Text = s.Naziv;
            tbCena.Text = ((int)s.Cena).ToString();
            switch (s.Kategorija)
            {
                case "Hrana":
                    rbHrana.IsChecked = true; break;
                case "Piće":
                    rbPice.IsChecked = true; break;
                case "Dodatak":
                    rbDodatak.IsChecked = true; break;
                case "Ostalo":
                    rbOstalo.IsChecked = true; break;
            }
        }
        private bool komparacija()
        {
            var rez = (from s in DC.Stavkas
                       select s).ToList();

            foreach (Stavka s in rez)
            {
                if ((Stavka)listBoxStavka.SelectedItem == s)
                    return true;
            }

            return false;
        }
        private void listBoxStavka_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Stavka s = new Stavka();
            s = (Stavka)listBoxStavka.SelectedItem;
            btnIzbrisi.IsEnabled = true;

            if (s == null)
            {
                napuniListuStavki();
            }
            else
            {
                if (String.IsNullOrEmpty(tbCena.Text) && String.IsNullOrEmpty(tbNaziv.Text) || komparacija())
                    popuni(s);
                else
                    if (MessageBox.Show("Da li ste sigurni da želite da obustavite unos ili izmenu stavke?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    popuni(s);
            }
        }
        
        private void btnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(cekiraniRB()))
            {
                MessageBox.Show("Morate izabrani neku od kategorija.");
            }
            else
            {
                if (!String.IsNullOrEmpty(tbCena.Text) && !String.IsNullOrEmpty(tbNaziv.Text))
                {
                    float broj;
                    bool validan;
                    validan = float.TryParse(tbCena.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out broj);
                    if (validan)
                    {
                        Stavka sl = new Stavka();
                        sl = (Stavka)listBoxStavka.SelectedItem;

                        if (MessageBox.Show("Naziv: " + tbNaziv.Text + ", Kategorija: " + cekiraniRB() + ", Cena: " + (decimal)broj + "RSD.", "Da li ste sigurni?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            sl.Naziv = tbNaziv.Text;
                            sl.Cena = (decimal)broj;
                            sl.Kategorija = cekiraniRB();
                            try
                            {
                                DC.SubmitChanges();
                                MessageBox.Show("Uspešna izmena stavke!");
                                napuniListuStavki();
                                reset();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Neuspešna izmena stavke!");
                            }
                        }
                    }
                    else
                        MessageBox.Show("Cena mora biti upisana isključivo brojevima.");
                }
                else
                    MessageBox.Show("Morate upisati nešto u sva polja.");
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            napuniListuStavki();
        }
    }
}
