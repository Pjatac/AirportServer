# AirportServer
Airport runway simulator
Server side is ready for running.

If the workflow was interrupted before completion, to restart the airport server, delete from Airport project the "Migrations" directory and file "airport.db"
After that, in PM console run commands:
1) Add-Migration -Name airport
2) Update-Database

You can configure tuning in file "settings.cs" from project "CommonLibruary"

For viewing DB you can download and install SQLiteDatabasePortable from the link below
https://drive.google.com/open?id=1Tz_SKVnMJb-CJbcIN3wECllwN5rsFq3a

Also, to view all the events you can open the file.txt from path ..\AirportServer\Airport\bin\Debug\netcoreapp2.2
In the same directory stores the history of the movement of 1000 flights in "filetmp.txt"
