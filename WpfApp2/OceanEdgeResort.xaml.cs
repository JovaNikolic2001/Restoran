using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for OceanEdgeResort.xaml
    /// </summary>
    public partial class OceanEdgeResort : Window
    {
        RestoranDataContext DC = new RestoranDataContext();
        SqlConnection konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
        Osoblje admin = new Osoblje();
        public OceanEdgeResort (Osoblje admin)
        {
            InitializeComponent();
            this.admin = admin;
            if (admin.Admin)
            {
                btnPregledSmena.Visibility = Visibility.Hidden;
                btnRefresh.Margin = new Thickness(30, 15, 0, 0);
            }
            rasporediStolove();
        }

        private void rasporediStolove()
        {
            Zauzet1.Visibility = Visibility.Hidden; OdStrane1.Visibility = Visibility.Hidden;
            Zauzet2.Visibility = Visibility.Hidden; OdStrane2.Visibility = Visibility.Hidden;
            Zauzet3.Visibility = Visibility.Hidden; OdStrane3.Visibility = Visibility.Hidden;
            Zauzet4.Visibility = Visibility.Hidden; OdStrane4.Visibility = Visibility.Hidden;
            Zauzet5.Visibility = Visibility.Hidden; OdStrane5.Visibility = Visibility.Hidden;
            Zauzet6.Visibility = Visibility.Hidden; OdStrane6.Visibility = Visibility.Hidden;
            Zauzet7.Visibility = Visibility.Hidden; OdStrane7.Visibility = Visibility.Hidden;
            Zauzet8.Visibility = Visibility.Hidden; OdStrane8.Visibility = Visibility.Hidden;
            Zauzet9.Visibility = Visibility.Hidden; OdStrane9.Visibility = Visibility.Hidden;
            Zauzet10.Visibility = Visibility.Hidden; OdStrane10.Visibility = Visibility.Hidden;
            Zauzet11.Visibility = Visibility.Hidden; OdStrane11.Visibility = Visibility.Hidden;

            List<Sto> stolovi = (from p in DC.Porudzbinas
                                 join s in DC.Stos on p.StoID equals s.StoID
                                 where p.Zavrseno == false
                                 select s).ToList();

            foreach (Sto s in stolovi)
            {
                var o = (from po in DC.Porudzbinas
                             join os in DC.Osobljes on po.OsobljeID equals os.OsobljeID
                             where po.StoID == s.StoID && po.Zavrseno == false
                             select os).ToList();
                string ime = o[0].Ime + " " + o[0].Prezime;

                switch (s.StoID)
                {
                    case 1:
                        OdStrane1.Content = ime ; Zauzet1.Visibility = Visibility.Visible ; OdStrane1.Visibility = Visibility.Visible ; break;
                    case 2:
                        OdStrane2.Content = ime; Zauzet2.Visibility = Visibility.Visible; OdStrane2.Visibility = Visibility.Visible; break; ;
                    case 3:
                        OdStrane3.Content = ime; Zauzet3.Visibility = Visibility.Visible; OdStrane3.Visibility = Visibility.Visible; break; ;
                    case 4:
                        OdStrane4.Content = ime; Zauzet4.Visibility = Visibility.Visible; OdStrane4.Visibility = Visibility.Visible; break; ;
                    case 5:
                        OdStrane5.Content = ime; Zauzet5.Visibility = Visibility.Visible; OdStrane5.Visibility = Visibility.Visible; break; ;
                    case 6:
                        OdStrane6.Content = ime; Zauzet6.Visibility = Visibility.Visible; OdStrane6.Visibility = Visibility.Visible; break; ;
                    case 7:
                        OdStrane7.Content = ime; Zauzet7.Visibility = Visibility.Visible; OdStrane7.Visibility = Visibility.Visible; break; ;
                    case 8:
                        OdStrane8.Content = ime; Zauzet8.Visibility = Visibility.Visible; OdStrane8.Visibility = Visibility.Visible; break; ;
                    case 9:
                        OdStrane9.Content = ime; Zauzet9.Visibility = Visibility.Visible; OdStrane9.Visibility = Visibility.Visible; break; ;
                    case 10:
                        OdStrane10.Content = ime; Zauzet10.Visibility = Visibility.Visible; OdStrane10.Visibility = Visibility.Visible; break; ;
                    case 11:
                        OdStrane11.Content = ime; Zauzet11.Visibility = Visibility.Visible; OdStrane11.Visibility = Visibility.Visible; break; ;
                }
            }
        }

        private void btnPregledSmena_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();

            foreach (Window w in Application.Current.Windows)
                if (w != this)
                    if (w.Title == "Smene")
                    {
                        window = w;
                    }

            if (window.Title == "Smene")
            {
                window.WindowState = WindowState.Minimized;
                window.WindowState = WindowState.Normal;
                window.Focus();
            }
            else
            {
                SmeneForma smene = new SmeneForma(admin);
                smene.Show();
            }
        }

        private void btnSto1_Click(object sender, RoutedEventArgs e)
        {
            
            Button button = e.Source as Button;
            var rez = (from p in DC.Porudzbinas
                       join o in DC.Osobljes on p.OsobljeID equals o.OsobljeID
                       where p.StoID == Int32.Parse(button.Content.ToString()) && p.Zavrseno == false
                       select o).ToList();

            if (admin.Admin)
            {
                Window window = new Window();

                foreach (Window w in Application.Current.Windows)
                    if (w != this)
                        if (w.Title == "Nova porudžbina za sto broj " + button.Content.ToString())
                        {
                            window = w;
                        }

                if (window.Title == "Nova porudžbina za sto broj " + button.Content.ToString())
                {
                    window.WindowState = WindowState.Minimized;
                    window.WindowState = WindowState.Normal;
                    window.Focus();
                }
                else
                {
                    NovaPorudzbina porudzbina = new NovaPorudzbina(admin, e.Source);
                    porudzbina.Show();

                    porudzbina = null;
                }
            }
            else if (rez.Count == 0 || rez[0].OsobljeID == admin.OsobljeID)
            {
                NovaPorudzbina porudzbina = new NovaPorudzbina(admin, e.Source);
                porudzbina.ShowDialog();

                porudzbina = null;
            }
            else
            {
                MessageBox.Show("Sto je vec zauzet od strane - " + rez[0].Ime + " " + rez[0].Prezime);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!admin.Admin)
                foreach (Window w in Application.Current.Windows)
                    if (w != this)
                        if(!(w.Title == "Login"))
                            w.Close();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            rasporediStolove();
        }
    }
}
