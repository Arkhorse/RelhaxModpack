﻿using System.Collections;

namespace RelhaxModpack
{
    //A static class to exist throughout the entire application life, will always have translations
    public static class Translations
    {
        //Enumerator to determine which translated string to return
        public enum Languages { English = 0, German = 1, Polish = 2 };
        public static Languages language = Languages.English;//set it to this default
        public static Hashtable english = new Hashtable();
        public static Hashtable german = new Hashtable();
        public static Hashtable polish = new Hashtable();
        //load hashes on application startup

        public static string getTranslatedString(string componetName)
        {
            switch (language)
            {
                case (Languages.English):
                    if (english.Contains(componetName))
                    {
                        return (string)english[componetName];
                    }
                    break;
                case (Languages.German):
                    if (german.Contains(componetName))
                    {
                        return (string)german[componetName];
                    }
                    break;
                case (Languages.Polish):
                    if (polish.Contains(componetName))
                    {
                        return (string)polish[componetName];
                    }
                    break;
            }
            Utils.appendToLog("ERROR: no value in language hash for key: " + componetName + ": Language: " + language);
            return componetName;
        }
        //method to load each translated string based on which language is selected
        public static void loadHashes()
        {
            //Syntax is as follows:
            //languageName.Add("componetName","TranslatedString");
            //General expressions
            english.Add("yes", "yes");
            german.Add("yes", "ja");
            polish.Add("yes", "Tak");

            english.Add("no", "no");
            german.Add("no", "nein");
            polish.Add("no", "Nie");

            english.Add("cancel", "Cancel");
            german.Add("cancel", "Abbrechen");
            polish.Add("cancel", "Anuluj");

            english.Add("warning", "WARNING");
            german.Add("warning", "WARNUNG");
            polish.Add("warning", "OSTRZEŻENIE");

            english.Add("critical", "CRITICAL");
            german.Add("critical", "KRITISCH");
            polish.Add("critical", "BŁĄD KRYTYCZNY");

            english.Add("information", "Information");
            german.Add("information", "Information");
            polish.Add("information", "Informacja");

            //Section: MainWindow

            //Componet: installRelhaxMod
            //The button for installing the modpack
            english.Add("installRelhaxMod", "Start Mod Selection");
            german.Add("installRelhaxMod", "Auswahl der Mods");
            polish.Add("installRelhaxMod", "Przejdź Do Wyboru Modyfikacji");

            //Componet: uninstallRelhaxMod
            //
            english.Add("uninstallRelhaxMod", "Uninstall Relhax Modpack");
            german.Add("uninstallRelhaxMod", "Relhax Modpack deinstallieren");
            polish.Add("uninstallRelhaxMod", "Odinstaluj Paczkę Relhax");

            //Componet: forceManuel
            //
            english.Add("forceManuel", "Force manual game detection");
            german.Add("forceManuel", "Erzwinge manuelle Spielerkennung");
            polish.Add("forceManuel", "Wymuś ręczną weryfikację ścieżki gry");

            //Componet: forceManuel
            //
            english.Add("languageSelectionGB", "Language Selection");
            german.Add("languageSelectionGB", "Sprachauswahl");
            polish.Add("languageSelectionGB", "Wybór języka");

            //Componet: formPageLink
            //
            english.Add("formPageLink", "View Modpack Form Page (NA)");
            german.Add("formPageLink", "Zeige Modpack Formularseite (NA)");
            polish.Add("formPageLink", "Forum Paczki (NA)");

            //Componet: saveUserDataCB
            //
            english.Add("saveUserDataCB", "Save user data");
            german.Add("saveUserDataCB", "Benutzerdaten speichern");
            polish.Add("saveUserDataCB", "Zapisz ustawienia użytkownika");

            //Componet: cleanInstallCB
            //
            english.Add("cleanInstallCB", "Clean Installation (Recommended)");
            german.Add("cleanInstallCB", "Saubere Installation (Empfohlen)");
            polish.Add("cleanInstallCB", "Czysta instalacja (Zalecane)");

            //Componet: cancerFontCB
            //
            english.Add("cancerFontCB", "Comic Sans Font");
            german.Add("cancerFontCB", "Comic Sans Schriftart");
            polish.Add("cancerFontCB", "Czcionka Comic Sans");

            //Componet: backupModsCheckBox
            //
            english.Add("backupModsCheckBox", "Backup current mods folder");
            german.Add("backupModsCheckBox", "Sicherung des aktuellen Modsordner");
            polish.Add("backupModsCheckBox", "Zrób kopię zapasową obecnego pliku z modyfikacjami");

            //Componet: settingsGroupBox
            //
            english.Add("settingsGroupBox", "RelHax ModPack Settings");
            german.Add("settingsGroupBox", "RelHax ModPack Einstellungen");
            polish.Add("settingsGroupBox", "Ustawienia Paczki RelHax");

            //Componet: darkUICB
            //
            english.Add("darkUICB", "Dark UI");
            german.Add("darkUICB", "Dunkle Benutzeroberflaeche");
            polish.Add("darkUICB", "Ciemny Interfejs");

            //Componet: cleanUninstallCB
            //
            english.Add("cleanUninstallCB", "Clean uninstallation");
            german.Add("cleanUninstallCB", "Saubere Deinstallation");
            polish.Add("cleanUninstallCB", "Czysta deinstalacja");

            //Componet: saveLastInstallCB
            //
            english.Add("saveLastInstallCB", "Save last install\'s config");
            german.Add("saveLastInstallCB", "Speicherung der letzten Installation");
            polish.Add("saveLastInstallCB", "Zapisz ostatnią konfigurację instalacji");

            //Componet: largerFontButton
            //
            english.Add("largerFontButton", "Larger Font");
            german.Add("largerFontButton", "Groessere Schriftart");
            polish.Add("largerFontButton", "Większa czcionka");

            //Componet: loadingImageGroupBox
            //
            english.Add("loadingImageGroupBox", "Loading Image");
            german.Add("loadingImageGroupBox", "Bild laden");
            polish.Add("loadingImageGroupBox", "Ładowanie obrazka");

            //Componet: standardImageRB
            //
            english.Add("standardImageRB", "Standard");
            german.Add("standardImageRB", "Standard");
            polish.Add("standardImageRB", "Podstawowe");

            //Componet: findBugAddModLabel
            //
            english.Add("findBugAddModLabel", "Find a bug? Want a mod added?");
            german.Add("findBugAddModLabel", "Fehler gefunden? Willst Du einen Mod hinzufuegen?");
            polish.Add("findBugAddModLabel", "Znalazłeś błąd? Chcesz dodać mod?");

            //Componet: cancelDownloadButton
            //
            english.Add("cancelDownloadButton", "Cancel Download");
            german.Add("cancelDownloadButton", "Download abbrechen");
            polish.Add("cancelDownloadButton", "Anuluj pobieranie");
            //Section: FirstLoadHelper

            //Componet: helperText
            //
            english.Add("helperText", "Welcome to the RelHax Modpack! I have tried to make the modpack as straight-forward as possible, but questions may still arise. Hover over (or right click) a setting to have it explained. You won't see this dialog box again, unless you delete the settings xml file.");
            german.Add("helperText", "Willkommen im RelHax Modpack! Ich habe versucht, das Modpack so einfach wie möglich zu gestalten, aber Fragen können dennoch entstehen. Rechtsklick auf eine Einstellung erklaert diese dann. Du siehst diese Dialogbox nicht mehr, ausser du loescht die xml Datei.");
            polish.Add("helperText", "Witamy w paczce RelHax! Próbowałem stworzyć jak najprostszą w użytku paczkę modyfikacji, ale wciąż możesz mieć pytania. Kliknik PPM na opcji, by wyświetlić opis. Nie zobaczysz tej wiadomości ponownie, dopóki nie usuniesz pliku ustawień xml.");

            //Component: donateLabel
            english.Add("donateLabel", "Donation for further development");
            german.Add("donateLabel", "Spende für die Weiterentwicklung");
            polish.Add("donateLabel", "Dotacja na dalszy rozwój");

            //Section: ModSelectionList

            //Componet: continueButton
            //
            english.Add("continueButton", "Install");
            german.Add("continueButton", "Installieren");
            polish.Add("continueButton", "Zainstaluj");

            //Componet: cancelButton
            //
            english.Add("cancelButton", "Cancel");
            german.Add("cancelButton", "Abbrechen");
            polish.Add("cancelButton", "Anuluj");

            //Componet: helpLabel
            //
            english.Add("helpLabel", "Right-click a mod name to preview it");
            german.Add("helpLabel", "Klick mit rechten Maustaste auf einen Mod-Namen, um eine Vorschau zu sehen");
            polish.Add("helpLabel", "PPM by wyświetlić opis");

            //Componet: loadConfigButton
            //
            english.Add("loadConfigButton", "Load selection");
            german.Add("loadConfigButton", "Auswahl laden");
            polish.Add("loadConfigButton", "Wczytaj konfigurację z pliku");

            //Componet: saveConfigButton
            //
            english.Add("saveConfigButton", "Save selection");
            german.Add("saveConfigButton", "Auswahl speichern");
            polish.Add("saveConfigButton", "Zapisz konfigurację w pliku");

            //Componet: label2
            //
            english.Add("label2", "\"*\" tab indicates single selection tab");
            german.Add("label2", "Ein Tab mit einem\"*\" kann nur eins der Mods ausgewählt werden.");
            polish.Add("label2", "\"*\" wskazuje pojedynczą kartę wyboru");

            //Componet: clearSelectionsButton
            //
            english.Add("clearSelectionsButton", "Clear selections");
            german.Add("clearSelectionsButton", "Auswahl löschen");
            polish.Add("clearSelectionsButton", "Wyczyść wybór");

            //Componet: readingDatabase
            //
            english.Add("readingDatabase", "Reading Database");
            german.Add("readingDatabase", "Lese Datenbank");
            polish.Add("readingDatabase", "Wczytywanie baz danych");

            //Componet: buildingUI
            //
            english.Add("buildingUI", "Building UI");
            german.Add("buildingUI", "Erstelle UI");
            polish.Add("buildingUI", "Budowanie interfejsu");

            //Section: Preview
            //Componet: nextPicButton
            //
            english.Add("nextPicButton", "next");
            german.Add("nextPicButton", "weiter");
            polish.Add("nextPicButton", "Dalej");

            //Componet: previousPicButton
            //
            english.Add("previousPicButton", "previous");
            german.Add("previousPicButton", "zurück");
            polish.Add("previousPicButton", "Wstecz");

            //Componet: devLinkLabel
            //
            english.Add("devLinkLabel", "Developer Website");
            german.Add("devLinkLabel", "Entwickler Webseite");
            polish.Add("devLinkLabel", "Strona Dewelopera");

            //Section: VersionInfo
            //Componet: updateAcceptButton
            //
            english.Add("updateAcceptButton", "yes");
            german.Add("updateAcceptButton", "ja");
            polish.Add("updateAcceptButton", "Tak");

            //Componet: updateDeclineButton
            //
            english.Add("updateDeclineButton", "no");
            german.Add("updateDeclineButton", "nein");
            polish.Add("updateDeclineButton", "Nie");

            //Componet: newVersionAvailableLabel
            //
            english.Add("newVersionAvailableLabel", "New Version Available");
            german.Add("newVersionAvailableLabel", "Neue Version verfügbar");
            polish.Add("newVersionAvailableLabel", "Dostępna Nowa Wersja");

            //Componet: updateQuestionLabel
            //
            english.Add("updateQuestionLabel", "Update?");
            german.Add("updateQuestionLabel", "Aktualisieren?");
            polish.Add("updateQuestionLabel", "Zaktualizować?");

            //Componet: problemsUpdatingLabel
            //
            english.Add("problemsUpdatingLabel", "If you are having problems updating, please");
            german.Add("problemsUpdatingLabel", "Wenn Sie Probleme mit der Aktualisierung haben, bitte");
            polish.Add("problemsUpdatingLabel", "Jeśli masz problemy z aktualizają proszę");

            //Componet: 
            //
            english.Add("clickHereUpdateLabel", "click here.");
            german.Add("clickHereUpdateLabel", "klick hier.");
            polish.Add("clickHereUpdateLabel", "kliknij tutaj");

            //Section: PleaseWait
            //Componet: label1
            //
            english.Add("label1", "Loading...please wait...");
            german.Add("label1", "Lädt...bitte warten...");
            polish.Add("label1", "Ładowanie... proszę czekać...");
            //Section: Messages of MainWindow

            //Componet: 
            //
            english.Add("Downloading", "Downloading");
            german.Add("Downloading", "Wird heruntergeladen");
            polish.Add("Downloading", "Pobieranie");

            //Componet: 
            //
            english.Add("patching", "Patching");
            german.Add("patching", "Patching");
            polish.Add("patching", "Patchowanie");

            //Componet: 
            //
            english.Add("done", "Done");
            german.Add("done", "Fertig");
            polish.Add("done", "Zrobione");

            //Componet: 
            //
            english.Add("idle", "Idle");
            german.Add("idle", "Leerlauf");
            polish.Add("idle", "Bezczynny");

            //Componet: 
            //
            english.Add("status", "Status:");
            german.Add("status", "Status:");
            polish.Add("status", "Stan:");

            //Componet: 
            //
            english.Add("canceled", "Canceled");
            german.Add("canceled", "Abgebrochen");
            polish.Add("canceled", "Anulowano");

            //Componet: 
            //
            english.Add("appSingleInstance", "Checking for single instance");
            german.Add("appSingleInstance", "Prüfung auf Einzelinstanz");
            polish.Add("appSingleInstance", "Sprawdzanie ");

            //Componet: 
            //
            english.Add("checkForUpdates", "Checking for updates");
            german.Add("checkForUpdates", "Auf Updates prüfen");
            polish.Add("checkForUpdates", "Sprawdzanie aktualizacji");

            //Componet: 
            //
            english.Add("verDirStructure", "Verifying directory structure");
            german.Add("verDirStructure", "Verifizierung der Verzeichnisstruktur");
            polish.Add("verDirStructure", "Sprawdzanie struktury dostępu");

            //Componet: 
            //
            english.Add("loadingSettings", "Loading Settings");
            german.Add("loadingSettings", "Einstellungen laden");
            polish.Add("loadingSettings", "Ładowanie ustawnień");

            //Componet: 
            //
            english.Add("loadingTranslations", "Loading Translations");
            german.Add("loadingTranslations", "Laden der Übersetzungen");
            polish.Add("loadingTranslations", "ładowanie tłumaczenia");

            //Componet: 
            //
            english.Add("loading", "Loading");
            german.Add("loading", "Laden");
            polish.Add("loading", "Ładowanie");

            //Componet: 
            //
            english.Add("uninstalling", "Uninstalling");
            german.Add("uninstalling", "Deinstallieren");
            polish.Add("uninstalling", "Deinstalacja w toku");

            //Componet: 
            //
            english.Add("installingFonts", "Installing Fonts");
            german.Add("installingFonts", "Installieren von Schriftarten");
            polish.Add("installingFonts", "Instalowanie czcionek");

            //Componet: 
            //
            english.Add("loadingExtractionText", "Loading Extraction Text");
            german.Add("loadingExtractionText", "Extraktionstext laden");
            polish.Add("loadingExtractionText", "Ładowanie tekstu");

            //Componet: 
            //
            english.Add("extractingRelhaxMods", "Extracting RelHax Mods");
            german.Add("extractingRelhaxMods", "Extrahieren von RelHax Mods");
            polish.Add("extractingRelhaxMods", "Wypakowywanie modyfikacji RelHax");

            //Componet: 
            //
            english.Add("extractingUserMods", "Extracting User Mods");
            german.Add("extractingUserMods", "Extrahieren von benutzerdefinierten Mods");
            polish.Add("extractingUserMods", "Wypakowywanie modyfikacji użytkownika");

            //Componet: 
            //
            english.Add("startingSmartUninstall", "Starting smart uninstall");
            german.Add("startingSmartUninstall", "Starten der intelligenten Deinstallation");
            polish.Add("startingSmartUninstall", "Rozpoczynanie inteligentnej deinstalacji");

            //Componet: 
            //
            english.Add("copyingFile", "Copying file");
            german.Add("copyingFile", "Kopieren von Dateien");
            polish.Add("copyingFile", "Kopiowanie plików");

            //Componet: 
            //
            english.Add("deletingFile", "Deleting file");
            german.Add("deletingFile", "Löschen von Dateien");
            polish.Add("deletingFile", "Usuwanie plików");

            //Componet: 
            //
            english.Add("of", "of");
            german.Add("of", "von");
            polish.Add("of", "z");

            //Componet: 
            //
            english.Add("forceManuelDescription", "This option is for forcing a manual World of Tanks game" +
                    "location detection. Check this if you are having problems with automatically locating the game.");
            german.Add("forceManuelDescription", "Diese Option ist für die manuelle selektion des World of Tanks Spiel-" +
                    "speicherortes. Nutze dies wenn Du Probleme mit der automatischen Suche des Spiels hast.");
            polish.Add("forceManuelDescription", "Ta opcja wymusza ręczne znalezienie lokacji gry World of Tanks." +
                    "Zaznacz, jeśli masz problem z automatycznym znalezieniem ścieżki dostępu do gry.");

            //Componet: 
            //
            english.Add("cleanInstallDescription", "This recommended option will empty your res_mods folder before installing" +
                    "your new mod selections. Unless you know what you are doing, it is recommended that you keep this on to avoid problems.");
            german.Add("cleanInstallDescription", "Diese empfohlene Option leert den Ordner res_mods vor der Installation" +
                    "Deiner neuen Mod-Auswahl. Bis du genau weißt, was du tust, empfohlen wir, dass du das weiter behältst, um Probleme zu vermeiden.");
            polish.Add("cleanInstallDescription", "To rekomendowane ustawienie usunie zawartość folderu res_mods przed instalacją" +
                     "nowej konfiguracji modów. Jeśli nie wiesz co robisz zalecamy włączyć tą opcję, aby uniknąć problemów.");

            //Componet: 
            //
            english.Add("backupModsDescription", "Select this to make a backup of your current res_mods folder." +
                    "They are stored in the 'RelHaxModBackup' folder, saved in a folder inside by a time stamp.");
            german.Add("backupModsDescription", "Wähle diese Option, um eine Sicherungskopie Deines aktuellen res_mods-Ordners zu erstellen." +
                    "Sie werden im Ordner 'RelHaxModBackup' gespeichert und in einem Ordner nach innen durch einen Zeitstempel gespeichert.");
            polish.Add("backupModsDescription", "Zaznacz, aby zrobić kopię zapasową folderu res_mods." +
                     "Pliki będą przechowane w folderze RelHaxModBackup, zapisane w folderze oznaczonym datą.");

            //Componet: 
            //
            english.Add("comicSansDescription", "Enable Comic Sans font");
            german.Add("comicSansDescription", "Schriftart Comic Sans aktivieren");
            polish.Add("comicSansDescription", "Włącz czcionkę Comic Sans");

            //Componet: 
            //
            english.Add("enlargeFontDescription", "Enlarge font");
            german.Add("enlargeFontDescription", "Schriftart vergrössern");
            polish.Add("enlargeFontDescription", "Powiększ czcionkę");

            //Componet: 
            //
            english.Add("selectGifDesc", "Select a loading gif for the mod preview window.");
            german.Add("selectGifDesc", "Wähle ein Lade-Gif fuer das Vorschaufenster des Mods.");
            polish.Add("selectGifDesc", "Załaduj gif w oknie podglądu.");

            //Componet: 
            //
            english.Add("saveLastConfigInstall", "If this is selected, the installer will, upon selection window showing, load the last installed config you used.");
            german.Add("saveLastConfigInstall", "Wenn dies ausgewählt ist, lädt das Installationsprogramm die zuletzt installierte Config im Auswahlfenster, die Du verwendet hast.");
            polish.Add("saveLastConfigInstall", "Przy zaznaczeniu, instalator załaduje ostatnią użytą konfigurację w oknie wyboru modyfikacji.");

            //Componet:
            //
            english.Add("saveUserDataDesc", "If this is selected, the installer will save user created data (like session stats from previous battles)");
            german.Add("saveUserDataDesc", "Wenn dies ausgewählt ist, speichert das Installationsprogramm die vom Benutzer erstellten Daten (wie Sitzungsstatistiken aus früheren Gefechten)");
            polish.Add("saveUserDataDesc", "Przy zaznaczeniu, instalator zachowa pliki danych użytkownika (takie jak statystyki sesji z poprzednich bitew)");

            //Componet: 
            //
            english.Add("cleanUninstallDescription", "Selected - All mods will be erased\nNot Selected - Only Modpack installed mods will be erased");
            german.Add("cleanUninstallDescription", "Ausgewählt - Alle Mods werden gelöscht\nNicht ausgewählt - Nur Mods, die vom Modpack installiert wurden, werden gelöscht");
            polish.Add("cleanUninstallDescription", "Zaznaczone - Wszystkie mody zostaną usunięte\nNie zaznaczone - Tylko zainstalowane mody z paczki zostaną usunięte");

            //Componet: 
            //
            english.Add("darkUIDesc", "Toggle the DarkUI mode. Usefull for working with the modpack at night.");
            german.Add("darkUIDesc", "Auf dunklen UI Modus umschalten. Nützlich für die Arbeit mit dem Modpack in der Nacht.");
            polish.Add("darkUIDesc", "Zmień interfejs na ciemny. Przydatneprzy pracy z paczką w nocy.");

            //Componet: 
            //
            english.Add("failedToDownload_1", "Failed to download");
            german.Add("failedToDownload_1", "Fehler beim Herunterladen");
            polish.Add("failedToDownload_1", "Ściąganie zakończone niepowodzeniem, plik");

            //Componet: 
            //
            english.Add("failedToDownload_2", ". If you know which mod this is, uncheck it and you should be fine. It will be fixed soon. Restart this when it exits");
            german.Add("failedToDownload_2", ". Wenn du weisst, welcher Mod das ist, deaktiviere ihn und alles sollte funktionieren. Es wird bald behoben. Starte das Programm neu nach dem beenden.");
            polish.Add("failedToDownload_2", ". Jeśli wiesz który to mod, odznacz go i wszystko powinno byćw porządku. Wkrótce naprawimy błąd. Zrestartuj, jeśli problem pojawia się ponownie.");

            //Componet: 
            //
            english.Add("fontsPromptInstallHeader", "Admin to install fonts?");
            german.Add("fontsPromptInstallHeader", "Admin zum Installieren von Schriftarten?");
            polish.Add("fontsPromptInstallHeader", "Uprawnienia administratora, by zainstalować czcionki?");

            //Componet: 
            //
            english.Add("fontsPromptInstallText", "Do you have admin rights?");
            german.Add("fontsPromptInstallText", "Hast Du Admin-Rechte?");
            polish.Add("fontsPromptInstallText", "Czy masz uprawnienia administratora?");

            //Componet: 
            //
            english.Add("fontsPromptError_1", "Unable to install fonts. Some mods may not work properly. Fonts are located in ");
            german.Add("fontsPromptError_1", "Schriftarten können nicht installiert werden. Einige Mods funktionieren möglicherweise nicht richtig. Schriften befinden sich in ");
            polish.Add("fontsPromptError_1", "Niepowodzenie przy instalacji czcionek. Niektóre modyfikacje mogą nie działać prawidłowo. Czcionki znajdują się w ");

            //Componet: 
            //
            english.Add("fontsPromptError_2", "\\_fonts. Either install them yourself or run this again as Administrator.");
            german.Add("fontsPromptError_2", "\\_fonts. Entweder installiere sie selbst oder führe es erneut als Administrator aus.");
            polish.Add("fontsPromptError_2", "\\_fonts. Albo zainstalujesz je własnoręcznie, albo uruchom jako administrator.");

            //Componet: 
            //
            english.Add("cantDownloadNewVersion", "Unable to download new version, exiting.");
            german.Add("cantDownloadNewVersion", "Die neue Version kann nicht heruntergeladen werden.");
            polish.Add("cantDownloadNewVersion", "Niepowodzenie przy pobieraniu nowej wersji.");

            //Componet: 
            //
            english.Add("cantStartNewApp", "Unable to start application, but it is located in \n");
            german.Add("cantStartNewApp", "Kann die Anwendung nicht starten, aber sie befindet sich in \n");
            polish.Add("cantStartNewApp", "Niepowodzenie przy uruchamianiu aplikacji znajdującej się w \n");

            //Componet: 
            //
            english.Add("autoDetectFailed", "The auto-detection failed. Please use the 'force manual' option");
            german.Add("autoDetectFailed", "Die automatische Erkennung ist fehlgeschlagen. Bitte benutzen Sie die 'erzwinge manuelle' Option");
            polish.Add("autoDetectFailed", "Niepowodzenie automatycznego wykrywania. Proszę wybrać opcję ręcznego znajdowania ścieżki gry.");

            //Componet: MainWindow_Load
            //
            english.Add("anotherInstanceRunning", "CRITICAL: Another Instance of the relic mod manager is already running");
            german.Add("anotherInstanceRunning", "KRITISCH: Eine weitere Instanz des Relic Mod Managers läuft bereits");
            polish.Add("anotherInstanceRunning", "BŁĄD KRYTYCZNY: Inna instancja relic mod managera jest uruchomiona");

            //Componet: 
            //
            english.Add("skipUpdateWarning", "WARNING: You are skipping updating. Database Compatability is not guarenteed");
            german.Add("skipUpdateWarning", "WARNUNG: Sie überspringen die Aktualisierung. Datenbankkompatibilität ist nicht garantiert");
            polish.Add("skipUpdateWarning", "OSTRZEŻENIE: Pomijasz aktualizację! Może wystąpić niezgodność wersji.");

            //Componet: 
            //
            english.Add("patchDayMessage", "The modpack is curretly down for patch day testing and mods updating. Sorry for the inconvience. If you are a database manager, please add the command arguement");
            german.Add("patchDayMessage", "Das Modpack ist zur Zeit für Patch-Tag-Tests und das Updaten von Mods nicht erreichbar. Entschuldige die Unannehmlichkeiten. Wenn Sie ein Datenbankmanager sind, fügen Sie bitte das Befehlsargument hinzu");
            polish.Add("patchDayMessage", "Paczka nie działa ze względu na testy i aktualizację modyfikacji. Przepraszamy za utrudnienia. Jeśli zarządzasz bazą danych, proszę dodać odpowiednią komendę");

            //Componet: 
            //
            english.Add("configNotExist", "ERROR: {0} does NOT exist, loading in regular mode");
            german.Add("configNotExist", "FEHLER: {0} existiert nicht, laedt im regulaeren Modus");
            polish.Add("configNotExist", "BŁĄD: {0} nie istnieje, ładowanie podstawowego trybu");

            //Componet: 
            //
            english.Add("autoAndClean", "ERROR: clean installation is set to false. You must set this to true and restart the application for auto install to work. Loading in regular mode.");
            german.Add("autoAndClean", "FEHLER: Die saubere Installation ist abgewählt/deaktiviert. Du musst diese Option auswählen und die Anwendung für die automatische Installation neu starten, damit sie funktioniert. Lädt im regulären Modus.");
            polish.Add("autoAndClean", "BŁĄD: wyłączono czystą instalację. Musisz ją włączyć i ponownie uruchomić aplikację, by automatyczna instalacja zadziałała. Ładowanie w trybie podstawowym.");

            //Componet: 
            //
            english.Add("autoAndFirst", "ERROR: First time loading cannot be an auto install mode, loading in regular mode");
            german.Add("autoAndFirst", "FEHLER: Erstmaliges Laden kann kein automatischer Installationsmodus sein, lade im regulären Modus");
            polish.Add("autoAndFirst", "BŁĄD: Pierwsze ładowanie nie może być automatyczną instalacją, ładowanie w trybie podstawowym");

            //Componet: 
            //
            english.Add("confirmUninstallHeader", "Confirmation");
            german.Add("confirmUninstallHeader", "Bestätigung");
            polish.Add("confirmUninstallHeader", "Potwierdź");

            //Componet: 
            //
            english.Add("confirmUninstallMessage", "Confirm you wish to uninstall?");
            german.Add("confirmUninstallMessage", "Bestätige, wenn du installieren möchtest?");
            polish.Add("confirmUninstallMessage", "Potwierdzić deinstalację?");

            //Componet: 
            //
            english.Add("uninstallingText", "Uninstalling...");
            german.Add("uninstallingText", "Deinstalliere...");
            polish.Add("uninstallingText", "Deinstalacja w toku...");

            //Componet: 
            //
            english.Add("specialMessage1", "If you are seeing this, it means that you have a specific computer configuration that is affected by a bug I can't replicate on my developer system. It's harmless, but if you could send your relHaxLog to me I can fix it and you can stop seeing this message");
            german.Add("specialMessage1", "Wenn Du dies siehst, bedeutet dies, dass Du eine bestimmte Computer-Konfiguration hast, die von einem Fehler betroffen ist, den ich nicht auf meinem Entwicklersystem replizieren kann. Es ist harmlos, aber wenn du dein relHaxLog an mich schicken könntest, kann ich es beheben und du wirst diese Nachricht zukuenftig nicht mehr sehen");
            polish.Add("specialMessage1", "Jeśli to widzisz, to znaczy, że masz specificzną konfigurację komputera afektowany przez bug, który nie mogę kopiować na moim systemie. Jest nieszkodliwy, ale jeśli możesz mi przesłać relHaxLog to postaram się naprawić błąd, abyś nie widział tej wiadomości w przyszłości");

            //Componet: 
            //
            english.Add("extractionErrorMessage", "Error deleting the folder res_mods or mods. Either World of Tanks is running or your file and folder security permissions are incorrect");
            german.Add("extractionErrorMessage", "Fehler beim Löschen des Ordners res_mods oder Mods. Entweder World of Tanks läuft oder Ihre Datei- und Ordnersicherheitsberechtigungen sind falsch");
            polish.Add("extractionErrorMessage", "Błąd usuwania folderu res_mods lub mods. Albo World of Tanks jest uruchomione, albo twój plik i folder mają nieprawidłowe zabezpieczenia dostępu");

            //Componet: 
            //
            english.Add("extractionErrorHeader", "Error");
            german.Add("extractionErrorHeader", "Fehler");
            polish.Add("extractionErrorHeader", "Błąd");

            //Componet: 
            //
            english.Add("deleteErrorHeader", "close out of folders");
            german.Add("deleteErrorHeader", "Ausschliessen von Ordnern");
            polish.Add("deleteErrorHeader", "zamknij foldery");

            //Componet: 
            //
            english.Add("deleteErrorMessage", "Please close all explorer windows in mods or res_mods (or deeper), and click ok to continue.");
            german.Add("deleteErrorMessage", "Bitte schließen Sie alle Explorer-Fenster in Mods oder res_mods (oder Unterordner), und klicken Sie auf OK, um fortzufahren.");
            polish.Add("deleteErrorMessage", "Proszę zamknij folder mods lub res_mods (lub podfoldery), a następnie kliknij kontynuację.");

            //Componet: 
            //
            english.Add("noUninstallLogMessage", "The log file containg the installed files list (installedRelhaxFiles.log) does not exist. Would you like to remove all mods instead?");
            german.Add("noUninstallLogMessage", "Die Protokolldatei mit der Liste der installierten Dateien (installedRelhaxFiles.log) existiert nicht. Möchtest Du alle Mods stattdessen entfernen?");
            polish.Add("noUninstallLogMessage", "Plik logów zawierający listę instalacyjną (installedRelhaxFiles.log) nie istnieje. Czy chciałbyś usunąć modyfikacje?");

            //Componet: 
            //
            english.Add("noUninstallLogHeader", "Remove all mods");
            german.Add("noUninstallLogHeader", "Entferne alle Mods");
            polish.Add("noUninstallLogHeader", "Usuń wszystkie modyfikacje");

            //Section: Messages from ModSelectionList

            //Componet: testModeDatabaseNotFound
            //
            english.Add("testModeDatabaseNotFound", "CRITICAL: TestMode Database not found at:\n{0}");
            german.Add("testModeDatabaseNotFound", "KRITISCH: Die Datanbank für den Testmodus wurde nicht gefunden:\n{0}");
            polish.Add("testModeDatabaseNotFound", "BŁĄD KRYTYCZNY: Baza danych Trybu Testowego nie znaleziona w lokalizacji:\n{0}");

            //Componet: 
            //
            english.Add("duplicateMods", "CRITICAL: Duplicate mod name detected");
            german.Add("duplicateMods", "KRITISCH: Duplizierter Modname wurde erkannt");
            polish.Add("duplicateMods", "BŁĄD KRYTYCZNY: Wykryto zduplikowaną nazwę modyfikacji");

            //Componet: 
            //
            english.Add("databaseReadFailed", "CRITICAL: Failed to read database");
            german.Add("databaseReadFailed", "KRITISCH: Datenbank konnte nicht gelesen werden");
            polish.Add("databaseReadFailed", "BŁĄD KRYTYCZNY: Nie udało się wczytać bazy danych");

            //Componet: 
            //
            english.Add("configSaveSuccess", "Config Saved Successfully");
            german.Add("configSaveSuccess", "Konfiguration wurde erfolgreich gespeichert");
            polish.Add("configSaveSuccess", "Udało się zapisać konfigurację");

            //Componet: 
            //
            english.Add("selectConfigFile", "Select a user preference file to load");
            german.Add("selectConfigFile", "Wählen Sie die benutzerdefinierte Datei aus, die geladen werden soll");
            polish.Add("selectConfigFile", "Wybierz plik preferencji do wczytania");

            //Componet: 
            //
            english.Add("configLoadFailed", "The preference file could not be loaded, loading in standard mode");
            german.Add("configLoadFailed", "Die Konfigurationsdatei konnte nicht geladen werden, lade im Standard Modus");
            polish.Add("configLoadFailed", "Nie można wczytać pliku knfiguracji, ładowanie trybu podstawowego");

            //Componet: 
            //
            english.Add("modNotFound", "The mod, \"{0}\" was not found in the modpack. It could have been renamed or removed.");
            german.Add("modNotFound", "Der Mod, \"{0}\" wurde im Modpack nicht gefunden. Er könnte umbenannt oder entfernt worden sein.");
            polish.Add("modNotFound", "Modyfikacja \"{0}\" nie została znaleziona w paczce. Sprawdź, czy nie została usunięta lub zmieniona nazwa.");

            //Componet: 
            //
            english.Add("configNotFound", "The config \"{0}\" was not found for mod \"{1}\". It could have been renamed or removed.");
            german.Add("configNotFound", "Die Config \"{0}\" wurde nicht für den Mod gefunden \"{1}\". Er könnte umbenannt oder entfernt worden sein.");
            polish.Add("configNotFound", "Konfiguracja \"{0}\" nie została znaleziona dla modyfikacji \"{1}\". Sprawdź, czy nie została usunięta lub zmieniona nazwa.");

            //Componet: 
            //
            english.Add("modDeactivated", "The mod \"{0}\" is currently deactivated in the modpack and could not to selected to install.");
            german.Add("modDeactivated", "Der Mod \"{0}\" ist derzeit im Modpack deaktiviert und kann für die Installation nicht markiert werden.");
            polish.Add("modDeactivated", "");

            //Componet: 
            //
            english.Add("configDeactivated", "The config \"{0}\" of \"{1}\" is currently deactivated in the modpack and could not to selected to install.");
            german.Add("configDeactivated", "Die Konfiguration \"{0}\" von \"{1}\" ist derzeit im Modpack deaktiviert und kann für die Installation nicht markiert werden.");
            polish.Add("configDeactivated", "");

            //Componet: 
            //
            english.Add("modsNotFoundTechnical", "The following mods could not be found and were most likely removed. There are only technical names available:\n{0}");
            german.Add("modsNotFoundTechnical", "Die folgenden Modifikationen können nicht gefundenw werden und scheinen gelöscht zu sein. Es sind leider nur technische Namen verfügbar:\n{0}");
            polish.Add("modsNotFoundTechnical", "");

            //Componet: 
            //
            english.Add("oldSavedConfigFile", "The saved preferences file your are using is in an outdated format and will be inaccurate in the future. Convert it to the new format? (A backup of the old format will be made)");
            german.Add("oldSavedConfigFile", "Die Konfigurationsdatei die benutzt wurde, wird in Zukunft immer ungenauer werden. Soll auf das neue Standardformat umgestellt werden? (Eine Sicherung des alten Formats erfolgt)");
            polish.Add("oldSavedConfigFile", "");

            //Componet: 
            //
            english.Add("prefrencesSet", "Preferences Set");
            german.Add("prefrencesSet", "Bevorzugte Einstellungen");
            polish.Add("prefrencesSet", "Preferowane Ustawienia");

            //Componet: 
            //
            english.Add("selectionsCleared", "Selections Cleared");
            german.Add("selectionsCleared", "Auswahlen Gelöscht");
            polish.Add("selectionsCleared", "Usunięto Zaznaczenia");

            //Componet: 
            //
            english.Add("patchingSystemDeneidAccessMessage", "The patching system was denied access to the patch folder. Retry as Administrator. If you see this again, you need to fix your file and folder security permissions");
            german.Add("patchingSystemDeneidAccessMessage", "Dem Patching-System wurde der Zugriff auf den Patch-Ordner verweigert. Wiederholen als Administrator. Wenn Sie dies wieder sehen, müssen Sie Ihre Datei- und Ordnersicherheitsberechtigungen reparieren");
            polish.Add("patchingSystemDeneidAccessMessage", "Nie uzyskano dostępu do folderu patcha. Spróbuj ponownie jako administrator. Jeśli widzisz to ponownie, to zmień ustawienia pozwoleń dostępu do folderów.");

            //Componet: 
            //
            english.Add("patchingSystemDeneidAccessHeader", "Access Deneid");
            german.Add("patchingSystemDeneidAccessHeader", "Zugriff abgelehnt");
            polish.Add("patchingSystemDeneidAccessHeader", "Odmowa dostępu");

            //Componet: 
            //
            english.Add("databaseNotFound", "Database not found at supplied URL");
            german.Add("databaseNotFound", "Datenbank nicht an der angegebenen URL gefunden");
            polish.Add("databaseNotFound", "Nie znaleziono bazy danych pod wskazanym URL");

            //Componet:
            //
            english.Add("WoTRunningHeader", "WoT is Running");
            german.Add("WoTRunningHeader", "WoT wird gerade ausgeführt.");
            polish.Add("WoTRunningHeader", "WoT jest uruchomiony");

            //Componet:
            //
            english.Add("WoTRunningMessage", "Please close World of Tanks to continue");
            german.Add("WoTRunningMessage", "Um Fortzufahren, schliesse bitte World of Tanks.");
            polish.Add("WoTRunningMessage", "Proszę zamknąć World of Tanks, aby kontynuować");

            //Componet:
            //
            english.Add("InstallingTo", "Installing to");
            german.Add("InstallingTo", "Installiere nach");
            polish.Add("InstallingTo", "Instalowanie w");

            //Componet:
            //
            english.Add("zipReadingErrorHeader", "Incomplete Download");
            german.Add("zipReadingErrorHeader", "Unvollständiger Download");
            polish.Add("zipReadingErrorHeader", "Ściąganie niekompletne");

            //Componet:
            //
            english.Add("zipReadingErrorMessage1", "The zip file");
            german.Add("zipReadingErrorMessage1", "Die Zip-Datei");
            polish.Add("zipReadingErrorMessage1", "Plik skomresowany formatu ZIP ");

            //Componet:
            //
            english.Add("zipReadingErrorMessage2", "could not be read, most likely due to an incomplete download. Please try again.");
            german.Add("zipReadingErrorMessage2", "Konnte nicht gelesen werden, da es höchstwahrscheinlich ein unvollständiger Download ist. Bitte versuche es später nochmal.");
            polish.Add("zipReadingErrorMessage2", "Nie można odczytać, prawdopodobnie niekompletność pobranych plików. Proszę spróbować ponownie");

            //Componet:
            //
            english.Add("zipReadingErrorMessage3", "Could not be read.");
            german.Add("zipReadingErrorMessage3", "Konnte nicht gelesen werden.");
            polish.Add("zipReadingErrorMessage3", "Nie można odczytać.");

            //Componet: Mod Selection view Group Box
            //
            english.Add("ModSelectionListViewSelection", "Selection View");
            german.Add("ModSelectionListViewSelection", "Auswahl anzeigen");
            polish.Add("ModSelectionListViewSelection", "Widok wyborów");

            //Componet: Mod selection view default (relhax)
            //
            english.Add("selectionDefault", "Default");
            german.Add("selectionDefault", "Standard");
            polish.Add("selectionDefault", "Domyślne");

            //Componet: Mod selection view legacy (OMC)
            //
            english.Add("selectionLegacy", "OMC Legacy");
            german.Add("selectionLegacy", "OMC (Baumstruktur)");
            polish.Add("selectionLegacy", "OMC Rozwijana lista");

            //Componet: Mod selection explanation
            //
            english.Add("selectionViewMode", "Select a mod selection list view mode\nDefault: Relhax list view mode\nLegacy: OMC tree list view mode");
            german.Add("selectionViewMode", "Anzeigearten und Auswahl der Modifikationen\nStandard: Relhax Listendarstellung\nalternativ: OMC Baumstruktur");
            polish.Add("selectionViewMode", "Wybierz tryb widoku wyborów modyfikacji\nDomyślnie: Tryb widoku listy RelHax\nSpuścizna: Tryb widoku listy OMC");

            //Componet: Detected client version
            //
            english.Add("detectedClientVersion", "Detected Client version");
            german.Add("detectedClientVersion", "Erkannte Client Version");
            polish.Add("detectedClientVersion", "Wykryta wersja klienta gry");

            //Componet: Supported client versions
            //
            english.Add("supportedClientVersions", "Supported Clients");
            german.Add("supportedClientVersions", "Unterstützte Clients");
            polish.Add("supportedClientVersions", "Wspomagane wersje klienta gry");

            //Componet: Supported clients notice
            //
            english.Add("supportNotGuarnteed", "This client version is not offically supported. Mods may not work.");
            german.Add("supportNotGuarnteed", "Diese Client-Version wird nicht offiziell unterstützt. Die Mods könnten nicht funktionieren oder sogar Dein World of Tanks beschädigen.");
            polish.Add("supportNotGuarnteed", "Ta wersja klienta gry nie jest oficjalnie wspomagana. Modyfikacje mogą nie działać prawidłowo.");

            //Componet: Failed To Delete folder
            //
            english.Add("folderDeleteFailed", "Failed to delete the folder");
            german.Add("folderDeleteFailed", "Löschen des Verzeichnis fehlgeschlagen");
            polish.Add("folderDeleteFailed", "Próba usunięcia folderu zakończona niepowodzeniem");

            //Componet: Failed To Delete file
            //
            english.Add("fileDeleteFailed", "Failed to delete the file");
            german.Add("fileDeleteFailed", "Löschen der Datei fehlgeschlagen");
            polish.Add("fileDeleteFailed", "Próba usunięcia pliku zakończona niepowodzeniem");

            //Componet: Found zip fiels to delete 1
            //
            english.Add("foundOldFilesDelete1", "The installer has found the following files that are old and can be deleted");
            german.Add("foundOldFilesDelete1", "Wir haben folgende veralteten Dateien gefunden die gelöscht werden können");
            polish.Add("foundOldFilesDelete1", "Instalator znalazł następujące stare pliki, które mogą zostać usunięte");

            //Componet: Found zip files to delete 2
            //
            english.Add("foundOldFilesDelete2", "Would you like to delete them?");
            german.Add("foundOldFilesDelete2", "Möchtest du das sie gelöscht werden?");
            polish.Add("foundOldFilesDelete2", "Czy chcesz je usunąć?");

            //Componet: Expand current tab option
            //
            english.Add("expandAllButton", "Expand Current Tab");
            german.Add("expandAllButton", "Erweitere alle Einträge der aktuellen Registerkarte");
            polish.Add("expandAllButton", "Rozwiń bieżącą kartę");

            //Componet: Colapse current tab option
            //
            english.Add("colapseAllButton", "Collapse Current Tab");
            german.Add("colapseAllButton", "Reduziere alle Einträge der aktuellen Registerkarte");
            polish.Add("colapseAllButton", "Zwiń bieżącą kartę");

            //Componet: font_MouseEnter
            //
            english.Add("font_MouseEnter", "Select a scale mode to use.\nFont is manual scaling, DPI is auto scaling.");
            german.Add("font_MouseEnter", "Wähle einen Skalierungsgrad.\nFont ist manuelle Skalierung, DPI ist automatiche Skalierung");
            polish.Add("font_MouseEnter", "Wybierz tryb skali, który ma zostać użyty.\nCzcionka jest ręcznym skalowaniem, DPI jest autoskalowanie.");

            //Componet: selectionView_MouseEnter
            //
            english.Add("selectionView_MouseEnter", "Select a mod selection list view.");
            german.Add("selectionView_MouseEnter", "wähle eine der Listenansichten.");
            polish.Add("selectionView_MouseEnter", "Wybierz listę wyboru modyfikacji.");

            //Componet: language_MouseEnter
            //
            english.Add("language_MouseEnter", "Select your preferred language.");
            german.Add("language_MouseEnter", "wähle Deine bevorzugte Sprache");
            polish.Add("language_MouseEnter", "Wybierz preferowany język.");

            //Componet: fontSizeGB
            //
            english.Add("fontSizeGB", "Scaling Mode");
            german.Add("fontSizeGB", "Skalierungsmodus");
            polish.Add("fontSizeGB", "Tryb skalowania");

            //Componet: expandNodesDefault
            //
            english.Add("expandNodesDefault", "Expand all");
            german.Add("expandNodesDefault", "Alle erweitern");
            polish.Add("expandNodesDefault", "Rozwiń wszystkie");

            //Component: expandAllDesc
            //
            english.Add("expandAllDesc", "Select this to have all options automatically expand. It applies for the Legacy Selection only.");
            german.Add("expandAllDesc", "Erweitere alle Einträge auf allen Registerkarten automatisch. Nur bei Ansicht als Baumstruktur.");
            polish.Add("expandAllDesc", "Zaznacz to, aby wszystkie opcje zostały automatycznie rozwinięte. Dotyczy tylko opcji Legacy Selection.");

            //Component: disableBordersCB
            //
            english.Add("disableBordersCB", "Disable borders");
            german.Add("disableBordersCB", "Begrenzungen deaktivieren");
            polish.Add("disableBordersCB", "Wyłącz obramowanie");

            //Component: disableBordersDesc
            //
            english.Add("disableBordersDesc", "Disable the black borders around each mod and config sublevel.");
            german.Add("disableBordersDesc", "Deaktiviere die schwarzen Ränder um jeden Mod und Config sublevel.");
            polish.Add("disableBordersDesc", "Wyłącz czarne obwiednie wokół każdego mod i config podpoziomu.");

            //Component: moveOutOfTanksLocation
            //
            english.Add("moveOutOfTanksLocation", "The modpack can not be run from the World_of_Tanks directory. Please move the application and try again.");
            german.Add("moveOutOfTanksLocation", "Das Modpack kann nicht aus dem World_of_Tanks Verzeichnis laufen. Bitte verschiebe die Anwendung in ein anderes Verzeichnis und versuchen Sie es erneut.");
            polish.Add("moveOutOfTanksLocation", "Modpack nie może być uruchomiony z katalogu World_of_Tanks. Przenieś aplikację i spróbuj ponownie.");

            //Section saveConfig
            english.Add("selectWhereToSave", "Select where to save user prefs");
            german.Add("selectWhereToSave", "Bitte wähle wo die Speicherung erfolgen soll");
            polish.Add("selectWhereToSave", "Wybór lokalizacji zapisu preferencji użytkownika");

            //Section addModTreeview
            english.Add("updated", "updated");
            german.Add("updated", "aktualisiert");
            polish.Add("updated", "zaktualizowane");

        }
    }
}
