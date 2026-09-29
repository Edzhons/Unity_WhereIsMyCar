# 🚗 Kur mans vāģ's?

Vienkārša 2D puzles spēle, kas izstrādāta **Unity**, kurā spēlētājam uz kartes jānovieto dažādas automašīnas tām paredzētajās vietās.

Spēlētājam katra automašīna ir jāaizvelk uz atbilstošo vietu un jāpielāgo tās:

- pozīcija;
- izmērs;
- rotācija;
- spoguļattēls.

Kad visi parametri atbilst paredzētajai vietai, automašīna tiek uzskatīta par pareizi novietotu.

---

## 🎮 Spēles vadība

| Taustiņš | Darbība |
|---|---|
| `Kreisā peles poga` | Vilkt automašīnu |
| `Z` | Rotēt pretēji pulksteņrādītāja virzienam |
| `X` | Rotēt pulksteņrādītāja virziená |
| `↑` | Palielināt augstumu |
| `↓` | Samazināt augstumu |
| `←` | Samazināt platumu |
| `→` | Palielināt platumu |
| `Space` | Izveidot spoguļattēlu |

Automašīna tiek uzskatīta par pareizi novietotu, ja:

- tai ir pareizais tags;
- rotācijas starpība ir pieļaujamajā robežā;
- platuma un augstuma starpība ir pieļaujamajā robežā;
- spoguļattēla stāvoklis atbilst paredzētajai vietai.

---

## 🧩 Randomizācija

Spēlē tiek izmantotas randomizētas automašīnu un to vietu sākuma pozīcijas.

Spēlē kopā ir:

- **12 automašīnas**;
- **12 automašīnu vietas**;
- **17 iespējamās automašīnu pozīcijas**;
- **17 iespējamās automašīnu vietu pozīcijas**.

Katras spēles sākumā pozīcijas tiek sajauktas, un 12 no 17 pozīcijām tiek izmantotas.

Tas nozīmē, ka katrā spēlē:

- 12 pozīcijas tiek aizņemtas;
- 5 pozīcijas paliek tukšas.

Tiek randomizēti arī automašīnu parametri:

- pozīcija;
- rotācija;
- platums;
- augstums;
- spoguļattēls.

Tādējādi katra spēles palaišana veido atšķirīgu puzles konfigurāciju.

---

## 🖼️ Ekrānattēli

### Sākuma ekrāns

![Galvenā spēle](Assets/startMenu.png)

### Spēles piemērs

![Spēles piemērs](Assets/gameplay.png)

### Uzvaras ekrāns

![Pareizi novietota automašīna](Assets/winPopup.png)

---

## 🛠️ Izmantotās tehnoloģijas

- **Unity**
- **C#**
- Unity UI / `RectTransform`
- Unity Event System
- Git
- GitHub

---

## 📁 Projekta struktūra

Galvenie spēlē izmantotie skripti:

```text
Scripts/
├── DragAndDropScript.cs
├── DropPlaceScript.cs
├── GameObjectsScript.cs
├── ObjectTransformationScript.cs
└── ScreenBoundaryScript.cs
```

### `DragAndDropScript.cs`

Atbild par automašīnu vilkšanu ar peli.

### `DropPlaceScript.cs`

Atbild par automašīnas ievietošanu paredzētajā vietā un pārbauda, vai automašīna atbilst vietai.

Tiek pārbaudīts:

- tags;
- rotācija;
- izmērs;
- spoguļattēls.

### `GameObjectsScript.cs`

Satur spēles objektu atsauces un apstrādā automašīnu un to vietu randomizāciju.

### `ObjectTransformationScript.cs`

Atbild par izvēlētās automašīnas parametru mainīšanu:

- rotāciju;
- platumu;
- augstumu;
- spoguļattēlu.

### `ScreenBoundaryScript.cs`

Nodrošina, ka vilktie objekti paliek atļautajā kartes/ekrāna apgabalā.

---

# ✅ TODO

## 🎯 Spēles pamatfunkcionalitāte

- [x] Automašīnu vilkšana
- [x] Automašīnu un vietu sasaistīšana ar tagiem
- [x] Automašīnu rotēšana
- [x] Automašīnu izmēra mainīšana
- [x] Automašīnu spoguļošana
- [x] Pareizas novietošanas pārbaude
- [x] Nepareizas novietošanas apstrāde
- [x] Nejaušas automašīnu pozīcijas
- [x] Nejaušas automašīnu vietu pozīcijas
- [x] Nejauša rotācija
- [x] Nejaušs izmērs
- [x] Nejaušs spoguļattēls
- [x] Pilnībā pārbaudīt randomizāciju

---

## 🏠 Sākuma izvēlne

- [x] Izveidot sākuma izvēlni
- [x] Pievienot **Līmenis 1** pogu
- [x] Pievienot **Iziet** pogu
- [x] Izveidot sākuma izvēlnes fonu
- [x] Savienot **Līmenis 1** pogu ar spēles ainu
- [x] Savienot **Iziet** pogu ar programmas aizvēršanu
- [x] Pārbaudīt ainu pārslēgšanu

---

## 🔊 Skaņa

- [x] Automašīnas mijiedarbības skaņa
- [x] Pareizas novietošanas skaņa
- [x] Nepareizas novietošanas skaņa
- [x] Pievienot sākuma izvēlnes mūziku
- [x] Pievienot spēles fona mūziku
- [x] Pievienot pogu skaņas
- [x] Pievienot skaņas izslēgšanas iespēju

---

## ✨ Animācijas un vizuālie efekti

- [x] Pievienot pogu animācijas
- [x] Pievienot pogu nospiešanas animācijas
- [x] Pievienot pareizas novietošanas animāciju
- [x] Pievienot vizuālu reakciju nepareizas novietošanas gadījumā
- [x] Pievienot nelielas kartes/vides animācijas

---

## 🏆 Spēles pabeigšana

- [x] Noteikt, kad visas automašīnas ir pareizi novietotas
- [x] Izveidot spēles pabeigšanas ekrānu/paziņojumu
- [x] Pievienot pabeigšanas skaņu
- [x] Pievienot iespēju sākt puzli no jauna
- [x] Pievienot iespēju atgriezties sākuma izvēlnē

---

## ⚙️ Uzlabošana un noslēdzošā izstrāde

- [x] Uzlabot lietotāja saskarni
- [x] Pievienot vizuālu norādi izvēlētajai automašīnai
- [x] Uzlabot nepareizas novietošanas vizuālo reakciju
- [x] Pārbaudīt spēli dažādās ekrāna izšķirtspējās
- [x] Pārbaudīt dažādus ekrāna malu attiecību formātus
- [x] Novērst atlikušos Unity brīdinājumus/kļūdas, ja tie ietekmē spēli
- [x] Veikt pilnu spēles testēšanu

---

## 🐛 Zināmās problēmas

Pašlaik nav zināmu spēli bloķējošu problēmu.

---

## 📌 Projekta statuss

**Izstrādes stadija:** Aktīva izstrāde

Spēles galvenā puzles funkcionalitāte ir izveidota. Tālāk plānots izstrādāt sākuma izvēlni, papildināt skaņas un animācijas, uzlabot vizuālo noformējumu un veikt gala testēšanu.

---

## 👨‍💻 Autors

**Edžus Krūmiņš**

Programmēšanas tehniķis  
Liepājas Valsts tehnikums

---

## 📜 Licence

Šis projekts ir izstrādāts mācību un noslēguma projekta vajadzībām.

Visas tiesības aizsargātas.
