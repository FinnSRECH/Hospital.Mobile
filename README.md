# Hospital.Mobile starter

Dit is een startproject voor Iteratie 3 van de ziekenhuiscasus.

## Wat zit er al in?
- .NET MAUI project op .NET 10
- Medewerkerslogin voor chirurg Emma
- Eenvoudig mobiel dashboard
- Persoonlijke planning
- Detailpagina voor consultaties en operaties
- Voorbereid notitieveld en afrondknop
- Mock data in `HospitalMobileDataService`

## Testaccount
- E-mail: `emma.jansen@hospital.nl`
- Wachtwoord: `Welkom123!`

## Relatie met Iteratie 1 en 2
De starter gebruikt dezelfde domeinbegrippen (patiënt, consultatie, operatie, planning). In een volgende stap kun je desgewenst `Hospital.Domain` als project reference koppelen, of de mock service vervangen door gedeelde data.

## Nog te bouwen
1. Echte patiëntinformatie en behandelingstatus
2. Consultatie/operatie afronden
3. Notities opslaan
4. SQLite offline opslag
5. Biometrische authenticatie
6. Camera/foto-functionaliteit
7. HCI-afwerking en testen

## Toevoegen aan je solution
Pak de map uit naast je andere Hospital-projecten en kies in Visual Studio:
`Solution > Add > Existing Project... > Hospital.Mobile.csproj`

De starter is gebaseerd op de standaardstructuur van je eerdere MAUI-project, maar bevat geen custom fonts of extra packages.
