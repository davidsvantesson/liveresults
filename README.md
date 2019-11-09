This is a special version of the live client that calculates total results over several (but independent) stages (simultaneously as publishing live results). The total results can also be published live. Used at O-event, Borås.

# Usage instructions
Instructions in swedish.

## Före tävling:
- Ladda ner programmet:
(Kompilerad version)
- Packa upp zip filen i en mapp.

## Liveresultat etapp: 
0. Etapp 2 och 3: Lägg in "OeventTotal.db" i mappen som programmet körs ifrån (se ovan)
1. Starta LiveResults.Client.exe
2. Etapp 1: Skapa ny Totalresultat databas (Initiera första etappen)
   Etapp 2 och 3: Klicka på "Initiera nästa Etapp" (aktiv etapp visas nedanför knapparna)
3. Välj "OLA, SOFT". Skriv in som vanligt; databas, välj tävling, skriv in tävlingens (etappens) CompetitionID. 
4. Klicka på Start.
5. Starta liveresultat för totalresultat (se nedan)
6. När tävlingen är klar, klicka på "Stop".
7. Stäng fönstret och programmet.
Efter: Skicka "OeventTotal.db" till nästa etapp-arrangör (efter att du stängt ned live totalresultat, se nedan)

## Liveresultat för totalresultat:
(Behöver inte köras etapp 1)
1. Starta LiveResults.Client.exe (En ny instans av programmet, som körs samtidigt med "liveresultat etapp").
2. Välj "TotalResultat". Skriv in CompetitionID för totalresultaten och antal etapper (3).
3. Klicka på Start.
4. När tävlingen är klar, klicka på "Stop". 
5. Stäng fönstret och programmet

## Ändring av resultat
Klienten är inte ett tävlingsadministrativt system utan bygger på att resultaten för en etapp inte ändras efter att den "stängs" (efter nästa etapp initieras). Om något resultat för en tidigare etapp är fel så måste alla totalresultat beräknas om. Gör det genom att klicka på "Nollställ resultat och sätt till första etapp". Då behålls information om alla löpare men resultaten kan beräknas om. För att beräkna om resultat från alla etapper är det enklast att använda en "IOF XML" fil. Tänk på att xml filen måste läggas in i mappen efter det att inläsningen startar (liveresultat-clienten söker efter nya filer i mappen) I övrigt är det samma steg som ovan. Se dock om att beräkna totalresultat offline nedan.

OBS: Det är förståss också möjligt att radera hela databasen och börja om från början. Dock stämmer de interna ID-numren på löparna då inte längre överens med liveresultat-sidan, det bör därför undvikas.


## Beräkna totalresultaten offline
Om du beräknar totalresultat för en etapp efter etappen är avslutat vill du antagligen inte ladda upp etappresultaten till liveresultat-sidan. Du måste då ändra i filen "LiveResults.Client.exe.config" innan du startar live-klienten.
Ändra `<add key="runoffline" value="false" />` till `<add key="runoffline" value="true" />`
För att undvika att klienten buffrar existerande resultat från liveresultat sidan, skriv in en CompetitionId som inte finns.

## Ignorerade klasser
Du kan lägga in klasser som inte ska vara med i beräkningen av totalresultat i "LiveResults.Client.exe.config", med key "totalIgnoreClasses".
