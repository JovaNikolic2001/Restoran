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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Administrator.xaml
    /// </summary>
    public partial class Administrator : Window
    {
        Osoblje admin = new Osoblje();        

        public Administrator(Osoblje admin)
        {
            this.admin = admin;
            InitializeComponent();
            lUlogovan.Content = admin.Ime + " " + admin.Prezime;
        }

        private void btnDodavanjeStavke_Click(object sender, RoutedEventArgs e)
        {
            if (admin.Admin == true)
            {
                Window window = new Window();

                foreach (Window w in Application.Current.Windows)
                    if (w != this)
                        if (w.Title == "Izmena stavki")
                        {
                            window = w;
                        }

                if (window.Title == "Izmena stavki")
                {
                    window.WindowState = WindowState.Minimized;
                    window.WindowState = WindowState.Normal;
                    window.Focus();
                }
                else
                {
                    IzmenaStavke ds = new IzmenaStavke();
                    ds.Show();
                }
            }
            else
                MessageBox.Show("Morate biti admin za ovu funkciju.");
        }

        private void btnOsoblje_Click(object sender, RoutedEventArgs e)
        {            
            if (admin.Admin == true)
            {
                Window window = new Window();

                foreach (Window w in Application.Current.Windows)
                    if (w != this)
                        if (w.Title == "Osoblje")
                        {
                            window = w;
                        }

                if (window.Title == "Osoblje")
                {
                    window.WindowState = WindowState.Minimized;
                    window.WindowState = WindowState.Normal;
                    window.Focus();
                }
                else
                {
                    OsobljeForma osoblje = new OsobljeForma(admin);
                    osoblje.Show();
                }                
            }
            else
                MessageBox.Show("Morate biti admin za ovu funkciju.");
        }

        private void btnPorudzbine_Click(object sender, RoutedEventArgs e)
        {
            if (admin.Admin == true)
            {
                Window window = new Window();

                foreach (Window w in Application.Current.Windows)
                    if (w != this)
                        if (w.Title == "Porudžbine")
                        {
                            window = w;
                        }

                if (window.Title == "Porudžbine")
                {
                    window.WindowState = WindowState.Minimized;
                    window.WindowState = WindowState.Normal;
                    window.Focus();
                }
                else
                {
                    PorudzbineForma porudzbineForma = new PorudzbineForma(admin);
                    porudzbineForma.Show();
                }
            }
            else
                MessageBox.Show("Morate biti admin za ovu funkciju.");
        }

        private void btnSmene_Click(object sender, RoutedEventArgs e)
        {
            if (admin.Admin == true)
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
            else
                MessageBox.Show("Morate biti admin za ovu funkciju.");
        }

        private void btnNovaPorudzbina_Click(object sender, RoutedEventArgs e)
        {
            if (admin.Admin == true)
            {
                Window window = new Window();

                foreach (Window w in Application.Current.Windows)
                    if (w != this)
                        if (w.Title == "OceanEdgeResort")
                        {
                            window = w;
                        }

                if (window.Title == "OceanEdgeResort")
                {
                    window.WindowState = WindowState.Minimized;
                    window.WindowState = WindowState.Normal;
                    window.Focus();
                }
                else
                {
                    OceanEdgeResort oer = new OceanEdgeResort(admin);
                    oer.Show();
                }
            }
            else
                MessageBox.Show("Morate biti admin za ovu funkciju.");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
                if (w != this)
                    if (!(w.Title == "Login"))
                        w.Close();
        }
    }
}
