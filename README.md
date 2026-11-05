# Automašīnu puzle un Hanojas torņi

Unity izstrādāta loģikas spēle ar diviem spēles režīmiem: automašīnu
izvietošanas puzli un klasisko Hanojas torņu uzdevumu. Projektā iekļauta
spēles progresija, zvaigžņu atlīdzības, kopīga spēles valūta, veikals ar
atbloķējamām disku tekstūrām un palīgtokeniem, kā arī Unity Ads
integrācija.

## Saturs

-   [Spēles režīmi](#spēles-režīmi)
    -   [Automašīnu puzle](#automašīnu-puzle)
    -   [Hanojas torņi](#hanojas-torņi)
-   [Progresija un zvaigžņu sistēma](#progresija-un-zvaigžņu-sistēma)
-   [Veikals](#veikals)
-   [Reklāmas](#reklāmas)
-   [Projekta struktūra](#projekta-struktūra)
-   [Projekta palaišana](#projekta-palaišana)
-   [Vadība](#vadība)
-   [Piezīmes izstrādātājiem](#piezīmes-izstrādātājiem)

## Spēles režīmi

Projektā ir trīs Unity ainas:

  -----------------------------------------------------------------------
  Aina                                Apraksts
  ----------------------------------- -----------------------------------
  `StartMenu`                         Galvenā izvēlne, spēles režīmu
                                      izvēle, veikals un citas izvēlnes
                                      funkcijas

  `CityScene`                         Automašīnu puzles spēles režīms

  `TowerOfHanoi`                      Hanojas torņu loģikas spēle
  -----------------------------------------------------------------------

### Galvenā izvēlne

Galvenā izvēlne ir spēles centrālais ekrāns. Tajā spēlētājs var
izvēlēties kādu no spēles režīmiem un atvērt spēles veikalu. Izvēlnē ir
arī interaktīvi elementi un skaņas efekti.

**Ekrānuzņēmums --- galvenā izvēlne**

![Galvenās izvēlnes ekrānuzņēmums](docs/screenshots/main-menu.png)

### Automašīnu puzle

Šajā režīmā spēlētājam ar vilkšanas un nomešanas principu jānovieto
automašīnas tām paredzētajās stāvvietās kartē. Lai automašīna atbilstu
mērķa vietai, var būt nepieciešams mainīt tās izmēru, spoguļattēlu vai
pagriezienu.

Spēles funkcionalitāte: - Automašīnu pārvietošana ar vilkšanas un
nomešanas metodi. - Automašīnu izmēra, spoguļattēla un pagrieziena
maiņa. - Taimeris un dzīvību sistēma. - Lidojoši šķēršļi, kuru sadursme
var atņemt spēlētājam dzīvību. - Uzvaras un zaudējuma ekrāni. - Zvaigžņu
atlīdzība atkarībā no izpildes laika.

**Ekrānuzņēmums --- automašīnu puzles spēle**

![Automašīnu puzles spēles
ekrānuzņēmums](docs/screenshots/car-puzzle-gameplay.PNG)

**Ekrānuzņēmums --- automašīnu puzles rezultāti**

![Automašīnu puzles rezultātu
ekrānuzņēmums](docs/screenshots/car-puzzle-results.PNG)

### Hanojas torņi

Hanojas torņi ir papildu loģikas spēles režīms, kura pamatā ir
klasiskais disku pārvietošanas uzdevums. Mērķis ir pārvietot visu disku
kaudzi no pirmā stieņa uz trešo, ievērojot noteikumus:

1.  Vienā gājienā drīkst pārvietot tikai vienu disku.
2.  Pārvietot drīkst tikai stieņa augšējo disku.
3.  Lielāku disku nedrīkst novietot uz mazāka diska.

Spēlei ir **trīs grūtības līmeņi**:

  Līmenis         Disku skaits   Minimālais gājienu skaits
  ------------- -------------- ---------------------------
  1\. līmenis                3                           7
  2\. līmenis                4                          15
  3\. līmenis                5                          31

Spēlētājs pārvieto diskus starp trim stieņiem. Spēle uzskaita veikto
gājienu skaitu un salīdzina to ar teorētiski minimālo gājienu skaitu.

#### Zvaigžņu vērtējums un līmeņu atbloķēšana

Pēc līmeņa pabeigšanas spēlētājs saņem zvaigznes atkarībā no veikto
gājienu skaita:

-   **3 zvaigznes:** līmenis atrisināts ar minimālo iespējamo gājienu
    skaitu.
-   **2 zvaigznes:** līmenis atrisināts ar ne vairāk kā 1,5 reizēm
    lielāku gājienu skaitu par minimumu.
-   **1 zvaigzne:** līmenis pabeigts, izmantojot vairāk gājienu.

Katram līmenim tiek saglabāts labākais iegūtais zvaigžņu rezultāts. Pēc
līmeņa pabeigšanas tiek atbloķēts nākamais līmenis. Līmeņu izvēlnē
redzams katra līmeņa pieejamības statuss un iegūtais zvaigžņu vērtējums.

#### Padomi un gājiena atsaukšana

Spēlētājam pieejamas divas palīgfunkcijas: - **Padoms:** norāda, kuru
disku pārvietot un uz kuru stieni to novietot. - **Atsaukt gājienu:**
atceļ pēdējo derīgo gājienu.

Padomus un gājienu atsaukšanu var izmantot ar veikalā iegādātiem
tokeniem. Ja spēlētājam tokenu nav, spēle var piedāvāt noskatīties
atlīdzības reklāmu. Palīgfunkcija tiek piešķirta tikai pēc veiksmīgi
pabeigtas reklāmas noskatīšanās.

**Ekrānuzņēmums --- Hanojas torņu līmeņu izvēlne**

![Hanojas torņu līmeņu izvēlnes
ekrānuzņēmums](docs/screenshots/hanoi-level-selection.png)

**Ekrānuzņēmums --- Hanojas torņu spēles process**

![Hanojas torņu spēles
ekrānuzņēmums](docs/screenshots/hanoi-gameplay.png)

**Ekrānuzņēmums --- Hanojas torņu līmeņa rezultāts**

![Hanojas torņu rezultātu
ekrānuzņēmums](docs/screenshots/hanoi-results.png)

## Progresija un zvaigžņu sistēma

Abi spēles režīmi papildina kopējo zvaigžņu bilanci.

-   Automašīnu puzlē zvaigznes tiek piešķirtas atkarībā no līmeņa
    izpildes laika.
-   Hanojas torņos zvaigznes tiek piešķirtas atkarībā no gājienu
    efektivitātes.
-   Zvaigžņu bilance tiek saglabāta lokāli, izmantojot Unity
    `PlayerPrefs`.
-   Iegūtās zvaigznes var iztērēt spēles veikalā.
-   Hanojas torņu līmeņu atbloķēšana un katra līmeņa labākais zvaigžņu
    rezultāts arī tiek saglabāts lokāli.

Kopējo zvaigžņu bilanci pārvalda statiskā klase `StarBank`.

## Veikals

Veikals ir pieejams galvenajā izvēlnē un izmanto kopējo zvaigžņu
bilanci. Tajā pieejami pastāvīgi pirkumi, disku vizuālie noformējumi un
patērējamu palīgtokenu komplekti.

Projektā pašlaik konfigurētie piedāvājumi:

  ------------------------------------------------------------------------
  Prece                 Veids                                         Cena
  --------------------- --------------------- ----------------------------
  Reklāmu atspējošana   Pastāvīgs pirkums                     30 zvaigznes
  (`bez_reklamam`)                            

  Marmora diska izskats Diska izskats                         15 zvaigznes
  (`marmors`)                                 

  Zelta diska izskats   Diska izskats                         25 zvaigznes
  (`zelts`)                                   

  Koka diska izskats    Diska izskats                          8 zvaigznes
  (`koks`)                                    

  Padomu tokeni         3 tokenu komplekts                     5 zvaigznes
  (`padomi`)                                  

  Gājienu atsaukšanas   3 tokenu komplekts                     5 zvaigznes
  tokeni                                      
  (`atsauksanas`)                             

  Krāsainais /          Noklusējuma izskats                      Bezmaksas
  noklusējuma diska                           
  izskats (`krasainie`)                       
  ------------------------------------------------------------------------

Iegādātos disku noformējumus var izvēlēties Hanojas torņu režīmā. Tokenu
komplekti nodrošina trīs izmantošanas reizes. Veikals atjauno attēloto
zvaigžņu bilanci un piedāvājumu stāvokli pēc zvaigžņu vai preču
īpašumtiesību izmaiņām.

**Ekrānuzņēmums --- spēles veikals**

![Veikala ekrānuzņēmums](docs/screenshots/shop.png)

## Reklāmas

Projektā izmantota Unity Ads integrācija, kuras darbību nodrošina
skripti mapē `Assets/Scripts/Ads/`.

Integrācijā iekļauts: - Pilnekrāna reklāmas (`Interstitial Ads`). -
Atlīdzības reklāmas (`Rewarded Ads`). - Baneru reklāmas
(`Banner Ads`). - Reklāmu pārvaldnieks, kas saglabājas starp ainām. -
Veikala pirkums reklāmu atspējošanai.

Atlīdzības reklāmas Hanojas torņu režīmā var izmantot kā alternatīvu
padomu vai gājiena atsaukšanas tokeniem, ja spēlētājam to nav.

> Reklāmu pieejamība un attēlošana ir atkarīga no Unity Ads
> konfigurācijas, platformas atbalsta, izvietojumu ID un Unity Ads
> informācijas paneļa iestatījumiem. Izstrādes laikā izmanto testēšanas
> režīmu.

## Projekta struktūra

``` text
Assets/
├── Animations/
├── Editor/
├── Fonts/
├── Prefabs/
├── Resources/
├── Scenes/
│   ├── StartMenu.unity
│   ├── CityScene.unity
│   └── TowerOfHanoi.unity
├── Scripts/
│   ├── Ads/
│   │   ├── AdManager.cs
│   │   ├── AdsInitializer.cs
│   │   ├── BannerAd.cs
│   │   ├── InterstitialAd.cs
│   │   └── RewardedAds.cs
│   ├── Shop/
│   │   ├── StarBank.cs
│   │   ├── ShopPanel.cs
│   │   ├── ShopOffer.cs
│   │   ├── StarBalanceLabel.cs
│   │   └── ...
│   ├── TowerOfHanoi/
│   │   ├── HanoiGameManager.cs
│   │   ├── HanoiRod.cs
│   │   └── HanoiDisk.cs
│   ├── GameManagerScript.cs
│   ├── GameObjectsScript.cs
│   ├── DragAndDropScript.cs
│   └── ...
└── ...
```

### Galvenie skripti

  -----------------------------------------------------------------------
  Skripts                             Atbildība
  ----------------------------------- -----------------------------------
  `StartMenuScript.cs`                Galvenās izvēlnes navigācija,
                                      skaņas un interaktīvās izvēlnes
                                      darbības

  `GameManagerScript.cs`              Automašīnu puzles taimeris,
                                      dzīvības, uzvaras/zaudējuma
                                      stāvoklis un laika zvaigžņu
                                      atlīdzība

  `GameObjectsScript.cs`              Automašīnu objekti un to
                                      novietošanas loģika

  `DragAndDropScript.cs`              Automašīnu UI objektu pārvietošana

  `HanoiGameManager.cs`               Hanojas torņu līmeņi, atļautie
                                      gājieni, gājienu skaits, zvaigznes,
                                      padomi, atsaukšana un progresija

  `HanoiRod.cs`                       Disku uzskaite uz stieņiem un disku
                                      nomešanas vietu apstrāde

  `HanoiDisk.cs`                      Disku inicializācija, pārvietošana,
                                      animācija un padoma izcelšana

  `StarBank.cs`                       Kopējā zvaigžņu bilance, veikala
                                      pirkumi, disku noformējumi un
                                      palīgtokeni

  `ShopPanel.cs` / `ShopOffer.cs`     Veikala panelis, pirkumu apstrāde
                                      un piedāvājumu saskarne

  `AdManager.cs`                      Reklāmu pārvaldība starp ainām

  `AdsInitializer.cs`                 Unity Ads inicializācija

  `InterstitialAd.cs`                 Pilnekrāna reklāmu ielāde un
                                      attēlošana

  `RewardedAds.cs`                    Atlīdzības reklāmu ielāde,
                                      attēlošana un pabeigšanas apstrāde

  `BannerAd.cs`                       Baneru reklāmu ielāde, novietojums
                                      un redzamība
  -----------------------------------------------------------------------

## Projekta palaišana

1.  Instalē Unity versiju, kas ir saderīga ar projektā izmantotajām
    pakotnēm.
2.  Atver projektu, izmantojot Unity Hub.
3.  Sagaidi, līdz Unity importē resursus un atrisina pakotņu atkarības.
4.  Atver ainu `Assets/Scenes/StartMenu.unity`.
5.  Nospied **Play**, lai sāktu spēli no galvenās izvēlnes.

Projektā izmantoto ainu nosaukumi ir `StartMenu`, `CityScene` un
`TowerOfHanoi`. Saglabā šos nosaukumus, jo skriptos ir atsauces uz ainām
pēc to nosaukumiem.

Veidojot Android versiju, pārliecinies, ka instalēts Android Build
Support modulis un konfigurēti nepieciešamie Unity Ads iestatījumi.
Pirms produkcijas versijas izveides pārbaudi Android Game ID un reklāmu
izvietojumu ID skriptos un Unity Ads informācijas panelī.

## Vadība

### Automašīnu puzle

-   Velc automašīnas ar peli vai skārienekrānu.
-   Novieto katru automašīnu tai paredzētajā vietā.
-   Izmanto ekrānā pieejamās vadības pogas.

### Hanojas torņi

-   Velc stieņa augšējo disku uz citu stieni.
-   Gājiens tiek noraidīts, ja tas pārkāpj disku izmēru noteikumus.
-   Ja pieejams, izmanto pogas **Padoms** un **Atsaukt**.
-   Pabeidz pašreizējo līmeni, lai atbloķētu nākamo.

## Piezīmes izstrādātājiem

-   Spēlētāja progress, zvaigžņu bilance, pirkumi, izvēlētais diska
    noformējums, Hanojas torņu atbloķētie līmeņi un labākie zvaigžņu
    rezultāti tiek saglabāti ierīcē, izmantojot `PlayerPrefs`.
-   Projektā ir testēšanas funkcijas: `F1` piespiedu kārtā pabeidz
    automašīnu puzles vai Hanojas torņu līmeni atkarībā no aktīvā
    režīma; `F2` piešķir 50 zvaigznes. Galvenajā izvēlnē ir arī slēpta
    pīles mijiedarbība, kas piešķir testa zvaigznes. Pirms publiskas
    spēles versijas izdošanas šīs testēšanas funkcijas nepieciešams
    noņemt vai atspējot.
-   Projekts satur Unity Editor skriptus mapē `Assets/Editor/`.
-   Spēlē izmantoti Unity UI un TextMeshPro komponenti.

------------------------------------------------------------------------
