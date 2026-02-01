# Ricettario

Voglio sviluppare un'applicazione chiamata "Ricettario" che consenta agli utenti di creare, gestire e condividere ricette culinarie. L'applicazione dovrebbe avere le seguenti funzionalità principali.

## Analisi funzionale

1. **Creazione di Ricette**: Gli utenti devono poter creare nuove ricette inserendo il nome della ricetta, gli ingredienti, le istruzioni di preparazione, il tempo di cottura e delle foto della ricetta. La particolarità che voglio implementare è l'automatismo per cui l'utente inserisce gli ingredienti e l'app calcola tutte le percentuali rispetto alla farina totale. Questo permetterà all'utente, successivamente, di adattare facilmente le quantità degli ingredienti in base alla quantità di farina desiderata. Inoltre l'utente potrà anche inserire il peso di un pezzo finito e la quantità di pezzi desiderati per calcolare automaticamente la quantità di farina e degli altri ingredienti necessari.
2. **Gestione delle Ricette**: Gli utenti devono poter modificare, eliminare e organizzare le loro ricette in categorie o raccolte personalizzate.
3. **Condivisione delle Ricette**: Gli utenti devono poter condividere le loro ricette con altri utenti tramite link o sui social media. Inoltre, vorrei implementare una funzionalità che permetta agli utenti di esportare le ricette in formato PDF o di stamparle direttamente dall'app.
4. **Ricerca e Filtri**: Gli utenti devono poter cercare ricette in base a vari criteri come nome, ingredienti, tempo di cottura, difficoltà, ecc. Vorrei anche includere filtri per diete specifiche (es. vegana, senza glutine).
5. **Interfaccia Utente Intuitiva**: L'applicazione deve avere un'interfaccia utente semplice e intuitiva, con un design accattivante che renda facile la navigazione tra le varie sezioni dell'app.
6. **Account Utente**: L'app è accessibile solo previa generazione di un account. Vorrei anche implementare l'autenticazione tramite social media per facilitare la registrazione e il login. Google, Apple, X e Facebook.
7. **Preferiti e Commenti**: Gli utenti devono poter salvare le ricette nei preferiti e lasciare commenti o recensioni sulle ricette degli altri utenti.
8. **Notifiche**: Vorrei implementare un sistema di notifiche per informare gli utenti su nuove ricette, commenti sui loro post o aggiornamenti dell'app.
9. **Multilingua**: L'app dovrebbe supportare più lingue per raggiungere un pubblico più ampio.
10. **Backup e Sincronizzazione**: Vorrei includere una funzionalità di backup e sincronizzazione per permettere agli utenti di salvare le loro ricette nel cloud e accedervi da diversi dispositivi.

## Tecnologie Consigliate
1. **Frontend**: Angular 21 e bootstrap latest version. Frontend responsive per garantire una buona esperienza utente su dispositivi mobili e desktop.
2. **Backend**: C# NET 10 con ASP.NET Core per gestire le API RESTful e la logica di business.
3. **Database**: SQL Server per la gestione dei dati delle ricette
4. **Autenticazione**: Autenticazione a due fattori + autenticazione tramite social media con le tecnologie che ritieni più adatte.
5. **Hosting**: Installazione on premis su server proprietario con Windows Server 2022.
6. **Versionamento**: Utilizzo di Git per il controllo delle versioni del codice sorgente.
7. **Testing**: Implementazione di test unitari e di integrazione per garantire la qualità del codice.
8. **CI/CD**: Configurazione di pipeline di integrazione continua e distribuzione continua (CI/CD) per automatizzare il processo di build, test e deployment dell'applicazione.

## Font Ufficiale dell'App
⚠️ **IMPORTANTE**: I font ufficiali e OBBLIGATORI per tutta l'applicazione KneadHub sono:

### Font Body: Montserrat
- **URL**: https://fonts.google.com/specimen/Montserrat
- **Utilizzo**: TUTTO il testo dell'applicazione (body, UI, labels, buttons, inputs, paragraphs, etc.)
- **Pesi utilizzati**: 300 (light), 400 (regular), 500 (medium), 600 (semibold), 700 (bold)
- **Variabile CSS**: `--kh-font-body: 'Montserrat', system-ui, -apple-system, sans-serif`

### Font Titoli: DM Serif Display
- **URL**: https://fonts.google.com/specimen/DM+Serif+Display
- **Utilizzo**: Titoli di sezione, headings (h1, h2, h3), titoli decorativi
- **Pesi utilizzati**: 400 (regular), italic disponibile
- **Variabile CSS**: `--kh-font-display: 'DM Serif Display', Georgia, serif`

**Eccezione consentita**:
- Logo "KneadHub": `Pacifico` (solo per il logotipo)

**Regola imperativa**: Quando si crea o modifica qualsiasi componente UI, utilizzare SEMPRE Montserrat come font body e DM Serif Display per i titoli. Non introdurre altri font senza esplicita approvazione.

## MCP Server da utilizzare
1. **Frontend**: angular-cli - DEVI SEMPRE USARE QUESTO MCP SERVER quando SVILUPPI IL FRONTEND
3. **Database**: MSSQL MCP - DEVI SEMPRE USARE QUESTO MCP SERVER quando effettui operazioni da/verso IL DATABASE

