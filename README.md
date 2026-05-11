# Productivity Planner - Aplikacja i API
Projekt stworzony w .NET 10, który składa się z dwóch części:

1. Web API (serwer i baza danych ASP.NET Core)

2. Aplikacji mobilnej (klient .NET MAUI na system Android)

3. Aplikacji Webowej w technologii Angular

4.  Aplikacji desktopowej w technologii WPF
   
# Sposób uruchomienia:

Początkowo musimy uruchomić za pomocą visual studio przynajmmniej 2 projekty naraz (API i wybraną aplikacje), aby to zrobić musimy:

1. Kliknąć prawym przyciskiem myszy na solucję (samą górę drzewa projektów po prawej) i wybrać Properties (Właściwości).

2. Wejść w zakładkę Startup Project.

3. Zaznaczyć Multiple startup projects.

4. Ustaw akcję Start dla **ProductivityPlanner.API** i wybranej aplikacji np. **ProductivityPlanner.Mobile** (Dodając debugowanie android)

5. Kliknij zastosuj

**Upewnij się że w plikach serwisowych port zgadza się z portem przez który łączysz się z API**

# Uruchomienie

Naciśnij F5 i zaczekaj aż wszystko się uruchomi

utwórz konto za pomocą panelu swagger (link: https://localhost:port/swagger) **w miejscu port podaj swój port za pomocą którego łączysz się z API**
