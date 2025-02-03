using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for PorudzbineForma.xaml
    /// </summary>
    public partial class PorudzbineForma : Window
    {
        
        RestoranDataContext DC = new RestoranDataContext();
        Osoblje admin = new Osoblje();
        public PorudzbineForma(Osoblje admin)
        {
            InitializeComponent();
            this.admin = admin;
            lUlogovan.Content += admin.Ime + " " + admin.Prezime;
            napuniDataGrid();
        }
        private string vremeOdPorudzbine(DateTime vremePor)
        {
            var razlika = DateTime.Now - vremePor;

            var dani = razlika.Days;
            var sati = razlika.Hours;
            var minuti = razlika.Minutes;
            var sekunde = razlika.Seconds;

            if(dani > 0)
                return dani.ToString() + "d " + sati.ToString() + "h " + minuti.ToString() + "m " + sekunde.ToString() + "s";
            return sati.ToString() + "h " + minuti.ToString() + "m " + sekunde.ToString() + "s";
        }

        public void napuniDataGrid()
        {
            #pragma warning disable CS0252 
            if (btnPorudzbine.Content == "Sve porudzbine")
            {
                napuniPrviGrid();
            }
            else
            {
                var listaPor = (from pl in DC.Porudzbinas
                                where pl.Zavrseno == false && pl.OsobljeID == admin.OsobljeID
                                select pl).ToList();

                dataGrid.Items.Clear();
                foreach (var item in listaPor)
                {
                    var rez = (from ul in DC.Uslugas
                               where ul.PorudzbinaID == item.PorudzbinaID
                               select ul).DefaultIfEmpty().ToList();
                    if (rez.Count > 0)
                        dataGrid.Items.Add((from p in DC.Porudzbinas
                                            join o in DC.Osobljes on p.OsobljeID equals o.OsobljeID
                                            where p.PorudzbinaID == item.PorudzbinaID
                                            select new
                                            {
                                                p.PorudzbinaID,
                                                p.StoID,
                                                p.VremePorudzbine,
                                                o.Ime,
                                                o.Prezime,
                                                Vreme = vremeOdPorudzbine(p.VremePorudzbine),
                                                SveUkupno = (from u in DC.Uslugas
                                                             where u.PorudzbinaID == p.PorudzbinaID
                                                             select u.Ukupno).Sum()
                                            })
                                            .Single());
                }
                btnPorudzbine.Content = "Sve porudzbine";
            }
            #pragma warning restore CS0252 
        }

        private void napuniPrviGrid()
        {
            var listaPor = (from pl in DC.Porudzbinas
                            where pl.Zavrseno == false
                            select pl).ToList();

            dataGrid.Items.Clear();
            foreach (var item in listaPor)
            {
                var rez = (from ul in DC.Uslugas
                           where ul.PorudzbinaID == item.PorudzbinaID
                           select ul).DefaultIfEmpty().ToList();
                if (rez[0] != null)
                    dataGrid.Items.Add((from p in DC.Porudzbinas
                                        join o in DC.Osobljes on p.OsobljeID equals o.OsobljeID
                                        where p.PorudzbinaID == item.PorudzbinaID
                                        select new
                                        {
                                            p.PorudzbinaID,
                                            p.StoID,
                                            p.VremePorudzbine,
                                            o.Ime,
                                            o.Prezime,
                                            Vreme = vremeOdPorudzbine(p.VremePorudzbine),
                                            SveUkupno = (from u in DC.Uslugas
                                                         where u.PorudzbinaID == p.PorudzbinaID
                                                         select u.Ukupno).Sum()
                                        })
                                        .Single());
            }
            btnPorudzbine.Content = "Moje porudzbine";
        }

        private void dataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            var kolonaInfo = dataGrid.SelectedCells[0];
            var pID = (kolonaInfo.Column.GetCellContent(kolonaInfo.Item) as TextBlock).Text;
            DataGrid DG = e.DetailsElement.FindName("dataGridRow") as DataGrid;
            if (DG != null)
            {
                DG.ItemsSource = (from u in DC.Uslugas
                                  join s in DC.Stavkas on u.StavkaID equals s.StavkaID
                                  where u.PorudzbinaID == Int32.Parse(pID)
                                  select new { s.Naziv, s.Kategorija, s.Cena, u.Količina, u.Ukupno }).ToList();
            }
        }

        private void btnIzbrisiPorudzbinu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Da li ste sigurni da želite da izbrišete porudžbinu?", "Brisanje porudžbine?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
                var kolonaInfo = dataGrid.SelectedCells[0];
                var pID = (kolonaInfo.Column.GetCellContent(kolonaInfo.Item) as TextBlock).Text;

                konekcija.Open();
                try
                {
                    SqlCommand komanda = new SqlCommand();
                    komanda.Connection = konekcija;
                    komanda.CommandText = $"DELETE FROM Usluga WHERE PorudzbinaID = '{pID}'";
                    komanda.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Neuspešno brisanje usluga! - " + ex.Message);
                }
                finally
                {
                    konekcija.Close();
                }

                var rezul = (from p in DC.Porudzbinas
                           where p.PorudzbinaID.Equals(Int32.Parse(pID))
                           select p).Single();

                DC.Porudzbinas.DeleteOnSubmit(rezul);
                try
                {
                    DC.SubmitChanges();
                    MessageBox.Show("Uspešno brisanje porudžbine!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Neuspešno brisanje porudžbine! - " + ex.Message);
                }
                napuniDataGrid();
            }
        }

        private void btnZatvoriPorudzbinu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Da li ste sigurni da želite da zatvorite porudžbinu?", "Zatvaranje porudžbine!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var konekcija = new SqlConnection(Properties.Settings.Default.RestoranConnectionString);
                var kolonaInfo = dataGrid.SelectedCells[0];
                var pID = (kolonaInfo.Column.GetCellContent(kolonaInfo.Item) as TextBlock).Text;

                var rezul = (from p in DC.Porudzbinas
                             where p.PorudzbinaID.Equals(Int32.Parse(pID))
                             select p).Single();
                rezul.Zavrseno = true;

                try
                {
                    DC.SubmitChanges();
                    MessageBox.Show("Uspešno kompletiranje porudžbine!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Neuspešno kompletiranje porudžbine! - " + ex.Message);
                }
                napuniDataGrid();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            napuniPrviGrid();
        }

        private void btnPorudzbine_Click(object sender, RoutedEventArgs e)
        {
            napuniDataGrid();
        }
    }
}
