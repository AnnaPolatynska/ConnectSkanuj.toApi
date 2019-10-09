﻿using Newtonsoft.Json.Linq;
using nsSkanuj;
using nsXml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace nsTEST_Skanuj_to
{

    //connection stringi z appconfig
    //KRG411\SQLEXPRESS
    //<add name="ConnectionSQL" connectionString="Data source=10.0.1.155;database='vat';User id=vat;Password=95f)ek4n9!" providerName="System.Data.SqlClient" />
    //<add name = "ConnectionSQL" connectionString="Data source=KRG411\SQLEXPRESS;database='skanujTo';User id=skanuj;Password=alamakota" providerName="System.Data.SqlClient" />

    class Program
    {

        #region _ogólnodostępne
        //private string _endFileName = System.Configuration.ConfigurationManager.AppSettings["finalPath"].ToString();
        static bool _multi = false; //multipages True - powoduje analizę rozbicia dokumentów.Domyślnie false.
        public static string _connString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionSQL"].ConnectionString;
        public static int _idDocument; //id dokumentu do pobrania 8185910
        static string _fileName;
        public static int _J_idDocument;
        public static int _J_parent_doc_id;
        public static int _J_pages;//wielostronicowy
        public static int _J_stateDoc;//Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 
        public static string _J_nameDoc;
        public static int _J_for_process; // jeżeli 1 = do wygenerowania, 0 = wygenerowano xml
        public static string _J_uploadDate;// Data wgrania dokumentu
        public static int _J_user_id;
        //public static int _J_count_pages; // licznik stron
        public static int _newIdDocument; // id nowo wgranego dokumentu
        public static string _documentName; //nazwa wgranego dokumentu
        public static string _notice;
        //user
        public static string _tokenS; //token połączenia
        public static int _idUser; // id firmy wgrywającej dane = KRGroup: 7082762
        public static int _nipUserCompany; //Nip kr group
        public static string _nameUserCompany;
        public string _companyName;
        //buyer
        public static string _nipBuyer; //nip nabywcy 
        public static string _nameBuyer; //nazwa nabywcy
        public static int _idBuyer; //id nabywcy
        public static string _adressBuyer; // adres nabywcy
        //Contraktor
        public static string _nipContractor; //nip sprzedawcy
        public static int _idContractor;//id firmy z dokumentu do pobrania
        public static string _nameContractor; //nazwa firmy z dokumentu
        //public static string _adressContractor;// adres sprzedawcy
        public static int _statusDoc = 0; //statusExp - statusExp atrybutu (0 - nie wymagający weryfikacji, 1 - wymagający weryfikacji, 2 - zweryfikowany)
        public static int _stateForProcess; // status dokumentu w bazie
        //2DB
        public static int _2DB_doc_id;
        public static string _2DB_name;
        public static int _2DB_state;
        public static string _2DB_uploaded_date;
        public static int _2DB_user_id;
        public static int _2DB_parent_doc_id;
        public static int _2DB_pages;
        public static int _2DB_total_pages;
        public static int _2DB_start_page;
        public static int _2DB_end_page;
        //dane odczytane z db
        public static int _idDocInDB;
        public static int _stateDocInDB;
        //dane do xml
        public static string _BruttoWalutaPodstawowa;
        public static string _CategoryDesc;
        public static string _CzyNieKompletnaPozycja;
        public static string _DataSprzedazy;
        public static string _DataWplywu;
        public static string _DataWystawienia;
        public static string _FakturaKorygowana;
        public static string _Kategoria;
        public static string _KategoriaId;
        public static string _KontoBankowe;
        public static string _Korygujaca;
        public static string _KursWaluty;
        public static string _MiesiacKsiegowy;
        public static string _NabywcaAdres;
        public static string _NabywcaKod;
        public static string _NabywcaMiejscowosc;
        public static string _NabywcaNazwa;
        public static string _NabywcaNip;
        public static string _NettoWalutaPodstawowa;
        public static string _NrFaktury;
        public static string _NrZamowienia;
        public static string _PrzyczynaKorekty;
        public static string _RazemBrutto;
        public static string _RazemNetto;
        public static string _RazemVAT;
        public static string _SposobPlatnosci;
        public static string _SprzedawcaAdres;
        public static string _SprzedawcaKod;
        public static string _SprzedawcaMiejscowosc;
        public static string _SprzedawcaNazwa;
        public static string _SprzedawcaNip;
        public static string _TerminPlatnosci;
        public static string _VatWalutaPodstawowa;
        public static string _Waluta;
        public static string _Zaplacono;
        public static string _rBruttoWalutaPodstawowa;
        public static string _rCategoryDesc;
        public static string _rCzyNieKompletnaPozycja;
        public static string _rDataSprzedazy;
        public static string _rDataWplywu;
        public static string _rDataWystawienia;
        public static string _rFakturaKorygowana;
        public static string _rKategoria;
        public static string _rKategoriaId;
        public static string _rKontoBankowe;
        public static string _rKorygujaca;
        public static string _rKursWaluty;
        public static string _rMiesiacKsiegowy;
        public static string _rNabywcaAdres;
        public static string _rNabywcaKod;
        public static string _rNabywcaMiejscowosc;
        public static string _rNabywcaNazwa;
        public static string _rNabywcaNip;
        public static string _rNettoWalutaPodstawowa;
        public static string _rNrFaktury;
        public static string _rNrZamowienia;
        public static string _rPrzyczynaKorekty;
        public static string _rRazemBrutto;
        public static string _rRazemNetto;
        public static string _rRazemVAT;
        public static string _rSposobPlatnosci;
        public static string _rSprzedawcaAdres;
        public static string _rSprzedawcaKod;
        public static string _rSprzedawcaMiejscowosc;
        public static string _rSprzedawcaNazwa;
        public static string _rSprzedawcaNip;
        public static string _rTerminPlatnosci;
        public static string _rVatWalutaPodstawowa;
        public static string _rWaluta;
        public static string _rZaplacono;
        List<PozycjaXml> _listaPozycji = new List<PozycjaXml>(); //lista pozycji wyszczególnionych na fa.
        List<StronaPDF> _stronyPDF = new List<StronaPDF>();
        public PdfDocument _pdfDoc = null;

        #endregion

        public static void Main(string[] args)
        {
            Program program = new Program();
            program.WriteToFile(DateTime.Now + " program skanuj to uruchomiony");
            ////Logowanie, uzyskanie tokenu i pobranie danych firmy.
            try
            {
                _tokenS = program.GetToken("w.radzikowski@krgroup.pl", "5cfa2f8d51092").token;//ok pobiera token.
            }
            catch { program.WriteToFile("Problem z uzyskaniem tokenu."); }

            try
            {
                program.getUserCompany();//ok Pobranie danych firmy własnej id user(kr group) 7085933
            }
            catch { program.WriteToFile("Problem z pobraniem firmy " + _idUser + " - " + _nameUserCompany + " (" + DateTime.Now + ")."); }
            Console.WriteLine(" ");

            ////Wrzuca pliki z podanej lokalizacji do API
            string startPath = System.Configuration.ConfigurationManager.AppSettings["startPath"].ToString();
            string pdfPath = System.Configuration.ConfigurationManager.AppSettings["pdfPath"].ToString();

            // string finalPath = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine(startPath);

            //tworzy katalog dla pdf, jeżeli jeszcze nie został stworzony.
            if (!System.IO.Directory.Exists(pdfPath))
            {
                System.IO.Directory.CreateDirectory(pdfPath);
            }

            string[] files = System.IO.Directory.GetFiles(startPath, "*.pdf");
            foreach (string s in files)
            {
                System.IO.FileInfo fi = null;
                try
                {
                    fi = new System.IO.FileInfo(s);
                }
                catch (System.IO.FileNotFoundException e)
                {
                    program.WriteToFile(DateTime.Now + " " + e.Message + " - " + startPath);
                    continue;
                }
                //Console.WriteLine("Pliki: {0} : {1}", fi.Name, fi.Directory);


                //Wgranie dokumentu do Api - dane do MSSerwer.
                string fileName = fi.Name.ToString(); //dynamicznie zmieniana nazwa
                string path = fi.Directory.ToString() + "/" + fileName;
                program.uploadDocument(_idUser, fileName, path, _multi);

                //zapewnienie odpowiedniego kodowania pdf
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = File.Open(startPath + "\\" + fileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = XPdfForm.FromStream(stream))
                    { }
                }
                var inputDocument1 = PdfReader.Open(startPath + "\\" + fileName, PdfDocumentOpenMode.Import);

                string sfilename1 = fileName.Substring(0, (fileName.Length) - 4);

            }//foreach

            //OK pobiera listę gotowych dokumentów dla wszystkich dokumentów z jsona i zapis w dB
            program.GetDocumentList();

            program.Select();

            //program.SelectPages(sfilename1);// zapisuje do tabeli ilości stron dla szukanej nazwy dokumentu
            //int id = _2DB_doc_id;
            //program.SelectPages_Id(id);
            program.CuttingPDF();//tnie pdf na stony

            GenerateXMLFromDoc();  ////OK Generuje xml

            string name = _2DB_name;
            int End_page = _2DB_end_page;
            int Start_page = _2DB_start_page;

            //Console.WriteLine(sfilename1 + " " + id + " end " + End_page + " start " + Start_page);

            //int count = inputDocument1.PageCount;
            //int koniec = 0;
            //if (End_page <= count)
            //{
            //    koniec = count;
            //}
            //else
            //{
            //    koniec = End_page;
            //}
            //string filenamePDF = sfilename1 + "_" + id + "(" + Start_page + "-" + End_page + ")z" + koniec + ".pdf";

            //int indEnd = End_page - 1;
            //int indStart = Start_page - 1;
            //int str = End_page - Start_page;

            //var outputDocument = new PdfDocument();

            //if (str == 0)
            //{
            //    outputDocument.AddPage(inputDocument1.Pages[indEnd]);
            //    outputDocument.Save(pdfPath + "\\" + filenamePDF);
            //}
            //else
            //{
            //    var pg = 0;

            //    for (pg = (indStart); pg <= (indEnd); pg++)
            //    {
            //        outputDocument.AddPage(inputDocument1.Pages[pg]);
            //    }

            //    outputDocument.Save(pdfPath + "\\" + filenamePDF);
            //}

            //program.WriteToFile("Utworzono pdf o nazwie " + filenamePDF);

            //int licznik = 0;
            //if (_2DB_state == 5)
            //{
            //    Console.WriteLine(_J_idDocument + " jest dokumentem o statusie 5 - wielostronicowy zaprzestano dalszego przetwarzania w API.");
            //    //program.WriteToFile(_J_idDocument + "jest dokumentem o statusie 5 - wielostronicowy zaprzestano przetwarzania.");

            //    program.CuttingPDF();//tnie pdf na stony

            //    GenerateXMLFromDoc();  ////OK Generuje xml
            //}
            //else if (_2DB_state == 1)
            //{
            //    name = fileName;
            //    licznik = licznik++;
            //    program.WriteToFile(DateTime.Now + " Plik w przetwarzaniu przez API. Podjęto " + licznik + " próbę rozpoznania dokumentu " + nazwa);

            //    //Tworzy katalog dla plików z problemem
            //    string errorPath = System.Configuration.ConfigurationManager.AppSettings["errorPath"].ToString();
            //    if (!System.IO.Directory.Exists(errorPath))
            //    {
            //        System.IO.Directory.CreateDirectory(errorPath);
            //    }
            //    System.IO.Directory.SetCurrentDirectory(errorPath);

            //    if (licznik == 3)
            //    {
            //        System.IO.File.Move(startPath, errorPath);
            //    }
            //}
            //else if (_2DB_state == 0)
            //{
            //    string nazwa = fileName;
            //    licznik = licznik++;
            //    program.WriteToFile(DateTime.Now + " Plik poprawnie dodany do API. Podjęto " + licznik + " próbę rozpoznania dokumentu " + nazwa);

            //    //Tworzy katalog dla plików z problemem
            //    string errorPath = System.Configuration.ConfigurationManager.AppSettings["errorPath"].ToString();
            //    if (!System.IO.Directory.Exists(errorPath))
            //    {
            //        System.IO.Directory.CreateDirectory(errorPath);
            //    }
            //    System.IO.Directory.SetCurrentDirectory(errorPath);

            //    if (licznik == 3)
            //    {
            //        System.IO.File.Move(startPath, errorPath);
            //    }
            //}
            //else
            //{
            //    //program.CuttingPDF();//tnie pdf na stony

            //    GenerateXMLFromDoc();  ////OK Generuje xml
            //}



            //}//try uploadDocument
            //        catch
            //        {
            //            program.WriteToFile("Problem z wgraniem dokumentu " + _documentName + " - " + _newIdDocument + " (" + DateTime.Now + ").");
            //        }










            //LAB 

            #region Wgranie Wskazanego 1 Documentu do API
            ////WSKAZANIE Wgrania dokumentu do Api - dane do MSSerwer.
            //try
            //{

            //    program.uploadDocument(_idUser, _fileName, _path, _multi); //OK Wgranie dokumentu zwraca(_idDocument, _documentName, _2DB_state, _2DB_uploaded_date, _2DB_user_id, _2DB_validated, _notice)
            //    Console.WriteLine("notice " + _notice);
            //    Console.WriteLine("C _idDocument -> " + _idDocument.ToString() + " nazwa " + _documentName.ToString());


            //    if (_notice == "") //jeżeli plik jest nowy
            //    {
            //        InsertIntoDB();//OK zapisuje do bazy dane nowego dokumentu do śledzenia.

            //        Console.WriteLine("Plik o nazwie " + _documentName + " został poprawnie dodany (" + _uploadDate + "). Id dokumentu " + _idDocument + ").");
            //        //program.WriteToFile("Plik o nazwie " + _documentName + " został poprawnie dodany (" + _uploadDate + "). Id dokumentu " + _idDocument + ").");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Plik już istnieje.");
            //    }
            //}//try uploadDocument
            //catch { program.WriteToFile("Problem z wgraniem dokumentu " + _documentName + " - " + _idDocument + " (" + DateTime.Now + ")."); }
            #endregion
            //Console.WriteLine(" ");

            //sprawdzenie statusu dokumentów i aktualizacja w bazie.
            //Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 

            //UpdateStatusAllDocInDB();//OK Odczyt statusu Dokumentu z DB oraz jego aktualizacja w bazie po zmianie w json.

            //int badanyDoc = 8492820;
            //Console.WriteLine("Badany doc -> " + badanyDoc);
            //try
            //{
            //    program.GetDataFromDoc(badanyDoc); //OK pobiera dane z dokumentu o podanym id
            //}
            //catch { Console.WriteLine("nie istnieje dokument o podanym id " + badanyDoc); }
            //Console.WriteLine(" ");
            //program.FillPositionFromDocToXml(badanyDoc);//OK wgrywa poszczególne pozycję z fa do xml.
            //Console.WriteLine(" ");
            //program.CreateXML();//OK zapis danych do xml

            //int doc_id = 1234;
            //DeleteDB(doc_id); //OK Kasuje dane dokumentu po jego id.

            //UpdateStateDB(doc_id); //OK aktualizuje dane statusu dokumentu i validacji po id dokumentu


            //OK sprawdz czy dokument ma statusExp - statusExp atrybutu (0 - nie wymagający weryfikacji, 1 - wymagający weryfikacji, 2 - zweryfikowany)
            //Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 

            //program.WriteInfoToFile(badanyDoc);


            //program.GetVeryficationStateByIdDoc(badanyDoc); // stateDoc dokumentu o podanym id
            Console.WriteLine(" ");

            //AktualizujStateInDB();////Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 

            // FindStatusInDB(8401183); //OK sprawdza w Bazie status dokumentu o podanym id
            //Console.WriteLine(" FindStatusInDB - stateDoc => " + _stateForProcess);
            Console.WriteLine(" ");





            ////////////Porównuje status dokumentów
            //////////int idDocTEST = badanyDoc;//8401183;
            //////////int stateFPTEST = _statusDoc;//1;
            //////////int validTEST = 0;
            //////////if (stateDoc.ToString() == _stateForProcess.ToString())
            //////////{
            //////////    Console.WriteLine("Procesy są takie same. Status w bazie " + stateDoc.ToString() + " process " + _stateForProcess.ToString());
            //////////}
            //////////else
            //////////{
            //////////    Console.WriteLine("Procesy się różnią " + stateDoc.ToString() + " process " + _stateForProcess.ToString());
            //////////    AktualizujStatusDoc(idDocTEST, stateFPTEST, validTEST);
            //////////}
            //Console.WriteLine(" ");

            // AktualizujStatusDoc(idDocTEST, stateFPTEST, validTEST);

            //program.GetDataFromDoc(8492820);




            //_documentName = "3TEST.JPG";
            //program.GetIdDocByName( _documentName);
            //program.GetExportStatusByIdDoc(badanyDoc); // Sprawdza status dokumentu
            //TODO program.SetDocumentExportStatus(1, badanyDoc);

            // program.GetAllDocumentList();//OK sprawdza statusExp dokumentów
            // Console.WriteLine(" ");

            //Console.WriteLine(" ");
            //program.GetStateDocumentList(3); // Lista dokumentów gotowych do pobrania
            //Console.WriteLine(" ");

            //program.ListIdToVeryfication(stateDoc); //TODO lista znajduje wszystko

            //int doc = 8185917;
            //program.GetInfoIfDocumentExist(doc); //ok Lista pobiera dane o stanie wszystkich dokumentów


            // Console.WriteLine(" ");
            //_idUser = 8185903;
            //_documentName = "FV_30.PDF"; //id doc 8185904

            //program.GetIdListByCmpID(_idUser, _documentName); //lista po id company

            //////////Pobiera dane dokumentu po id
            ////////try
            ////////{
            ////////    program.GetDocumentById(badanyDoc); //ok zwraca dane dokumentu o ile dokument jest wgrany 
            ////////    Console.WriteLine("_idContractor ->" + _idContractor + " nazwa: " + _nameContractor + " NIP sprzedawcy: " + _nipContractor + " adres " + _adressContractor);
            ////////    Console.WriteLine("idBuyer -> " + _idBuyer + " nazwa " + _nameBuyer + " nip " + _nipBuyer + " adres " + _adressBuyer);
            ////////    Console.WriteLine("GetDocumentById(" + _idDocument + ")" + " nazwa dokumentu -> " + _documentName);
            ////////}
            ////////catch { Console.WriteLine("Nie można pobrać danych dokumentu " + _idDocument + " nazwa dokumentu-> " + _documentName + "."); }
            ////////Console.WriteLine(" ");

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
            // program.GetStatusDocumentList(_idUser);
            // Console.WriteLine(" ");

            //// program.getDocumentId(_idUser, _fileName, _path, _multi); //ok pobiera dane z tabeli wgranych dokumentów

            // Console.WriteLine("getDocumentId()" + _idDocument.ToString());//id dokumentu
            // Console.WriteLine(" ");


            int idDocToDelete = 8354510;
            //program.DeleteDocument(idDocToDelete); //Ok kasuje wskazany dokument
            //Console.WriteLine(" ");
            //////////////program.addCompanies();//dodaje do firmy kontrahentów

        }//Main

        #region logowanie_do_API
        // // // // // // // LOGOWANIE do API  https://app.skanuj.to/api // //
        /// <summary>
        /// LOGOWANIE
        /// </summary>
        /// <param name="email"></param>
        /// <param name="apikey"></param>
        /// <returns></returns>
        public nsSkanuj.Token GetToken(string email, string apikey)
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
            //Console.WriteLine("getUserCompany -> " + content);// odpowiedz
            return Execute<SkApiResponse>(request);
        }//getUserCompany()
        #endregion

        //public SkApiResponse ifDocumentExist(int company_id, int document_id)
        //{
        //    Program program = new Program();
        //    var client = new RestClient("http://app.skanuj.to/api");

        //    var request = new RestRequest(Method.POST);
        //    request.Resource = "document";
        //    request.AddHeader("token", _tokenS.ToString());
        //    request.AddHeader("id", _idUser.ToString());

        //    request.AddParameter("mode", "upload-file", ParameterType.GetOrPost);
        //    request.AddParameter("company_id", company_id, ParameterType.HttpHeader);
        //    request.AddParameter("doc_id", document_id, ParameterType.HttpHeader);
        //    request.AddParameter("source", "integracja", ParameterType.GetOrPost);
        //    request.AddParameter("response_type", "FULL", ParameterType.GetOrPost);

        //    IRestResponse restResponse = client.Execute(request);
        //    var content = restResponse.Content;
        //    var JsonArrayString = content;

        //    dynamic data = JObject.Parse(JsonArrayString);
        //    _idDocument = (data["good-uploads"][0]["doc_id"]);
        //    _documentName = (data["good-uploads"][0]["name"]);
        //    _uploadDate = (data["good-uploads"][0]["uploaded_date"]);
        //    _notice = (data["page_info"]["notice"]); //poprawność wgrania
        //    int infoState = (data["good-uploads"][0]["state"]);

        //    if (infoState == 5)
        //    {
        //        Console.WriteLine("dokement o id " + _idDocument + " state = 5 Dokument istnieje");// odpowiedz

        //    }
        //    else { Console.WriteLine("dokement o id " + _idDocument + "state = " + infoState); }

        //    Console.WriteLine(content);// odpowiedz
        //    return Execute<SkApiResponse>(request);
        //}//SkApiResponse


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

            _newIdDocument = (data["good-uploads"][0]["doc_id"]);
            _documentName = (data["good-uploads"][0]["name"]);
            _2DB_state = (data["good-uploads"][0]["state"]);
            _2DB_uploaded_date = (data["good-uploads"][0]["uploaded_date"]);
            _2DB_user_id = (data["good-uploads"][0]["user_id"]);
            _notice = (data["page_info"]["notice"]); //poprawność wgrania

            //Console.WriteLine(" uploadDocument --->>>>> _newIdDocument " + _newIdDocument + " _documentName " + _documentName + " _notice " + _notice + " _2DB_state " + _2DB_state + " _2DB_uploaded_date " + _2DB_uploaded_date + "  _2DB_user_id " + _2DB_user_id);
            Console.WriteLine("uploadDocument ->" + content);// odpowiedz
            return Execute<SkApiResponse>(request);
        }//SkApiResponse


        #region PDF
        public void CuttingPDF()
        {
            // Pocięcie istniejącego pdf na pojedyńcze faktury
            string startPath = System.Configuration.ConfigurationManager.AppSettings["startPath"].ToString();
            string pdfPath = System.Configuration.ConfigurationManager.AppSettings["pdfPath"].ToString();
            string[] filesPdf = System.IO.Directory.GetFiles(startPath, "*.pdf");



            // po wszystkich nazwach pdf
            foreach (string p in filesPdf)// dla każdego pliku w folderze
            {
                //odczytuje nazwy plików ze wskazanego folderu 
                System.IO.FileInfo file = null;
                try
                {
                    file = new System.IO.FileInfo(p);
                }
                catch (System.IO.FileNotFoundException e)
                {
                    continue;
                }
                string filename1 = file.Name.ToString(); //dynamicznie zmieniana nazwa

                //zapewnienie odpowiedniego kodowania pdf
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = File.Open(startPath + "\\" + filename1, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = XPdfForm.FromStream(stream))
                    { }
                }
                var inputDocument1 = PdfReader.Open(startPath + "\\" + filename1, PdfDocumentOpenMode.Import);

                string sfilename1 = filename1.Substring(0, (filename1.Length) - 4);
                //SelectPages(sfilename1);// zapisuje do tabeli ilości stron dla szukanej nazwy dokumentu

                foreach (StronaPDF strona in _stronyPDF)
                {
                    //int count = inputDocument1.PageCount;
                    int countDB = strona.Pages;
                    //int koniec = 0;
                    //if (strona.End_page <= count)
                    //{
                    //    koniec = count;
                    //}
                    //else
                    //{
                    //    koniec = strona.End_page;
                    //}
                    string filenamePDF = strona.NazwaDoc + "(" + strona.Start_page + "-" + strona.End_page + ")z" + strona.Pages + ".pdf";

                    int indEnd = strona.End_page - 1;
                    int indStart = strona.Start_page - 1;
                    int str = strona.End_page - strona.Start_page;

                    var outputDocument = new PdfDocument();

                    if (str == 0)
                    {
                        outputDocument.AddPage(inputDocument1.Pages[indEnd]);
                        outputDocument.Save(pdfPath + "\\" + filenamePDF);
                    }
                    else
                    {
                        var pg = 0;
                        try
                        {
                            for (pg = (indStart); pg <= (indEnd); pg++)
                            {
                                outputDocument.AddPage(inputDocument1.Pages[pg]);
                            }

                        }
                        catch { goto Koniec; }
                        outputDocument.Save(pdfPath + "\\" + filenamePDF);
                    }
                Koniec:;

                }//foreach
                WriteToFile("Utworzono pdf o nazwie " + filename1);
            }//foreach
        }//CuttingPDF()


        PdfDocument PdfDoc { get { return _pdfDoc; } }

        public System.IO.MemoryStream PdfStream
        {
            get
            {
                MemoryStream memStream = new MemoryStream();
                _pdfDoc.Save(memStream);
                return memStream;
            }
        }//public System.IO.MemoryStream PdfStream

        public void LoadPDFFile(string file)
        {
            _pdfDoc = new PdfDocument(file);

        }//LoadPDFFile(string file)

        /// <summary>
        /// Metoda dodająca do bieżącego PDF-a strony dokumentu przekazywanego jako parametr
        /// </summary>
        /// <param name="path">ścieżka do dokumentu,który należy dopisać</param>
        /// <returns>true, jeżeli wszystko ok, false w przeciwnym razie</returns>
        public bool Append(string path)
        {
            if (File.Exists(path))
            {
                PdfDocument appendedDoc = null;
                try
                {
                    appendedDoc = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                }
                catch { return false; }

                AppendPagesFromDoc(appendedDoc);
            }
            return true;
        }//public bool Append(string path)

        public bool Append(System.IO.Stream stream)
        {
            //Sprawdzenie czy plik istnieje
            if (stream != null)
            {
                //Jeżeli strumien jest poprawnym Pdfem
                PdfDocument appendedDoc = null;
                try
                {
                    appendedDoc = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                }
                catch
                {
                    return false;
                }

                //Przepisać strony z nowego dokumentu
                AppendPagesFromDoc(appendedDoc);
            }
            return true;
        }//public bool Append(Stream stream)


        /// <summary>
        /// Dopisuje do bieżącego dokumentu przekazany dokument
        /// </summary>
        /// <param name="appendedDoc">dokument ze stronami do dopisania do bieżącego dokumentu.</param>
        private void AppendPagesFromDoc(PdfDocument appendedDoc)
        {
            if (_pdfDoc == null)
            {
                //Jeżeli to jest pierwszy dodawany dokument
                _pdfDoc = appendedDoc;
            }
            else
            {
                Console.WriteLine("Kolejny dokument do dodania.");
                //Iteracja przez strony appendowanego dokumentu
                for (int i = 0; i < appendedDoc.PageCount; i++)
                {
                    PdfPage page = appendedDoc.Pages[i];
                    _pdfDoc.AddPage(page);
                }
            }
        }//private void AppendPagesFromDoc(PdfDocument appendedDoc)

        public void SavePdf(string path)
        {
            _pdfDoc.Save(path);
        }//Save

        public void SavePdf(System.IO.Stream stream)
        {
            _pdfDoc.Save(stream);
        }





        #endregion

        #region 2DB

        public static void DeleteDB(int id_doc)
        {
            string connString = _connString;
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlDelete = "DELETE FROM dbo.WgraneDoc WHERE doc_id = " + id_doc + ";";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlDelete, sqlConnection))
                {
                    sqlConnection.Open();

                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
        }//DeleteDB(int id_doc)

        public static void DeleteOldIDFromDB(int idDoc)
        {
            string connString = _connString;
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlDelete = "DELETE FROM dbo.WgraneDoc WHERE id = " + idDoc + ";";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlDelete, sqlConnection))
                {
                    sqlConnection.Open();

                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
        }//DeleteDB(int id_doc)


        public static void DeleteFromDBPages0()
        {
            string connString = _connString;
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlDelete = "DELETE FROM dbo.WgraneDoc;";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlDelete, sqlConnection))
                {
                    sqlConnection.Open();

                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
        }//DeleteDB(int id_doc)


        /// <summary>
        /// Aktualizuje dane statusu w bazie danych
        /// </summary>
        /// <param name="id_doc"></param>
        public static void UpdateStateDB(int id_doc)
        {
            int state = _J_stateDoc;
            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlUpdate = "UPDATE dbo.WgraneDoc SET state = @state WHERE doc_id = " + id_doc + ";";

            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlUpdate, sqlConnection1))
                {
                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@state", state);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }
            }
        }//UpdateStateDB(int id_doc)

        public static void UpdatePagesDB(int docId)
        {
            int pages = int.Parse(_J_pages.ToString());

            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlUpdate = "UPDATE dbo.WgraneDoc SET pages = @pages WHERE doc_id = " + docId + ";";

            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlUpdate, sqlConnection1))
                {
                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@pages", pages);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }
            }
            Console.WriteLine("UpdatePagesDB " + docId + " pages " + pages);
        }//UpdateParentIdDB(string docName)
        public static void UpdateParentIdDB(string docName)
        {
            int idDoc = _J_idDocument;
            int idParentDoc = _J_parent_doc_id;

            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlUpdate = "UPDATE dbo.WgraneDoc SET parent_doc_id = @parent_doc_id WHERE name = " + docName + ";";

            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlUpdate, sqlConnection1))
                {
                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@parent_doc_id", idParentDoc);
                    //command.Parameters.AddWithValue("@doc_id", idDoc);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }
            }
        }//UpdateParentIdDB(string docName)

        /// <summary>
        /// Aktualizuje dane dokumentu (id, parent_doc_id, pages) w bazie danych po nazie dokumentu.
        /// </summary>
        /// <param name="nazwaDoc">nazwa dokumentu</param>
        public static void UpdateDataInDB(string nazwaDoc)
        {
            int state = _J_stateDoc;
            int idDoc = _J_idDocument;
            int pages = _J_pages;
            int parent_doc_id = _J_parent_doc_id;

            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlUpdate = "UPDATE dbo.WgraneDoc SET state = @state, id = @id, pages = @pages, parent_doc_id = @parent_doc_id WHERE name = '" + nazwaDoc.ToString() + "';";

            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlUpdate, sqlConnection1))
                {
                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@id", idDoc);
                    command.Parameters.AddWithValue("@pages", pages);
                    command.Parameters.AddWithValue("@parent_doc_id", parent_doc_id);
                    command.Parameters.AddWithValue("@state", state);
                    //command.Parameters.AddWithValue("@name", nazwaDoc);
                    Console.WriteLine("UpdateDataInDB(string nazwaDoc) " + idDoc + " nazwa " + _J_nameDoc + " parent " + parent_doc_id + " pages " + pages);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }
            }
        }//UpdateStateDB(int id_doc)

        /// <summary>
        /// Aktualizuje dane dokumentu (id, parent_doc_id, pages) w bazie danych po nazie dokumentu.
        /// </summary>
        /// <param name="nazwaDoc">nazwa dokumentu</param>
        public static void UpdateDataInDB(int id_Doc)
        {
            int state = _J_stateDoc;
            int pages = _J_pages;
            int parent_doc_id = _J_parent_doc_id;
            string nazwaDoc = _J_nameDoc.ToString();

            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlUpdate = "UPDATE dbo.WgraneDoc SET state = @state, name = @name, pages = @pages, parent_doc_id = @parent_doc_id, for_process = @for_process, error = @error WHERE doc_id = " + id_Doc + ";";

            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlUpdate, sqlConnection1))
                {
                    sqlConnection1.Open();
                    //command.Parameters.AddWithValue("@id", id_Doc);
                    command.Parameters.AddWithValue("@pages", pages);
                    command.Parameters.AddWithValue("@parent_doc_id", parent_doc_id);
                    command.Parameters.AddWithValue("@state", state);
                    command.Parameters.AddWithValue("@name", nazwaDoc);
                    command.Parameters.AddWithValue("@for_process", true);
                    command.Parameters.AddWithValue("@error", 0);
                    Console.WriteLine("UpdateDataInDB(string nazwaDoc) " + id_Doc + " nazwa " + _J_nameDoc + " parent " + parent_doc_id + " pages " + pages);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }
            }
        }//UpdateStateDB(int id_doc)

        public static void InsertIntoDB()
        {
            string connString = _connString;
            //SqlConnection sqlConnection = new SqlConnection(connString);
            //SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM dbo.WgraneDoc;", sqlConnection);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            //sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            //sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlInsert = ("INSERT INTO dbo.WgraneDoc (doc_id, name, state, uploaded_date, user_id, parent_doc_id, pages, for_process, error) VALUES (@doc_id, @name, @state, @uploaded_date, @user_id, @parent_doc_id, @pages, @for_process, @error);");
            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlInsert, sqlConnection1))
                {
                    int idDoc = _newIdDocument;
                    string short_documentName = _documentName.Substring(0, _documentName.Length - 4);
                    int parent_doc_id = 0;
                    int pages = 0;
                    bool for_process = true;
                    int error = 0;

                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@doc_id", value: idDoc);
                    command.Parameters.AddWithValue("@name", short_documentName);
                    command.Parameters.AddWithValue("@state", value: _2DB_state);
                    command.Parameters.AddWithValue("@uploaded_date", value: _2DB_uploaded_date);
                    command.Parameters.AddWithValue("@user_id", value: _2DB_user_id);
                    command.Parameters.AddWithValue("parent_doc_id", value: parent_doc_id);
                    command.Parameters.AddWithValue("pages", value: pages);
                    command.Parameters.AddWithValue("for_process", value: for_process);
                    command.Parameters.AddWithValue("error", value: error);
                    //Console.WriteLine("InsertIntoDB() --->>>>>  _newIdDocument " + _newIdDocument + " _documentName " + _documentName + " _notice " + _notice + " _2DB_state " + _2DB_state + " _2DB_uploaded_date " + _2DB_uploaded_date + _2DB_user_id " + _2DB_user_id);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }//using
            }//using

        }//InsertIntoDB()

        public static void InsertAllIntoDB()
        {
            string connString = _connString;
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlInsert = ("INSERT INTO dbo.WgraneDoc (doc_id, name, state, uploaded_date, user_id, parent_doc_id, pages, for_process, start_page, end_page, error) VALUES (@doc_id, @name, @state, @uploaded_date, @user_id, @parent_doc_id, @pages, @for_process, @start_page, @end_page, @error);");
            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlInsert, sqlConnection1))
                {
                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@doc_id", _J_idDocument);
                    command.Parameters.AddWithValue("@name", _J_nameDoc);
                    command.Parameters.AddWithValue("@state", _J_stateDoc);
                    command.Parameters.AddWithValue("@uploaded_date", _J_uploadDate);
                    command.Parameters.AddWithValue("@user_id", _J_user_id);
                    command.Parameters.AddWithValue("parent_doc_id", _J_parent_doc_id);
                    command.Parameters.AddWithValue("pages", _J_pages);
                    command.Parameters.AddWithValue("for_process", _J_for_process);
                    command.Parameters.AddWithValue("start_page", 0);
                    command.Parameters.AddWithValue("end_page", 0);
                    command.Parameters.AddWithValue("error", 0);
                    //command.Parameters.AddWithValue("count_pages", _J_count_pages);
                    // Console.WriteLine("InsertIntoDB() --->>>>>  " + _J_nameDoc + "ilość stron " + _J_pages + " id doc " + _J_idDocument + " id parent " + _J_parent_doc_id + " " + _J_uploadDate + " user " + _J_user_id);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }//using
            }//using

        }//InsertIntoDB()


        public static void UpdateForProcessDB(int docId)
        {
            int for_process = int.Parse(_J_for_process.ToString());

            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlUpdate = "UPDATE dbo.WgraneDoc SET for_process = @for_process WHERE doc_id = " + docId + ";";

            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlUpdate, sqlConnection1))
                {
                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@for_process", for_process);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }
            }
        }//UpdateForProcessDB


        /// <summary>
        /// Aktualizuje numery stron dokumentu w bazie.
        /// </summary>
        public static void UpdateStronyInDB(int docId)
        {
            int start_page = int.Parse(_2DB_start_page.ToString());
            int end_page = int.Parse(_2DB_end_page.ToString());

            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            var sqlUpdate = "UPDATE dbo.WgraneDoc SET start_page = @start_page, end_page = @end_page WHERE doc_id = " + docId + ";";

            using (SqlConnection sqlConnection1 = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(sqlUpdate, sqlConnection1))
                {
                    sqlConnection1.Open();
                    command.Parameters.AddWithValue("@start_page", start_page);
                    command.Parameters.AddWithValue("@end_page", end_page);
                    command.ExecuteNonQuery();
                    sqlConnection1.Close();
                }
            }

        }//UpdateForProcessDB

        /// <summary>
        /// aktualizuje id, parent_doc_id, strony
        /// </summary>
        public static void UpdateInfoDocInDB()
        {
            Program program = new Program();

            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            // "_stateForProcess": 1, /* 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM dbo.WgraneDoc;", sqlConnection);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                _2DB_doc_id = int.Parse(dataRow["doc_id"].ToString());
                _2DB_name = dataRow["name"].ToString();
                _2DB_pages = int.Parse((dataRow[@"pages"].ToString()));
                _2DB_parent_doc_id = int.Parse((dataRow[@"parent_doc_id"].ToString()));

                Console.WriteLine("w DB Dokument " + _2DB_name + " ma " + _2DB_pages + " stron ");

                //sprawdzenie z jsona
                program.GetInfoDocumentList(_2DB_name);

            }// foreach
            sqlConnection.Close();
        }//UpdateInfoDocInDB()

        public void SelectDataToDB(int idDoc)
        {
            Program program = new Program();
            try
            {
                string connString = _connString;
                SqlConnection sqlConnection = new SqlConnection(connString);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * from dbo.WgraneDoc WHERE doc_id = " + idDoc + ";", sqlConnection);
                DataSet dataSet = new DataSet("dbo.WgraneDoc");
                sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
                sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
                DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

                bool id_exist = false;
                int DB_doc_id = 0;
                string DB_nazwa = string.Empty;
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    DB_doc_id = int.Parse(dataRow[@"doc_id"].ToString());
                }
                if (idDoc == DB_doc_id)
                {
                    id_exist = true;
                }

                if (id_exist == true)
                {
                    UpdateDataInDB(idDoc);
                }
                else
                {
                    InsertAllIntoDB(); //OK Wgrywa wszystko z jsona.
                }
            }
            catch { program.WriteToFile("problem z aktualizacją dancy w DB."); }
        }//Select


        public void Select()
        {
            Program program = new Program();
            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * from dbo.WgraneDoc;", sqlConnection);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                _2DB_name = (dataRow[@"name"].ToString());
                _2DB_doc_id = int.Parse(dataRow[@"doc_id"].ToString());
                _2DB_parent_doc_id = int.Parse(dataRow[@"parent_doc_id"].ToString());
                _2DB_pages = int.Parse(dataRow[@"pages"].ToString());
                _2DB_start_page = int.Parse(dataRow[@"start_page"].ToString());
                _2DB_end_page = int.Parse(dataRow[@"end_page"].ToString());
                var strona = new nsSkanuj.StronaPDF();
                strona.Doc_id = _2DB_doc_id;
                strona.NazwaDoc = _2DB_name.ToString();
                strona.Start_page = _2DB_start_page;
                strona.End_page = _2DB_end_page;
                strona.Parent_doc_id = _2DB_parent_doc_id;
                strona.Pages = _2DB_pages;

                _stronyPDF.Add(new StronaPDF(strona.NazwaDoc, strona.Doc_id, strona.Start_page, strona.End_page, strona.Pages, strona.Parent_doc_id));
                Console.WriteLine("Select " + _2DB_name + "id " + _2DB_doc_id + " parent_doc_id " + _2DB_parent_doc_id + " pages " + _2DB_pages + " start " + _2DB_start_page + " end " + _2DB_end_page);

            }
        }


        /// <summary>
        /// Odczytuje dane dokumentu z tabeli w bazie.
        /// </summary>
        /// <param name="nazwaDoc"></param>
        public void SelectPages(string nazwaDoc)
        {
            Program program = new Program();
            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * from dbo.WgraneDoc WHERE name = '" + nazwaDoc + "';", sqlConnection);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                _2DB_name = (dataRow[@"name"].ToString());
                _2DB_doc_id = int.Parse(dataRow[@"doc_id"].ToString());
                _2DB_parent_doc_id = int.Parse(dataRow[@"parent_doc_id"].ToString());
                _2DB_pages = int.Parse(dataRow[@"pages"].ToString());
                _2DB_start_page = int.Parse(dataRow[@"start_page"].ToString());
                _2DB_end_page = int.Parse(dataRow[@"end_page"].ToString());

                //var strona = new nsSkanuj.StronaPDF();
                //strona.Doc_id = _2DB_doc_id;
                //strona.NazwaDoc = _2DB_name.ToString();
                //strona.Start_page = _2DB_start_page;
                //strona.End_page = _2DB_end_page;
                //strona.Parent_doc_id = _2DB_parent_doc_id;
                //strona.Pages = _2DB_pages;

                //_stronyPDF.Add(new StronaPDF(strona.NazwaDoc, strona.Doc_id, strona.Start_page, strona.End_page, strona.Pages, strona.Parent_doc_id));
                //Console.WriteLine("SelectPages(string nazwaDoc) " + _2DB_name + "id " + _2DB_doc_id + " parent_doc_id " + _2DB_parent_doc_id + " pages " + _2DB_pages + " start " + _2DB_start_page + " end " + _2DB_end_page);
            }//foreach

        }//SelectPages(int idDoc)



        public void SelectPages_Id(int idDoc)
        {
            Program program = new Program();
            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * from dbo.WgraneDoc WHERE doc_id = " + idDoc + ";", sqlConnection);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                _2DB_name = (dataRow[@"name"].ToString());
                _2DB_doc_id = int.Parse(dataRow[@"doc_id"].ToString());
                _2DB_parent_doc_id = int.Parse(dataRow[@"parent_doc_id"].ToString());
                _2DB_pages = int.Parse(dataRow[@"pages"].ToString());
                _2DB_start_page = int.Parse(dataRow[@"start_page"].ToString());
                _2DB_end_page = int.Parse(dataRow[@"end_page"].ToString());

                Console.WriteLine(" ");
                Console.WriteLine("Select id " + _2DB_name + "id " + _2DB_doc_id + " parent_doc_id " + _2DB_parent_doc_id + " pages " + _2DB_pages + " start " + _2DB_start_page + " end " + _2DB_end_page);
                Console.WriteLine(" ");
                string pdfPath = System.Configuration.ConfigurationManager.AppSettings["pdfPath"].ToString();
                string startPath = System.Configuration.ConfigurationManager.AppSettings["startPath"].ToString();
                int counter = dataTable.Rows.Count;

                string Pdf_name = _2DB_name + ".pdf";

                var inputDocument1 = PdfReader.Open(startPath + "\\" + Pdf_name, PdfDocumentOpenMode.Import);


                int koniec = 0;

                string filenamePDF = _2DB_name + "_" + _2DB_doc_id + "(" + _2DB_start_page + "-" + _2DB_end_page + ")z" + koniec + ".pdf";

                int indEnd = _2DB_end_page - 1;
                int indStart = _2DB_start_page - 1;
                int str = _2DB_end_page - _2DB_start_page;

                var outputDocument = new PdfDocument();

                if (str == 0)
                {
                    outputDocument.AddPage(inputDocument1.Pages[indEnd]);
                    outputDocument.Save(pdfPath + "\\" + filenamePDF);
                }
                else
                {
                    var pg = 0;

                    for (pg = (indStart); pg <= (indEnd); pg++)
                    {
                        outputDocument.AddPage(inputDocument1.Pages[pg]);
                    }

                    outputDocument.Save(pdfPath + "\\" + filenamePDF);

                }
                //var strona = new nsSkanuj.StronaPDF();
                //strona.Doc_id = _2DB_doc_id;
                //strona.NazwaDoc = _2DB_name.ToString();
                //strona.Start_page = _2DB_start_page;
                //strona.End_page = _2DB_end_page;
                //strona.Parent_doc_id = _2DB_parent_doc_id;
                //strona.Pages = _2DB_pages;

                //_stronyPDF.Add(new StronaPDF(strona.NazwaDoc, strona.Doc_id, strona.Start_page, strona.End_page, strona.Pages, strona.Parent_doc_id));

            }

        }//SelectPages(int idDoc)
        public static void CountPages(int idDoc)
        {
            Program program = new Program();
            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * from[Nr_STRON] WHERE doc_id = " + idDoc + ";", sqlConnection);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                //zapis ilości stron z widoku
                //_2DB_doc_id = int.Parse(dataRow["doc_id"].ToString());
                //_2DB_name = dataRow["name"].ToString();
                //_2DB_parent_doc_id = int.Parse((dataRow[@"parent_doc_id"].ToString()));

                //_2DB_pages = int.Parse((dataRow[@"pages"].ToString()));
                _2DB_start_page = int.Parse((dataRow[@"start"].ToString()));
                _2DB_end_page = int.Parse((dataRow[@"end"].ToString()));
                // Console.WriteLine("w DB Dokument " + _2DB_name + " ma " + _2DB_pages + " stron ");

                //sprawdzenie z jsona
                // program.GetInfoDocumentList(_2DB_name);

            }// foreach
            sqlConnection.Close();
        }//CountPages()






        /// <summary>
        /// Generuje xml z dokumentów w DB
        /// </summary>
        public static void GenerateXMLFromDoc()
        {
            Program program = new Program();

            string connString = _connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            // "_stateForProcess": 1, /* 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM dbo.WgraneDoc;", sqlConnection);
            DataSet dataSet = new DataSet("dbo.WgraneDoc");
            sqlDataAdapter.FillSchema(dataSet, SchemaType.Source, "dbo.WgraneDoc");
            sqlDataAdapter.Fill(dataSet, "dbo.WgraneDoc");
            DataTable dataTable = dataSet.Tables["dbo.WgraneDoc"];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                int idDocum = int.Parse(dataRow["doc_id"].ToString());
                _documentName = dataRow["name"].ToString();
                _J_parent_doc_id = int.Parse(dataRow["parent_doc_id"].ToString());
                _J_pages = int.Parse(dataRow["pages"].ToString());
                _J_for_process = 0;
                try
                {
                    program.GetDataFromDoc(idDocum); //OK pobiera dane z dokumentu o podanym id
                }
                catch { program.WriteToFile(DateTime.Now + "GetDataFromDoc -> nie istnieje dokument o podanym id " + idDocum); }
                Console.WriteLine(" ");
                try
                {
                    program.FillPositionFromDocToXml(idDocum);//OK wgrywa poszczególne pozycję z fa do xml.
                    Console.WriteLine(DateTime.Now + " FillPositionFromDocToXml -> wgrano dane do XML z API dla dokumentu " + idDocum);
                }

                catch { Console.WriteLine(DateTime.Now + " FillPositionFromDocToXml -> problem z pobraniem danych z API dotyczy dokumentu " + idDocum); }



                program.CreateXML();//OK zapis danych do xml

                //odhaczenie pliku, z którego wygenerowano xml => czy wykasować plik z API

                if (_J_for_process == 0)
                {
                    UpdateForProcessDB(idDocum);
                    // TODO odblokować  DeleteDB(idDocum);

                    // TODO odblokować wykasowanie dokumentów z API     program.DeleteDocument(idDocum);
                }
                else
                {
                    UpdateForProcessDB(idDocum);
                }

                _idDocument = idDocum;
            }// foreach

            sqlConnection.Close();
        }//GenerateXMLFromDoc

        public List<DocumentList> GetInfoFromDocumentList(int idDoc)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/search";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());
            //request.AddParameter("id", idDoc, ParameterType.HttpHeader);
            request.AddQueryParameter("id", idDoc.ToString());

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            int dlugosc = data.all_count;
            dynamic data1 = JObject.Parse(jArray[dlugosc - 1].ToString());
            _J_stateDoc = data1.state;
            _J_nameDoc = data1.name;
            _J_idDocument = data1.id;

            request.AddQueryParameter("state", _J_stateDoc.ToString());
            request.AddQueryParameter("name", _J_nameDoc.ToString());

            var result = content.Select(data2 => new { _J_idDocument, _J_nameDoc, _J_stateDoc }).ToList();

            Console.WriteLine("lista " + _J_idDocument + " " + _J_nameDoc + " " + _J_stateDoc);
            Console.WriteLine(" ");
            //Console.WriteLine("id " + _idDocument + " nazwa " + _J_nameDoc + " status " + _J_stateDoc);
            // Console.WriteLine("GetStatusDocumentList search-> " + content);// odpowiedz
            Console.WriteLine(" ");
            return Execute<List<DocumentList>>(request);
        }//GetStatusDocumentList(int idDoc)


        /// <summary>
        /// OK lista dokumentów po id danego usera.
        /// </summary>
        /// <param name="id">id dokumentu</param>
        /// <returns></returns>
        public List<DocumentList> GetStatusDocumentList(int idDoc)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/search";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());
            request.AddParameter("id", idDoc, ParameterType.HttpHeader);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            int dlugosc = data.all_count;
            Console.WriteLine("");
            for (int i = 0; i <= (dlugosc - 1); i++)
            {
                dynamic data2 = JObject.Parse(jArray[i].ToString());
                //odcięcie rozszerzenia z nazwy plików
                string long_J_nameDoc = data2.name;

                _J_nameDoc = long_J_nameDoc.Substring(0, long_J_nameDoc.Length - 4);
            }
            Console.WriteLine(" ");
            return Execute<List<DocumentList>>(request);
        }//GetStatusDocumentList(int idDoc)

        /// <summary>
        /// Pobiera z JSON listę dokumentów i zapisuje do bazy danych
        /// </summary>
        /// <returns></returns>
        public List<DocumentList> GetDocumentList()
        {
            Program program = new Program();

            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/search";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());
            // request.AddParameter("name", nameDoc, ParameterType.HttpHeader);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            int id_doc = 0;

            int dlugosc = data.all_count;
            Console.WriteLine("");
            for (int i = 0; i <= (dlugosc - 1); i++)
            {
                try
                {
                    dynamic data1 = JObject.Parse(jArray[i].ToString());
                    //odcięcie rozszerzenia z nazwy plików
                    string long_J_nameDoc = data1.name;
                    _J_idDocument = data1.id;
                    _J_nameDoc = long_J_nameDoc.Substring(0, long_J_nameDoc.Length - 4);
                    _J_stateDoc = data1.state;
                    _J_uploadDate = data1.uploaded_date;
                    _J_pages = data1.pages;
                    _J_for_process = 1;
                    _J_user_id = data1.user_id;

                    // jeżeli _J_parent_doc_id jest NULLem wstaw 0
                    int? jparent_doc_id = (data1.parent_doc_id);
                    int b = jparent_doc_id ?? 0;
                    if (b > 0)
                    {
                        _J_parent_doc_id = (data1.parent_doc_id);
                    }
                    else
                    {
                        _J_parent_doc_id = 0;
                    }
                    id_doc = _J_idDocument;

                    SelectDataToDB(id_doc);

                    if (_J_parent_doc_id != 0)
                    {
                        CountPages(id_doc);//liczy strony wg widoku
                    }
                    else
                    {
                        _2DB_end_page = 1;
                        _2DB_start_page = 1;
                    }
                    Console.WriteLine("DANE Z JSON -> " + _J_nameDoc + "ilość stron " + _J_pages + " id doc " + _J_idDocument + " id parent " + _J_parent_doc_id + " " + _J_uploadDate + " user " + _J_user_id + " start " + _2DB_start_page + " end " + _2DB_end_page);

                    UpdateStronyInDB(id_doc);// aktualizuje start i end strony 

                    program.WriteToFile("Poprawnie dodano dane JSON dla dokumentu " + id_doc);
                    //SelectPages(id_doc);
                }
                catch
                {
                    program.WriteToFile("Problem z wgraniem danych JSON dla dokumentu " + id_doc);
                }
            }//for


            //Console.WriteLine("GetInfoDocumentList search-> " + content);// odpowiedz
            return Execute<List<DocumentList>>(request);
        }//GetInfoDocumentList(string nameDoc)


        public List<DocumentList> GetInfoDocumentList(string nameDoc)
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document/mode/search";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());
            request.AddParameter("name", nameDoc, ParameterType.HttpHeader);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            JArray jArray = JArray.Parse(JsonArrayString);
            dynamic data = JObject.Parse(jArray[0].ToString());
            int dlugosc = data.all_count;
            Console.WriteLine("");
            for (int i = 0; i <= (dlugosc - 1); i++)
            {
                dynamic data1 = JObject.Parse(jArray[i].ToString());
                //odcięcie rozszerzenia z nazwy plików
                string long_J_nameDoc = data1.name;
                _J_nameDoc = long_J_nameDoc.Substring(0, long_J_nameDoc.Length - 4);

                if (_J_nameDoc == _2DB_name)
                {
                    _J_idDocument = data1.id;
                    _J_pages = data1.pages;
                    _J_stateDoc = data1.state;


                    // jeżeli _J_parent_doc_id  jest NULLem wstaw 0
                    int? jparent_doc_id = (data1.parent_doc_id);
                    int b = jparent_doc_id ?? 0;
                    if (b > 0)
                    {
                        _J_parent_doc_id = (data1.parent_doc_id);
                    }
                    else
                    {
                        _J_parent_doc_id = 0;
                    }
                    //Console.WriteLine("DANE Z JSON - > " + _J_nameDoc + "ilość stron " + _J_pages + " id doc " + _J_idDocument + " id parent " + _J_parent_doc_id);



                    //aktualizuje nr stron w bazie.
                    int idDoc = _J_idDocument;
                    if (_2DB_pages != _J_pages)
                    {
                        //Console.WriteLine(" id doc " + _J_idDocument + " parent " + _J_parent_doc_id + " " + _J_nameDoc + "ilość stron w bazie " + _2DB_pages + " rożni się od j " + _J_pages);
                        //try
                        //{
                        UpdatePagesDB(idDoc);
                        //Console.WriteLine("Zaktualizowano nr stron w DB");
                        //}
                        //catch
                        //{
                        //    Console.WriteLine(" problem z aktualizacją stron dokumentu " + idDoc);
                        //}  
                    }
                    else
                    {
                        //Console.WriteLine(" id doc " + _J_idDocument + " parent " + _J_parent_doc_id + " " + _J_nameDoc + " ilość stron w bazie " + _2DB_pages + " nr stron zgodne z j " + _J_pages);
                    }


                    //jeżeli nie posiada stron wykasuj
                    //if (_2DB_pages == 0)
                    //{
                    //    int idDoWykasowania = _J_idDocument;
                    //    Console.WriteLine(_J_idDocument + " dokument nie posiada stron");
                    //    DeleteOldIDFromDB(idDoWykasowania);
                    //}



                    //jeżeli _J_parent_doc_id posiada numer inny niż 0 
                    if (_J_parent_doc_id != 0)
                    {
                        _J_idDocument = data1.id;
                        _2DB_user_id = data1.user_id;
                        _2DB_uploaded_date = data1.uploaded_date;
                        //               InsertIntoDB(_J_nameDoc);
                        //                 Console.WriteLine("Dopisano do bazy id_children " + _J_nameDoc);
                        //update nr strony
                        //UpdatePagesDB(_J_nameDoc);

                    }
                    else
                    {
                        Console.WriteLine(_J_nameDoc + " jest " + _J_pages + " stronicowe " + _J_idDocument);
                        if (_2DB_pages == 0)
                        {
                            DeleteFromDBPages0();
                            Console.WriteLine("wykasowano " + _J_pages + " stronicowy " + _J_idDocument);
                        }
                    }

                }//if
                 // else { Console.WriteLine("niedopasowanie " + _2DB_name + " różne " + _J_nameDoc); }
            }//for

            //Console.WriteLine("GetInfoDocumentList serch-> " + content);// odpowiedz
            Console.WriteLine(" ");
            return Execute<List<DocumentList>>(request);
        }//GetInfoDocumentList(string nameDoc)

        #endregion

        /// <summary>
        /// Loggi do pliku w katalogu ArchiwumX
        /// </summary>
        /// <param name="Message"></param>
        public void WriteToFile(string Message)
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + "\\ArchiwumX";
            string logpath = System.Configuration.ConfigurationManager.AppSettings["logPath"].ToString();
            if (!Directory.Exists(logpath))
            {
                Directory.CreateDirectory(logpath);
            }
            System.IO.Directory.SetCurrentDirectory(logpath);

            //currentDirName = System.IO.Directory.GetCurrentDirectory();
            //Console.WriteLine("currentDirName " + currentDirName);
            //string logFilepath = AppDomain.CurrentDomain.BaseDirectory + "\\ArchiwumX\\ServiceLog.txt";
            string logFilepath = logpath + "\\ServiceLog.txt";
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
            // request.AddParameter("_stateForProcess", stateDoc, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;

            dynamic data = JObject.Parse(JsonArrayString);
            string deleteInfo = data["code"];
            string cont = "98";
            if (deleteInfo == cont)
            {
                WriteToFile(DateTime.Now + "Problem ze skasowaniem dokumentu o ID " + idDoc + " API zwraca komunikat -> " + deleteInfo);
            }
            else
            {
                WriteToFile(DateTime.Now + " Pomyślnie skasowano dokument z API " + idDoc + ".");
            }
            //Console.WriteLine("DeleteDocument -> " + content);// odpowiedz
            return Execute<SkApiResponse>(request);

        }//SkApiResponse

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
        /// Pobiera listę wszystkich dokumentów 
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
            Console.WriteLine("GetInfoIfDocumentExist() -> " + content);// zwraca listę wszystkich dokumentów _stateForProcess 2 i 3
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

            //Console.WriteLine("GetStatusDocumentList -> " + content);// odpowiedz
            return Execute<List<DocumentList>>(request);
        }//GetExportedDocumentList

        //{"id":8185917,"user_id":7082762,



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

        // // // // // // Funkcja pobierająca dane pojedynczego dokumentu - rozszerzane dane.


        public DocumentOneXt GetIdParentIdFromDoc(int idDoc) // // // // ok
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", idDoc, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);

            string long_J_NameDoc = (data["name"]);
            _J_nameDoc = long_J_NameDoc.Substring(0, long_J_NameDoc.Length - 4);

            _J_idDocument = (data["id"]);
            _J_pages = data["pages"];

            // jeżeli _J_parent_doc_id  jest NULLem wstaw 0
            int? jparent_doc_id = (data["parent_doc_id"]);
            int b = jparent_doc_id ?? 0;
            if (b > 0)
            {
                _J_parent_doc_id = (data["parent_doc_id"]);
                Console.WriteLine(" id " + _idDocument + " J nazwa " + _J_nameDoc + "_J_parent_doc_id " + _J_parent_doc_id);
            }
            else
            {
                _J_parent_doc_id = 0;
                Console.WriteLine(" id " + _idDocument + " J nazwa " + _J_nameDoc + "_J_parent_doc_id " + _J_parent_doc_id);
            }

            Console.WriteLine(" ");

            //Console.WriteLine("GetIdParentIdFromDoc(int idDoc) " + content);
            return Execute<DocumentOneXt>(request);
        }//GetIdParentIdFromDoc(int idDoc)



        #region Dane_do_XML
        /// <summary>
        /// OK Odszukuje id dokumentu wgranego wcześniej po nazwie dokumentu.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>dane dokumentu z okresleniem poprawności rozpoznania poszczególnych pozycji.</returns>
        public DocumentOneXt GetDataFromDoc(int idDoc) // // // // ok
        {
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", idDoc, ParameterType.GetOrPost);

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
                _BruttoWalutaPodstawowa = value;
                _rBruttoWalutaPodstawowa = rozp;
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
                _CategoryDesc = value1;
                _rCategoryDesc = rozp1;
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
                _CzyNieKompletnaPozycja = value2;
                _rCzyNieKompletnaPozycja = rozp2;
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
                _DataSprzedazy = value3;
                _rDataSprzedazy = rozp3;
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
                _DataWplywu = value4;
                _rDataWplywu = rozp4;
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
                _DataWystawienia = value6;
                _rDataWystawienia = rozp6;
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
                _FakturaKorygowana = value7;
                _rFakturaKorygowana = rozp7;
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
                _Kategoria = value8;
                _rKategoria = rozp8;
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
                _KategoriaId = value5;
                _rKategoriaId = rozp5;
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
                _KontoBankowe = value9;
                _rKontoBankowe = rozp9;
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
                _Korygujaca = value10;
                _rKorygujaca = rozp10;
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
                _KursWaluty = value11;
                _rKursWaluty = rozp11;
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
                _MiesiacKsiegowy = value12;
                _rMiesiacKsiegowy = rozp12;
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
                _NabywcaAdres = value13;
                _rNabywcaAdres = rozp13;
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
                _NabywcaKod = value14;
                _rNabywcaKod = rozp14;
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
                _NabywcaMiejscowosc = value15;
                _rNabywcaMiejscowosc = rozp15;
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
                _NabywcaNazwa = value16;
                _rNabywcaNazwa = rozp16;
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
                _NabywcaNip = value17;
                _rNabywcaNip = rozp17;
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
                _NettoWalutaPodstawowa = value18;
                _rNettoWalutaPodstawowa = rozp18;
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
                _NrFaktury = value19;
                _rNrFaktury = rozp19;
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
                _NrZamowienia = value20;
                _rNrZamowienia = rozp20;
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
                _PrzyczynaKorekty = value21;
                _rPrzyczynaKorekty = rozp21;
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
                _RazemBrutto = value22;
                _rRazemBrutto = rozp22;
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
                _RazemNetto = value23;
                _rRazemNetto = rozp23;
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
                _RazemVAT = value24;
                _rRazemVAT = rozp24;
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
                _SposobPlatnosci = value25;
                _rSposobPlatnosci = rozp25;
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
                _SprzedawcaAdres = value26;
                _rSprzedawcaAdres = rozp26;
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
                _SprzedawcaKod = value27;
                _rSprzedawcaKod = rozp27;
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
                _SprzedawcaMiejscowosc = value28;
                _rSprzedawcaMiejscowosc = rozp28;
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
                _SprzedawcaNazwa = value29;
                _rSprzedawcaNazwa = rozp29;
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
                _SprzedawcaNip = value30;
                _rSprzedawcaNip = rozp30;
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
                _TerminPlatnosci = value31;
                _rTerminPlatnosci = rozp31;
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
                _VatWalutaPodstawowa = value32;
                _rVatWalutaPodstawowa = rozp32;
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
                _Waluta = value33;
                _rWaluta = rozp33;
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
                _Zaplacono = value34;
                _rZaplacono = rozp34;
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
            _documentName = (data["name"]);

            //Console.WriteLine("GetDataFromDoc " + content);
            return Execute<DocumentOneXt>(request);
        }//GetDataFromDoc(int id)



        /// <summary>
        /// Zapisuje do tabeli pozycję art z faktury z walidacją (do zapisu w XML)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DocumentOneXt FillPositionFromDocToXml(int idDoc) // // // // ok
        {
            Program program = new Program();
            var client = new RestClient("http://app.skanuj.to/api");
            var request = new RestRequest();
            request.Resource = "document";
            request.AddHeader("token", _tokenS.ToString());
            request.AddHeader("company_id", _idUser.ToString());

            request.AddParameter("mode", "one-xt", ParameterType.GetOrPost);
            request.AddParameter("id", idDoc, ParameterType.GetOrPost);

            IRestResponse restResponse = client.Execute(request);
            var content = restResponse.Content;
            var JsonArrayString = content;
            dynamic data = JObject.Parse(JsonArrayString);
            _documentName = (data["name"]);

            //pobranie rozpoznanych wartości dokumentu. 

            foreach (var val in data["positions"])
            {
                try
                {
                    //Array.Resize(ref data["positions"], suma + 1);
                    //dane z Json
                    string idProduct = val["IdProduct"];
                    string product_code = val["product_code"];
                    string nazwa = val["Nazwa"];
                    double ilosc = val["Ilosc"];
                    string jednostka = val["Jednostka"];
                    double cena = val["Cena"];
                    double brutto = val["Brutto"];
                    double netto = val["Netto"];
                    int stawkaVAT = val["StawkaVAT"];
                    double vAT = val["VAT"];
                    int validation = val["is_valid"]; // pobiera validation 

                    var pozycja = new nsXml.PozycjaXml();
                    pozycja.IdProduct = idProduct.ToString();
                    pozycja.Product_code = product_code.ToString();
                    pozycja.Nazwa = nazwa.ToString();
                    pozycja.Ilosc = double.Parse(ilosc.ToString());
                    pozycja.Jednostka = jednostka.ToString();
                    pozycja.Cena = double.Parse(cena.ToString());
                    pozycja.Brutto = double.Parse(brutto.ToString());
                    pozycja.Netto = double.Parse(netto.ToString());
                    pozycja.StawkaVAT = int.Parse(stawkaVAT.ToString());
                    pozycja.Vat = double.Parse(vAT.ToString());
                    pozycja.Validation = double.Parse(validation.ToString());

                    _listaPozycji.Add(new PozycjaXml(pozycja.IdProduct, pozycja.Product_code, pozycja.Nazwa, pozycja.Ilosc, pozycja.Jednostka, pozycja.Cena, pozycja.Brutto, pozycja.Netto, pozycja.StawkaVAT, pozycja.Vat, pozycja.Validation));
                    WriteToFile("Poprawnie rozpoznano wartości dokumentu XML z API.");
                }
                catch { }
            }//foreach
            return Execute<DocumentOneXt>(request);
        }//FillPositionFromDocToXml(int id)
        #endregion

        /// <summary>
        /// Tworzy strukturę xml.
        /// </summary>
        /// <param name="filepath"></param>
        public void CreateXML()
        {
            XDocument xml = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Dokument", new XAttribute("version", "2.0"),
                new XElement("Dane", new XElement("Test", "1.0"),
                 new XElement("Nabywca",
                     new XElement("NabywcaAdres", new XAttribute("is_valid", _rNabywcaAdres), _NabywcaAdres),
                     new XElement("NabywcaKod", new XAttribute("is_valid", _rNabywcaKod), _NabywcaKod),
                     new XElement("NabywcaMiejscowosc", new XAttribute("is_valid", _rNabywcaMiejscowosc), _NabywcaMiejscowosc),
                     new XElement("NabywcaNazwa", new XAttribute("is_valid", _rNabywcaNazwa), _NabywcaNazwa),
                     new XElement("NabywcaNip", new XAttribute("is_valid", _rNabywcaNip), _NabywcaNip)
                ),
                new XElement("Sprzedawca",
                     new XElement("SprzedawcaAdres", new XAttribute("is_valid", _rSprzedawcaAdres), _SprzedawcaAdres),
                     new XElement("SprzedawcaKod", new XAttribute("is_valid", _rSprzedawcaKod), _SprzedawcaKod),
                     new XElement("SprzedawcaMiejscowosc", new XAttribute("is_valid", _rSprzedawcaMiejscowosc), _SprzedawcaMiejscowosc),
                     new XElement("SprzedawcaNazwa", new XAttribute("is_valid", _rSprzedawcaNazwa), _SprzedawcaNazwa),
                     new XElement("SprzedawcaNip", new XAttribute("is_valid", _rSprzedawcaNip), _SprzedawcaNip),
                     new XElement("KontoBankowe", new XAttribute("is_valid", _rKontoBankowe), _KontoBankowe)
                ),
                new XElement("Numeracja_i_daty",
                     new XElement("NrFaktury", new XAttribute("is_valid", _rNrFaktury), _NrFaktury),
                     new XElement("FakturaKorygowana", new XAttribute("is_valid", _rFakturaKorygowana), _FakturaKorygowana),
                     new XElement("Korygujaca", new XAttribute("is_valid", _rKorygujaca), _Korygujaca),
                     new XElement("PrzyczynaKorekty", new XAttribute("is_valid", _rPrzyczynaKorekty), _PrzyczynaKorekty),
                     new XElement("DataSprzedazy", new XAttribute("is_valid", _rDataSprzedazy), _DataSprzedazy),
                     new XElement("DataWplywu", new XAttribute("is_valid", _rDataWplywu), _DataWplywu),
                     new XElement("DataWystawienia", new XAttribute("is_valid", _rDataWystawienia), _DataWystawienia),
                     new XElement("MiesiacKsiegowy", new XAttribute("is_valid", _rMiesiacKsiegowy), _MiesiacKsiegowy)
                     ),
                new XElement("Kwoty",
                     new XElement("NrZamowienia", new XAttribute("is_valid", _rNrZamowienia), _NrZamowienia),
                     // new XElement("CzyNieKompletnaPozycja", new XAttribute("is_valid", _rCategoryDesc), _CategoryDesc),
                     new XElement("CzyNieKompletnaPozycja", new XAttribute("is_valid", _rCzyNieKompletnaPozycja), _CzyNieKompletnaPozycja),
                     //new XElement("Kategoria", new XAttribute("is_valid", _rKategoria), _Kategoria),
                     //new XElement("KategoriaId", new XAttribute("is_valid",_rKategoriaId), _KategoriaId),
                     new XElement("Waluta", new XAttribute("is_valid", _rWaluta), _Waluta),
                     new XElement("KursWaluty", new XAttribute("is_valid", _rKursWaluty), _KursWaluty),
                        new XElement("Pozycje",
                        from pozycja in _listaPozycji
                        select new XElement("pozycja", new XAttribute("is_valid", pozycja.Validation),
                            new XElement("Nazwa", pozycja.Nazwa),
                            new XElement("Brutto", pozycja.Brutto),
                            new XElement("Cena", pozycja.Cena),
                            new XElement("IdProductField", pozycja.IdProduct),
                            new XElement("Ilosc", pozycja.Ilosc),
                            new XElement("Jednostka", pozycja.Jednostka),
                            new XElement("Netto", pozycja.Netto),
                            new XElement("StawkaVAT", pozycja.StawkaVAT),
                             new XElement("VAT", pozycja.Vat),
                            new XElement("product_code", pozycja.Product_code)
                        ),
                     new XElement("BruttoWalutaPodstawowa", new XAttribute("is_valid", _rBruttoWalutaPodstawowa), _BruttoWalutaPodstawowa),
                     new XElement("NettoWalutaPodstawowa", new XAttribute("is_valid", _rNettoWalutaPodstawowa), _NettoWalutaPodstawowa),
                     new XElement("RazemNetto", new XAttribute("is_valid", _rRazemNetto), _RazemNetto),
                     new XElement("RazemVAT", new XAttribute("is_valid", _rRazemVAT), _RazemVAT),
                     new XElement("VatWalutaPodstawowa", new XAttribute("is_valid", _rVatWalutaPodstawowa), _VatWalutaPodstawowa),
                     new XElement("RazemBrutto", new XAttribute("is_valid", _rRazemBrutto), _RazemBrutto),
                     new XElement("SposobPlatnosci", new XAttribute("is_valid", _rSposobPlatnosci), _SposobPlatnosci),
                     new XElement("TerminPlatnosci", new XAttribute("is_valid", _rTerminPlatnosci), _TerminPlatnosci),
                     new XElement("Zaplacono", new XAttribute("is_valid", _rZaplacono), _Zaplacono)
                )
                ))));

            string longnazwa = _documentName.ToString().Replace('/', '_');
            string nazwa = longnazwa.Substring(0, longnazwa.Length - 4);

            string filename = nazwa + "_" + _idDocument + ".txt";

            string endpath = System.Configuration.ConfigurationManager.AppSettings["endPath"].ToString();
            if (!System.IO.Directory.Exists(endpath))
            {
                System.IO.Directory.CreateDirectory(endpath);
            }
            System.IO.Directory.SetCurrentDirectory(endpath);

            string currentDirName = System.IO.Directory.GetCurrentDirectory();
            //Console.WriteLine("currentDirName " + currentDirName);

            string path = currentDirName;
            string filepath = path + filename.ToString();

            if (!File.Exists(filepath))
            {
                try
                {
                    _J_for_process = 0;
                    xml.Save(filename);
                    WriteToFile("Wygenerowano " + filename + " " + DateTime.Now);

                }
                catch { WriteToFile("Problem z wygenerowaniem pliku" + filename + " " + DateTime.Now); }
            }
            else
            {
                try
                {
                    xml.Save(filename);
                    WriteToFile("Zaktualizowano plik " + filename + " " + DateTime.Now);
                    _J_for_process = 0;
                }
                catch
                {
                    WriteToFile("Problem z wygenerowaniem pliku" + filename + " " + DateTime.Now);
                }
            }

        }//CreateXML(string filename)


        //public void WriteInfoToFile(int idDoc)
        //{
        //    

        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    if (!File.Exists(filepath))
        //    {
        //        try
        //        {
        //            CreateXML(filepath);
        //            WriteToFile("Wygenerowano " + filename + " " + DateTime.Now);
        //        }
        //        catch { WriteToFile("Problem z wygenerowaniem pliku" + filename + " " + DateTime.Now); }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            CreateXML(filepath);
        //            WriteToFile("Zaktualizowano plik " + filename + " " + DateTime.Now);
        //        }
        //        catch
        //        { WriteToFile("Problem z wygenerowaniem pliku" + filename + " " + DateTime.Now); }
        //    }

        //}// WriteInfoToFile

        /// <summary>
        /// Pokazuje poziom odczytu pozycji w dokumencie o podanym id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DocumentOneXt GetDataVeryficationByIdDoc(int id) // // // // ok
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

            //dane faktury
            foreach (var val in data["positions"])
            {
                string nameProduct = val["Nazwa"];
                int Ilosc = val["Ilosc"];
                int Validation = val["is_valid"]; // pobiera validation 
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

            Console.WriteLine(" GetVeryficationStateByIdDoc(int id) " + content);
            return Execute<DocumentOneXt>(request);
        }// GetDocumentById(int id)



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
            //data = JArray.Parse(content);

            _documentName = (data[@"name"]);
            _stateForProcess = (data[@"state"]);////Stan: 0 dodany 1 w przetwarzaniu 2 zweryfikowany 3 do weryfikacji 4 wielostronicowy brak akcji 

            if (_stateForProcess == 0) { Console.WriteLine("Dokument " + _documentName + " został dodany (0)"); }
            else if (_stateForProcess == 1)
            {
                Console.WriteLine("Dokument " + _documentName + "  w przetworzeniu (1)");
            }
            else if (_stateForProcess == 2)
            {
                Console.WriteLine("Dokument " + _documentName + "  poprawnie zweryfikowany (2)");
            }
            else if (_stateForProcess == 3)
            {
                Console.WriteLine("Dokument  " + _documentName + " do weryfikacji (3).");
                //dane faktury
                foreach (var val in data["positions"])
                {
                    string nameProduct = val["Nazwa"];
                    int Ilosc = val["Ilosc"];
                    int Validation = val["is_valid"]; // pobiera validation 
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
            _J_stateDoc = _stateForProcess;
            Console.WriteLine("stateDoc => " + _J_stateDoc);

            //Console.WriteLine(" GetVeryficationStateByIdDoc(int id) " + content);
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
            //int _stateForProcess = (data["_stateForProcess"]);
            //if (_stateForProcess == 0) { Console.WriteLine("Dokument został dodany (0)"); }
            //else if (_stateForProcess == 1) { Console.WriteLine("Dokument w przetworzeniu (1)"); }
            //else if (_stateForProcess == 2) { Console.WriteLine("Dokument zweryfikowany (2)"); }
            //else if (_stateForProcess == 3)
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

    }//Program

}//nsTEST_Skanuj_to
