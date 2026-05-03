# dCom

Desktop WPF aplikacija za komunikaciju sa Modbus TCP uređajem/simulatorom, periodično očitavanje tačaka i prikaz/obradu analognih i digitalnih signala.

## O projektu

`dCom` je .NET Framework 4.8 rešenje organizovano u više projekata:

- **dCom** – WPF korisnički interfejs, ViewModel-i, učitavanje konfiguracije
- **Common** – zajednički interfejsi, enumeracije i pomoćne klase
- **Modbus** – Modbus TCP komunikacija i implementacija funkcija
- **ProcessingModule** – obrada signala, alarmi, EGU konverzija i periodični polling

Aplikacija učitava konfiguraciju iz fajla `RtuCfg.txt`, uspostavlja TCP konekciju prema Modbus uređaju/simulatoru i zatim periodično čita ili upisuje vrednosti registara/coils-a. Dobijene vrednosti se obrađuju i prikazuju kroz GUI.

## Glavne funkcionalnosti

- Modbus TCP komunikacija
- Implementirane Modbus funkcije:
  - `01` Read Coils
  - `02` Read Discrete Inputs
  - `03` Read Holding Registers
  - `04` Read Input Registers
  - `05` Write Single Coil
  - `06` Write Single Register
- Periodični acquisition/polling nad konfigurisanim tačkama
- Obrada analognih i digitalnih tačaka
- EGU konverzija i osnovna alarm logika
- Prikaz statusa konekcije i log poruka u aplikaciji
- Učitavanje i validacija konfiguracije iz `RtuCfg.txt`

## Tehnologije

- C#
- WPF
- .NET Framework 4.8
- Visual Studio solution (`dCom.sln`)

## Struktura projekta

```text
 dCom/
 ├── dCom.sln
 ├── Common/
 ├── Modbus/
 ├── ProcessingModule/
 └── dCom/
     ├── Configuration/
     ├── ViewModel/
     ├── Converters/
     ├── Utils/
     ├── MainWindow.xaml
     ├── ControlWindow.xaml
     └── RtuCfg.txt
```

## Konfiguracija

Aplikacija koristi fajl `RtuCfg.txt` kao ulaznu konfiguraciju.

Primer iz projekta:

```txt
STA 77
TCP 47961

DO_REG  1 2300  0  0     1     0  DO  @STOP       1
DO_REG  1 2302  0  0     1     0  DO  @Ventil_V1  1
DO_REG  1 2305  0  0     1     0  DO  @P1         1
DO_REG  1 2306  0  0     1     0  DO  @P2         1
IN_REG   1 2200  0  0  4095     0  AI  @L          3
HR_INT  1 1400  0  0  4095     0  AO  @N1         3
```

### Značenje osnovnih parametara

- **STA** – Modbus unit/slave adresa
- **TCP** – TCP port na koji se aplikacija povezuje
- ostale linije definišu tačke koje se čitaju ili upisuju

Ako `RtuCfg.txt` ne postoji u izlaznom folderu, aplikacija pokušava da traži konfiguracioni fajl ručno.

## Pokretanje

### Preduslovi

- Windows
- Visual Studio 2019/2022 ili noviji koji podržava .NET Framework 4.8
- .NET Framework 4.8 Developer Pack
- Modbus TCP simulator ili stvarni uređaj

### Koraci

1. Otvori `dCom.sln` u Visual Studio-u.
2. Restore/učitaj sve projekte iz solution-a.
3. Build solution.
4. Pokreni Modbus TCP simulator ili obezbedi dostupan uređaj.
5. Proveri da se port i adresa iz simulatora poklapaju sa vrednostima iz `RtuCfg.txt`.
6. Pokreni `dCom` kao startup projekat.

## Kako aplikacija radi

1. Pri startu se učitava konfiguracija iz `RtuCfg.txt`.
2. Formira se lista analognih i digitalnih tačaka.
3. Pokreću se pozadinske niti za:
   - periodični timer
   - acquisition/polling
   - komunikaciju sa Modbus uređajem
4. Za svaku tačku se prema intervalu iz konfiguracije šalju read komande.
5. Primljene vrednosti se obrađuju kroz `ProcessingModule`:
   - ažuriranje raw vrednosti
   - timestamp
   - EGU proračun
   - alarm stanje
6. GUI prikazuje trenutno stanje i log komunikacije.

## Autor

Projekat je pripremljen/popravljen kao studentski ili vežbovni Modbus/WPF projekat za demonstraciju komunikacije, akvizicije i obrade tačaka.

Bojana Kovacic PR150/2022
