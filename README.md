# CleaningRobot

How to build & run it:
1) To build solution go into root and run "dotnet build" or build it via Visual Studio
2) To run application go from root folder into CleaningRobot subfolder and run "dotnet run <source.json> <result.json>"
   
   <img width="879" alt="image" src="https://github.com/JohnFromPrague/CodingTest/assets/18370349/43b058f6-bf23-4d63-a2bc-938c642c0de0">


Remarks:
1) In PDF example of JSON files are valid, however attached JSON files are not in same format/valid. Maps contain "null" instead of null. Visited and cleaned arrays do not have comma separator between items. I've fixed those problems in attached files. You can see all JSON files in \CleaningRobot.Tests\Resources\
