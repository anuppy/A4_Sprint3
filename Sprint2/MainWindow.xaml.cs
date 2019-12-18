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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Forms; //für den Exit-Fenster
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

using INFITF; // für Catia
using MECMOD;
using PARTITF;


//using MinimalCatia;


//---------------CatiaConnection--------------

class CatiaConnection
{
    INFITF.Application hsp_catiaApp;
    MECMOD.PartDocument hsp_catiaPart;
    MECMOD.Sketch hsp_catiaProfil;

    public bool CATIALaeuft()
    {
        try
        {
            object catiaObject = System.Runtime.InteropServices.Marshal.GetActiveObject(
                "CATIA.Application");
            hsp_catiaApp = (INFITF.Application)catiaObject;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Boolean ErzeugePart()
    {
        INFITF.Documents catDocuments1 = hsp_catiaApp.Documents;
        hsp_catiaPart = catDocuments1.Add("Part") as MECMOD.PartDocument;
        return true;
    }

    public void ErstelleLeereSkizze()
    {
        // geometrisches Set auswaehlen und umbenennen
        HybridBodies catHybridBodies1 = hsp_catiaPart.Part.HybridBodies;
        HybridBody catHybridBody1;
        try
        {
            catHybridBody1 = catHybridBodies1.Item("Geometrisches Set.1");
        }
        catch (Exception)
        {
            MessageBox.Show("Kein geometrisches Set gefunden! " + Environment.NewLine +
                "Ein PART manuell erzeugen und ein darauf achten, dass 'Geometisches Set' aktiviert ist.",
                "Fehler", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        catHybridBody1.set_Name("Profile");
        // neue Skizze im ausgewaehlten geometrischen Set anlegen
        Sketches catSketches1 = catHybridBody1.HybridSketches;
        OriginElements catOriginElements = hsp_catiaPart.Part.OriginElements;
        Reference catReference1 = (Reference)catOriginElements.PlaneYZ;
        hsp_catiaProfil = catSketches1.Add(catReference1);

        // Achsensystem in Skizze erstellen 
        ErzeugeAchsensystem();

        // Part aktualisieren
        hsp_catiaPart.Part.Update();
    }

    private void ErzeugeAchsensystem()
    {
        object[] arr = new object[] {0.0, 0.0, 0.0,
                                         0.0, 1.0, 0.0,
                                         0.0, 0.0, 1.0 };
        hsp_catiaProfil.SetAbsoluteAxisData(arr);
    }

    public void ErzeugeRechteckprofil(string currentItemName,double[] parameterListe) 
    {
        //  ()  <--breite-->     b
 
        double b = parameterListe[0];
        double h = parameterListe[1];
        //double e = parameterListe[2];
        //double f = parameterListe[3];

        // Skizze umbenennen
        hsp_catiaProfil.set_Name(currentItemName);
        // Skizze oeffnen
        Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();
        // erst die Punkte
        Point2D catPoint2D1 = catFactory2D1.CreatePoint(-b/2, h/2);
        Point2D catPoint2D2 = catFactory2D1.CreatePoint(b / 2, h / 2);
        Point2D catPoint2D3 = catFactory2D1.CreatePoint(b / 2, -h / 2);
        Point2D catPoint2D4 = catFactory2D1.CreatePoint(-b / 2, -h / 2);
        // dann die Linien
        Line2D catLine2D1 = catFactory2D1.CreateLine(-b / 2, h / 2, b / 2, h / 2);
        catLine2D1.StartPoint = catPoint2D1;
        catLine2D1.EndPoint = catPoint2D2;

        Line2D catLine2D2 = catFactory2D1.CreateLine(b / 2, h / 2, b / 2, -h / 2);
        catLine2D2.StartPoint = catPoint2D2;
        catLine2D2.EndPoint = catPoint2D3;

        Line2D catLine2D3 = catFactory2D1.CreateLine(b / 2, -h / 2, -b / 2, -h / 2);
        catLine2D3.StartPoint = catPoint2D3;
        catLine2D3.EndPoint = catPoint2D4;

        Line2D catLine2D4 = catFactory2D1.CreateLine(-b / 2, -h / 2, -b / 2, h / 2);
        catLine2D4.StartPoint = catPoint2D4;
        catLine2D4.EndPoint = catPoint2D1;

        // Skizzierer verlassen
        hsp_catiaProfil.CloseEdition();
        // Part aktualisieren
        hsp_catiaPart.Part.Update();
        }

    public void ErzeugeIkprofil(string currentItemName, double[] parameterListe)
    {
        //  ()  <--breite-->     b

        double H = parameterListe[0];
        double h = parameterListe[1];
        double B = parameterListe[2];
        double b = parameterListe[3];

        // Skizze umbenennen
        hsp_catiaProfil.set_Name(currentItemName);
        // Skizze oeffnen
        Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();
        // erst die Punkte
        Point2D catPoint2D1 = catFactory2D1.CreatePoint(B/2,-H/2);
        Point2D catPoint2D2 = catFactory2D1.CreatePoint(B/2,-h/2);
        Point2D catPoint2D3 = catFactory2D1.CreatePoint(b/2,-h/2);
        Point2D catPoint2D4 = catFactory2D1.CreatePoint(b/2,h/2);
        Point2D catPoint2D5 = catFactory2D1.CreatePoint(B/2,h/2);
        Point2D catPoint2D6 = catFactory2D1.CreatePoint(B/2,H/2);
        Point2D catPoint2D7 = catFactory2D1.CreatePoint(-B/2,H/2);
        Point2D catPoint2D8 = catFactory2D1.CreatePoint(-B/2,h/2);
        Point2D catPoint2D9 = catFactory2D1.CreatePoint(-b/2,h/2);
        Point2D catPoint2D10 = catFactory2D1.CreatePoint(-b/2,-h/2);
        Point2D catPoint2D11 = catFactory2D1.CreatePoint(-B/2,-h/2);
        Point2D catPoint2D12 = catFactory2D1.CreatePoint(-B/2,-H/2);

        // dann die Linien
        Line2D catLine2D1 = catFactory2D1.CreateLine(B/2,-H/2,B/2,-h/2);
        catLine2D1.StartPoint = catPoint2D1;
        catLine2D1.EndPoint = catPoint2D2;

        Line2D catLine2D2 = catFactory2D1.CreateLine(B/2,-h/2,b/2,-h/2);
        catLine2D2.StartPoint = catPoint2D2;
        catLine2D2.EndPoint = catPoint2D3;

        Line2D catLine2D3 = catFactory2D1.CreateLine(b/2,-h/2,b/2,h/2);
        catLine2D3.StartPoint = catPoint2D3;
        catLine2D3.EndPoint = catPoint2D4;

        Line2D catLine2D4 = catFactory2D1.CreateLine(b/2,h/2,B/2,h/2);
        catLine2D4.StartPoint = catPoint2D4;
        catLine2D4.EndPoint = catPoint2D5;

        Line2D catLine2D5 = catFactory2D1.CreateLine(B/2,h/2,B/2,H/2);
        catLine2D5.StartPoint = catPoint2D5;
        catLine2D5.EndPoint = catPoint2D6;

        Line2D catLine2D6 = catFactory2D1.CreateLine(B/2,H/2,-B/2,H/2);
        catLine2D6.StartPoint = catPoint2D6;
        catLine2D6.EndPoint = catPoint2D7;

        Line2D catLine2D7 = catFactory2D1.CreateLine(-B/2,H/2, -B / 2, h / 2);
        catLine2D7.StartPoint = catPoint2D7;
        catLine2D7.EndPoint = catPoint2D8;

        Line2D catLine2D8 = catFactory2D1.CreateLine(-B / 2, h / 2,-b/2,h/2);
        catLine2D8.StartPoint = catPoint2D8;
        catLine2D8.EndPoint = catPoint2D9;

        Line2D catLine2D9 = catFactory2D1.CreateLine(-b/2,h/2,-b/2,-h/2);
        catLine2D9.StartPoint = catPoint2D9;
        catLine2D9.EndPoint = catPoint2D10;

        Line2D catLine2D10 = catFactory2D1.CreateLine(-b/2,-h/2, -B / 2, -h / 2);
        catLine2D10.StartPoint = catPoint2D10;
        catLine2D10.EndPoint = catPoint2D11;

        Line2D catLine2D11 = catFactory2D1.CreateLine(-B / 2, -h / 2, -B / 2, -H / 2);
        catLine2D11.StartPoint = catPoint2D11;
        catLine2D11.EndPoint = catPoint2D12;

        Line2D catLine2D12 = catFactory2D1.CreateLine(-B / 2, -H / 2, B / 2, -H / 2);
        catLine2D12.StartPoint = catPoint2D12;
        catLine2D12.EndPoint = catPoint2D1;

        // Skizzierer verlassen
        hsp_catiaProfil.CloseEdition();
        // Part aktualisieren
        hsp_catiaPart.Part.Update();
    }


    public void ErzeugeTkprofil(string currentItemName, double[] parameterListe)
    {
        //  ()  <--breite-->     b

        double H = parameterListe[0];
        double h = parameterListe[1];
        double B = parameterListe[2];
        double b = parameterListe[3];

        // Skizze umbenennen
        hsp_catiaProfil.set_Name(currentItemName);
        // Skizze oeffnen
        Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();
        // erst die Punkte
        Point2D catPoint2D1 = catFactory2D1.CreatePoint(B / 2, -H / 2);
        Point2D catPoint2D2 = catFactory2D1.CreatePoint(B / 2, -H / 2+h);
        Point2D catPoint2D3 = catFactory2D1.CreatePoint(b / 2, -H / 2+h);
        Point2D catPoint2D4 = catFactory2D1.CreatePoint(b / 2, H / 2);
        Point2D catPoint2D5 = catFactory2D1.CreatePoint(-b / 2, H / 2);
        Point2D catPoint2D6 = catFactory2D1.CreatePoint(-b / 2,- H / 2+h);
        Point2D catPoint2D7 = catFactory2D1.CreatePoint(-B / 2, -H / 2+h);
        Point2D catPoint2D8 = catFactory2D1.CreatePoint(-B / 2, -H / 2);

        // dann die Linien
        Line2D catLine2D1 = catFactory2D1.CreateLine(B / 2, -H / 2, B / 2, -H / 2 + h);
        catLine2D1.StartPoint = catPoint2D1;
        catLine2D1.EndPoint = catPoint2D2;

        Line2D catLine2D2 = catFactory2D1.CreateLine(B / 2, -H / 2 + h, b / 2, -H / 2 + h);
        catLine2D2.StartPoint = catPoint2D2;
        catLine2D2.EndPoint = catPoint2D3;

        Line2D catLine2D3 = catFactory2D1.CreateLine(b / 2, -H / 2 + h, b / 2, H / 2);
        catLine2D3.StartPoint = catPoint2D3;
        catLine2D3.EndPoint = catPoint2D4;

        Line2D catLine2D4 = catFactory2D1.CreateLine(b / 2, H / 2, -b / 2, H / 2);
        catLine2D4.StartPoint = catPoint2D4;
        catLine2D4.EndPoint = catPoint2D5;

        Line2D catLine2D5 = catFactory2D1.CreateLine(-b / 2, H / 2, -b / 2, -H / 2 + h);
        catLine2D5.StartPoint = catPoint2D5;
        catLine2D5.EndPoint = catPoint2D6;

        Line2D catLine2D6 = catFactory2D1.CreateLine(-b / 2, -H / 2 + h, -B / 2, -H / 2 + h);
        catLine2D6.StartPoint = catPoint2D6;
        catLine2D6.EndPoint = catPoint2D7;

        Line2D catLine2D7 = catFactory2D1.CreateLine(-B / 2, -H / 2 + h, -B / 2, -H / 2);
        catLine2D7.StartPoint = catPoint2D7;
        catLine2D7.EndPoint = catPoint2D8;

        Line2D catLine2D8 = catFactory2D1.CreateLine(-B / 2, -H / 2, B / 2, -H / 2);
        catLine2D8.StartPoint = catPoint2D8;
        catLine2D8.EndPoint = catPoint2D1;

        // Skizzierer verlassen
        hsp_catiaProfil.CloseEdition();
        // Part aktualisieren
        hsp_catiaPart.Part.Update();
    }

    public void ErzeugeDreieckprofil(string currentItemName, double[] parameterListe)
    {
        double b = parameterListe[0];
        double h = parameterListe[1];

        // Skizze umbenennen
        hsp_catiaProfil.set_Name(currentItemName);
        // Skizze oeffnen
        Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();
        // erst die Punkte
        Point2D catPoint2D1 = catFactory2D1.CreatePoint(b / 2, -h / 2);
        Point2D catPoint2D2 = catFactory2D1.CreatePoint(0, h / 2);
        Point2D catPoint2D3 = catFactory2D1.CreatePoint(-b / 2, -h / 2);
        // dann die Linien
        Line2D catLine2D1 = catFactory2D1.CreateLine(b / 2, -h / 2, 0, h / 2);
        catLine2D1.StartPoint = catPoint2D1;
        catLine2D1.EndPoint = catPoint2D2;

        Line2D catLine2D2 = catFactory2D1.CreateLine(0, h / 2, -b / 2, -h / 2);
        catLine2D2.StartPoint = catPoint2D2;
        catLine2D2.EndPoint = catPoint2D3;

        Line2D catLine2D3 = catFactory2D1.CreateLine(-b / 2, -h / 2, b / 2, -h / 2);
        catLine2D3.StartPoint = catPoint2D3;
        catLine2D3.EndPoint = catPoint2D1;

        // Skizzierer verlassen
        hsp_catiaProfil.CloseEdition();
        // Part aktualisieren
        hsp_catiaPart.Part.Update();
    }

    public void ErzeugeKreisprofil(string currentItemName, double[] parameterListe)
    {
        double D = parameterListe[0];
        

        // Skizze umbenennen
        hsp_catiaProfil.set_Name(currentItemName);
        // Skizze oeffnen
        Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();
        // Kreis
        Circle2D Circle2D1 = catFactory2D1.CreateClosedCircle(0.0,0.0,D/2);
                  
        // Skizzierer verlassen
        hsp_catiaProfil.CloseEdition();
        // Part aktualisieren
        hsp_catiaPart.Part.Update();
    }

    public void ErzeugeKreisRingprofil(string currentItemName, double[] parameterListe)
    {
        double D = parameterListe[0];
        double d = parameterListe[1];

        // Skizze umbenennen
        hsp_catiaProfil.set_Name(currentItemName);
        // Skizze oeffnen
        Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();
        // Kreis
        Circle2D Circle2D1 = catFactory2D1.CreateClosedCircle(0.0, 0.0, D / 2);
        Circle2D Circle2D2 = catFactory2D1.CreateClosedCircle(0.0, 0.0, d / 2);

        // Skizzierer verlassen
        hsp_catiaProfil.CloseEdition();
        // Part aktualisieren
        hsp_catiaPart.Part.Update();
    }

    public void ErzeugeBalken(Double profilLaenge)
    {
        // Hauptkoerper in Bearbeitung definieren
        hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;

        // Block(Balken) erzeugen
        ShapeFactory catShapeFactory1 = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;
        Pad catPad1 = catShapeFactory1.AddNewPad(hsp_catiaProfil, profilLaenge);

        // Block umbenennen
        catPad1.set_Name("Balken");

        // Part aktualisieren
        hsp_catiaPart.Part.Update();
    }
}
//-----------CatiaConnection------------------


//-----------CatiaControl-------------------
public class CatiaControl
{
    public CatiaControl(string currentItemName, double[] parameterListe,double profilLaenge)
    {
        try
        {
            CatiaConnection cc = new CatiaConnection();

            // Finde Catia Prozess
            if (cc.CATIALaeuft())
            {
                cc.ErzeugePart();
                // Erstelle eine Skizze
                cc.ErstelleLeereSkizze();
                // Generiere ein Profil
                switch (currentItemName)
                    {
                    case "it_Rechteckprofil":
                        cc.ErzeugeRechteckprofil(currentItemName,parameterListe);
                        break;
                    case "it_IProfil":
                        cc.ErzeugeIkprofil(currentItemName, parameterListe);
                        break;
                    case "it_TProfil":
                        cc.ErzeugeTkprofil(currentItemName, parameterListe);
                        break;
                    case "it_Dreieck":
                        cc.ErzeugeDreieckprofil(currentItemName, parameterListe);
                        break;
                    case "it_KeisProfil":
                        cc.ErzeugeKreisprofil(currentItemName, parameterListe);
                        break;
                    case "it_KreisringProfil":
                        cc.ErzeugeKreisRingprofil(currentItemName, parameterListe);
                        break;
                }

               
                // Extrudiere Balken
                cc.ErzeugeBalken(profilLaenge);
            }
            else
            {
                Console.WriteLine("Laufende Catia Application nicht gefunden");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Exception aufgetreten");
        }
        Console.WriteLine("Fertig - Taste drücken.");
    }
    //static void Main(string[] args){new CatiaControl();}
}
//--------------CatiaControl-----------------


namespace Sprint3
{
    public partial class MainWindow : System.Windows.Window
    {
        //-----

        //-----

        public MainWindow()
        {
            InitializeComponent();
        }


        //---------------- SPRINT 3 --------------------------

        private void btn_Catia_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)tw_profilAuswahl.SelectedItem;
            string currentItemName = (string)item.Name;

            double[] parameterListe = new double[4];
            double profilLaenge= Convert.ToDouble(tb_Laenge.Text);
            
            if (tb_3.Text=="" || tb_4.Text=="")
            {
                tb_2.Text = "0";
                tb_3.Text = "0";
                tb_4.Text = "0";
            }


            double var1 = Convert.ToDouble(tb_1.Text);
            double var2 = Convert.ToDouble(tb_2.Text);
            double var3 = Convert.ToDouble(tb_3.Text);
            double var4 = Convert.ToDouble(tb_4.Text);

            
                parameterListe[0] = var1;
                parameterListe[1] = var2;
                parameterListe[2] = var3;
                parameterListe[3] = var4;
            
           
           new CatiaControl(currentItemName, parameterListe, profilLaenge);
        }



        //---------------- SPRINT 3 --------------------------


        //Programm wird beendet
        private void btn_Ende_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show(
                "Wollen Sie das Programm wirklich beenden?",
                "Programmende",
                 MessageBoxButtons.YesNo);

            // Schießt das Programm, wenn result==true
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }
        }

        //Berechne alles, je nachdem was gerade ausgewählt ist
        private void btn_Rechne_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)tw_profilAuswahl.SelectedItem;
            string currentItemName = (string)item.Name; //name des aktuell ausgewählten Profils

            
            if (currentItemName == "it_Rechteckprofil")
            {
                rechne_Rechteck();
            }
            else if (currentItemName == "it_IProfil")
            {
                rechne_IProfil();
            }
            else if (currentItemName == "it_TProfil")
            {
                rechne_TProfil();
            }
            else if (currentItemName == "it_Dreieck")
            {
                rechne_Dreieck();
            }
            else if (currentItemName == "it_KeisProfil")
            {
                rechne_Kreis();
            }
            else if (currentItemName == "it_KreisringProfil")
            {
                rechne_Kreisring();
            }

        }

        //Berechne Volumen ist für alle Profile gleich
        private void rechne_VolumentUndMasse()
        {

            double vol;
            vol = Convert.ToDouble(tb_Flaeche.Text) * Convert.ToDouble(tb_Laenge.Text);
            tb_Volumen.Text = Convert.ToString(Math.Round(vol,6));

            double mas;
            mas = Convert.ToDouble(tb_Volumen.Text) * Convert.ToDouble(tb_Dichte.Text);
            tb_Masse.Text = Convert.ToString(Math.Round(mas, 6));

            double pre;
            pre = Convert.ToDouble(tb_Masse.Text) * Convert.ToDouble(tb_KgPreis.Text);
            tb_Preis.Text = Convert.ToString(Math.Round(pre, 6));
        }

        //BERECHNE RECHTECK
      private void rechne_Rechteck()
        {
            double b = Convert.ToDouble(tb_1.Text);
            double h = Convert.ToDouble(tb_2.Text);
            
            

            //FLÄCHE
            tb_Flaeche.Text = Convert.ToString(Math.Round(
                b*h,
                6));

            //VOLUMEN und MASSE
            rechne_VolumentUndMasse();

            //TRÄGHEIT
            double Iy, Iz;
            Iy = b * Math.Pow(h,3) / 12;
            Iz = h * Math.Pow(b, 3) / 12;

            tb_out1.Text = Convert.ToString(Math.Round(Iy,6));
            tb_out2.Text = Convert.ToString(Math.Round(Iz, 6));

            //SCHWERPUNKT
            tb_out3.Text = "(0; 0)";
        }

        //BERECHNE  I-PROFIL
        private void rechne_IProfil()
        {
            double H = Convert.ToDouble(tb_1.Text);
            double h = Convert.ToDouble(tb_2.Text);
            double B = Convert.ToDouble(tb_3.Text);
            double b = Convert.ToDouble(tb_4.Text);
            

            if (B < b)
            {
                MessageBoxResult msgAbfrage = MessageBox.Show("B muss größer als b sein!");
                return;
            }
            else if (H < h)
            {
                MessageBoxResult msgAbfrage = MessageBox.Show("H muss größer als h sein!");
                return;
            }

            //FLÄCHE
            tb_Flaeche.Text = Convert.ToString(Math.Round(
                B*H-h*(B-b),
                6));

            //VOLUMEN und MASSE
            rechne_VolumentUndMasse();

            //TRÄGHEIT
            double Iy;
            Iy = (B * Math.Pow(H, 3) - (B-b)* Math.Pow(h, 3)) / 12;
            
            tb_out1.Text = Convert.ToString(Math.Round(Iy, 6));

            //SCHWERPUNKT
            tb_out3.Text = "(0; 0)";

        }

        //BERECHNE  T-PROFIL
        private void rechne_TProfil()
        {
            double H = Convert.ToDouble(tb_1.Text);
            double h = Convert.ToDouble(tb_2.Text);
            double B = Convert.ToDouble(tb_3.Text);
            double b = Convert.ToDouble(tb_4.Text);
            
            if (B < b)
            {
                MessageBoxResult msgAbfrage = MessageBox.Show("B muss größer als b sein!");
                return;
            }
            else if (H < h)
            {
                MessageBoxResult msgAbfrage = MessageBox.Show("H muss größer als h sein!");
                return;
            }

            //FLÄCHE
            tb_Flaeche.Text = Convert.ToString(Math.Round(
                B*(H-h)+h*b,
                6));
            //VOLUMEN und MASSE
            rechne_VolumentUndMasse();
            //TRÄGHEIT
            double Iy;
            Iy = (B * Math.Pow(H, 3) + (B-b) * Math.Pow(h, 3)) / 12;
            
            tb_out1.Text = Convert.ToString(Math.Round(Iy, 6));

            //SCHWERPUNKT
            tb_out3.Text = "(0; " + Math.Round(
                        (b*(H-h)*h/2+B*h*(h-H)/2)/(b*(H-h)+B*h)
                , 4)+")";

        }

        //BERECHNE DREIECK
        private void rechne_Dreieck()
        {
            double h = Convert.ToDouble(tb_1.Text);
            double b = Convert.ToDouble(tb_2.Text);

            


            //FLÄCHE
            tb_Flaeche.Text = Convert.ToString(Math.Round(
                (h*b)/2,
                6));
            //VOLUMEN und MASSE
            rechne_VolumentUndMasse();
            //TRÄGHEIT
            double Iy; double Iz;
            Iy = (b*Math.Pow(h,3))/36;
            Iz = (h * Math.Pow(b, 3)) / 48;

            tb_out1.Text = Convert.ToString(Math.Round(Iy, 6));
            tb_out2.Text = Convert.ToString(Math.Round(Iz, 6));

            //SCHWERPUNKT
            tb_out3.Text = "(0; "+Math.Round( -(h/2-h/3) ,4) + ")";
        }

        //BERECHNE KREIS
        private void rechne_Kreis()
        {
            double d = Convert.ToDouble(tb_1.Text);

            //FLÄCHE
            tb_Flaeche.Text = Convert.ToString(Math.Round(
                Math.PI*Math.Pow(d,2)/4,
                6));
            //VOLUMEN und MASSE

            rechne_VolumentUndMasse();

            //TRÄGHEIT
            double I;
            I = (Math.PI*Math.Pow(d,4)) / 64;

            tb_out1.Text = Convert.ToString(Math.Round(I, 6));

            //SCHWERPUNKT
            tb_out3.Text = "(0; 0)";

        }

        //BERECHNE KREISRING
        private void rechne_Kreisring()
        {
            double D = Convert.ToDouble(tb_1.Text);
            double d = Convert.ToDouble(tb_2.Text);
            
            if (D < d)
            {
                MessageBoxResult msgAbfrage = MessageBox.Show("D muss größer als d sein!");
                return;
            }
          

            //FLÄCHE
            tb_Flaeche.Text = Convert.ToString(Math.Round(
                Math.PI * (Math.Pow(D, 2)- Math.Pow(d, 2)) / 4,
                6));

            //VOLUMEN und MASSE
            rechne_VolumentUndMasse();

            //TRÄGHEIT
            double I;
            I = (Math.PI * (Math.Pow(D, 4)- Math.Pow(d, 4))) / 64;

            tb_out1.Text = Convert.ToString(Math.Round(I, 6));

            //SCHWERPUNKT
            tb_out3.Text = "(0; 0)";

        }


        //-----------------------------------


        //Einlesen der Zahlen
        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }


        private void testIfNumeric(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)e.Source;
            string neu = "";
            bool punktVorhanden = false;

            for (int i = 0; i < tb.Text.Length; i++)
            {
                char c = tb.Text[i];
                if (char.IsDigit(c) || (c == '-' && i == 0))
                {
                    neu += c;
                }
                else if (c == ',' && !punktVorhanden)
                {
                    neu += c;
                    punktVorhanden = true;
                }
            }

            tb.Text = neu;
        }
            //Zeige dem Profil entsprechendes Bild
            private void zeigeBild(string location)
        {
            img_Image.Source = new BitmapImage(new Uri(location, UriKind.Relative));
            img_Image.Visibility = Visibility.Visible;
        }

        //-----------------------------------

        //WILLKOMMEN
        private void it_Willkommen_Selected(object sender, RoutedEventArgs e)
        {
            lb_Willkommen.Visibility = Visibility.Visible;

            img_Image.Visibility = Visibility.Hidden;
            profilName.Visibility = Visibility.Hidden;

            label1.Visibility = Visibility.Hidden;
            tb_1.Visibility = Visibility.Hidden;
            label2.Visibility = Visibility.Hidden;
            tb_2.Visibility = Visibility.Hidden;
            label3.Visibility = Visibility.Hidden;
            tb_3.Visibility = Visibility.Hidden;
            label4.Visibility = Visibility.Hidden;
            tb_4.Visibility = Visibility.Hidden;

            lb_Dichte.Visibility = Visibility.Hidden;
            tb_Dichte.Visibility = Visibility.Hidden;

            lb_Laenge.Visibility = Visibility.Hidden;
            tb_Laenge.Visibility = Visibility.Hidden;

            lb_Masse.Visibility = Visibility.Hidden;
            tb_Masse.Visibility = Visibility.Hidden;

            btn_Rechne.IsEnabled = false;

            lb_Flaeche.Visibility = Visibility.Hidden;
            tb_Flaeche.Visibility = Visibility.Hidden;
            lb_Volumen.Visibility = Visibility.Hidden;
            tb_Volumen.Visibility = Visibility.Hidden;

            lb_out1.Visibility = Visibility.Hidden;
            lb_out2.Visibility = Visibility.Hidden;
            lb_out3.Visibility = Visibility.Hidden;
            lb_Preis.Visibility = Visibility.Hidden;
            lb_KgPreis.Visibility = Visibility.Hidden;

            tb_out1.Visibility = Visibility.Hidden;
            tb_out2.Visibility = Visibility.Hidden;
            tb_out3.Visibility = Visibility.Hidden;
            tb_Preis.Visibility = Visibility.Hidden;
            tb_KgPreis.Visibility = Visibility.Hidden;
        }


        //EIN PROFIL WURDE AUSGEWÄHLT
        private void it_Profil_Selected(object sender, RoutedEventArgs e)
        {
            lb_Willkommen.Visibility = Visibility.Hidden;

            lb_Dichte.Visibility = Visibility.Visible;
            tb_Dichte.Visibility = Visibility.Visible;
            lb_Dichte.Content = "Dichte [kg/mm^3]";


            lb_Laenge.Visibility = Visibility.Visible;
            tb_Laenge.Visibility = Visibility.Visible;
            lb_Laenge.Content = "Länge [mm]";

            btn_Rechne.IsEnabled = true;

            lb_Volumen.Content = "Volumen [mm^3]";
            lb_Flaeche.Content = "Flaeche [mm^2]";
            lb_Masse.Content = "Masse [kg]";

            lb_Flaeche.Visibility = Visibility.Visible;
            tb_Flaeche.Visibility = Visibility.Visible;

            lb_Volumen.Visibility = Visibility.Visible;
            tb_Volumen.Visibility = Visibility.Visible;

            lb_Masse.Visibility = Visibility.Visible;
            tb_Masse.Visibility = Visibility.Visible;

            lb_out3.Visibility = Visibility.Visible;
            tb_out3.Visibility = Visibility.Visible;

            lb_Preis.Content = "Profil Preis [€]";
            lb_Preis.Visibility = Visibility.Visible;
            tb_Preis.Visibility = Visibility.Visible;

            lb_KgPreis.Content = "Materialpreis [€/kg]";
            lb_KgPreis.Visibility = Visibility.Visible;
            tb_KgPreis.Visibility = Visibility.Visible;


            tb_1.Text = ""; tb_2.Text = ""; tb_3.Text = ""; tb_4.Text = "";
            tb_Flaeche.Text = ""; tb_Masse.Text = "";tb_Volumen.Text = ""; tb_out1.Text = ""; tb_out2.Text = ""; tb_out3.Text= "";

        }

        //RECHTECKT
        private void it_Rechteckprofil_Selected(object sender, RoutedEventArgs e)
        {
            profilName.Content = it_Rechteckprofil.Header;
            profilName.Visibility = Visibility.Visible;

            zeigeBild("Resources/img_Rechteck.jpg");

            label1.Visibility = Visibility.Visible;
            label1.Content = "b [mm]";
            tb_1.Visibility = Visibility.Visible;

            label2.Visibility = Visibility.Visible;
            label2.Content = "h [mm]";
            tb_2.Visibility = Visibility.Visible;

            lb_out1.Content = "Iy [mm^4]";
            lb_out2.Content = "Iz [mm^4]";

            lb_out1.Visibility = Visibility.Visible;
            lb_out2.Visibility = Visibility.Visible;

            tb_out1.Visibility = Visibility.Visible;
            tb_out2.Visibility = Visibility.Visible;

            label3.Visibility = Visibility.Hidden;
            tb_3.Visibility = Visibility.Hidden;

            label4.Visibility = Visibility.Hidden;
            tb_4.Visibility = Visibility.Hidden;

            lb_Preis.Content = "Profil Preis [€]";
            lb_Preis.Visibility = Visibility.Visible;
            tb_Preis.Visibility = Visibility.Visible;

            lb_KgPreis.Content = "Materialpreis [€/kg]";
            lb_KgPreis.Visibility = Visibility.Visible;
            tb_KgPreis.Visibility = Visibility.Visible;

        }

        //I-Profil
        private void it_IProfil_Selected(object sender, RoutedEventArgs e)
        {
            profilName.Content =  it_IProfil.Header;
            profilName.Visibility = Visibility.Visible;

            zeigeBild("Resources/img_IProfil.jpg");

            lb_out1.Visibility = Visibility.Visible;
            tb_out1.Visibility = Visibility.Visible;

            lb_out1.Content = "Iy [mm^4]";
            lb_out2.Content = "";

            lb_out2.Visibility = Visibility.Hidden;
            tb_out2.Visibility = Visibility.Hidden;

            label1.Visibility = Visibility.Visible;
            label1.Content = "H [mm]";
            tb_1.Visibility = Visibility.Visible;

            label2.Visibility = Visibility.Visible;
            label2.Content = "h [mm]";
            tb_2.Visibility = Visibility.Visible;

            label3.Visibility = Visibility.Visible;
            label3.Content = "B [mm]";
            tb_3.Visibility = Visibility.Visible;

            label4.Visibility = Visibility.Visible;
            label4.Content = "b [mm]";
            tb_4.Visibility = Visibility.Visible;

            lb_Preis.Content = "Profil Preis [€]";
            lb_Preis.Visibility = Visibility.Visible;
            tb_Preis.Visibility = Visibility.Visible;

            lb_KgPreis.Content = "Materialpreis [€/kg]";
            lb_KgPreis.Visibility = Visibility.Visible;
            tb_KgPreis.Visibility = Visibility.Visible;

        }

        //T-Profil
        private void it_TProfil_Selected(object sender, RoutedEventArgs e)
        {
            profilName.Content = it_TProfil.Header;
            profilName.Visibility = Visibility.Visible;

            zeigeBild("Resources/img_TProfil.jpg");

            lb_out1.Visibility = Visibility.Visible;
            tb_out1.Visibility = Visibility.Visible;

            lb_out1.Content = "Iy [mm^4]";
            lb_out2.Content = "";

            lb_out2.Visibility = Visibility.Hidden;
            tb_out2.Visibility = Visibility.Hidden;

            label1.Visibility = Visibility.Visible;
            label1.Content = "H [mm]";
            tb_1.Visibility = Visibility.Visible;

            label2.Visibility = Visibility.Visible;
            label2.Content = "h [mm]";
            tb_2.Visibility = Visibility.Visible;

            label3.Visibility = Visibility.Visible;
            label3.Content = "B [mm]";
            tb_3.Visibility = Visibility.Visible;

            label4.Visibility = Visibility.Visible;
            label4.Content = "b [mm]";
            tb_4.Visibility = Visibility.Visible;

            lb_Preis.Content = "Profil Preis [€]";
            lb_Preis.Visibility = Visibility.Visible;
            tb_Preis.Visibility = Visibility.Visible;

            lb_KgPreis.Content = "Materialpreis [€/kg]";
            lb_KgPreis.Visibility = Visibility.Visible;
            tb_KgPreis.Visibility = Visibility.Visible;

        }

        //Kreis
        private void it_Kreis_Selected(object sender, RoutedEventArgs e)
        {
            profilName.Content = it_KeisProfil.Header;
            profilName.Visibility = Visibility.Visible;

            zeigeBild("Resources/img_Kreis.jpg");

            lb_out1.Visibility = Visibility.Visible;
            tb_out1.Visibility = Visibility.Visible;

            lb_out1.Content = "I [mm^4]";
            lb_out2.Content = "";

            lb_out2.Visibility = Visibility.Hidden;
            tb_out2.Visibility = Visibility.Hidden;

            label1.Visibility = Visibility.Visible;
            label1.Content = "d [mm]";
            tb_1.Visibility = Visibility.Visible;

            label2.Visibility = Visibility.Hidden;
            label2.Content = "";
            tb_2.Visibility = Visibility.Hidden;

            label3.Visibility = Visibility.Hidden;
            label3.Content = "";
            tb_3.Visibility = Visibility.Hidden;

            label4.Visibility = Visibility.Hidden;
            label4.Content = "";
            tb_4.Visibility = Visibility.Hidden;

            lb_Preis.Content = "Profil Preis [€]";
            lb_Preis.Visibility = Visibility.Visible;
            tb_Preis.Visibility = Visibility.Visible;

            lb_KgPreis.Content = "Materialpreis [€/kg]";
            lb_KgPreis.Visibility = Visibility.Visible;
            tb_KgPreis.Visibility = Visibility.Visible;

        }

        //Kreis-Ring
        private void it_Kreisring_Selected(object sender, RoutedEventArgs e)
        {
            profilName.Content = it_KreisringProfil.Header;
            profilName.Visibility = Visibility.Visible;

            zeigeBild("Resources/img_Kreisring.jpg");

          
            lb_out1.Visibility = Visibility.Visible;
            tb_out1.Visibility = Visibility.Visible;
            
            lb_out1.Content = "I [mm^4]";
            lb_out2.Content = "";

            lb_out2.Visibility = Visibility.Hidden;
            tb_out2.Visibility = Visibility.Hidden;

            label1.Visibility = Visibility.Visible;
            label1.Content = "D [mm]";
            tb_1.Visibility = Visibility.Visible;

            label2.Visibility = Visibility.Visible;
            label2.Content = "d [mm]";
            tb_2.Visibility = Visibility.Visible;

            label3.Visibility = Visibility.Hidden;
            label3.Content = "";
            tb_3.Visibility = Visibility.Hidden;

            label4.Visibility = Visibility.Hidden;
            label4.Content = "";
            tb_4.Visibility = Visibility.Hidden;

            lb_Preis.Content = "Profil Preis [€]";
            lb_Preis.Visibility = Visibility.Visible;
            tb_Preis.Visibility = Visibility.Visible;

            lb_KgPreis.Content = "Materialpreis [€/kg]";
            lb_KgPreis.Visibility = Visibility.Visible;
            tb_KgPreis.Visibility = Visibility.Visible;

        }

        //Dreieck
        private void it_Dreieck_Selected(object sender, RoutedEventArgs e)
        {
            profilName.Content = it_Dreieck.Header;
            profilName.Visibility = Visibility.Visible;

            zeigeBild("Resources/img_Dreieck.jpg");

            lb_out1.Content = "Iy [mm^4]";
            lb_out2.Content = "Iz [mm^4]";

            lb_out1.Visibility = Visibility.Visible;
            tb_out1.Visibility = Visibility.Visible;

            lb_out2.Visibility = Visibility.Visible;
            tb_out2.Visibility = Visibility.Visible;

            label1.Visibility = Visibility.Visible;
            label1.Content = "h [mm]";
            tb_1.Visibility = Visibility.Visible;

            label2.Visibility = Visibility.Visible;
            label2.Content = "b [mm]";
            tb_2.Visibility = Visibility.Visible;

            label3.Visibility = Visibility.Hidden;
            label3.Content = "";
            tb_3.Visibility = Visibility.Hidden;

            label4.Visibility = Visibility.Hidden;
            label4.Content = "";
            tb_4.Visibility = Visibility.Hidden;

            lb_Preis.Content = "Profil Preis [€]";
            lb_Preis.Visibility = Visibility.Visible;
            tb_Preis.Visibility = Visibility.Visible;

            lb_KgPreis.Content = "Materialpreis [€/kg]";
            lb_KgPreis.Visibility = Visibility.Visible;
            tb_KgPreis.Visibility = Visibility.Visible;


        }


    }
}
