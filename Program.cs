using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
namespace TEST_Skanuj_to
{

    class Program
    {
        static string _fileName = "Test.pdf";
        static string _path = "C:/Users/polann/Desktop/pliki_do_konectora/Test.pdf";
        static bool _multi = false; //multipages True - powoduje analizę rozbicia dokumentów.Domyślnie false.

        public static string _tokenS; //token połączenia
        public static int _idCompany; // id firmy do pobrania
        public static int _idDocument; //id dokumentu do pobrania

        static void Main(string[] args)
        {
            Program program = new Program();
            program.GetToken("w.radzikowski@krgroup.pl", "5cfa2f8d51092");
            _tokenS = program.GetToken("w.radzikowski@krgroup.pl", "5cfa2f8d51092").token;

            program.getCompany();//Pobranie danych firmy

            program.getId();
            Console.WriteLine("test id" + _idCompany.ToString());

            program.uploadDocument(_idCompany, _fileName, _path, _multi); //wgranie dokumentu.

            //program.uploadDocument1(_tokenS, _path);
            //program.uploadDocument(_companyId, _fileName, _path, _multi);//int company_id, string file_name, string path, bool multi

            //program.uploadDocumentTest(_fileName, _path, _multi);

            // program.GetDocumentById(_idCompany);
        }//Main


        // // // // // // // LOGOWANIE do API  https://app.skanuj.to/api // //
        /// <summary>
        /// LOGOWANIE
        /// </summary>
        /// <param name="email"></param>
        /// <param name="apikey"></param>
        /// <returns></returns>
        public Token GetToken(string email, string apikey)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest("auth", RestSharp.Method.POST);
            request.AddParameter("email", "w.radzikowski@krgroup.pl");
            request.AddParameter("apikey", "5cfa2f8d51092"); //klucz API 5cfa2f8d51092
            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine(content);// odpowiedz

            return Execute<Token>(request);
        }//public Token GetToken(string email, string apikey)

        public class Token
        {
            public string token { get; set; }
            public string client_id { get; set; }
            public string user { get; set; }
            public string pass { get; set; }
            public bool first_run { get; set; }
        }//Token

        public SkApiResponse getCompany()
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest("user", RestSharp.Method.POST);
            //var request = new RestRequest(Method.POST);
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "get-user-company", ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine(content);// odpowiedz

            return Execute<SkApiResponse>(request);
        }//getCompany()



        public SkApiResponse getId()
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest("user", RestSharp.Method.POST);
            //var request = new RestRequest(Method.POST);
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "get-user-company", ParameterType.GetOrPost);

            //[{"id":7085933,"name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA","nip":"1132602742","dir_name":"KR GROUP SPOLKA Z OGRANICZONA ODPOWIEDZIALNOSCIA SPOLKA KOMANDYTOWA . 7085933.1132602742","dbname":""}]
            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            Console.WriteLine("_idCompany -> " + data.id);
            _idCompany = data.id;
            request.AddQueryParameter("id", _idCompany.ToString());

            return Execute<SkApiResponse>(request);
        }// getId()

        public List<CompanyList> GetCompanyLists(int company_id)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest("user", RestSharp.Method.POST);

            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "add-companies", ParameterType.GetOrPost);

            //if (name != "Wybierz")
            //{
            //    request.AddParameter("name", name, ParameterType.GetOrPost);
            //}
            request.AddParameter("company_id", company_id, ParameterType.HttpHeader);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine(content);// odpowiedz

            return Execute<List<CompanyList>>(request);

        }//AddCompany()

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

        /// <summary>
        /// Wgranie dokumentu.
        /// </summary>
        /// <param name="company_id"></param>
        /// <param name="file_name"></param>
        /// <param name="path"></param>
        /// <param name="multi"></param>
        /// <returns></returns>
        public SkApiResponse uploadDocument(int company_id, string file_name, string path, bool multi)
        {
            var client = new RestClient("http://app.skanuj.to/api");

            var request = new RestRequest(Method.POST);
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("id", _idCompany.ToString());
            request.AddParameter("mode", "upload-file", ParameterType.GetOrPost);
            request.AddParameter("company_id", company_id, ParameterType.HttpHeader);
            request.AddParameter("multipages", multi, ParameterType.GetOrPost);
            request.AddParameter("source", "integracja", ParameterType.GetOrPost);
            request.AddFile(file_name, path);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine(content);// odpowiedz

            return Execute<SkApiResponse>(request);

        }//SkApiResponse

        public class SkApiResponse
        {
            public string msg { get; set; }
            public int code { get; set; }
        }//class SkApiResponse


        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            Uri siteUri = new Uri(" https://app.skanuj.to/api ");
            client.BaseUrl = siteUri;
            if (!string.IsNullOrEmpty("token"))
            {
                request.AddParameter("token", _tokenS, ParameterType.HttpHeader); //w każdym zapytaniu
            }
            if (!string.IsNullOrEmpty("id"))
            {
                request.AddParameter("id", _idCompany, ParameterType.HttpHeader); //w każdym zapytaniu
            }
            var response = client.Execute<T>(request);
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
            return response.Data;
        }//T Execute<T>(RestRequest request) where T : new()


        //public T Execute<T>(RestRequest request) where T : new()
        //{
        //    var client = new RestClient();
        //    request.Timeout = 600000;
        //    client.Timeout = 600000;
        //    client.BaseUrl = new Uri(Konektor.Config.Default.urlApi);
        //    if (Konektor.Config.Default.aThis)
        //        client.Authenticator = new HttpBasicAuthenticator(Konektor.Config.Default.aUser, Konektor.Config.Default.aPass);
        //    //Config.Default.token = "Optima";

        //    request.AddParameter("identstr", Konektor.Config.Default.identstr, ParameterType.HttpHeader);
        //    if (!string.IsNullOrEmpty(Konektor.Config.Default.token))
        //    {
        //        request.AddParameter("token", Konektor.Config.Default.token, ParameterType.HttpHeader); // used on every request
        //    }

        //    var response = client.Execute<T>(request);
        //    if (!response.Content.Contains("[[]]"))
        //    {
        //        if (response.ErrorException != null)
        //        {
        //            Logger.LogMessage(response.ErrorException, "Api -> " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + response.ErrorException.Message);
        //            return default(T);
        //        }
        //    }
        //    return response.Data;

        //}

        // // // // // // Funkcja pobierająca dane pojedynczego dokumentu
        public DocumentOneXt GetDocumentById(int id)
        {
            var request = new RestRequest();
            request.Resource = "document";
            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", id, ParameterType.GetOrPost);
            return Execute<DocumentOneXt>(request);
        }// GetDocumentById(int id)




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
            //public string IdProduct { get; set; }

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


    }//Program

}//TEST_Skanuj_to
