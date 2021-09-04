@ECHO OFF
:: PATHS
SET MYSQLBIN=C:\Program Files\MySQL\MySQL Server 5.6\bin
SET MYSQL="%MYSQLBIN%\mysql.exe"
SET MYSQLDUMP="%MYSQLBIN%\mysqldump.exe"
:: VARIABLES
SET DbName=livereader2
SET DbUser=root
SET DbPass=zer0@bit
:: TAKE DATABASE BACKUP
%MYSQL% -u %DbUser% -p"%DbPass%" < %DbName%.sql
PAUSE