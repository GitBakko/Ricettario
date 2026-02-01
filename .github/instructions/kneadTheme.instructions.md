# Prompt per Generazione e Applicazione Tema KneadHub (Angular 21)

**Utente:** Stefano  
**App:** KneadHub – gestione smart ricette di panificazione  
**Tecnologia frontend:** Angular 21 (app già sviluppata e quasi completa al 100%)  
**Obiettivo:** SOLO generare, gestire e applicare un tema UI/UX completo basato sui due mockup (desktop + mobile), senza toccare logica, componenti o struttura esistente.

## Input Visivi – Due Mockup

## Mockup di Riferimento
- Desktop: [mockup_desktop.jpg] - Layout a 3 colonne (sidebar + grid + sidebar destra)
- Mobile: [mockup_mobile.jpg] - Layout verticale con bottom navigation

## Logo
- **Favicon**: `favicon.png` - Baguette/pane stilizzato, usare come favicon, icona mobile, e dove serve solo l'icona
- **Logo sfondo scuro**: `logo.png` - Logo con scritta chiara, per header/navbar su sfondi scuri (es. gradient marrone)
- **Logo sfondo chiaro**: `logo_light.png` - Logo con scritta scura, per sfondi chiari o pagine con background crema

## Immagine di Sfondo
- **Background Kitchen**: `background-kitchen.jpg` - Cucina rustica con pane fresco, taglieri in legno, farina sparsa. Usare sfocata (CSS blur) come sfondo delle pagine principali.

### 1. Mockup Desktop (layout ampio)
- Sfondo: immagine sfocata cucina rustica con pane fresco, taglieri legno, farina sparsa, toni caldi beige/marrone/arancione chiaro.
- Container principale: bianco/crema, angoli arrotondati soft (~16–24px), ombra leggera.
- Logo: "KneadHub" font handwritten/organico bold + icona baguette stilizzata marrone pane (#D2691E / #8B4513).
- Barra ricerca: centrata, sfondo beige chiaro (#FFF5E6), placeholder "Search recipes, tips, and more…", bordi arrotondati, icona lente marrone.
- Top-right: icona campanella (badge rosso), avatar circolare.
- Sidebar sinistra: icone + testo verticale ("Home", "Recipes", "Community", "Profile", menu in basso), testo marrone scuro, hover arancione.
- Contenuto principale: masonry/grid card ricette (3–4 colonne), foto pane realistica + overlay gradient scuro in basso per titolo bianco bold, cuore like (grigio → rosso hover), badge "New"/"Popular".
- Destra: "Featured Baker" (card profilo circolare, nome, stelle dorate #FFD700), "Trending Now" (mini card verticali), bottone "Connect" arancione (#F4A261).
- Vibe: caldo, artigianale, invitante, gradienti organici, ombre soft, tipografia mista (sans-serif body + serif/handwritten titoli/logo).

### 2. Mockup Mobile (responsive)
- Layout verticale full-screen, stesso sfondo kitchen warm.
- Header: logo piccolo centrato + menu hamburger sinistra, avatar/notifiche destra.
- Barra ricerca: sotto header, larga, stile beige.
- Navigazione: bottom bar icone oppure sidebar laterale scorrevole.
- Card ricette: stacked verticali (1–2 per riga), stile compatto, swipe/tap per like.
- Sezioni laterali (Featured + Trending): overlay cards semi-trasparenti con glassmorphism (backdrop-filter: blur), floating o popover.
- Responsività: sezioni laterali diventano floating cards o scroll orizzontale su mobile.

## Obiettivo del Tema
Genera tema coerente **KneadHubTheme** ispirato esattamente ai mockup, con:
- gradient "lievitazione"
- animazioni subtle
- icone pane-inspired
- gestione dinamica (light/dark mode + switch)
- applicazione senza alterare logica/componenti core

## Tecnologia & Libertà di Scelta
- Angular 21
- **Gestione tema (Theme Service o equivalente):** sentiti completamente libero di scegliere l'approccio che ritieni migliore e più adatto (servizio injectable con Subject/BehaviorSubject, provider globale, toggle classi su root/body, Angular Material Theme se presente, o qualunque soluzione moderna, scalabile e performante). **Non sei vincolato a un'implementazione specifica** — decidi tu la migliore.
- Best practices: CSS variables in `:root` (styles.scss), SCSS modulare, media queries responsive, Angular Animations per effetti.

## Componenti UI Chiave
1. **Recipe Card**: 
   - Foto full-bleed con overlay gradient
   - Badge "New"/"Popular" in alto a sinistra
   - Cuore like in alto a destra
   - Titolo + meta info in basso

2. **Featured Baker Card**:
   - Avatar circolare
   - Nome + stelle rating (#FFD700)
   - Bottone "Connect" arancione

3. **Trending Now**:
   - Mini card con thumbnail + titolo
   - Layout verticale stacked

## Breakpoints
- Desktop: >= 1200px (3 colonne: sidebar + grid + aside)
- Tablet: 768px - 1199px (2 colonne: grid + floating panels)
- Mobile: < 768px (1 colonna + bottom nav + floating cards)

## Dark Mode
- Background: #0F0A05 (marrone scurissimo)
- Surface: #1E1A15
- Text: #F5F0EB
- Accent: Più vividi (+10% saturazione)

## Micro-interazioni
- Card hover: translateY(-4px) + shadow expand (0.2s ease)
- Like heart: scale(1.3) + fill red (#E63946) con pulse
- Page transition: fade-in 0.3s
- Skeleton loading: shimmer gradient animation

## Struttura File Consigliata
```
src/
├── styles/
│   ├── _variables.scss      # CSS custom properties (:root)
│   ├── _theme-light.scss    # Light mode overrides
│   ├── _theme-dark.scss     # Dark mode overrides (prep)
│   ├── _animations.scss     # Keyframes & transitions
│   └── _components.scss     # Component-specific styles
├── assets/
│   ├── i18n/
│   │   ├── it.json          # Traduzioni italiano
│   │   └── en.json          # Traduzioni inglese
│   ├── images/
│   │   ├── favicon.png      # Favicon/icona
│   │   ├── logo.png         # Logo per sfondo scuro
│   │   ├── logo_light.png   # Logo per sfondo chiaro
│   │   └── background-kitchen.jpg  # Sfondo cucina
│   └── fonts/               # Custom fonts (se self-hosted)
└── app/
    ├── core/
    │   ├── services/
    │   │   └── theme.service.ts     # Theme management
    │   └── data/
    │       └── mock-data.ts         # Dati mock centralizzati
    └── components/
        ├── sidebar/                 # Navigazione laterale
        ├── featured-baker/          # Card baker in evidenza
        ├── trending-now/            # Ricette trending
        └── bottom-nav/              # Nav mobile
```

## ⛔ Da NON Fare
- Non modificare la logica dei componenti esistenti
- Non alterare le rotte o la struttura di navigazione
- Non cambiare i modelli dati o i servizi API
- Non rimuovere funzionalità esistenti
- Non sovrascrivere stili Bootstrap core senza necessità

## Priorità Implementazione
1. 🔴 **Alta**: Variabili CSS globali + palette colori
2. 🔴 **Alta**: Recipe Card styling (componente più visibile)
3. 🔴 **Alta**: Navbar e layout principale
4. 🟡 **Media**: Layout 3 colonne desktop
5. 🟡 **Media**: Responsive mobile + bottom nav
6. � **Media**: Multilingua (i18n) - setup base
7. 🟢 **Bassa**: Dark mode toggle
8. 🟢 **Bassa**: Animazioni avanzate e micro-interazioni

## Multilingua (i18n)
L'app deve supportare più lingue fin dall'inizio. Requisiti:
- **Lingue iniziali**: Italiano (default), English
- **Approccio consigliato**: `ngx-translate` per flessibilità e facilità d'uso
- **File traduzioni**: `src/assets/i18n/it.json`, `src/assets/i18n/en.json`
- **Selector lingua**: Dropdown nel header (vicino avatar) o nelle impostazioni profilo
- **Persistenza**: Salvare preferenza lingua in localStorage
- **Contenuto da tradurre**:
  - Label UI (bottoni, menu, placeholder)
  - Messaggi di errore e successo
  - Testi statici (Coming Soon, titoli sezioni)
  - **NON tradurre**: Nomi ricette, ingredienti, istruzioni (contenuto utente)

## Mock Data Index
Tutti i dati mock devono essere centralizzati in `src/app/core/data/mock-data.ts` con commento `// @MOCK:` per tracciabilità:

| Costante | Tipo | Usato In |
|----------|------|----------|
| `MOCK_FEATURED_BAKERS` | `FeaturedBaker[]` | FeaturedBakerComponent |
| `MOCK_TRENDING_RECIPES` | `TrendingRecipe[]` | TrendingNowComponent |
| `MOCK_NAV_ITEMS` | `NavItem[]` | SidebarComponent |
| `MOCK_NOTIFICATION_COUNT` | `number` | Header (badge campanella) |
| `MOCK_RECIPE_BADGES` | `Map<number, string>` | RecipeCard ("New"/"Popular") |

## Hamburger Menu (Mobile)
- Stile: Bottone circolare beige chiaro (`#FFF5E6`) con icona hamburger marrone (`#3D2B1F`)
- Deve matchare lo stile del bottone avatar (circolare, stesso sizing)
- On click: apre sidebar come overlay da sinistra con animazione slide-in
- Backdrop semi-trasparente per chiudere

## Placeholder Pages
- **Community** (`/community`): Pagina "Coming Soon" con illustrazione pane e messaggio
- **Profile** (`/profile`): Pagina "Coming Soon" con avatar placeholder
- Stile: Centrato, sfondo crema, testo marrone, icona decorativa

## Procedi step-by-step (ultra-dettagliato)

1. **Analisi dei Mockup**  
   Estrai e documenta il tema da entrambi i mockup:  
   - **Palette Colori** (definisci --var CSS):  
     --background: #FDF8F0 (crema farina)  
     --surface: #FFFFFF / #1E1E1E (light/dark)  
     --primary: #D2691E (marrone pane)  
     --accent: #F4A261 (arancione caldo)  
     --text-primary: #3D2B1F  
     --text-secondary: #6B4E31  
     --highlight: #FFD700 (dorato stelle/rating)  
     --gradient-pane: linear-gradient(135deg, #FFF5E6, #F4E4D0)  
     --error: #E63946 (badge rosso)  
   - **Tipografia**: 'Playfair Display' o serif per titoli/logo, 'Inter'/'Roboto' per body; sizes: h1/logo 36–48px, card-title 18–20px, body 14–16px  
   - **Bordi & Ombre**: border-radius 16–24px; box-shadow 0 8px 24px rgba(139,69,19,0.08)  
   - **Componenti chiave**: card ricetta (foto full + overlay gradient), nav icons (stroke marrone), like heart (scale + fill rosso), glassmorphism mobile (backdrop-filter: blur(12px); bg rgba(255,255,255,0.7))  
   - **Animazioni**: fade-in card on scroll, hover scale 1.03 + shadow grow, heart pulse on like  
   - **Varianti**: Light (base mockup), Dark (sfondo #0F0A05, testi chiari, accenti vividi)

2. **Proposta Tema**  
   Descrivi 'KneadHubTheme' come tema artigianale-tech (pane + modern minimal), config strutturata (es. JSON-like per colori, fonts, radii).

3. **Generazione Codice**  
   - Implementa gestione tema con l’approccio che preferisci  
   - Global Styles (styles.scss): :root variabili, classi light/dark, media queries  
   - Esempi applicazione in componenti (es. recipe-card.component.scss)  
   - Inizializzazione tema in App Component / root  
   - Snippet Angular Animations per card entry, heart like  
   - Responsive: media queries + flex/grid adjustments (sidebar → bottom nav su mobile)  
   - Asset: suggerisci dove mettere bg kitchen, icone SVG pane in assets/

4. **Ottimizzazioni & Test**  
   - Accessibilità: WCAG AA contrasto, aria-label icone  
   - Performance: CSS vars efficienti, animazioni GPU-accelerated, lazy images  
   - Test: Lighthouse mobile/desktop, theme switch, responsive devtools

**Rendi il tema super figo, caldo e invitante come pane appena sfornato, scalabile e pronto per Angular 21.**  
Output in sezioni chiare con codice in blocchi Markdown.  
Suggerisci file structure (es. src/themes/, src/assets/icons/).  
Proponi estensioni future se rilevanti (es. tema utente personalizzato).

**Sfrutta i server MCP che hai a disposizione**
sfrutta angular-cli e context7 per ritrovare tutte le best practices aggiornate per implementare il tema a regola d'arte su Angular 21.