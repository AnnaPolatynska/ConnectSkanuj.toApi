using System;
using System.Collections.Generic;
using System.Text;

namespace nsXml
{
    class Pozycja
    {
        private string IdProductField;
        private string ProductcodeField;
        private string NazwaField;
        private string IloscField;
        private string JednostkaField;
        private string CenaField;
        private string BruttoField;
        private string NettoField;
        private string StawkaVATField;
        private string VatField;

        public string IdProduct
        {
            get { return IdProductField; }
            set { IdProductField = value; }
        }
        public string Product_code
        {
            get { return ProductcodeField; }
            set { ProductcodeField = value; }
        }
        public string Nazwa
        {
            get { return NazwaField; }
            set { NazwaField = value; }
        }
        public string Ilosc
        {
            get { return IloscField; }
            set { IloscField = value; }
        }
        public string Jednostka
        {
            get { return JednostkaField; }
            set { JednostkaField = value; }
        }
        public string Cena
        {
            get { return CenaField; }
            set { CenaField = value; }
        }
        public string Brutto
        {
            get { return BruttoField; }
            set { BruttoField = value; }
        }
        public string Netto
        {
            get { return NettoField; }
            set { NettoField = value; }
        }
        public string StawkaVAT
        {
            get { return StawkaVATField; }
            set { StawkaVATField = value; }
        }
        public string Vat
        {
            get { return VatField; }
            set { VatField = value; }
        }

        public Pozycja(string IdProductField, string ProductcodeField, string NazwaField, string IloscField, string JednostkaField, string CenaField, string BruttoField, string NettoField, string StawkaVATField, string VatField)
        {
            this.IdProduct = IdProductField;
            this.Product_code = ProductcodeField;
            this.Nazwa = NazwaField;
            this.Ilosc = IloscField;
            this.Jednostka = JednostkaField;
            this.Cena = CenaField;
            this.Brutto = BruttoField;
            this.Netto = NettoField;
            this.StawkaVAT = StawkaVATField;
            this.Vat = VatField;
        }

    }//class Pozycja

    class Nabywca
    {
        public string NabywcaAdres { get; set; }
        public string NabywcaKod { get; set; }
        public string NabywcaMiejscowosc { get; set; }
        public string NabywcaNazwa { get; set; }
        public string NabywcaNip { get; set; }
        public string NettoWalutaPodstawowa { get; set; }
        public string NrFaktury { get; set; }
        public string NrZamowienia { get; set; }
        public string PrzyczynaKorekty { get; set; }
        public string RazemBrutto { get; set; }
        public string RazemNetto { get; set; }
        public string RazemVAT { get; set; }
        public string SposobPlatnosci { get; set; }
        public string SprzedawcaAdres { get; set; }
        public string SprzedawcaKod { get; set; }
        public string SprzedawcaMiejscowosc { get; set; }
        public string SprzedawcaNazwa { get; set; }
        public string SprzedawcaNip { get; set; }
        public string TerminPlatnosci { get; set; }
        public string VatWalutaPodstawowa { get; set; }
        public string Waluta { get; set; }
        public string Zaplacono { get; set; }
    }
    public partial class Atrybuty
    {
        public string BruttoWalutaPodstawowa { get; set; }
        public string CategoryDesc { get; set; }
        public string CzyNieKompletnaPozycja { get; set; }
        public string DataSprzedazy { get; set; }
        public string DataWplywu { get; set; }
        public string DataWystawienia { get; set; }
        public string FakturaKorygowana { get; set; }
        public string Kategoria { get; set; }
        public string KategoriaId { get; set; }
        public string KontoBankowe { get; set; }
        public string Korygujaca { get; set; }
        public string KursWaluty { get; set; }
        public string MiesiacKsiegowy { get; set; }

        public Atrybuty(string bruttoWalutaPodstawowa, string categoryDesc, string czyNieKompletnaPozycja, string dataSprzedazy, string dataWplywu, string dataWystawienia, string fakturaKorygowana,
            string kategoria, string kategoriaId, string kontoBankowe, string korygujaca, string kursWaluty, string miesiacKsiegowy)
        {
            BruttoWalutaPodstawowa = bruttoWalutaPodstawowa;
            CategoryDesc = categoryDesc;
            CzyNieKompletnaPozycja = czyNieKompletnaPozycja;
            DataSprzedazy = dataSprzedazy;
            DataWplywu = dataWplywu;
            DataWystawienia = dataWystawienia;
            FakturaKorygowana = fakturaKorygowana;
            Kategoria = kategoria;
            KategoriaId = kategoriaId;
            KontoBankowe = kontoBankowe;
            Korygujaca = korygujaca;
            KursWaluty = kursWaluty;
            MiesiacKsiegowy = miesiacKsiegowy;
        }

    }
}
