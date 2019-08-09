using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using File = System.IO.File;

namespace TEST_Skanuj_to
{

    //connection stringi z appconfig
    //KRG411\SQLEXPRESS
    //<add name="ConnectionSQL" connectionString="Data source=KRG411\SQLEXPRESS;database='skanujTo';User id=KRG\polann;Password=Tykwa13!" providerName="System.Data.SqlClient" />
    //<add name="ConnectionSQL" connectionString="Data source=10.0.1.155;database='skanujTo';User id=SkanujTo;Password=Sk@nu7T0" providerName="System.Data.SqlClient" />
    //<add name="ConnectionSQL" connectionString="Data source=10.0.1.155;database='skanujTo';User id=KRG\polann;Password=Tykwa13!" providerName="System.Data.SqlClient" />
    //<add name="ConnectionSQL" connectionString="Data source=10.0.1.155;database='vat';User id=vat;Password=95f)ek4n9!" providerName="System.Data.SqlClient" />
    class Program
    {

        static string _fileName = System.Configuration.ConfigurationManager.AppSettings["fileName"].ToString();//"Test.pdf";//"Test.pdf";
        static string _path = System.Configuration.ConfigurationManager.AppSettings["path"].ToString() + _fileName; //C:\\Konektora "C:/Users/polann/Desktop/pliki_do_konectora/Test.pdf";
        static bool _multi = false; //multipages True - powoduje analizę rozbicia dokumentów.Domyślnie false.

        public static string _connString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionSQL"].ConnectionString;

        public static int _idDocument; //id dokumentu do pobrania 8185910
        public static int _newIdDocument; // id nowo wgranego dokumentu

        public static string _documentName; //nazwa wgranego dokumentu
        public static string _uploadDate;// Data wgrania dokumentu
        public static string _notice;

        //user
        public static string _tokenS; //token połączenia
        public static int _idUser; // id firmy wgrywającej dane = KRGroup: 7082762
        public static int _nipUserCompany; //Nip kr group
        public static string _nameUserCompany;

        //buyer
        public static string _nipBuyer; //nip nabywcy 
        public static string _nameBuyer; //nazwa nabywcy
        public static int _idBuyer; //id nabywcy
        public static string _adressBuyer; // adres nabywcy

        //Contraktor
        public static string _nipContractor; //nip sprzedawcy
        public static int _idContractor;//id firmy z dokumentu do pobrania
        public static string _nameContractor; //nazwa firmy z dokumentu
        public static string _adressContractor;// adres sprzedawcy

        public static int _statusDoc = 0; //statusExp - statusExp atrybutu (0 - nie wymagający weryfikacji, 1 - wymagający weryfikacji, 2 - zweryfikowany)

        public static int _stateDoc = 3;//Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 

        //miesiąc księgowy(obsługiwany format "yyyyy-MM", np. "2016-11")
        //static DateTime date = new DateTime(2019, 06, 01);
        //public static string _month = date.ToString("yyyy-MM", DateTimeFormatInfo.InvariantInfo);

        //public static string _month1 = DateTime.Now.ToString("yyyy-MM", DateTimeFormatInfo.InvariantInfo); //obecna data

        public static void Main(string[] args)
        {
            Program program = new Program();
            ////Logowanie, uzyskanie tokenu i pobranie danych firmy.
            try
            {
                _tokenS = program.GetToken("w.radzikowski@krgroup.pl", "5cfa2f8d51092").token;//ok pobiera token.
            }
            catch { program.WriteToFile("Problem z uzyskaniem tokenu."); }
            Console.WriteLine(" ");

            try
            {
                program.getUserCompany();//ok Pobranie danych firmy własnej id user(kr group) 7085933
                Console.WriteLine("C _idUser -> " + _idUser.ToString());
                Console.WriteLine("C _nameUserCompany -> " + _nameUserCompany);
                Console.WriteLine("C _NipMyCompany -> " + _nipUserCompany);
                Console.WriteLine(" ");
            }
            catch { program.WriteToFile("Problem z pobraniem firmy " + _idUser + " - " + _nameUserCompany + " (" + DateTime.Now + ")."); }
            Console.WriteLine(" ");

            ////Działanie programu od wgrania dokumentu.
            try
            {
                program.uploadDocument(_idUser, _fileName, _path, _multi); //OK Wgranie dokumentu 8354510

                ZapiszDoBazy();//zapisuje do bazy dane nowego dokumentu do śledzenia.

                Console.WriteLine("C _idDocument -> " + _idDocument.ToString());
                Console.WriteLine("C nazwa wgranego dokumentu ->" + _documentName);
                //Console.WriteLine("data wgrania dokumentu -> " + _uploadDate);
                Console.WriteLine("C info czy ponownie wgrane ->" + _notice);
                _newIdDocument = _idDocument;

                if (_notice == "file_already_exists")
                {
                    Console.WriteLine("Nie da się wgrać pliku o Id " + _idDocument + ". Plik " + _documentName + " został już wcześniej wgrany.");
                    //program.WriteToFile("Nie da się wgrać pliku. Plik " + _documentName + " - Id dokumentu " + _idDocument + " został już wcześniej wgrany.");
                    SprawdzStatus(8401183);
                }
                else
                {
                    Console.WriteLine("Plik o nazwie " + _documentName + " został poprawnie dodany (" + _uploadDate + "). Id dokumentu " + _idDocument + ").");
                    //program.WriteToFile("Plik o nazwie " + _documentName + " został poprawnie dodany (" + _uploadDate + "). Id dokumentu " + _idDocument + ").");
                    _newIdDocument = _idDocument;//id nowo wgranego documentu 

                    //TODO zapis plików do sledzenia do tabeli?
                    // program.GetExportStatusByIdDoc(badanyDoc); // Sprawdza status dokumentu

                    // sprawdza czy dokument ma statusExp - statusExp atrybutu (0 - nie wymagający weryfikacji, 1 - wymagający weryfikacji, 2 - zweryfikowany)
                    //Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 
                    program.GetVeryficationStateByIdDoc(_newIdDocument);

                }
            }
            catch { program.WriteToFile("Problem z wgraniem dokumentu " + _documentName + " - " + _idDocument + " (" + DateTime.Now + ")."); }
            Console.WriteLine(" ");



            SprawdzStatus(8401183); 



            //program.GetDataFromDoc(_documentName);


            //OK sprawdz czy dokument ma statusExp - statusExp atrybutu (0 - nie wymagający weryfikacji, 1 - wymagający weryfikacji, 2 - zweryfikowany)
            //Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 
            int badanyDoc = 8397401;
            Console.WriteLine("Badany doc -> " + badanyDoc);
            program.GetVeryficationStateByIdDoc(badanyDoc);
            Console.WriteLine(" ");
            program.GetDataFromDoc(badanyDoc);

            //_documentName = "3TEST.JPG";
            //program.GetIdDocByName( _documentName);
            //program.GetExportStatusByIdDoc(badanyDoc); // Sprawdza status dokumentu
            //TODO program.SetDocumentExportStatus(1, badanyDoc);

            // program.GetAllDocumentList();//OK sprawdza statusExp dokumentów
            Console.WriteLine(" ");

            //Console.WriteLine(" ");
            //program.GetStateDocumentList(3); // Lista dokumentów gotowych do pobrania
            //Console.WriteLine(" ");

            //program.ListIdToVeryfication(_stateDoc); //TODO lista znajduje wszystko

            //int doc = 8185917;
            //program.GetInfoIfDocumentExist(doc); //ok Lista pobiera dane o stanie wszystkich dokumentów


            // Console.WriteLine(" ");
            //_idUser = 8185903;
            //_documentName = "FV_30.PDF"; //id doc 8185904

            //program.GetIdListByCmpID(_idUser, _documentName); //lista po id company

            //Pobiera dane dokumentu po id
            try
            {
                program.GetDocumentById(badanyDoc); //ok zwraca dane dokumentu o ile dokument jest wgrany 
                Console.WriteLine("_idContractor ->" + _idContractor + " nazwa: " + _nameContractor + " NIP sprzedawcy: " + _nipContractor + " adres " + _adressContractor);
                Console.WriteLine("idBuyer -> " + _idBuyer + " nazwa " + _nameBuyer + " nip " + _nipBuyer + " adres " + _adressBuyer);
                Console.WriteLine("GetDocumentById(" + _idDocument + ")" + " nazwa dokumentu -> " + _documentName);
            }
            catch { Console.WriteLine("Nie można pobrać danych dokumentu " + _idDocument + " nazwa dokumentu-> " + _documentName + "."); }
            Console.WriteLine(" ");

            // program.GetDocumentsListByCmpID(_idUser);
            // Console.WriteLine(" ");
            // program.GetIdList(_idUser);
            // Console.WriteLine(" ");

            // program.GetLastIdDocumentFromList(); //OK pobiera dane z ostatniego wgranego dokumentu z listy
            // Console.WriteLine("nazwa sprzedawcy -> " + _nameContractor + " id " + _idContractor + " NIP " + _nipContractor);
            // Console.WriteLine("nazwa nabywcy -> " + _nameBuyer + " id " + _idBuyer + " NIP " + _nipBuyer);
            // Console.WriteLine("id ostatniego wgranego dokumentu " + _idDocument);
            // Console.WriteLine(" ");


            //program.GetExportedDocumentList(_statusDoc);// dokumenty wyeksportowane

            // program.GetDocumentByContractorNip(_nipContractor);
            // Console.WriteLine(" ");



            // Console.WriteLine("wejście dla _idUser " + _idUser.ToString()); //company_id 7085933 data.id-> 8185904
            // program.GetIdDocumentList(_idUser);
            // Console.WriteLine(" ");

            //// program.getDocumentId(_idUser, _fileName, _path, _multi); //ok pobiera dane z tabeli wgranych dokumentów

            // Console.WriteLine("getDocumentId()" + _idDocument.ToString());//id dokumentu
            // Console.WriteLine(" ");


            int idDocToDelete = 8354510;
            //program.DeleteDocument(idDocToDelete); //Ok kasuje wskazany dokument
            Console.WriteLine(" ");
            //////////////program.addCompanies();//dodaje do firmy kontrahentów

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
            _tokenS = content;

            //WriteToFile("Pomyślnie zalogowano przy użyciu tokena "+ email + " (" + DateTime.Now + ").");
            Console.WriteLine("GetToken ->" + content);// odpowiedz
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
        /// Pobiera dane firmy: praca w kontekście firm.
        /// </summary>
        /// <returns>Dane firmy: id, nip, dir_name, dbname</returns>
        public SkApiResponse getUserCompany()
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

            _idUser = data.id;
            _nipUserCompany = data.nip;
            _nameUserCompany = data.name;

            //WriteToFile("Pomyślnie zalogowano " + _idUser + " - " + _nameUserCompany + " (" + DateTime.Now + ").");
            Console.WriteLine("getUserCompany -> " + content);// odpowiedz
            return Execute<SkApiResponse>(request);
        }//getUserCompany()

        public SkApiResponse ifDocumentExist(int company_id, int document_id)
        {
            Program program = new Program();
            var client = new RestClient("http://app.skanuj.to/api");

            var request = new RestRequest(Method.POST);
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("id", _idUser.ToString());

            request.AddParameter("mode", "upload-file", ParameterType.GetOrPost);
            request.AddParameter("company_id", company_id, ParameterType.HttpHeader);
            request.AddParameter("doc_id", document_id, ParameterType.HttpHeader);
            request.AddParameter("source", "integracja", ParameterType.GetOrPost);
            request.AddParameter("response_type", "FULL", ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;

            dynamic data = JObject.Parse(JsonArrayString);
            _idDocument = (data["good-uploads"][0]["doc_id"]);
            _documentName = (data["good-uploads"][0]["name"]);
            _uploadDate = (data["good-uploads"][0]["uploaded_date"]);
            _notice = (data["page_info"]["notice"]); //poprawność wgrania
            int infoState = (data["good-uploads"][0]["state"]);

            if (infoState == 5)
            {
                Console.WriteLine("dokement o id " + _idDocument + " state = 5 Dokument istnieje");// odpowiedz

            }
            else { Console.WriteLine("dokement o id " + _idDocument + "state = " + infoState); }

            Console.WriteLine(content);// odpowiedz
            return Execute<SkApiResponse>(request);
        }//SkApiResponse


        public static int _2DB_doc_id;
        public static string _2DB_name;
        public static int _2DB_state;
        public static string _2DB_uploaded_date;
        public static int _2DB_user_id;
        public static int _2DB_validated;

        /// <summary>
        /// Wgranie dokumentu.
        /// </summary>
        /// <param name="company_id"></param>
        /// <param name="file_name"></param>
        /// <param name="path"></param>
        /// <param name="multi"></param>
        /// <returns>uploadDocument</returns>
        public SkApiResponse uploadDocument(int company_id, string file_name, string path, bool multi)
        {
            Program program = new Program();
            var client = new RestClient("http://app.skanuj.to/api");

            var request = new RestRequest(Method.POST);
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("id", _idUser.ToString());

            request.AddParameter("mode", "upload-file", ParameterType.GetOrPost);
            request.AddParameter("company_id", company_id, ParameterType.HttpHeader);
            request.AddParameter("multipages", multi, ParameterType.GetOrPost);
            request.AddParameter("source", "integracja", ParameterType.GetOrPost);
            request.AddParameter("response_type", "FULL", ParameterType.GetOrPost);
            request.AddFile(file_name, path);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);
            int infoState = (data["good-uploads"][0]["state"]);

            _2DB_doc_id = (data["good-uploads"][0]["doc_id"]);
            _2DB_name = (data["good-uploads"][0]["name"]);
            _2DB_state = (data["good-uploads"][0]["state"]);
            _2DB_uploaded_date = (data["good-uploads"][0]["uploaded_date"]);
            _2DB_user_id = (data["good-uploads"][0]["user_id"]);
            _2DB_validated = (data["good-uploads"][0]["validated"]);
            _notice = (data["page_info"]["notice"]); //poprawność wgrania

            _idDocument = _2DB_doc_id;
            _documentName = _2DB_name;
            _uploadDate = _2DB_uploaded_date.ToString();

            //if (infoState == 5)
            //{
            //    Console.WriteLine("state = 5 Dokument już istnieje");// odpowiedz

            //}
            //else { Console.WriteLine("state = " + infoState); }

            Console.WriteLine("uploadDocument ->" + content);// odpowiedz
            return Execute<SkApiResponse>(request);
        }//SkApiResponse


        public static void ZapiszDoBazy()
        {
            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM dbo.WgraneDoc WHERE state = 3;", sqlConnection);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlInsert = ("INSERT INTO dbo.WgraneDoc(doc_id, name, state, uploaded_date, user_id, validated) VALUES (@doc_id, @name, @state, @uploaded_date, @user_id, @validated);");

            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlInsert, sqlConnection1))
                {
                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@doc_id", value: _2DB_doc_id);
                    command.Parameters.AddWithValue("@name", value: _2DB_name);
                    command.Parameters.AddWithValue("@state", value: _2DB_state);
                    command.Parameters.AddWithValue("@uploaded_date", value: _2DB_uploaded_date);
                    command.Parameters.AddWithValue("@user_id", value: _2DB_user_id);
                    command.Parameters.AddWithValue("@validated", value: _2DB_validated);

                }//using
            }//using

        }//ZapiszDoBazy()
       
        public static void SprawdzStatus(int idDOC)
        {
            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            // "state": 1, /* 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM dbo.WgraneDoc WHERE doc_id = " + idDOC + ";", sqlConnection); //WHERE state = 3
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            int stateForProcess;
            
            foreach (DataRow dataRow in dataTable.Rows)
            {
                stateForProcess = int.Parse((dataRow[@"state"].ToString()));
                if (stateForProcess == 0)
                {
                    idDOC = int.Parse(dataRow["doc_id"].ToString());
                    Console.WriteLine("Dokument " + idDOC + " ma status " + stateForProcess + " dodany");
                }
                else if (stateForProcess == 1)
                {
                    idDOC = int.Parse(dataRow["doc_id"].ToString());
                    Console.WriteLine("Dokument " + idDOC + " ma status " + stateForProcess + " w przetwarzaniu");
                }
                else if (stateForProcess == 2)
                {
                    idDOC = int.Parse(dataRow["doc_id"].ToString());
                    Console.WriteLine("Dokument " + idDOC + " ma status " + stateForProcess + " w zweryfikowany");
                }
                else if (stateForProcess == 3)
                {
                    idDOC = int.Parse(dataRow["doc_id"].ToString());
                    Console.WriteLine("Dokument " + idDOC + " ma status " + stateForProcess + " do weryfikacji");
                }
                else if (stateForProcess == 4)
                {
                    idDOC = int.Parse(dataRow["doc_id"].ToString());
                    Console.WriteLine("Dokument " + idDOC + " ma status " + stateForProcess + " wielostronicowy brak akcji");
                }
                else
                {
                    idDOC = int.Parse(dataRow["doc_id"].ToString());
                    Console.WriteLine("Problem z ustaleniem statusu " + stateForProcess + " dokumentu " + idDOC);
                }
            }// foreach

            sqlConnection.Close();
        }//SprawdzStatus()



        // zapis do pliku ???
        public void WriteDocToFile(int idDoc)
        {
            // końcowa nazwa pliku
            string _endFileName = System.Configuration.ConfigurationManager.AppSettings["endFileName"].ToString();//"Test.pdf";
            // katalog wyników
            string _endPath = System.Configuration.ConfigurationManager.AppSettings["endPath"].ToString();

            if (!Directory.Exists(_endPath))
            {
                Directory.CreateDirectory(_endPath);
            }

            if (!System.IO.File.Exists(_endFileName))
            {
                try
                {
                    //CreateDocument(enfFilePath)
                    WriteToFile("Utworzono dokument " + _fileName);
                }
                catch { WriteToFile("Problem z utworzeniem dokumentu " + _fileName); }
            }
            else
            {
                try
                {
                    //CreateDocument(enfFilePath)}
                    WriteToFile("Utworzono dokument " + _fileName);
                }
                catch { WriteToFile("Problem z utworzeniem dokumentu " + _fileName); }
            }

        }//WriteToFile

        /// <summary>
        /// Loggi do pliku w katalogu ArchiwumX
        /// </summary>
        /// <param name="Message"></param>
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\ArchiwumX";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string logFilepath = AppDomain.CurrentDomain.BaseDirectory + "\\ArchiwumX\\ServiceLog.txt";
            if (!File.Exists(logFilepath))
            {
                // Stworzenie pliku do zapisu.   
                using (StreamWriter sw = System.IO.File.CreateText(logFilepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(logFilepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }// WriteToFile(string Message)



        /// <summary>
        /// Kasuje dokument po id dokumentu.
        /// </summary>
        /// <param name="idDoc"></param>
        /// <returns></returns>
        public SkApiResponse DeleteDocument(int idDoc)
        {
            var client = new RestClient("http://app.skanuj.to/api");

            var request = new RestRequest(Method.DELETE);
            request.Resource = "document?id=NN";
            request.AddHeader("token", _tokenS.ToString());

            request.AddParameter("id", idDoc, ParameterType.GetOrPost);
            //request.AddParameter("mode", "change-statusExp", ParameterType.GetOrPost);
            //request.AddParameter("company_id", company_id, ParameterType.HttpHeader);
            // request.AddParameter("state", _stateDoc, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;

            dynamic data = JObject.Parse(JsonArrayString);
            string deleteInfo = data["code"];
            string cont = "98";
            if (deleteInfo == cont)
            {
                //WriteToFile("Nie da się skasować. Dokument o ID " + idDoc + " został już wcześniej skasowany.");
                Console.WriteLine("Nie da się skasować. Dokument o ID " + idDoc + " został już wcześniej skasowany.");
            }
            else
            {
                //WriteToFile("Pomyślnie skasowano dokument " + idDoc + " (" + DateTime.Now + ").");
                Console.WriteLine("Pomyślnie skasowano dokument " + idDoc + " (" + DateTime.Now + ").");
            }

            Console.WriteLine("DeleteDocument -> " + content);// odpowiedz
            return Execute<SkApiResponse>(request);

        }//SkApiResponse





        /// <summary>
        /// Zwraca Id wgranego dokumentu. ok pobiera dane z tabeli wgranych dokumentów
        ///
        //public SkApiResponse getDocumentId(int company_id, int document_id)
        //{
        //    var client = new RestClient("http://app.skanuj.to/api");

        //    var request = new RestRequest(Method.POST);
        //    request.Resource = "document";
        //    request.AddHeader("token", _tokenS.ToString());
        //    request.AddHeader("id", _idUser.ToString());
        //    request.AddParameter("mode", "upload-file", ParameterType.GetOrPost);
        //    request.AddParameter("company_id", company_id, ParameterType.HttpHeader);
        //    request.AddParameter("")
        //    request.AddParameter("source", "integracja", ParameterType.GetOrPost);


        //    IRestResponse restResponse = client.Execute(request);
        //    var content = restResponse.Content;
        //    var JsonArrayString = content;

        //    dynamic data = JObject.Parse(JsonArrayString);

        //    _idDocument = (data["good-uploads"][0]["doc_id"]);
        //    Console.WriteLine("_idDocument -> " + data["good-uploads"][0]["doc_id"]); //[JSON].good-uploads.[0].doc_id

        //    Console.WriteLine("getDocumentId - _idDocument -> " + _idDocument);

        //    foreach (var prop in data["good-uploads"])
        //        Console.WriteLine("_idDocument w pętli-> " + prop["doc_id"]);

        //    request.AddQueryParameter("doc_id", _idDocument.ToString());
        //    Console.WriteLine("getDocumentId - > " + content);
        //    return Execute<SkApiResponse>(request);
        // }//getDocumentId(int company_id, string file_name, string path, bool multi)




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
                request.AddParameter("id", _idUser, ParameterType.HttpHeader); //w każdym zapytaniu
            }
            if (!string.IsNullOrEmpty("id"))
            { request.AddParameter("id", _idDocument, ParameterType.HttpHeader); }
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



        //OK pobranie listy dokumentów w obrębie 1 firmy.
        public List<DocumentList> GetDocumentsListByCmpID(int company_id)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);

            request.AddParameter("company_id", _idUser, ParameterType.HttpHeader);
            request.AddParameter("count", 50, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            Console.WriteLine("GetDocumentsListByCmpID ->" + content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetDocumentsListByCmpID

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
        }//getUserCompany()

        /// <summary>
        /// Pobiera listę wszystkich dokumentów dokumentów 
        /// </summary>
        /// <param name="company_id"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<DocumentList> GetAllDocumentList()
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/all";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            //request.AddParameter("company_id", _idUser, ParameterType.HttpHeader);
            request.AddParameter("count", 50, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            //Console.WriteLine("data.id -> " + data.id);
            //_idDocument = data.id;
            request.AddQueryParameter("id", _idDocument.ToString());

            Console.WriteLine("GetAllDocumentList() -> " + content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetDocumentList

        /// <summary>
        /// Sprawdza statusExp wszystkich dokumentów w systemie
        /// </summary>
        /// <returns></returns>
        public List<DocumentList> GetInfoIfDocumentExist(int idDoc)
        {

            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/search";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            //request.AddParameter("company_id", _idUser, ParameterType.HttpHeader);
            request.AddParameter("count", 50, ParameterType.GetOrPost);
            request.AddQueryParameter("id", idDoc.ToString());

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());

            Console.WriteLine("data.id -> " + data.id);
            Console.WriteLine("nazwa faktury -> " + data.name);

            // sprzedawca
            _idContractor = data.contractor.id;
            _nameContractor = data.contractor.name;
            _nipContractor = data.attributes.SprzedawcaNip; //"PL5242667621",

            //nabywca
            _idBuyer = data.company.id;
            _nameBuyer = data.company.name;
            _nipBuyer = data.company.nip;//"1132602742"

            //foreach (var prop in data[0])
            //{
            //    Console.WriteLine("cmp_id w pętli-> " + prop["cmp_id"]);
            //}
            Console.WriteLine("GetInfoIfDocumentExist() -> " + content);// zwraca listę wszystkich dokumentów state 2 i 3
            return Execute<List<DocumentList>>(request);
        }//GetInfoIfDocumentExist




        /// <summary>
        /// Zwraca dane ostatniego wgranego dokumentu.
        /// </summary>
        /// <returns></returns>
        public List<DocumentList> GetLastIdDocumentFromList()
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/all";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            // request.AddParameter("count", 50, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString()); // 
            int dlugosc = data.all_count;
            dynamic data1 = JObject.Parse(jArray[dlugosc - 1].ToString());

            Console.WriteLine("kontraktor.id " + data1.contractor.id + "data.id -> " + data1.id); //pierwszy z listy
            _idDocument = data1.id; //id dokumentu

            //sprzedawca
            _idContractor = data1.contractor.id;
            _nameContractor = data1.contractor.name;
            _nipContractor = data1.attributes.SprzedawcaNip; //"PL5242667621",

            //nabywca
            _idBuyer = data1.company.id;
            _nameBuyer = data1.company.name;
            _nipBuyer = data1.company.nip;//"1132602742"

            /*kontraktor.id 9969824data.id -> 8185910
nazwa sprzedawcy-> "LINDSTROM" SPÓŁKA Z OGRANICZONĄ ODPOWIEDZIALNOŚCIĄ 9969824 NIP PL5222640524
nazwa nabywcy-> KR GROUP SPÓŁKA Z OGRANICZONĄ ODPOWIEDZIALNOŚCIĄ SPÓŁKA KOMANDYTOWA 7085933 NIP 1132602742
uploaded_date2019-07-01 11:44:30.868114
id ostatniego wgranego dokumentu 8185910*/

            request.AddQueryParameter("statusExp", _statusDoc.ToString());

            //Console.WriteLine("GetIdDocumentList -> " + content);// odpowiedz
            return Execute<List<DocumentList>>(request);
        }//GetExportedDocumentList

        //{"id":8185917,"user_id":7082762,

        // OK lista dokumentów danego usera.
        public List<DocumentList> GetIdDocumentList(int id)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/all";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("id", id, ParameterType.HttpHeader);
            //request.AddParameter("count", 3, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString()); // 
            int dlugosc = data.all_count;
            //company_id 7085933 data.id -> 8185904
            Console.WriteLine("kontraktor.id " + data.contractor.id + "data.id -> " + data.id); //pierwszy z listy
            _idDocument = data.id;
            _idContractor = data.contractor.id;
            _idUser = data.company_id;
            _nameContractor = data.contractor.name;
            Console.WriteLine("nazwa kontraktor-> " + data.contractor.name);
            Console.WriteLine("data-> " + data.contractor.name);
            Console.WriteLine("uploaded_date" + data.uploaded_date);

            dynamic data1 = JObject.Parse(jArray[dlugosc - 1].ToString()); // 

            Console.WriteLine("kontraktor.id " + data1.contractor.id + "data.id -> " + data1.id); //pierwszy z listy
            _idDocument = data1.id;
            _idContractor = data1.contractor.id;
            _idUser = data1.company_id;
            _nameContractor = data1.contractor.name;
            Console.WriteLine("nazwa kontraktor-> " + data1.contractor.name);
            Console.WriteLine("data-> " + data1.contractor.name);
            Console.WriteLine("uploaded_date" + data1.uploaded_date);

            //kontraktor.id 9969821data.id -> 8185917
            //nazwa kontraktor-> GRUPA MARCOVA POLSKA OFFICE SPÓŁKA Z OGRANICZONĄ ODPOWIEDZIALNOŚCIĄ SPÓŁKA KOMANDYTOWA


            /*kontraktor.id 9969820data.id -> 8185908
nazwa kontraktor-> "PRETOR" SPÓŁKA Z OGRANICZONĄ ODPOWIEDZIALNOŚCIĄ*/

            request.AddQueryParameter("id", id.ToString());
            request.AddQueryParameter("statusExp", _statusDoc.ToString());

            Console.WriteLine("GetIdDocumentList -> " + content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetExportedDocumentList




        public List<DocumentList> GetExportedDocumentList(int status)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/search";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());


            request.AddParameter("statusExp", _statusDoc, ParameterType.HttpHeader);
            request.AddParameter("count", 3, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());

            //Console.WriteLine("data.id -> " + data.id);
            //_idDocument = data.id;

            request.AddQueryParameter("id", _idDocument.ToString());
            request.AddQueryParameter("statusExp", _statusDoc.ToString());

            //Console.WriteLine("GetExportedDocumentList -> " + content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetExportedDocumentList


        // // // // // // Pobiera id dokumentów o statusie
        public List<DocumentList> ListIdToVeryfication(int stateDoc)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/search";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("state", stateDoc, ParameterType.HttpHeader);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            //request.AddQueryParameter("id", _idDocument.ToString());
            request.AddQueryParameter("state", stateDoc.ToString());
            request.AddQueryParameter("id", _idDocument.ToString());
            int status = data.status;
            request.AddQueryParameter("statusExp", status.ToString());


            _idDocument = data.id;
            int dlugosc = data.all_count;

            Console.WriteLine("ilość dokumentów " + dlugosc);

            //company_id 7085933 data.id -> 8185904
            //foreach (var cos in data[""]["contractor"]) { Console.WriteLine("id company " + data.id + " name " + data.name); }
            Console.WriteLine("id dokuemntu " + _idDocument + " statusExp doc - " + status + ", state - " + stateDoc); //pierwszy z listy

            Console.WriteLine("ListIdToVeryfication -> " + content);// odpowiedz
            return Execute<List<DocumentList>>(request);
        }//ListIdToVeryfication




        public List<DocumentList> GetStateDocumentList(int state)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/search";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("state", state, ParameterType.HttpHeader);
            request.AddParameter("count", 50, ParameterType.GetOrPost);

            request.AddQueryParameter("id", _idDocument.ToString());
            request.AddQueryParameter("state", state.ToString());

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            int licznik = 0;
            dynamic data = JObject.Parse(jArray[0].ToString());
            int count = data.all_count;

            while (licznik < count)
            {
                dynamic data1 = JObject.Parse(jArray[licznik].ToString());
                Console.WriteLine("id dokumentu-> " + data1.id + " statusExp -> " + data1.status + "state -> " + data1.state);

                licznik++;
            }//while




            Console.WriteLine("GetStateDocumentList -> " + content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetStateDocumentList

        /// <summary>
        /// Zwraca listę gotowych/rozpoznanych dokumentów 
        /// </summary>
        /// <param name="company_id"></param>
        /// <returns></returns>
        public List<DocumentList> GetIdListByCmpID(int d, string NazwaDoc)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "document/mode/search", ParameterType.GetOrPost);

            request.AddParameter("company_id", _idUser, ParameterType.HttpHeader);
            request.AddParameter("count", 50, ParameterType.GetOrPost);

            request.AddQueryParameter("name", NazwaDoc.ToString());
            request.AddQueryParameter("id", _idDocument.ToString());

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());

            Console.WriteLine("data.id -> " + _idDocument + " nazwa dokumentu " + NazwaDoc);

            /*[{
        "DataWystawienia": "2019-05-21",
        "NrFaktury": "2564\/MAG\/2019",
        "RazemBrutto": "530.63",
        "added_by_email": "w.radzikowski@krgroup.pl",
        "added_by_name": "Wojciech Radzikowski",
        "cmp_name": "KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA",
        "company_id": 7085933,
        "contractor_name": "GRUPA MARCOVA POLSKA OFFICE SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA",
        "file_path_hq": "https:\/\/app.skanuj.to\/customer\/get-pdf\/hash\/cb5c2a1727489e9ef8ce8d94ac1ddc1f\/id\/8185917",
        "hash": "cb5c2a1727489e9ef8ce8d94ac1ddc1f",
        "id": 8185917,
        "last_modified": "2019-07-01 11:54:57.20007",
        "registry": null,
        "type": 2,
        "type_dict": "Faktura zakupu",
        "verified_at": "2019-07-01 11:54:57.20007",
        "verified_by_email": "w.radzikowski@krgroup.pl",
        "verified_by_name": "Wojciech Radzikowski"
    }, ...
]*/


            Console.WriteLine(" .GetIdListByCmpID(_idUser); " + content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetDocumentsListByCmpID


        public List<DocumentList> GetIfDocExist(int idDoc)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);

            request.AddParameter("id", _idDocument, ParameterType.HttpHeader);
            request.AddParameter("count", 50, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());

            _idDocument = data.id;
            string stateDoc = data.state;
            string statusDoc = data.status;
            Console.WriteLine("id Doc -> " + _idDocument + " statusExp " + statusDoc + " state " + stateDoc);

            request.AddQueryParameter("id", _idDocument.ToString());

            Console.WriteLine(" .GetIdListByCmpID(_idUser); " + content);// odpowiedz
            return Execute<List<DocumentList>>(request);
        }//GetDocumentsListByCmpID


        public List<DocumentList> GetIdList(int company_id)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddParameter("mode", "all-documents-list", ParameterType.GetOrPost);
            request.AddParameter("company_id", _idUser, ParameterType.HttpHeader);
            request.AddParameter("count", 3, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            Console.WriteLine("_idDokument z GetIdLis -> " + data.id);
            _idDocument = data.id;
            request.AddQueryParameter("id", _idDocument.ToString());

            Console.WriteLine("GetIdList ->" + content);// odpowiedz

            return Execute<List<DocumentList>>(request);
        }//GetDocumentsListByCmpID






        /// <summary>
        /// Dodanie firmy dla prawidłowego rozpoznania dokumentów. ????
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


        // // // // // // Funkcja pobierająca dane pojedynczego dokumentu - rozszerzane dane.

        public DocumentOneXt GetDocumentByContractorNip(string nipContractor)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());


            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("contractor.nip", nipContractor, ParameterType.GetOrPost);
            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;


            Console.WriteLine("GetDocumentByContractorNip ->" + content);// odpowiedz

            return Execute<DocumentOneXt>(request);
        }// GetDocumentById(int id)

        public DocumentOneXt GetIdDocByName(string nameDoc) // // // // ok
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            // request.AddParameter("id", id, ParameterType.GetOrPost);
            request.AddParameter("name", nameDoc, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);

            //statusExp Eksportu dokumentu  - statusExp atrybutu (0 - nie wymagający weryfikacji, 1 - wymagający weryfikacji, 2 - zweryfikowany)
            int statusExp = (data["status"]);
            _documentName = (data["name"]);

            if (_documentName == nameDoc)
            {
                int idDoc = (data["id"]);
                Console.WriteLine("Dokument " + _documentName + " ma id " + idDoc);
            }
            else
            {
                Console.WriteLine("Nie znaleziono dokumentu " + _documentName);
            }

            Console.WriteLine("GetExportStatusByIdDoc ->" + content);// odpowiedz

            return Execute<DocumentOneXt>(request);
        }// GetDocumentById(int id)

        /// <summary>
        /// Odszukuje id dokumentu wgranego wcześniej po nazwie dokumentu.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DocumentOneXt GetDataFromDoc(int id) // // // // ok
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", id, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);
            _documentName = (data["name"]);
           //pobranie rozpoznanych wartości dokumentu. 
            string status1 = " wymaga zweryfikowania. Rozpoznanie na poziomie - (";
            string status1a = "). Do weryfikacji wartość: ";
            string puste = "Pole jest puste. Nie wykryto wartości w polu ";
            string status0 = " rozpoznanie w wysokości (";
            string status0a = "). Rozpoznana wartość ";
            try
            {
                string rozp = (data["attributes"]["BruttoWalutaPodstawowa"]["is_valid"]).ToString();
                int statusA = data["attributes"]["BruttoWalutaPodstawowa"]["status"];
                string value = (data["attributes"]["BruttoWalutaPodstawowa"]["value"]);
                string atr = "Brutto Waluta Podstawowa ";

                if (statusA == 1)
                {
                    Console.WriteLine(atr + status1 + rozp + status1a + value);
                }
                else
                {
                    if (value == "") { Console.WriteLine(puste + atr); }
                    else { Console.WriteLine(atr + status0 + rozp + status0a + value); }
                }
            }
            catch { };
            try
            {
                string rozp1 = (data["attributes"]["CategoryDesc"]["is_valid"]).ToString();
                int statusA1 = data["attributes"]["CategoryDesc"]["status"];
                string value1 = (data["attributes"]["CategoryDesc"]["value"]);
                string atr1 = "CategoryDesc ";
                if (statusA1 == 1)
                {
                    Console.WriteLine(atr1 + status1 + rozp1 + status1a + value1);
                }
                else
                {
                    if (value1 == "") { Console.WriteLine(puste + atr1); }
                    else { Console.WriteLine(atr1 + status0 + rozp1 + status0a + value1); }
                }
            }
            catch { };
            try
            {
                string rozp2 = (data["attributes"]["CzyNieKompletnaPozycja"]["is_valid"]).ToString();
                int statusA2 = data["attributes"]["CzyNieKompletnaPozycja"]["status"];
                string value2 = (data["attributes"]["CzyNieKompletnaPozycja"]["value"]);
                string atr2 = "CzyNieKompletnaPozycja ";
                if (statusA2 == 1)
                {
                    Console.WriteLine(atr2 + status1 + rozp2 + status1a + value2);
                }
                else
                {
                    if (value2 == "") { Console.WriteLine(puste + atr2); }
                    else { Console.WriteLine(atr2 + status0 + rozp2 + status0a + value2); }
                }
            }
            catch { };
            try
            {
                string rozp3 = (data["attributes"]["DataSprzedazy"]["is_valid"]).ToString();
                int statusA3 = data["attributes"]["DataSprzedazy"]["status"];
                string value3 = (data["attributes"]["DataSprzedazy"]["value"]);
                string atr3 = "DataSprzedazy ";
                if (statusA3 == 1)
                {
                    Console.WriteLine(atr3 + status1 + rozp3 + status1a + value3);
                }
                else
                {
                    if (value3 == "") { Console.WriteLine(puste + atr3); }
                    else { Console.WriteLine(atr3 + status0 + rozp3 + status0a + value3); }
                }
            }
            catch { };
            try
            {
                string rozp4 = (data["attributes"]["DataWplywu"]["is_valid"]).ToString();
                int statusA4 = data["attributes"]["DataWplywu"]["status"];
                string value4 = (data["attributes"]["DataWplywu"]["value"]);
                string atr4 = "DataWplywu ";
                if (statusA4 == 1)
                {
                    Console.WriteLine(atr4 + status1 + rozp4 + status1a + value4);
                }
                else
                {
                    if (value4 == "") { Console.WriteLine(puste + atr4); }
                    else { Console.WriteLine(atr4 + status0 + rozp4 + status0a + value4); }
                }
            }
            catch { };

            try
            {
                string rozp6 = (data["attributes"]["DataWystawienia"]["is_valid"]).ToString();
                int statusA6 = data["attributes"]["DataWystawienia"]["status"];
                string value6 = (data["attributes"]["DataWystawienia"]["value"]);
                string atr6 = "DataWystawienia ";
                if (statusA6 == 1)
                {
                    Console.WriteLine(atr6 + status1 + rozp6 + status1a + value6);
                }
                else
                {
                    if (value6 == "") { Console.WriteLine(puste + atr6); }
                    else { Console.WriteLine(atr6 + status0 + rozp6 + status0a + value6); }
                }
            }
            catch { };
            try
            {
                string rozp7 = (data["attributes"]["FakturaKorygowana"]["is_valid"]).ToString();
                int statusA7 = data["attributes"]["FakturaKorygowana"]["status"];
                string value7 = (data["attributes"]["FakturaKorygowana"]["value"]);
                string atr7 = "FakturaKorygowana ";
                if (statusA7 == 1)
                {
                    Console.WriteLine(atr7 + status1 + rozp7 + status1a + value7);
                }
                else
                {
                    if (value7 == "") { Console.WriteLine(puste + atr7); }
                    else { Console.WriteLine(atr7 + status0 + rozp7 + status0a + value7); }
                }
            }
            catch { };
            try
            {
                string rozp8 = (data["attributes"]["Kategoria"]["is_valid"]).ToString();
                int statusA8 = data["attributes"]["Kategoria"]["status"];
                string value8 = (data["attributes"]["Kategoria"]["value"]);
                string atr8 = "Kategoria ";
                if (statusA8 == 1)
                {
                    Console.WriteLine(atr8 + status1 + rozp8 + status1a + value8);
                }
                else
                {
                    if (value8 == "") { Console.WriteLine(puste + atr8); }
                    else { Console.WriteLine(atr8 + status0 + rozp8 + status0a + value8); }
                }
            }
            catch { };

            try
            {
                string rozp5 = (data["attributes"]["KategoriaId"]["is_valid"]).ToString();
                int statusA5 = data["attributes"]["KategoriaId"]["status"];
                string value5 = (data["attributes"]["KategoriaId"]["value"]);
                string atr5 = "KategoriaId ";
                if (statusA5 == 1)
                {
                    Console.WriteLine(atr5 + status1 + rozp5 + status1a + value5);
                }
                else
                {
                    if (value5 == "") { Console.WriteLine(puste + atr5); }
                    else { Console.WriteLine(atr5 + status0 + rozp5 + status0a + value5); }
                }
            }
            catch { };
            try
            {
                string rozp9 = (data["attributes"]["KontoBankowe"]["is_valid"]).ToString();
                int statusA9 = data["attributes"]["KontoBankowe"]["status"];
                string value9 = (data["attributes"]["KontoBankowe"]["value"]);
                string atr9 = "KontoBankowe ";
                if (statusA9 == 1)
                {
                    Console.WriteLine(atr9 + status1 + rozp9 + status1a + value9);
                }
                else
                {
                    if (value9 == "") { Console.WriteLine(puste + atr9); }
                    else { Console.WriteLine(atr9 + status0 + rozp9 + status0a + value9); }
                }
            }
            catch { };

            try
            {
                string rozp10 = (data["attributes"]["Korygujaca"]["is_valid"]).ToString();
                int statusA10 = data["attributes"]["Korygujaca"]["status"];
                string value10 = (data["attributes"]["Korygujaca"]["value"]);
                string atr10 = "Korygujaca ";
                if (statusA10 == 1)
                {
                    Console.WriteLine(atr10 + status1 + rozp10 + status1a + value10);
                }
                else
                {
                    if (value10 == "") { Console.WriteLine(puste + atr10); }
                    else { Console.WriteLine(atr10 + status0 + rozp10 + status0a + value10); }
                }
            }
            catch { };

            try
            {
                string rozp11 = (data["attributes"]["KursWaluty"]["is_valid"]).ToString();
                int statusA11 = data["attributes"]["KursWaluty"]["status"];
                string value11 = (data["attributes"]["KursWaluty"]["value"]);
                string atr11 = "KursWaluty ";
                if (statusA11 == 1)
                {
                    Console.WriteLine(atr11 + status1 + rozp11 + status1a + value11);
                }
                else
                {
                    if (value11 == "") { Console.WriteLine(puste + atr11); }
                    else { Console.WriteLine(atr11 + status0 + rozp11 + status0a + value11); }
                }
            }
            catch { };

            try
            {
                string rozp12 = (data["attributes"]["MiesiacKsiegowy"]["is_valid"]).ToString();
                int statusA12 = data["attributes"]["MiesiacKsiegowy"]["status"];
                string value12 = (data["attributes"]["MiesiacKsiegowy"]["value"]);
                string atr12 = "MiesiacKsiegowy ";
                if (statusA12 == 1)
                {
                    Console.WriteLine(atr12 + status1 + rozp12 + status1a + value12);
                }
                else
                {
                    if (value12 == "") { Console.WriteLine(puste + atr12); }
                    else { Console.WriteLine(atr12 + status0 + rozp12 + status0a + value12); }
                }
            }
            catch { };

            try
            {
                string rozp13 = (data["attributes"]["NabywcaAdres"]["is_valid"]).ToString();
                int statusA13 = data["attributes"]["NabywcaAdres"]["status"];
                string value13 = (data["attributes"]["NabywcaAdres"]["value"]);
                string atr13 = "NabywcaAdres ";
                if (statusA13 == 1)
                {
                    Console.WriteLine(atr13 + status1 + rozp13 + status1a + value13);
                }
                else
                {
                    if (value13 == "") { Console.WriteLine(puste + atr13); }
                    else { Console.WriteLine(atr13 + status0 + rozp13 + status0a + value13); }
                }
            }
            catch { };
            try
            {
                string rozp14 = (data["attributes"]["NabywcaKod"]["is_valid"]).ToString();
                int statusA14 = data["attributes"]["NabywcaKod"]["status"];
                string value14 = (data["attributes"]["NabywcaKod"]["value"]);
                string atr14 = "NabywcaKod ";
                if (statusA14 == 1)
                {
                    Console.WriteLine(atr14 + status1 + rozp14 + status1a + value14);
                }
                else
                {
                    if (value14 == "") { Console.WriteLine(puste + atr14); }
                    else { Console.WriteLine(atr14 + status0 + rozp14 + status0a + value14); }
                }
            }
            catch { };

            try
            {
                string rozp15 = (data["attributes"]["NabywcaMiejscowosc"]["is_valid"]).ToString();
                int statusA15 = data["attributes"]["NabywcaMiejscowosc"]["status"];
                string value15 = (data["attributes"]["NabywcaMiejscowosc"]["value"]);
                string atr15 = "NabywcaMiejscowosc ";
                if (statusA15 == 1)
                {
                    Console.WriteLine(atr15 + status1 + rozp15 + status1a + value15);
                }
                else
                {
                    if (value15 == "") { Console.WriteLine(puste + atr15); }
                    else { Console.WriteLine(atr15 + status0 + rozp15 + status0a + value15); }
                }
            }
            catch { };

            try
            {
                string rozp16 = (data["attributes"]["NabywcaNazwa"]["is_valid"]).ToString();
                int statusA16 = data["attributes"]["NabywcaNazwa"]["status"];
                string value16 = (data["attributes"]["NabywcaNazwa"]["value"]);
                string atr16 = "NabywcaNazwa ";
                if (statusA16 == 1)
                {
                    Console.WriteLine(atr16 + status1 + rozp16 + status1a + value16);
                }
                else
                {
                    if (value16 == "") { Console.WriteLine(puste + atr16); }
                    else { Console.WriteLine(atr16 + status0 + rozp16 + status0a + value16); }
                }
            }
            catch { };

            try
            {
                string rozp17 = (data["attributes"]["NabywcaNip"]["is_valid"]).ToString();
                int statusA17 = data["attributes"]["NabywcaNip"]["status"];
                string value17 = (data["attributes"]["NabywcaNip"]["value"]);
                string atr17 = "NabywcaNip ";
                if (statusA17 == 1)
                {
                    Console.WriteLine(atr17 + status1 + rozp17 + status1a + value17);
                }
                else
                {
                    if (value17 == "") { Console.WriteLine(puste + atr17); }
                    else { Console.WriteLine(atr17 + status0 + rozp17 + status0a + value17); }
                }
            }
            catch { };

            try
            {
                string rozp18 = (data["attributes"]["NettoWalutaPodstawowa"]["is_valid"]).ToString();
                int statusA18 = data["attributes"]["NettoWalutaPodstawowa"]["status"];
                string value18 = (data["attributes"]["NettoWalutaPodstawowa"]["value"]);
                string atr18 = "NettoWalutaPodstawowa ";
                if (statusA18 == 1)
                {
                    Console.WriteLine(atr18 + status1 + rozp18 + status1a + value18);
                }
                else
                {
                    if (value18 == "") { Console.WriteLine(puste + atr18); }
                    else { Console.WriteLine(atr18 + status0 + rozp18 + status0a + value18); }
                }
            }
            catch { };

            try
            {
                string rozp19 = (data["attributes"]["NrFaktury"]["is_valid"]).ToString();
                int statusA19 = data["attributes"]["NrFaktury"]["status"];
                string value19 = (data["attributes"]["NrFaktury"]["value"]);
                string atr19 = "NrFaktury ";
                if (statusA19 == 1)
                {
                    Console.WriteLine(atr19 + status1 + rozp19 + status1a + value19);
                }
                else
                {
                    if (value19 == "") { Console.WriteLine(puste + atr19); }
                    else { Console.WriteLine(atr19 + status0 + rozp19 + status0a + value19); }
                }
            }
            catch { };

            try
            {
                string rozp20 = (data["attributes"]["NrZamowienia"]["is_valid"]).ToString();
                int statusA20 = data["attributes"]["NrZamowienia"]["status"];
                string value20 = (data["attributes"]["NrZamowienia"]["value"]);
                string atr20 = "NrZamowienia ";
                if (statusA20 == 1)
                {
                    Console.WriteLine(atr20 + status1 + rozp20 + status1a + value20);
                }
                else
                {
                    if (value20 == "") { Console.WriteLine(puste + atr20); }
                    else { Console.WriteLine(atr20 + status0 + rozp20 + status0a + value20); }
                }
            }
            catch { };

            try
            {
                string rozp21 = (data["attributes"]["PrzyczynaKorekty"]["is_valid"]).ToString();
                int statusA21 = data["attributes"]["PrzyczynaKorekty"]["status"];
                string value21 = (data["attributes"]["PrzyczynaKorekty"]["value"]);
                string atr21 = "PrzyczynaKorekty ";
                if (statusA21 == 1)
                {
                    Console.WriteLine(atr21 + status1 + rozp21 + status1a + value21);
                }
                else
                {
                    if (value21 == "") { Console.WriteLine(puste + atr21); }
                    else
                    { Console.WriteLine(atr21 + status0 + rozp21 + status0a + value21); }
                }
            }
            catch { };

            try
            {
                string rozp22 = (data["attributes"]["RazemBrutto"]["is_valid"]).ToString();
                int statusA22 = data["attributes"]["RazemBrutto"]["status"];
                string value22 = (data["attributes"]["RazemBrutto"]["value"]);
                string atr22 = "RazemBrutto";
                if (statusA22 == 1)
                {
                    Console.WriteLine(atr22 + status1 + rozp22 + status1a + value22);
                }
                else
                {
                    if (value22 == "") { Console.WriteLine(puste + atr22); }
                    else { Console.WriteLine(atr22 + status0 + rozp22 + status0a + value22); }
                }
            }
            catch { };

            try
            {
                string rozp23 = (data["attributes"]["RazemNetto"]["is_valid"]).ToString();
                int statusA23 = data["attributes"]["RazemNetto"]["status"];
                string value23 = (data["attributes"]["RazemNetto"]["value"]);
                string atr23 = "RazemNetto";
                if (statusA23 == 1)
                {
                    Console.WriteLine(atr23 + status1 + rozp23 + status1a + value23);
                }
                else
                {
                    if (value23 == "") { Console.WriteLine(puste + atr23); }
                    else
                    { Console.WriteLine(atr23 + status0 + rozp23 + status0a + value23); }
                }
            }
            catch { };

            try
            {
                string rozp24 = (data["attributes"]["RazemVAT"]["is_valid"]).ToString();
                int statusA24 = data["attributes"]["RazemVAT"]["status"];
                string value24 = (data["attributes"]["RazemVAT"]["value"]);
                string atr24 = "RazemVAT";
                if (statusA24 == 1)
                {
                    Console.WriteLine(atr24 + status1 + rozp24 + status1a + value24);
                }
                else
                {
                    if (value24 == "") { Console.WriteLine(puste + atr24); }
                    else
                    {
                        Console.WriteLine(atr24 + status0 + rozp24 + status0a + value24);
                    }
                }
            }
            catch { };

            try
            {
                string rozp25 = (data["attributes"]["SposobPlatnosci"]["is_valid"]).ToString();
                int statusA25 = data["attributes"]["SposobPlatnosci"]["status"];
                string value25 = (data["attributes"]["SposobPlatnosci"]["value"]);
                string atr25 = "SposobPlatnosci";
                if (statusA25 == 1)
                {
                    Console.WriteLine(atr25 + status1 + rozp25 + status1a + value25);
                }
                else
                {
                    if (value25 == "") { Console.WriteLine(puste + atr25); }
                    else { Console.WriteLine(atr25 + status0 + rozp25 + status0a + value25); }
                }
            }
            catch { };

            try
            {
                string rozp26 = (data["attributes"]["SprzedawcaAdres"]["is_valid"]).ToString();
                int statusA26 = data["attributes"]["SprzedawcaAdres"]["status"];
                string value26 = (data["attributes"]["SprzedawcaAdres"]["value"]);
                string atr26 = "SprzedawcaAdres";
                if (statusA26 == 1)
                {
                    Console.WriteLine(atr26 + status1 + rozp26 + status1a + value26);
                }
                else
                {
                    if (value26 == "") { Console.WriteLine(puste + atr26); }
                    else { Console.WriteLine(atr26 + status0 + rozp26 + status0a + value26); }
                }
            }
            catch { };

            try
            {
                string rozp27 = (data["attributes"]["SprzedawcaKod"]["is_valid"]).ToString();
                int statusA27 = data["attributes"]["SprzedawcaKod"]["status"];
                string value27 = (data["attributes"]["SprzedawcaKod"]["value"]);
                string atr27 = "SprzedawcaKod";
                if (statusA27 == 1)
                {
                    Console.WriteLine(atr27 + status1 + rozp27 + status1a + value27);
                }
                else
                {
                    if (value27 == "") { Console.WriteLine(puste + atr27); }
                    else { Console.WriteLine(atr27 + status0 + rozp27 + status0a + value27); }
                }
            }
            catch { };

            try
            {
                string rozp28 = (data["attributes"]["SprzedawcaMiejscowosc"]["is_valid"]).ToString();
                int statusA28 = data["attributes"]["SprzedawcaMiejscowosc"]["status"];
                string value28 = (data["attributes"]["SprzedawcaMiejscowosc"]["value"]);
                string atr28 = "SprzedawcaMiejscowosc";
                if (statusA28 == 1)
                {
                    Console.WriteLine(atr28 + status1 + rozp28 + status1a + value28);
                }
                else
                {
                    if (value28 == "") { Console.WriteLine(puste + atr28); }
                    else { Console.WriteLine(atr28 + status0 + rozp28 + status0a + value28); }
                }
            }
            catch { };
            try
            {
                string rozp29 = (data["attributes"]["SprzedawcaNazwa"]["is_valid"]).ToString();
                int statusA29 = data["attributes"]["SprzedawcaNazwa"]["status"];
                string value29 = (data["attributes"]["SprzedawcaNazwa"]["value"]);
                string atr29 = "SprzedawcaNazwa";
                if (statusA29 == 1)
                {
                    Console.WriteLine(atr29 + status1 + rozp29 + status1a + value29);
                }
                else
                {
                    if (value29 == "") { Console.WriteLine(puste + atr29); }
                    else { Console.WriteLine(atr29 + status0 + rozp29 + status0a + value29); }
                }
            }
            catch { };
            try
            {
                string rozp30 = (data["attributes"]["SprzedawcaNip"]["is_valid"]).ToString();
                int statusA30 = data["attributes"]["SprzedawcaNip"]["status"];
                string value30 = (data["attributes"]["SprzedawcaNip"]["value"]);
                string atr30 = "SprzedawcaNip";
                if (statusA30 == 1)
                {
                    Console.WriteLine(atr30 + status1 + rozp30 + status1a + value30);
                }
                else
                {
                    if (value30 == "") { Console.WriteLine(puste + atr30); }
                    else { Console.WriteLine(atr30 + status0 + rozp30 + status0a + value30); }
                }
            }
            catch { };
            try
            {
                string rozp31 = (data["attributes"]["TerminPlatnosci"]["is_valid"]).ToString();
                int statusA31 = data["attributes"]["TerminPlatnosci"]["status"];
                string value31 = (data["attributes"]["TerminPlatnosci"]["value"]);
                string atr31 = "TerminPlatnosci";
                if (statusA31 == 1)
                {
                    Console.WriteLine(atr31 + status1 + rozp31 + status1a + value31);
                }
                else
                {
                    if (value31 == "") { Console.WriteLine(puste + atr31); }
                    else { Console.WriteLine(atr31 + status0 + rozp31 + status0a + value31); }
                }
            }
            catch { };
            try
            {
                string rozp32 = (data["attributes"]["VatWalutaPodstawowa"]["is_valid"]).ToString();
                int statusA32 = data["attributes"]["VatWalutaPodstawowa"]["status"];
                string value32 = (data["attributes"]["VatWalutaPodstawowa"]["value"]);
                string atr32 = "VatWalutaPodstawowa";
                if (statusA32 == 1)
                {
                    Console.WriteLine(atr32 + status1 + rozp32 + status1a + value32);
                }
                else
                {
                    if (value32 == "") { Console.WriteLine(puste + atr32); }
                    else { Console.WriteLine(atr32 + status0 + rozp32 + status0a + value32); }
                }
            }
            catch { };
            try
            {
                string rozp33 = (data["attributes"]["Waluta"]["is_valid"]).ToString();
                int statusA33 = data["attributes"]["Waluta"]["status"];
                string value33 = (data["attributes"]["Waluta"]["value"]);
                string atr33 = "Waluta";
                if (statusA33 == 1)
                {
                    Console.WriteLine(atr33 + status1 + rozp33 + status1a + value33);
                }
                else
                {
                    if (value33 == "") { Console.WriteLine(puste + atr33); }
                    else { Console.WriteLine(atr33 + status0 + rozp33 + status0a + value33); }
                }
            }
            catch { };
            try
            {
                string rozp34 = (data["attributes"]["Zaplacono"]["is_valid"]).ToString();
                int statusA34 = data["attributes"]["Zaplacono"]["status"];
                string value34 = (data["attributes"]["Zaplacono"]["value"]);
                string atr34 = "Zaplacono";
                if (statusA34 == 1)
                {
                    Console.WriteLine(atr34 + status1 + rozp34 + status1a + value34);
                }
                else
                {
                    if (value34 == "") { Console.WriteLine(puste + atr34); }
                    else { Console.WriteLine(atr34 + status0 + rozp34 + status0a + value34); }
                }
            }
            catch { };

            Console.WriteLine("GetDataFromDoc " + request);
            return Execute<DocumentOneXt>(request);
        }

        /// <summary>
        /// Pokazuje poziom odczytu pozycji w dokumencie o podanym id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DocumentOneXt GetVeryficationStateByIdDoc(int id) // // // // ok
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", id, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);

            _documentName = (data["name"]);

            ////Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 
            int state = (data["state"]);
            if (state == 0) { Console.WriteLine("Dokument został dodany (0)"); }
            else if (state == 1)
            {
                Console.WriteLine("Dokument w przetworzeniu (1)");

            }
            else if (state == 2)
            {
                Console.WriteLine("Dokument poprawnie zweryfikowany (2)");
            }
            else if (state == 3)
            {
                Console.WriteLine("Dokument do weryfikacji (3).");
                //dane faktury
                foreach (var val in data["positions"])
                {
                    string nameProduct = val["Nazwa"];
                    int Ilosc = val["Ilosc"];
                    int Validation = val["is_valid"];
                    string zero = "0";
                    string jeden = "1";

                    if (Validation.ToString() == zero)
                    {
                        Console.WriteLine("Problem z rozpoznaniem warości (" + Validation + "): Nazwa " + nameProduct + ", Ilosc " + Ilosc + "Brutto " + val["Brutto"] + ", Netto " + val["Netto"] + ", StawkaVAT " + val["StawkaVAT"] + ", kwota VAT " + val["VAT"]);
                    }
                    else if (Validation.ToString() == jeden)
                    {
                        Console.WriteLine("Poprawnie rozpoznane (" + Validation + "): Nazwa " + nameProduct + ", Ilosc " + Ilosc + "Brutto " + val["Brutto"] + ", Netto " + val["Netto"] + ", StawkaVAT " + val["StawkaVAT"] + ", kwota VAT " + val["VAT"]);

                    }
                }
            }



            return Execute<DocumentOneXt>(request);
        }// GetDocumentById(int id)

        //ustawienie statusu eksportu na dokumencie
        public DocumentOneXt SetDocumentExportStatus(int status, int idDoc)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", idDoc, ParameterType.GetOrPost);
            request.AddParameter("status", status, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);

            _idContractor = (data["contractor"]["id"]);
            int state = (data["state"]);
            int statusDoc = (data["status"]);
            Console.WriteLine("Id Doc " + idDoc + " state " + state + " status doc " + statusDoc);

            Console.WriteLine("SetDocumentExportStatus ->" + content);// odpowiedz

            return Execute<DocumentOneXt>(request);
        }


        public DocumentOneXt GetDocumentByNazwa(string nazwaDoc) // // // // ok
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("name", nazwaDoc, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);

            //dane z poszerzonych danych 1 dokumentu

            ////dane sprzedawcy
            //_idContractor = (data["contractor"]["id"]);
            //_nameContractor = (data["contractor"]["name"]);
            //_nipContractor = (data["contractor"]["nip"]);
            //string adressContractor = data["contractor"]["address"];
            ////danekupującego
            //_idBuyer = (data["company"]["id"]);
            //_nipBuyer = (data["company"]["nip"]);
            //_nameBuyer = (data["company"]["name"]);
            //string adressBuyer = (data["company"]["address"]);
            //int idDoc = (data["id"]);

            ////dane faktury
            //foreach (var val in data["positions"])
            //{
            //    string nameProduct = val["Nazwa"];
            //    int Ilosc = val["Ilosc"];
            //    int Validation = val["is_valid"];

            //    Console.WriteLine("Nazwa " + nameProduct + ", Ilosc " + Ilosc);
            //    Console.WriteLine("Brutto " + val["Brutto"] + ", Netto " + val["Netto"] + ", StawkaVAT " + val["StawkaVAT"] + ", kwota VAT " + val["VAT"]);
            //    if (Validation == 1) { Console.WriteLine("Poprawnie rozpoznane (" + Validation + ")."); }
            //    else { Console.WriteLine("Problem z rozpoznaniem warości (" + Validation + ")."); }
            //}


            //Console.WriteLine("GetDoc Id zwraca id " + idDoc + " dla nazwy " + nazwaDoc);
            Console.WriteLine("GetDocumentByNazwa ->" + content);// odpowiedz

            return Execute<DocumentOneXt>(request);
        }// GetDocumentById(int id)



        /// <summary>
        /// Zwraca dane poprawnie wgranego dokumentu po idDocumentu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DocumentOneXt GetDocumentById(int id) // // // // ok
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", id, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);

            //dane z poszerzonych danych 1 dokumentu

            //dane sprzedawcy
            _idContractor = (data["contractor"]["id"]);
            _nameContractor = (data["contractor"]["name"]);
            _nipContractor = (data["contractor"]["nip"]);
            string adressContractor = data["contractor"]["address"];
            //dane kupującego

            ////statusExp - statusExp atrybutu (0 - nie wymagający weryfikacji, 1 - wymagający weryfikacji, 2 - zweryfikowany)
            //int status = (data["statusExp"]);
            //if (status == 0) { Console.WriteLine("Dokument nie wymagający weryfikacji (0)"); }
            //else if (status == 1) { Console.WriteLine("Dokument wymaga weryfikacji (1)"); }
            //else { Console.WriteLine("Dokument zweryfikowany (2)"); }

            ////Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 
            //int state = (data["state"]);
            //if (state == 0) { Console.WriteLine("Dokument został dodany (0)"); }
            //else if (state == 1) { Console.WriteLine("Dokument w przetworzeniu (1)"); }
            //else if (state == 2) { Console.WriteLine("Dokument zweryfikowany (2)"); }
            //else if (state == 3)
            //{
            //    Console.WriteLine("Dokument do weryfikacji (3)");

            //}

            //else { Console.WriteLine("Dokument wielostronicowy brak akcji (4)"); }

            _idBuyer = (data["company"]["id"]);
            _nipBuyer = (data["company"]["nip"]);
            _nameBuyer = (data["company"]["name"]);
            string adressBuyer = (data["company"]["address"]);
            _documentName = (data["name"]);

            //dane faktury
            foreach (var val in data["positions"])
            {
                string nameProduct = val["Nazwa"];
                int Ilosc = val["Ilosc"];
                int Validation = val["is_valid"];

                Console.WriteLine("Nazwa " + nameProduct + ", Ilosc " + Ilosc);
                Console.WriteLine("Brutto " + val["Brutto"] + ", Netto " + val["Netto"] + ", StawkaVAT " + val["StawkaVAT"] + ", kwota VAT " + val["VAT"]);
                if (Validation == 1) { Console.WriteLine("Poprawnie rozpoznane (" + Validation + ")."); }
                else { Console.WriteLine("Problem z rozpoznaniem warości (" + Validation + ")."); }
            }

            Console.WriteLine("GetDocumentById ->" + content);// odpowiedz

            return Execute<DocumentOneXt>(request);
        }// GetDocumentById(int id)

        /*{"id":8185910,
         * "user_id":7082762,
         * "uploaded_date":"2019-07-01 11:44:30.868114",
         * "uploaded_type":1,
         * "statusExp":0,
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
         * "contractor":{"id":9969824,"name":"\"LINDSTROM\" SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104","country_code":"PL","nip":"PL5222640524",
         * "address":"ul. Marywilska 34","akronim":"\"LINDSPL52226405WARSZAWA","city":"Warszawa","country":"","post_code":"03-228", "post_code_sane":"03228","extra":"","incidental":0},
         * 
         * "attributes":{"NrFaktury":{"attribute_id":1,"value":"3830097","is_valid":"1","statusExp":2,"left":"425","top":"318","right":"513","bottom":"335","page":0,"aspect_ratio":"3.24","user_id":7082762},
         * "SprzedawcaNazwa":{"attribute_id":2,"value":"\"LINDSTROM\" SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104","is_valid":"1","statusExp":0,"left":"46","top":"1031","right":"136","bottom":"1040","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SprzedawcaNip":{"attribute_id":3,"value":"PL5222640524","is_valid":"1","statusExp":0,"left":"394","top":"1020","right":"481","bottom":"1027","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SprzedawcaAdres":{"attribute_id":4,"value":"ul. Marywilska 34","is_valid":"1","statusExp":0,"left":"46","top":"1043","right":"123","bottom":"1051","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SprzedawcaKod":{"attribute_id":6,"value":"03-228","is_valid":"1","statusExp":0,"left":"45","top":"1055","right":"82","bottom":"1062","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SprzedawcaMiejscowosc":{"attribute_id":7,"value":"Warszawa","is_valid":"1","statusExp":0,"left":"56","top":"1043","right":"115","bottom":"1051","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaNazwa":{"attribute_id":8,"value":"KR GROUP SP\u00d3\u0141KA Z OGRANICZON\u0104 ODPOWIEDZIALNO\u015aCI\u0104 SP\u00d3\u0141KA KOMANDYTOWA","is_valid":"1","statusExp":0,"left":"80","top":"140","right":"192","bottom":"151","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaNip":{"attribute_id":9,"value":"1132602742","is_valid":"1","statusExp":0,"left":"446","top":"83","right":"509","bottom":"91","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaAdres":{"attribute_id":10,"value":"ul. Skaryszewska 7","is_valid":"1","statusExp":0,"left":"57","top":"157","right":"166","bottom":"168","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaKod":{"attribute_id":12,"value":"03-802","is_valid":"1","statusExp":0,"left":"57","top":"175","right":"102","bottom":"184","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NabywcaMiejscowosc":{"attribute_id":13,"value":"Warszawa","is_valid":"1","statusExp":0,"left":"57","top":"191","right":"94","bottom":"201","page":0,"aspect_ratio":"3.24","user_id":null},
         * "DataSprzedazy":{"attribute_id":14,"value":"2019-05-20","is_valid":"1","statusExp":0,"left":"686","top":"97","right":"743","bottom":"105","page":0,"aspect_ratio":"3.24","user_id":null},
         * "DataWystawienia":{"attribute_id":15,"value":"2019-05-20","is_valid":"1","statusExp":0,"left":"209","top":"99","right":"272","bottom":"108","page":0,"aspect_ratio":"3.24","user_id":null},
         * "TerminPlatnosci":{"attribute_id":16,"value":"2019-06-03","is_valid":"1","statusExp":0,"left":"686","top":"69","right":"743","bottom":"77","page":0,"aspect_ratio":"3.24","user_id":null},
         * "SposobPlatnosci":{"attribute_id":17,"value":"Przelew","is_valid":"0","statusExp":1,"left":"446","top":"97","right":"487","bottom":"107","page":0,"aspect_ratio":"3.24","user_id":null},
         * "Waluta":{"attribute_id":18,"value":"PLN","is_valid":"1","statusExp":0,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "RazemNetto":{"attribute_id":19,"value":"199.68","is_valid":"1","statusExp":0,"left":"513","top":"530","right":"551","bottom":"539","page":0,"aspect_ratio":"3.24","user_id":null},
         * "RazemVAT":{"attribute_id":20,"value":"45.93","is_valid":"1","statusExp":0,"left":"612","top":"530","right":"644","bottom":"539","page":0,"aspect_ratio":"3.24","user_id":null},
         * "RazemBrutto":{"attribute_id":21,"value":"245.61","is_valid":"1","statusExp":0,"left":"687","top":"530","right":"723","bottom":"539","page":0,"aspect_ratio":"3.24","user_id":null},
         * "MiesiacKsiegowy":{"attribute_id":22,"value":"2019-06","is_valid":"1","statusExp":0,"left":null,"top":null,"right":null,"bottom":null,"page":0,"aspect_ratio":null,"user_id":null},
         * "KontoBankowe":{"attribute_id":24,"value":"PL61236000050000004550302897","is_valid":"1","statusExp":0,"left":"547","top":"1043","right":"731","bottom":"1051","page":0,"aspect_ratio":"3.24","user_id":null},
         * "Zaplacono":{"attribute_id":25,"value":"Brak danych","is_valid":"0","statusExp":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "KursWaluty":{"attribute_id":26,"value":"1.0000","is_valid":"0","statusExp":0,"left":null,"top":null,"right":null,"bottom":null,"page":0,"aspect_ratio":null,"user_id":null},
         * "DataWplywu":{"attribute_id":29,"value":"2019-05-20","is_valid":"1","statusExp":0,"left":null,"top":null,"right":null,"bottom":null,"page":0,"aspect_ratio":null,"user_id":null},
         * "CzyNieKompletnaPozycja":{"attribute_id":80,"value":"false","is_valid":"0","statusExp":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NettoWalutaPodstawowa":{"attribute_id":120,"value":"199.68","is_valid":"0","statusExp":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "BruttoWalutaPodstawowa":{"attribute_id":121,"value":"245.61","is_valid":"0","statusExp":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "VatWalutaPodstawowa":{"attribute_id":122,"value":"45.93","is_valid":"0","statusExp":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},
         * "NrZamowienia":{"attribute_id":200,"value":"","is_valid":"0","statusExp":1,"left":"-1","top":"-1","right":"-1","bottom":"-1","page":0,"aspect_ratio":"3.24","user_id":null},"Kategoria":"","KategoriaId":"","CategoryDesc":"",
         * "Korygujaca":{"attribute_id":27,"value":"0","is_valid":0,"statusExp":1,"left":-1,"top":-1,"right":-1,"bottom":-1,"page":0,"aspect_ratio":1,"user_id":null},
         * "FakturaKorygowana":{"attribute_id":129,"value":"0","is_valid":0,"statusExp":1,"left":-1,"top":-1,"right":-1,"bottom":-1,"page":0,"aspect_ratio":1,"user_id":null},
         * "PrzyczynaKorekty":{"attribute_id":219,"value":"","is_valid":0,"statusExp":1,"left":-1,"top":-1,"right":-1,"bottom":-1,"page":0,"aspect_ratio":1,"user_id":null}},
         * 
         * "positions":[{"is_valid":"1","statusExp":0,"VAT":17.22,"Code":"MBW2","Kategoria":"","KategoriaId":"","CategoryDesc":"","Kategoria2":"","KategoriaId2":"",
         * "Category2Desc":"","e
         * 
         *_id":"","product_code":"","product_type":"","product_desc":"","Nazwa":"TYP TRANSAKCJI: WYNAJEM MBW2-MATA NIEBIESKA 115X200","Ilosc":1,"Jednostka":"","Cena":74.88,"Netto":74.88,"StawkaVAT":23,"Brutto":92.1,"IdProduct":""},
         * 
         * {"VAT":28.7,"Code":"MBW4","is_valid":"1","statusExp":0,"Kategoria":"","KategoriaId":"","CategoryDesc":"","Kategoria2":"","KategoriaId2":"","Category2Desc":"","ext_id":"","product_code":"","product_type":"","product_desc":"","Nazwa":"MBW4-MATA NIEBIESKA 150X300","Ilosc":1,"Jednostka":"","Cena":124.8,"Netto":124.8,"StawkaVAT":23,"Brutto":153.5,"IdProduct":""},{"is_valid":"1","statusExp":0,"VAT":0.01,"Kategoria":"","KategoriaId":"","CategoryDesc":"","Kategoria2":"","KategoriaId2":"","Category2Desc":"","ext_id":"","product_code":"","product_type":"","product_desc":"","Nazwa":"CENT_DIFFERENCE_1","Ilosc":1,"Jednostka":"","Cena":0,"Netto":0,"StawkaVAT":23,"Brutto":0.01,"IdProduct":""}],
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
