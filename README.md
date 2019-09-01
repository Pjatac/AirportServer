# AirportServer
Server side is ready for running.

You can configure tuning in file "settings.cs" from project "CommonLibruary"

For viewing DB you can download and install SQLiteDatabasePortable from the link below
https://drive.google.com/open?id=1Tz_SKVnMJb-CJbcIN3wECllwN5rsFq3a

Also, to view all the events you can uncomment Logger on LineService.cs and Line.cs, and  open the file.txt from path ..\AirportServer\Airport\bin\Debug\netcoreapp2.2
*Note - not recommended use logger with more then 25 crafts
In the same directory stores the history of the movement of 1000 flights in "filetmp.txt"

*Note - if the workflow was interrupted before completion, to restart the airport server, delete from Airport project the "Migrations" directory and file "airport.db"
After that, in PM console run commands:
1) Add-Migration -Name airport
2) Update-Database
