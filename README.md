# ConnectSkanuj.toApi
Program napisany w c# /.net (Visual Studio) przy użyciu MS SQL Server. Ma na celu usprawnić proces obiegu dokumentów w KR Group Sp. z o.o. sp. K.

Założenia wstępne do programu:
1. Skany faktur wrzucone do wskazanego folderu są ładowane do API Skanuj.to.
2. Po rozpoznaniu dokumentów pobierana jest odpowiedz (JSON). Każda rozpoznana wartość ma być przepisana wraz z szacunkiem trafności jej rozpoznania w pliki xml do wskazanego folderu.
3. Pdf wejściowe są wielostronicowe, program ma ciąć je na serię dokumentów pdf, które zawierają pojedyncze faktury wraz z załącznikami.
4. Program ma zapisywać najważniejsze zdarzenia do pliku w wybranej lokalizacji.
5. Foldery docelowe dla plików, ścieżkę logowania do bazy i inne najważniejsze parametry programu mają być określone w pliku konfiguracyjnym.
