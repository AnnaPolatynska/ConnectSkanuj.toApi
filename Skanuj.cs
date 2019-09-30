using System;
using System.Collections.Generic;
using System.Text;

namespace nsSkanuj
{

    public partial class StronaPDF
    {
        private string NazwaDocField;
        private int Doc_idField;
        private int Start_pageField;
        private int End_pageField;
        private int PagesField;
        private int Parent_doc_idField;

        public string NazwaDoc
        {
            get { return NazwaDocField; }
            set { NazwaDocField = value; }
        }
        public int Doc_id
        {
            get { return Doc_idField; }
            set { Doc_idField = value; }
        }
        public int Start_page
        {
            get { return Start_pageField; }
            set { Start_pageField = value; }
        }
        public int End_page
        {
            get { return End_pageField; }
            set { End_pageField = value; }
        }
        public int Pages
        {
            get { return PagesField; }
            set { PagesField = value; }
        }
        public int Parent_doc_id
        {
            get { return Parent_doc_idField; }
            set { Parent_doc_idField = value; }
        }

        public StronaPDF(string NazwaDocField, int Doc_idField, int Start_pageField, int End_pageField, int PagesField, int Parent_doc_idField)
        {
            this.NazwaDoc = NazwaDocField;
            this.Doc_id = Doc_idField;
            this.Start_page = Start_pageField;
            this.End_page = End_pageField;
            this.Pages = PagesField;
            this.Parent_doc_id = Parent_doc_idField;
        }
        public StronaPDF()
        {
        }
    }
    public class Token
    {
        public string token { get; set; }
        public string client_id { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public bool first_run { get; set; }
    }//Token

    public class SkApiResponse
    {
        public string msg { get; set; }
        public int code { get; set; }
    }//class SkApiResponse

    public class DocumentList
    {
        public int id { get; set; }
        public string type_dict { get; set; }
        public string DataWystawienia { get; set; }
        public string NrFaktury { get; set; }
        public string added_by_name { get; set; }
        public string verified_by_name { get; set; }
        public double RazemBrutto { get; set; }
        public string cmp_name { get; set; }
        public string registry { get; set; }
        public string file_path_hq { get; set; }
        public string contractor_name { get; set; }
        public string verified_at { get; set; }
        public string last_modified { get; set; }
    }//DocumentList

    public class CompanyList
    {
        public int id { get; set; }
        public string name { get; set; }
        public int nip { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string post_code { get; set; }
        public string address { get; set; }
        public string dbname { get; set; }//nazwa bazy danych firmy / id w systemie zewnętrznym.
    }//CompanyList
    public class DocumentOneXt
    {
        public int id { get; set; }
        public int status { get; set; }
        public int type { get; set; }
        public string added_by_name { get; set; }
        public string verified_by_name { get; set; }
        public string hash { get; set; }
        public string type_dict { get; set; }
        public string file_path { get; set; }
        public string file_path_hq { get; set; }
        public int user_id { get; set; }
        public string comments { get; set; }
        public Contractor contractor { get; set; }
        public OneXtAttributes attributes { get; set; }
        public List<Position> positions { get; set; }
        public string series_name { get; set; }
        public Company company { get; set; }
        public IntegrationData user_integration { get; set; }
    }//class DocumentOneXt

    // do wykorzystania pozostałe struktury
    public class Contractor
    {
        public int id { get; set; }
        public string akronim { get; set; }
        public string name { get; set; }
        public string nip { get; set; }
        public string address { get; set; }
        public string post_code { get; set; }
        public string city { get; set; }
        public int incidental { get; set; }
        public string country { get; set; }
        public int nip_type { get; set; }
        public string country_code { get; set; }
    }//class Contractor

    public class OneXtAttributes
    {
        public OneXtAttribute NrFaktury { get; set; }
        public OneXtAttribute SprzedawcaNazwa { get; set; }
        public OneXtAttribute SprzedawcaAdres { get; set; }
        public OneXtAttribute SprzedawcaNip { get; set; }
        public OneXtAttribute SprzedawcaKod { get; set; }
        public OneXtAttribute SprzedawcaMiejscowosc { get; set; }
        public OneXtAttribute NabywcaNazwa { get; set; }
        public OneXtAttribute NabywcaAdres { get; set; }
        public OneXtAttribute NabywcaNip { get; set; }
        public OneXtAttribute NabywcaKod { get; set; }
        public OneXtAttribute NabywcaMiejscowosc { get; set; }
        public OneXtAttribute DataSprzedazy { get; set; }
        public OneXtAttribute DataWystawienia { get; set; }
        public OneXtAttribute TerminPlatnosci { get; set; }
        public OneXtAttribute SposobPlatnosci { get; set; }
        public OneXtAttribute Waluta { get; set; }
        public OneXtAttribute RazemNetto { get; set; }
        public OneXtAttribute RazemVAT { get; set; }
        public OneXtAttribute RazemBrutto { get; set; }
        public OneXtAttribute MiesiacKsiegowy { get; set; }
        public OneXtAttribute KontoBankowe { get; set; }
        public OneXtAttribute Zaplacono { get; set; }
        public OneXtAttribute ZaplaconoKwota { get; set; }
        public OneXtAttribute KursWaluty { get; set; }
        public OneXtAttribute DataWplywu { get; set; }
        public OneXtAttribute CzyNieKompletnaPozycja { get; set; }
        public OneXtAttribute NettoWalutaPodstawowa { get; set; }
        public OneXtAttribute BruttoWalutaPodstawowa { get; set; }
        public OneXtAttribute VatWalutaPodstawowa { get; set; }
        public OneXtAttribute FakturaKorygowana { get; set; }
        public OneXtAttribute Korygujaca { get; set; }
        public OneXtAttribute WewnatrzWsp { get; set; }
        public OneXtAttribute WystawionaBrutto { get; set; }
        public OneXtAttribute Portfel { get; set; }
        public string Kategoria { get; set; }
        public string KategoriaId { get; set; }
        public string KategoriaOpis { get; set; }
        public string CategoryDesc { get; set; }
        public string Registry { get; set; }
        public int RegistryType { get; set; }
    }// class OneXtAttributes

    public class OneXtAttribute
    {
        public int attribute_id { get; set; }
        public string value { get; set; }
    }//class OneXtAttribute

    public class Position
    {
        public string Nazwa { get; set; }

        private double ilosc;
        public double Ilosc { get { return ilosc; } set { ilosc = Convert.ToDouble(value); } }

        private double cena;
        public double Cena { get { return cena; } set { cena = Convert.ToDouble(value); } }

        private int stawkavat;
        public int StawkaVAT { get { return stawkavat; } set { stawkavat = Convert.ToInt32(value); } }

        public string Jednostka { get; set; }

        private double netto;
        public double Netto { get { return netto; } set { netto = Convert.ToDouble(value); } }

        private double vat;
        public double VAT { get { return vat; } set { vat = Convert.ToDouble(value); } }

        private double brutto;
        public double Brutto { get { return brutto; } set { brutto = Convert.ToDouble(value); } }

        public string Category { get; set; }
        public string Kategoria { get; set; }
        public string KategoriaId { get; set; }

        public string Category2 { get; set; }
        public string Kategoria2 { get; set; }
        public string KategoriaId2 { get; set; }

        public string acc_netto { get; set; }
        public string acc_gross { get; set; }
        public string acc_vat { get; set; }

        public string Category2APNet { get; set; }
        public string Category2APGross { get; set; }
        public string Category2APVAT { get; set; }

        public string accounting_kpir_column { get; set; }
        public string accounting_tax { get; set; }
        public string accounting_vat { get; set; }

        public string Code { get; set; }
        public string product_code { get; set; }
        public string ext_id { get; set; }
        public string IdProduct { get; set; }

        private double cenaZakupu;
        public double CenaZakupu { get { return cenaZakupu; } set { cenaZakupu = Convert.ToDouble(value); } }

        private double cenaSprzedazy;
        public double CenaSprzedazy { get { return cenaSprzedazy; } set { cenaSprzedazy = Convert.ToDouble(value); } }

        private double cenaBrutto;
        public double CenaBrutto { get { return cenaBrutto; } set { cenaBrutto = Convert.ToDouble(value); } }
        //public string IdProductField { get; set; }

        private string kategoriaOdliczenieVAT;
        public string KategoriaOdliczenieVAT { get { return kategoriaOdliczenieVAT; } set { kategoriaOdliczenieVAT = value; } }

        public string CategoryDesc { get; set; }
    }// class Position

    public class Company
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nip { get; set; }
        public string dir_name { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string post_code { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string dbname { get; set; }
        public int cmp_accounting_type { get; set; }
        //public string guid { get; set; } 
    }//class Company

    public class IntegrationData
    {
        public string company { get; set; }
        public string identstr { get; set; }
        public int program_type { get; set; }
    }//class IntegrationData

}
