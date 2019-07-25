using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace TEST_Skanuj_to
{
    class Program
    {
        static string _fileName = "Test.pdf";
        static string _path = "C:/Users/polann/Desktop/pliki_do_konectora/Test.pdf";
        static bool _multi = false; //multipages True - powoduje analizę rozbicia dokumentów.Domyślnie false.

        public static string _tokenS; //token połączenia
        public static int _idCompany; // id firmy do pobrania
        public static int _idDocument; //id dokumentu do pobrania 8185910

        //miesiąc księgowy(obsługiwany format "yyyyy-MM", np. "2016-11")
        static DateTime date = new DateTime(2019, 06, 13);
        public static string _month = date.ToString("yyyy-MM", DateTimeFormatInfo.InvariantInfo);

        static void Main(string[] args)
        {
            Program program = new Program();
            program.GetToken("w.radzikowski@krgroup.pl", "5cfa2f8d51092");
            _tokenS = program.GetToken("w.radzikowski@krgroup.pl", "5cfa2f8d51092").token;

            program.getCompany();//Pobranie danych firmy

            program.getCompanyId();// zwraca _idCompany
            //Console.WriteLine("id test " + _idCompany.ToString()); //id user(kr group) 7085933


            program.uploadDocument(_idCompany, _fileName, _path, _multi); //OK wgranie dokumentu.
            //Console.WriteLine("miesiac księgowy "+_month);

            program.getDocumentId(_idCompany, _fileName, _path, _multi);
           
            

            //TODO getDocumentId();
            // program.GetIdListByCmpID(_idCompany, _month);
            //program.getDocumentId(_idCompany, _month);
            Console.WriteLine(_idDocument);//id dokumentu do pobrania "doc_id":8327757



            //program.GetDocumentById(_idDocument); //OK

            //program.GetCompanyLists(_idCompany);

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

        /// <summary>
        /// Pobiera dane firmy
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Podiera _idCompany
        /// </summary>
        /// <returns></returns>
        public SkApiResponse getCompanyId()
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest("user", RestSharp.Method.POST);
            //var request = new RestRequest(Method.POST);
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "get-user-company", ParameterType.GetOrPost);
            /*[{"id":7085933,
             * "name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA",
             * "nip":"1132602742",
             * "dir_name":"KR GROUP SPOLKA Z OGRANICZONA ODPOWIEDZIALNOSCIA SPOLKA KOMANDYTOWA . 7085933.1132602742",
             * "dbname":""}]*/
            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            Console.WriteLine("_idCompany -> " + data.id);
            _idCompany = data.id;
            request.AddQueryParameter("id", _idCompany.ToString());

            return Execute<SkApiResponse>(request);
        }// getCompanyId()


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

        /// <summary>
        /// Zwraca Id wgranego dokumentu.
        /// </summary>
        /// <param name="company_id"></param>
        /// <param name="file_name"></param>
        /// <param name="path"></param>
        /// <param name="multi"></param>
        /// <returns></returns>
        public SkApiResponse getDocumentId(int company_id, string file_name, string path, bool multi)
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
            var JsonArrayString = content;
            
            dynamic data = JObject.Parse(JsonArrayString);
            _idDocument = (data["good-uploads"][0]["doc_id"]) -1;
            //Console.WriteLine("_idDocument -> " + data["good-uploads"][0]["doc_id"]); //[JSON].good-uploads.[0].doc_id
            Console.WriteLine("_idDocument -> " + _idDocument);
            
            //foreach (var prop in data["good-uploads"])
            //    Console.WriteLine("_idDocument x pętli-> " + prop["doc_id"]);
           
            request.AddQueryParameter("doc_id", _idDocument.ToString());

            return Execute<SkApiResponse>(request);
        }//getDocumentId(int company_id, string file_name, string path, bool multi)


       

        public class Rootobject
        {
            public GoodUploads[] gooduploads { get; set; }
            public object[] baduploads { get; set; }
            public object[] notices { get; set; }
            public string page_path { get; set; }
            public Page_Info page_info { get; set; }
        }

        public class Page_Info
        {
            public string name { get; set; }
            public int type { get; set; }
            public string tmp_name { get; set; }
            public int error { get; set; }
            public string size { get; set; }
            public Options options { get; set; }
            public bool validated { get; set; }
            public bool received { get; set; }
            public bool filtered { get; set; }
            public string[] validators { get; set; }
            public string destination { get; set; }
            public string file_extension { get; set; }
            public string hash { get; set; }
            public int user_id { get; set; }
            public string uploaded_date { get; set; }
            public int uploaded_type { get; set; }
            public string raw_path { get; set; }
            public string notice { get; set; }
            public string message_text { get; set; }
        }

        public class Options
        {
            public bool ignoreNoFile { get; set; }
            public bool useByteString { get; set; }
            public object magicFile { get; set; }
            public bool detectInfos { get; set; }
        }

        public class GoodUploads
        {
            public string name { get; set; }
            public int type { get; set; }
            public string tmp_name { get; set; }
            public int error { get; set; }
            public string size { get; set; }
            public Options1 options { get; set; }
            public bool validated { get; set; }
            public bool received { get; set; }
            public bool filtered { get; set; }
            public string[] validators { get; set; }
            public string destination { get; set; }
            public string file_extension { get; set; }
            public string hash { get; set; }
            public int user_id { get; set; }
            public string uploaded_date { get; set; }
            public int uploaded_type { get; set; }
            public string raw_path { get; set; }
            public string notice { get; set; }
            public string message_text { get; set; }
            public int doc_id { get; set; }
            public int state { get; set; }
        }

        public class Options1
        {
            public bool ignoreNoFile { get; set; }
            public bool useByteString { get; set; }
            public object magicFile { get; set; }
            public bool detectInfos { get; set; }
        }




        /*{"good-uploads":[..., options":{...}, ...,"validators":[...], ... "doc_id":8327957,], "bad-uploads":[],"notices":[], ...,page_info":{...} }*/

        /*{"good-uploads":
         * [{"name":"TEST.PDF","type":-1,"tmp_name":"\/tmp\/phpHX1uIU","error":0,"size":"300578",
         * "options":{"ignoreNoFile":false,"useByteString":true,"magicFile":null,"detectInfos":true},
         * "validated":false,
         * "received":false,
         * "filtered":false,
         * "validators":["Zend_Validate_File_Upload","Zend_Validate_File_Extension"],
         * "destination":"\/var\/www\/skanuj_to-git\/public\/frontend\/user-files\/e70ebc12a0d9ded769a90bcdbbb20c9a\/temp",
         * "file_extension":"PDF",
         * "hash":"1ab160b8eda95316b4c206f736e2f5a5",
         * "user_id":7082762,
         * "uploaded_date":"2019-07-25 09:52:55",
         * "uploaded_type":1,
         * "raw_path":"\/var\/www\/skanuj_to-git\/public\/frontend\/user-files\/e70ebc12a0d9ded769a90bcdbbb20c9a\/1ab160b8eda95316b4c206f736e2f5a5",
         * "notice":"file_already_exists",
         * "message_text":"Taki dokument zosta\u0142 ju\u017c dodany.",
         * "doc_id":8327957,"state":5}],
         * 
         * "bad-uploads":[],"notices":[],"page_path":"\/var\/www\/skanuj_to-git\/public\/frontend\/user-files\/e70ebc12a0d9ded769a90bcdbbb20c9a\/1ab160b8eda95316b4c206f736e2f5a5","page_info":{"name":"TEST.PDF","type":-1,"tmp_name":"\/tmp\/phpHX1uIU","error":0,"size":"300578","options":{"ignoreNoFile":false,"useByteString":true,"magicFile":null,"detectInfos":true},"validated":false,"received":false,"filtered":false,"validators":["Zend_Validate_File_Upload","Zend_Validate_File_Extension"],"destination":"\/var\/www\/skanuj_to-git\/public\/frontend\/user-files\/e70ebc12a0d9ded769a90bcdbbb20c9a\/temp","file_extension":"PDF","hash":"1ab160b8eda95316b4c206f736e2f5a5","user_id":7082762,"uploaded_date":"now()","uploaded_type":1,"raw_path":"\/var\/www\/skanuj_to-git\/public\/frontend\/user-files\/e70ebc12a0d9ded769a90bcdbbb20c9a\/1ab160b8eda95316b4c206f736e2f5a5","notice":"file_already_exists","message_text":"Taki dokument zosta\u0142 ju\u017c dodany."}} */



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


        string _identstrERP;// rodzaj integracji z systemem ERP.Dostępne wartości: optima, symfonia, symfoniamk, rakssql, lefthand, waprofakir, waprokaper, rachmistrzgt, rewizorgt, fakt, dgcs, r2fk, r2k, buchalter, tema, rzeczpospolitaMK, symfoniaForte, edi, impuls, varico,
        int _parametrCountDoc;// count określa liczbę dokumentów pobranych jednorazowo
                              //company_id - id firmy, której dokumenty chcemy pobrać


        //Funkcja pobierająca listę dokumentów dla wszystkich firm:
        //public List<DocumentList> GetDocumentsList(string month, string identstr = "")
        //{
        //    var request = new RestRequest();
        //    request.Resource = "document";
        //    request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);
        //    if (month != "Wybierz")
        //    {
        //        request.AddParameter("month", month, ParameterType.GetOrPost);
        //    }
        //    request.AddParameter("count", 350, ParameterType.GetOrPost);
        //    if (identstr != "")
        //    {
        //        return Execute<List<DocumentList>>(request, identstr);
        //    }
        //    else
        //    {
        //        return Execute<List<DocumentList>>(request);
        //    }
        //}


        //OK pobranie listy dokumentów w obrębie 1 firmy.
        public List<DocumentList> GetDocumentsListByCmpID(int company_id, string month)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);
            if (month != "Wybierz")
            {
                request.AddParameter("month", _month.ToString(), ParameterType.GetOrPost);
            }
            request.AddParameter("company_id", _idCompany, ParameterType.HttpHeader);
            request.AddParameter("count", 350, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine(content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetDocumentsListByCmpID

        /*
        [{"id":8185917,"hash":"cb5c2a1727489e9ef8ce8d94ac1ddc1f","type":2,"verified_at":"2019-07-01 11:54:57.20007","last_modified":"2019-07-01 11:54:57.20007",
            "company_id":7085933,"cmp_name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA",
            "added_by_name":"Wojciech Radzikowski","added_by_email":"w.radzikowski@krgroup.pl","verified_by_name":"Wojciech Radzikowski","verified_by_email":"w.radzikowski@krgroup.pl",
            
            "DataWystawienia":"2019-05-21",
            "NrFaktury":"2564\/MAG\/2019",
            "RazemBrutto":"530.63",
            "registry":null,
            "contractor_name":"GRUPA MARCOVA POLSKA OFFICE SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA",
            "type_dict":"Faktura zakupu",
            "file_path_hq":"https:\/\/app.skanuj.to\/customer\/get-pdf\/hash\/cb5c2a1727489e9ef8ce8d94ac1ddc1f\/id\/8185917"},
        
        {"id":8185903,"hash":"ae0e10fe371373de10201fb4fbc70e3e","type":2,"verified_at":"2019-07-18 08:45:29.175638","last_modified":"2019-07-18 08:45:29.175638",
        "company_id":7085933,"cmp_name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA",
        "added_by_name":"Wojciech Radzikowski","added_by_email":"w.radzikowski@krgroup.pl","verified_by_name":"Wojciech Radzikowski","verified_by_email":"w.radzikowski@krgroup.pl",
        
        "DataWystawienia":"2019-05-20",
        "NrFaktury":"A5\/5\/2019",
        "RazemBrutto":"159.36",
        "registry":null,
        "contractor_name":"TOMASZ DURKA",
        "type_dict":"Faktura zakupu",
        "file_path_hq":"https:\/\/app.skanuj.to\/customer\/get-pdf\/hash\/ae0e10fe371373de10201fb4fbc70e3e\/id\/8185903"},

{"id":8185904,"hash":"af1e9fbb4e30bb0fec69e3d6461bcbb7","type":2,"verified_at":"2019-07-18 11:12:06.931968","last_modified":"2019-07-18 11:12:06.931968",
"company_id":7085933,"cmp_name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA",
"added_by_name":"Wojciech Radzikowski","added_by_email":"w.radzikowski@krgroup.pl","verified_by_name":"Wojciech Radzikowski","verified_by_email":"w.radzikowski@krgroup.pl",

"DataWystawienia":"2019-05-17",
"NrFaktury":"3072019",
"RazemBrutto":"61.5",
"registry":null,
"contractor_name":"MONTA\u017b WYK\u0141ADZIN S\u0141AWOMIR WIELGUS",
"type_dict":"Faktura zakupu","file_path_hq":"https:\/\/app.skanuj.to\/customer\/get-pdf\/hash\/af1e9fbb4e30bb0fec69e3d6461bcbb7\/id\/8185904"},

{"id":8185905,"hash":"ec61b92362fb0222619fa3584196b91b","type":2,"verified_at":"2019-07-01 11:58:53.63739","last_modified":"2019-07-01 12:00:23.582567",
"company_id":7085933,"cmp_name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA","added_by_name":"Wojciech Radzikowski",
"added_by_email":"w.radzikowski@krgroup.pl","verified_by_name":"Wojciech Radzikowski","verified_by_email":"w.radzikowski@krgroup.pl",

"DataWystawienia":"2019-04-26",
"NrFaktury":"1160\/DN\/WAR\/04\/2019",
"RazemBrutto":"1076",
"registry":null,
"contractor_name":"AMSO sp\u00f3\u0142ka cywilna DAWID GRABOWSKI,MATEUSZ KWIECIE\u0143",
"type_dict":"Faktura zakupu","file_path_hq":"https:\/\/app.skanuj.to\/customer\/get-pdf\/hash\/ec61b92362fb0222619fa3584196b91b\/id\/8185905"},

{"id":8185910,"hash":"05c6e4602d8d4ae3c84928dfbd5e3d57","type":2,"verified_at":"2019-07-01 11:50:57.881573","last_modified":"2019-07-01 11:52:59.286936",
"company_id":7085933,"cmp_name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA",
"added_by_name":"Wojciech Radzikowski","added_by_email":"w.radzikowski@krgroup.pl","verified_by_name":"Wojciech Radzikowski","verified_by_email":"w.radzikowski@krgroup.pl",

"DataWystawienia":"2019-05-20","NrFaktury":"3830097","RazemBrutto":"245.61","registry":null,
"contractor_name":"\"LINDSTROM\" SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104",
"type_dict":"Faktura zakupu","file_path_hq":"https:\/\/app.skanuj.to\/customer\/get-pdf\/hash\/05c6e4602d8d4ae3c84928dfbd5e3d57\/id\/8185910"}]

    */

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
        }



        /// <summary>
        /// Pobiera dane firmy
        /// </summary>
        /// <returns></returns>
        public SkApiResponse getDocument()
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



        public List<DocumentList> GetIdListByCmpID(int company_id, string month)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);
            if (month != "Wybierz")
            {
                request.AddParameter("month", _month.ToString(), ParameterType.GetOrPost);
            }
            request.AddParameter("company_id", _idCompany, ParameterType.HttpHeader);
            request.AddParameter("count", 3, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            Console.WriteLine("_idDokument -> " + data.id);
            _idDocument = data.id;
            request.AddQueryParameter("id", _idDocument.ToString());



            Console.WriteLine(content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetDocumentsListByCmpID




        //
        public List<DocumentList> GetIdList(int company_id, string month)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);
            if (month != "Wybierz")
            {
                request.AddParameter("month", _month.ToString(), ParameterType.GetOrPost);
            }
            request.AddParameter("company_id", _idCompany, ParameterType.HttpHeader);
            request.AddParameter("count", 3, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            Console.WriteLine("_idDokument -> " + data.id);
            _idDocument = data.id;
            request.AddQueryParameter("id", _idDocument.ToString());



            Console.WriteLine(content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetDocumentsListByCmpID




        // pobranie id wysłanego dokumentu
        public SkApiResponse getDocumentId(int company_id, string month)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest("document", RestSharp.Method.POST);
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);
            if (month != "Wybierz")
            {
                request.AddParameter("month", _month.ToString(), ParameterType.GetOrPost);
            }
            request.AddParameter("company_id", _idCompany, ParameterType.HttpHeader);
            request.AddParameter("count", 350, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;

            var obj = JObject.Parse(content);


            JObject jObject = JObject.Parse(content);
            Console.WriteLine("id ---> " + jObject["id"]);

            //zwraca 0
            //IRestResponse restResponse = client.Execute(request);
            //var content = restResponse.Content;
            //var JsonArrayString = content;
            ////Newtonsoft.Json.JsonReaderException: „Error reading JArray from JsonReader. Current JsonReader item is not an array: StartObject. Path '', line 1, position 1.”
            //var result = JsonConvert.DeserializeObject<DocumentList>(JsonArrayString);
            //Console.WriteLine("_idDokumentu -> " + result.id);
            //_idDocument = result.id;
            //request.AddQueryParameter("id", _idDocument.ToString());

            return Execute<SkApiResponse>(request);
        }// getCompanyId()


        /*
        public List<DocumentList> GetDocumentsListByCmpID(int company_id, string month)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);
            if (month != "Wybierz")
            {
                request.AddParameter("month", _month.ToString(), ParameterType.GetOrPost);
            }
            request.AddParameter("company_id", _idCompany, ParameterType.HttpHeader);
            request.AddParameter("count", 350, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine(content);// odpowiedz
            return Execute<List<DocumentList>>(request);
        }//GetDocumentsListByCmpID
        */

        //TODO dodawanie firmy
        /// <summary>
        /// Dodanie firmy dla prawidłowego rozpoznania dokumentów.
        /// </summary>
        /// <returns></returns>
        public SkApiResponse addCompanies()
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest("user", RestSharp.Method.POST);
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "add-companies", ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine(content);// odpowiedz

            return Execute<SkApiResponse>(request);
        }//addCompanies()

        string _companyName;

        public List<CompanyList> GetCompanyLists(int company_id)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest("user", RestSharp.Method.POST);

            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "add-companies", ParameterType.GetOrPost);

            if (_companyName != "Wybierz")
            {
                request.AddParameter("name", _companyName, ParameterType.GetOrPost);
            }
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



        // // // // // // Funkcja pobierająca dane pojedynczego dokumentu
        public DocumentOneXt GetDocumentById(int id)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", id, ParameterType.GetOrPost);
            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine(content);// odpowiedz

            return Execute<DocumentOneXt>(request);
        }// GetDocumentById(int id)

        /*{"id":8185910,
         * "user_id":7082762,
         * "uploaded_date":"2019-07-01 11:44:30.868114",
         * "uploaded_type":1,
         * "status":0,
         * "type":2,
         * "category":null,
         * "name":"FAKTURA_3830097.PDF",
         * "hash":"05c6e4602d8d4ae3c84928dfbd5e3d57",
         * "pages":1,
         * "deleted":false,"comments":null,
         * "raw_path":"\/var\/www\/skanuj_to-git\/public\/frontend\/user-files\/e70ebc12a0d9ded769a90bcdbbb20c9a\/05c6e4602d8d4ae3c84928dfbd5e3d57",
         * "state":2,"verified_by":7082762,"last_modified":"2019-07-01 11:52:59.286936","split_for_many_docs":false,
         * "company_id":7085933,"locked_by":null,"registry_id":null,"resend_try_count":0,"worker_error":0,
         * "parent_doc_id":null,"is_parent_doc":false,"block_sending_to_flexi":false,
         * "description":"3830097 TYP TRANSAKCJI: WYNAJEM MBW2-MATA NIEBIESKA 115X200, MBW4-MATA NIEBIESKA 150X300, CENT_DIFFERENCE_1",
         * "verified_at":"2019-07-01 11:50:57.881573","was_resended_to_process":false,"verification_start":null,
         * "verification_stop":"2019-07-01 11:50:57.881573","verification_enter_count":0,"verification_total_idle":0,
         * "series_id":null,"temp_new_type":-1,"erased":false,"current_pdf":true,"sf_path":null,
         * "generate_thumbs":false,
         * "recovery_file":false,
         * "response_url":null,
         * "response_type":null,
         * "retrieved":false,
         * "sf_status":0,
         * "priority":0,
         * "series_name":"",
         * "verified_by_name":"Wojciech Radzikowski",
         * "verified_email":"w.radzikowski@krgroup.pl",
         * "added_by_name":"Wojciech Radzikowski",
         * "added_by_email":"w.radzikowski@krgroup.pl",
         * "type_dict":"Faktura zakupu",
         * "file_path":"https:\/\/app.skanuj.to\/customer\/get-pdf\/hash\/05c6e4602d8d4ae3c84928dfbd5e3d57\/id\/8185910",
         * "file_path_hq":"https:\/\/app.skanuj.to\/customer\/get-pdf\/hash\/05c6e4602d8d4ae3c84928dfbd5e3d57\/id\/8185910",
         *
         * 
         * "contractor":{"id":9969824,"name":"\"LINDSTROM\" SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104","country_code":"PL","nip":"PL5222640524",
         * "address":"ul. Marywilska 34","akronim":"\"LINDSPL52226405WARSZAWA","city":"Warszawa","country":"","post_code":"03-228", "post_code_sane":"03228","extra":"","incidental":0},
         * 
         * 
         * "attributes":{"NrFaktury":{"attribute_id":1,"value":"3830097","is_valid":"1","status":2,"left":"425","top":"318","right":"513","bottom":"335","page":0,"aspect_ratio":"3.24","user_id":7082762},
         * "SprzedawcaNazwa":{"attribute_id":2,"value":"\"LINDSTROM\" SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104","is_valid":"1","status":0,"left":"46","top":"1031","right":"136","bottom":"1040","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SprzedawcaNip":{"attribute_id":3,"value":"PL5222640524","is_valid":"1","status":0,"left":"394","top":"1020","right":"481","bottom":"1027","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SprzedawcaAdres":{"attribute_id":4,"value":"ul. Marywilska 34","is_valid":"1","status":0,"left":"46","top":"1043","right":"123","bottom":"1051","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SprzedawcaKod":{"attribute_id":6,"value":"03-228","is_valid":"1","status":0,"left":"45","top":"1055","right":"82","bottom":"1062","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SprzedawcaMiejscowosc":{"attribute_id":7,"value":"Warszawa","is_valid":"1","status":0,"left":"56","top":"1043","right":"115","bottom":"1051","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaNazwa":{"attribute_id":8,"value":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA","is_valid":"1","status":0,"left":"80","top":"140","right":"192","bottom":"151","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaNip":{"attribute_id":9,"value":"1132602742","is_valid":"1","status":0,"left":"446","top":"83","right":"509","bottom":"91","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaAdres":{"attribute_id":10,"value":"ul. Skaryszewska 7","is_valid":"1","status":0,"left":"57","top":"157","right":"166","bottom":"168","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaKod":{"attribute_id":12,"value":"03-802","is_valid":"1","status":0,"left":"57","top":"175","right":"102","bottom":"184","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaMiejscowosc":{"attribute_id":13,"value":"Warszawa","is_valid":"1","status":0,"left":"57","top":"191","right":"94","bottom":"201","page":0,"aspect_ratio":"3.24","user_id":null},
         * "DataSprzedazy":{"attribute_id":14,"value":"2019-05-20","is_valid":"1","status":0,"left":"686","top":"97","right":"743","bottom":"105","page":0,"aspect_ratio":"3.24","user_id":null},
         * "DataWystawienia":{"attribute_id":15,"value":"2019-05-20","is_valid":"1","status":0,"left":"209","top":"99","right":"272","bottom":"108","page":0,"aspect_ratio":"3.24","user_id":null},
         * "TerminPlatnosci":{"attribute_id":16,"value":"2019-06-03","is_valid":"1","status":0,"left":"686","top":"69","right":"743","bottom":"77","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SposobPlatnosci":{"attribute_id":17,"value":"Przelew","is_valid":"0","status":1,"left":"446","top":"97","right":"487","bottom":"107","page":0,"aspect_ratio":"3.24","user_id":null},
         * "Waluta":{"attribute_id":18,"value":"PLN","is_valid":"1","status":0,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "RazemNetto":{"attribute_id":19,"value":"199.68","is_valid":"1","status":0,"left":"513","top":"530","right":"551","bottom":"539","page":0,"aspect_ratio":"3.24","user_id":null},
         * "RazemVAT":{"attribute_id":20,"value":"45.93","is_valid":"1","status":0,"left":"612","top":"530","right":"644","bottom":"539","page":0,"aspect_ratio":"3.24","user_id":null},
         * "RazemBrutto":{"attribute_id":21,"value":"245.61","is_valid":"1","status":0,"left":"687","top":"530","right":"723","bottom":"539","page":0,"aspect_ratio":"3.24","user_id":null},
         * "MiesiacKsiegowy":{"attribute_id":22,"value":"2019-06","is_valid":"1","status":0,"left":null,"top":null,"right":null,"bottom":null,"page":0,"aspect_ratio":null,"user_id":null},
         * "KontoBankowe":{"attribute_id":24,"value":"PL61236000050000004550302897","is_valid":"1","status":0,"left":"547","top":"1043","right":"731","bottom":"1051","page":0,"aspect_ratio":"3.24","user_id":null},
         * "Zaplacono":{"attribute_id":25,"value":"Brak danych","is_valid":"0","status":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "KursWaluty":{"attribute_id":26,"value":"1.0000","is_valid":"0","status":0,"left":null,"top":null,"right":null,"bottom":null,"page":0,"aspect_ratio":null,"user_id":null},
         * "DataWplywu":{"attribute_id":29,"value":"2019-05-20","is_valid":"1","status":0,"left":null,"top":null,"right":null,"bottom":null,"page":0,"aspect_ratio":null,"user_id":null},
         * "CzyNieKompletnaPozycja":{"attribute_id":80,"value":"false","is_valid":"0","status":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NettoWalutaPodstawowa":{"attribute_id":120,"value":"199.68","is_valid":"0","status":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "BruttoWalutaPodstawowa":{"attribute_id":121,"value":"245.61","is_valid":"0","status":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "VatWalutaPodstawowa":{"attribute_id":122,"value":"45.93","is_valid":"0","status":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NrZamowienia":{"attribute_id":200,"value":"","is_valid":"0","status":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},"Kategoria":"","KategoriaId":"","CategoryDesc":"",
         * "Korygujaca":{"attribute_id":27,"value":"0","is_valid":0,"status":1,"left":-1,"top":-1,"right":-1,"bottom":-1,"page":0,"aspect_ratio":1,"user_id":null},
         * "FakturaKorygowana":{"attribute_id":129,"value":"0","is_valid":0,"status":1,"left":-1,"top":-1,"right":-1,"bottom":-1,"page":0,"aspect_ratio":1,"user_id":null},
         * "PrzyczynaKorekty":{"attribute_id":219,"value":"","is_valid":0,"status":1,"left":-1,"top":-1,"right":-1,"bottom":-1,"page":0,"aspect_ratio":1,"user_id":null}},
         * 
         * "positions":[{"is_valid":"1","status":0,"VAT":17.22,"Code":"MBW2","Kategoria":"","KategoriaId":"","CategoryDesc":"","Kategoria2":"","KategoriaId2":"",
         * "Category2Desc":"","ext_id":"","product_code":"","product_type":"","product_desc":"","Nazwa":"TYP TRANSAKCJI: WYNAJEM MBW2-MATA NIEBIESKA 115X200","Ilosc":1,"Jednostka":"","Cena":74.88,"Netto":74.88,"StawkaVAT":23,"Brutto":92.1,"IdProduct":""},
         * 
         * {"VAT":28.7,"Code":"MBW4","is_valid":"1","status":0,"Kategoria":"","KategoriaId":"","CategoryDesc":"","Kategoria2":"","KategoriaId2":"","Category2Desc":"","ext_id":"","product_code":"","product_type":"","product_desc":"","Nazwa":"MBW4-MATA NIEBIESKA 150X300","Ilosc":1,"Jednostka":"","Cena":124.8,"Netto":124.8,"StawkaVAT":23,"Brutto":153.5,"IdProduct":""},{"is_valid":"1","status":0,"VAT":0.01,"Kategoria":"","KategoriaId":"","CategoryDesc":"","Kategoria2":"","KategoriaId2":"","Category2Desc":"","ext_id":"","product_code":"","product_type":"","product_desc":"","Nazwa":"CENT_DIFFERENCE_1","Ilosc":1,"Jednostka":"","Cena":0,"Netto":0,"StawkaVAT":23,"Brutto":0.01,"IdProduct":""}],
         * 
         * "company":{"cmp_id":7085933,"cmp_creation_time":"2019-06-07 11:34:05.338385","cmp_update_time":"2019-07-24 11:34:58.460944","cmp_remove_time":"1970-01-01 00:00:00","cmp_version":1,"cmp_name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA","cmp_tax_number":"1132602742","cmp_tax_country_code":"","cmp_regon":null,"cmp_phone":"222628110","cmp_fax":null,"cmp_email":null,"cmp_web_page":null,"cmp_default_address_as_string":"ul. Skaryszewska 7, 03-802 Warszawa","cmp_test_account":false,"cmp_small_taxpayer":false,"cmp_vat_payer":true,"cmp_agreement_start_time":null,"cmp_removed":false,"cmp_type":"COMPANY","cmp_acc_office_fkey":null,"cmp_source_domain":"app.skanuj.to","cmp_source_params":"package=ST_MAXI&app=skto","cmp_branding_fkey":1,"cmp_owner_fkey":7082762,"cmp_registration_complete_date":"2019-06-07 12:48:24.650761","cmp_vat_declaration":null,"cmp_has_notverified_docs":true,"cmp_accounting_type":2,"cmp_cash_method":false,"cmp_display_name":"","cmp_migration_date":null,"cmp_provider":"","cmp_source_type":"WEB","cmp_campaign":"","cmp_email_invoice":null,"cmp_extra_information":null,"cmp_external_id":null,"id":7085933,"name":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA","nip":"1132602742","phone":"222628110","country":"PL","city":"Warszawa","post_code":"03-802","address":"ul. Skaryszewska 7","dir_name":"KR GROUP SPOLKA Z OGRANICZONA ODPOWIEDZIALNOSCIA SPOLKA KOMANDYTOWA . 7085933.1132602742","dbname":null},"percent":0,
         * "user_integration":{"user":"","pass":"","company":"","program_type":"","options":"","identstr":""}}*/


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
