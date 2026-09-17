# 🚗 Automašīnu puzle

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

## 🧩 Nejaušināšana

Spēlē tiek izmantotas nejaušinātas automašīnu un to vietu sākuma pozīcijas.

Spēlē kopā ir:

- **12 automašīnas**;
- **12 automašīnu vietas**;
- **17 iespējamās automašīnu pozīcijas**;
- **17 iespējamās automašīnu vietu pozīcijas**.

Katras spēles sākumā pozīcijas tiek sajauktas, un 12 no 17 pozīcijām tiek izmantotas.

Tas nozīmē, ka katrā spēlē:

- 12 pozīcijas tiek aizņemtas;
- 5 pozīcijas paliek tukšas.

Tiek nejaušināti arī automašīnu parametri:

- pozīcija;
- rotācija;
- platums;
- augstums;
- spoguļattēls.

Tādējādi katra spēles palaišana veido atšķirīgu puzles konfigurāciju.

---

## 🖼️ Ekrānattēli

### Galvenā spēle

<!-- Ievieto ekrānattēlu šeit -->

### Spēles piemērs

<!-- Ievieto ekrānattēlu šeit -->

### Pareizi novietota automašīna

<!-- Ievieto ekrānattēlu šeit -->

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

Satur spēles objektu atsauces un apstrādā automašīnu un to vietu nejaušināšanu.

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
- [ ] Pilnībā pārbaudīt nejaušināšanu
- [ ] Noregulēt spēles grūtības pakāpi

---

## 🏠 Sākuma izvēlne

- [ ] Izveidot sākuma izvēlni
- [ ] Pievienot **Spēlēt** pogu
- [ ] Pievienot **Iziet** pogu
- [ ] Pievienot spēles nosaukumu
- [ ] Izveidot sākuma izvēlnes fonu
- [ ] Savienot **Spēlēt** pogu ar spēles ainu
- [ ] Savienot **Iziet** pogu ar programmas aizvēršanu
- [ ] Pārbaudīt ainu pārslēgšanu

---

## 🔊 Skaņa

- [x] Automašīnas mijiedarbības skaņa
- [x] Pareizas novietošanas skaņa
- [x] Nepareizas novietošanas skaņa
- [ ] Pievienot sākuma izvēlnes mūziku
- [ ] Pievienot spēles fona mūziku
- [ ] Pievienot pogu skaņas
- [ ] Pievienot skaļuma regulēšanu
- [ ] Pievienot skaņas izslēgšanas iespēju

---

## ✨ Animācijas un vizuālie efekti

- [ ] Pievienot pogu animācijas
- [ ] Pievienot pogu nospiešanas animācijas
- [ ] Pievienot automašīnas vilkšanas animāciju
- [ ] Pievienot pareizas novietošanas animāciju
- [ ] Pievienot vizuālu reakciju nepareizas novietošanas gadījumā
- [ ] Pievienot pārejas animāciju starp izvēlni un spēli
- [ ] Pievienot spēles pabeigšanas animāciju
- [ ] Pievienot nelielas kartes/vides animācijas

---

## 🏆 Spēles pabeigšana

- [ ] Noteikt, kad visas automašīnas ir pareizi novietotas
- [ ] Izveidot spēles pabeigšanas ekrānu/paziņojumu
- [ ] Pievienot pabeigšanas skaņu
- [ ] Pievienot iespēju sākt puzli no jauna
- [ ] Pievienot iespēju atgriezties sākuma izvēlnē

---

## ⚙️ Uzlabošana un noslēdzošā izstrāde

- [ ] Uzlabot lietotāja saskarni
- [ ] Pievienot vizuālu norādi izvēlētajai automašīnai
- [ ] Uzlabot nepareizas novietošanas vizuālo reakciju
- [ ] Pārbaudīt spēli dažādās ekrāna izšķirtspējās
- [ ] Pārbaudīt dažādus ekrāna malu attiecību formātus
- [ ] Novērst atlikušos Unity brīdinājumus/kļūdas, ja tie ietekmē spēli
- [ ] Veikt pilnu spēles testēšanu
- [ ] Izveidot gala `.exe` versiju
- [ ] Pārbaudīt gala versiju uz cita datora

---

## 🐛 Zināmās problēmas

Pašlaik nav zināmu spēli bloķējošu problēmu.

Unity Console var parādīties atsevišķi renderēšanas brīdinājumi, kas pašlaik neietekmē spēles darbību.

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
