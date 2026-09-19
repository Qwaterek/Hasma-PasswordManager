<img width="414" height="520" alt="obraz" src="https://github.com/user-attachments/assets/4cbfc99f-b9ca-4900-91e5-018760facec4" />





EN----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Hasma, a minimalist password manager with a password generator and auto-login, written in C#.

The program is available in two languages: Polish and English (PL/EN).

What it includes:
Password generation — three levels: Weak (8 characters, letters), Medium (12 characters, +numbers), Strong (16 characters, +special characters).
Separate “Quick Password” mode — combines Polish words (animals, baked goods, cities...) with random Leetspeak and uppercase letters, resulting in passwords that are easy to remember.
Account manager — add, edit, search (by URL/username/email), automatically downloads the site’s favicon, and copies to the clipboard with a single click.
Export/import settings — a separate encrypted file for transferring accounts between devices, protected by your own password.
Master Password — the account database is encrypted with a key derived from your master password; without it, no one (including the program’s author) can read the stored data. You can change the password at any time (Tools → Change Master Password).
Auto-login — Selenium (Chrome/Firefox) opens each account in a separate tab and allows you to log in automatically.
Keyboard shortcuts, PL/EN.

Keyboard shortcuts:
F1 = Keyboard shortcuts menu
Ctrl+N = Create a new account
F3 = Open the accounts panel
Enter = Set focus on the search field and select text
F4 = Open the quick password generator
Ctrl+F5 = Save configurations
Ctrl+F6 = Load configurations
Ctrl+L = Log in to all portals
Double-click left mouse button = Copy to clipboard


System Requirements:
Windows 10/11
.NET Runtime
Chrome/Firefox (for auto-login)

Currently, Auto-login is in beta and may not work on all websites.

How the master password works and why it’s secure:

The program does NOT store your master password on disk in any form—neither
in plain text, nor encrypted, nor as a hash derived directly from it.
Only an AES-256 encryption key is derived from the password using the
PBKDF2-HMAC-SHA256 function (310,000 iterations, in accordance with OWASP recommendations).

As a result:
The same key can only be reconstructed by knowing the correct password

The 310,000 PBKDF2 iterations intentionally slow down any attempt to guess the password
  (a so-called “brute-force” attack)—cracking even a medium-strength password by trial and error
  would take an attacker many years of computation, even with access to the program’s files.

The accounts.json file, which stores your accounts, is encrypted with this
  very key (AES-256). Without knowing the master password, the file is
  an unreadable string of bytes—even to the program’s author.

The program verifies the password’s validity not by comparing it to the stored
  password, but by attempting to decrypt the “verifier”
  (the vault.meta.json file contains only random data and an encrypted
  checksum—never the key or password itself).

The same mechanism (password → PBKDF2 → AES-256 key, without saving the key)
also protects the configuration export files—each export file has its own random
data, so cracking one export file in no way makes it easier to crack
another one or the main account database.

IF YOU FORGET YOUR MASTER PASSWORD, THERE IS NO WAY TO RECOVER YOUR DATA, neither by the program’s author nor by anyone else.


Quick Password mode selects from a pool of 125 words and combines them into a random sequence, allowing you to create passwords that are easy to remember but difficult to guess.
    These passwords can include numbers and special characters, and can also be modified using Leetspeak and uppercase letters.

Exporting and importing configurations allows you to easily transfer accounts between different devices while maintaining data security.
    Each export is protected by a password you specify at the time of export (PBKDF2 + AES-256)—the file itself does not contain the decryption key,
	so without that password, the data in the file cannot be recovered.

This is a hobby project that has not been audited by external
security experts. The encryption mechanism is robust and consistent with standard
industry practice, but treat it as “reasonable data protection on
disk” rather than a fully certified enterprise-class solution.

I USED AI (Claude, GitHub Copilot) TO CREATE THIS PROGRAM (& I used Deepl to translate that text)
PL----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Hasma, minimalistyczny menedżer haseł z generatorem haseł i auto-logowaniem, napisanym w c#.

Program dostępny jest w 2 językach: polskim i angielskim(PL/EN).

co zawiera:
Generowanie haseł — trzy poziomy: Weak (8 znaków, litery), Medium (12 znaków, +cyfry), Strong (16 znaków, +znaki specjalne).
Osobny tryb "Quick Password" — skleja Polskie wyrazy (zwierzęta, wypieki, miasta...) z losowym leetspeakiem i wielkimi literami, czyli hasła łatwe do zapamiętania.
Menedżer kont — dodawanie, edycja, wyszukiwarka (po URL/loginie/mailu), favicon strony pobierany automatycznie, kopiowanie do schowka jednym kliknięciem.
Export/import configu — osobny zaszyfrowany plik do przenoszenia kont między urządzeniami, chroniony własnym hasłem.
Hasło główne (Master Password) — magazyn kont jest zaszyfrowany kluczem wyprowadzonym z Twojego hasła głównego; bez niego nikt (łącznie z autorem programu) nie odczyta zapisanych danych. Hasło można zmienić w dowolnym momencie (Tools → Change Master Password).
Auto-login — Selenium (Chrome/Firefox) otwiera każde konto w osobnej karcie i pozwala się zalogować automatycznie.
Skróty klawiszowe, PL/EN.

skróty klawiszowe:
F1 = Menu skrótów klawiszowych
Ctrl+N = Stwórz nowe konto
F3 = Otwórz panel kont
Enter = Ustaw fokus w polu wyszukiwania i zaznacz tekst
F4 = Otwórz generator szybkich haseł 
Ctrl+F5 = Zapisz konfiguracje
Ctrl+F6 = Wczytaj konfiguracje
Ctrl+L = Zaloguj się tto wszystkich portali
Podwójne kliknięcie LPM = Kopiuj do schowka
Przytrzymaj PPM = Odkrycie hasła

Wymagania sprzetowe:
Windows 10/11
.NET Runtime
Chrome/Firefox (do auto-logowania)

Aktualnie na tą chwile Auto-login jest w wersji beta i może nie działać na wszystkich stronach.

Jak działa hasło główne i dlaczego jest to bezpieczne:

Program NIE zapisuje Twojego hasła głównego na dysku w żadnej postaci - ani
jawnej, ani zaszyfrowanej, ani jako hash bezpośrednio z niego liczony.
Z hasła wyprowadzany jest jedynie klucz szyfrujący AES-256, za pomocą funkcji
PBKDF2-HMAC-SHA256 (310 000 iteracji, zgodnie z zaleceniami OWASP).

Dzięki temu:
Ten sam klucz da się odtworzyć wyłącznie znając poprawne hasło

310 000 iteracji PBKDF2 celowo spowalnia każdą próbę odgadnięcia hasła
  (tzw. brute-force) - złamanie nawet średniej jakości hasła metodą prób i błędów
  zajęłoby atakującemu wiele lat obliczeń, nawet mając dostęp do plików programu.

Plik accounts.json, w którym trzymane są Twoje konta, jest zaszyfrowany tym
  właśnie kluczem (AES-256). Bez znajomości hasła głównego plik jest
  nieczytelnym ciągiem bajtów - również dla autora programu.

Program rozpoznaje poprawność hasła nie przez porównywanie go z zapisanym
  hasłem, tylko poprzez próbę odszyfrowania "weryfikatora"
  (plik vault.meta.json zawiera tylko losowe dane i zaszyfrowany fragment
  kontrolny - nigdy sam klucz ani hasło).

Ten sam mechanizm (hasło → PBKDF2 → klucz AES-256, bez zapisywania klucza)
chroni też pliki eksportu konfiguracji - każdy plik eksportu ma własne, losowe
dane, więc złamanie jednego pliku eksportu w żaden sposób nie ułatwia złamania
innego ani głównego magazynu kont.

JEŚLI ZAPOMNISZ HASŁA GŁÓWNEGO, NIE MA MOŻLIWOŚCI ODZYSKANIA DANYCH, ani przez autora programu, ani przez nikogo innego.


Tryb Quick Password wybiera z pośród 125 słów i je łączy w losową kombinację, co pozwala na tworzenie haseł, które są łatwe do zapamiętania, ale trudne do odgadnięcia. 
	Hasła te mogą zawierać cyfry i znaki specjalne, a także mogą być modyfikowane za pomocą leetspeaku i wielkich liter.

Eksportowanie i importowanie konfiguracji pozwala na łatwe przenoszenie kont między różnymi urządzeniami, zachowując przy tym bezpieczeństwo danych. 
	Każdy eksport jest chroniony hasłem podanym przez Ciebie w momencie eksportu (PBKDF2 + AES-256) — sam plik nie zawiera klucza deszyfrującego, 
	więc bez tego hasła danych z pliku nie da się odzyskać.

Jest to projekt hobbystyczny, nieaudytowany przez zewnętrznych specjalistów
od bezpieczeństwa. Mechanizm szyfrowania jest solidny i zgodny ze standardową
praktyką branżową, ale traktuj go jako "rozsądne zabezpieczenie danych na
dysku", a nie jako w pełni certyfikowane rozwiązanie klasy enterprise.

DO STWORZENIA TEGO PROGRAMU UŻYŁEM AI (Claude, GitHub Copilot)
