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
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for SmeneForma.xaml
    /// </summary>
    public partial class SmeneForma : Window
    {
        RestoranDataContext DC = new RestoranDataContext();
        Osoblje admin = new Osoblje();
        
        public SmeneForma(Osoblje admin)
        {
            InitializeComponent();
            this.admin = admin;
            if (!admin.Admin)
            {
                btnSveSmene.Margin = new Thickness(260, 10, 0, 0);
                btnMojeSmene.Margin = new Thickness(50, 10, 0, 0);
                btnRefresh.Margin = new Thickness(218, 10, 0, 0);
                dataGridSmene.Margin = new Thickness(5, 45, 5, 10);
            }
            ucitajSmene();
        }

        private void prikaziSveSmene()
        {
            var rez = (from s in DC.Smenas
                       join i in DC.Istorijas on s.SmenaID equals i.SmenaID
                       join o in DC.Osobljes on i.OsobljeID equals o.OsobljeID
                       select new { o.Ime, o.Prezime, Datumi = s.Datum.Day + "." + s.Datum.Month + "." + s.Datum.Year, s.PocetakSmene, s.KrajSmene }).ToList();

            dataGridSmene.ItemsSource = rez.OrderBy(raz => raz.Ime);
            btnSveSmene.IsEnabled = false;
            btnMojeSmene.IsEnabled = true;
        }

        private void prikaziMojeSmene()
        {
            var rez = (from s in DC.Smenas
                       join i in DC.Istorijas on s.SmenaID equals i.SmenaID
                       join o in DC.Osobljes on i.OsobljeID equals o.OsobljeID
                       orderby s.PocetakSmene ascending
                       where o.OsobljeID == admin.OsobljeID
                       select new { o.Ime, o.Prezime, Datumi = s.Datum.Day + "." + s.Datum.Month + "." + s.Datum.Year, s.PocetakSmene, s.KrajSmene }).ToList();

            dataGridSmene.ItemsSource = rez.OrderBy(raz => raz.Ime);
            btnMojeSmene.IsEnabled = false;
            btnSveSmene.IsEnabled = true;
        }

        private void ucitajSmene() 
        {
            if (admin.Admin) 
            {
                prikaziSveSmene();
            }
            else
            {
                prikaziMojeSmene();
                btnDodajSmene.Visibility = Visibility.Hidden;
                btnRasporediSmene.Visibility = Visibility.Hidden;
            }
        }

        private void btnSveSmene_Click(object sender, RoutedEventArgs e)
        {
            prikaziSveSmene();
        }

        private void btnMojeSmene_Click(object sender, RoutedEventArgs e)
        {
            prikaziMojeSmene();
        }

        private void btnDodajSmene_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();

            foreach (Window w in Application.Current.Windows)
                if (w != this)
                    if (w.Title == "Dodavanje smena")
                    {
                        window = w;
                    }

            if (window.Title == "Dodavanje smena")
            {
                window.WindowState = WindowState.Minimized;
                window.WindowState = WindowState.Normal;
                window.Focus();
            }
            else
            {
                DodavanjeSmeneForma dsf = new DodavanjeSmeneForma(admin);
                dsf.Show();
            }
        }

        private void btnRasporediSmene_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();

            foreach (Window w in Application.Current.Windows)
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
                RasporediSmeneForma rsf = new RasporediSmeneForma(admin);
                rsf.Show();
            }
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            ucitajSmene();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (admin.Admin)
                prikaziSveSmene();
            else
                prikaziMojeSmene();
        }
    }
}
