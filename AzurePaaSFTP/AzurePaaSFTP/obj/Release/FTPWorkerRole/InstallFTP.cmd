REM Variables.

SETLOCAL EnableDelayedExpansion

SET FtpSiteName=FTPWorkerRole

SET PublicPort=21

SET DynamicPortFirst=10000

SET DynamicPortLast=10005

SET DynamicPortRange=%DynamicPortFirst%-%DynamicPortLast%

SET PublicIP=23.99.111.169

SET StorageName=azurewrath
SET StorageKey=CaZxDxmZnwHEB3/Xk0AwyghmjKsJ18HAeKQ97kLnw12jjFkAn2VY6SSvVJzlGu0RUmAYjpYBYQtFEGXUrxPAYQ==
SET ShareName=ftpshare
SET FtpDirectory=\\%StorageName%.file.core.windows.net\%ShareName%


REM Take care of the charactor escaping especially for passwords

SET SSLCertHash=‎c532d48e0189f6e6058caf0219ecd1ee786f0d64
SET SSLCertFileName=star_ranger_im.pfx
SET SSLCertPwd=Password01^^!

certutil -p !SSLCertPwd! -importpfx %SSLCertFileName% 

REM Add user.

net user %StorageName% /delete
net user %StorageName% %StorageKey% /add /Y
 
REM Install FTP.

start /w pkgmgr /iu:IIS-WebServerRole;IIS-FTPSvc;IIS-FTPServer;IIS-ManagementConsole

REM Configuring FTP site.

pushd %windir%\system32\inetsrv 

appcmd add site /name:%FtpSiteName% /bindings:ftp://*:%PublicPort% /physicalpath:"%FtpDirectory%" 

appcmd set vdir /vdir.name:"%FtpSiteName%/" /userName:%StorageName% /password:%StorageKey%


appcmd set config -section:system.applicationHost/sites /[name='%FtpSiteName%'].ftpServer.security.ssl.controlChannelPolicy:"SslAllow" 

appcmd set config -section:system.applicationHost/sites /[name='%FtpSiteName%'].ftpServer.security.ssl.dataChannelPolicy:"SslAllow" 

appcmd set config -section:system.applicationHost/sites /[name='%FtpSiteName%'].ftpServer.security.authentication.basicAuthentication.enabled:true 

appcmd set config %FtpSiteName% /section:system.ftpserver/security/authorization /-[users='*'] /commit:apphost

appcmd set config %FtpSiteName% /section:system.ftpserver/security/authorization /+[accessType='Allow',permissions='Read,Write',roles='',users='*'] /commit:apphost

appcmd set config /section:system.ftpServer/firewallSupport /lowDataChannelPort:%DynamicPortFirst% /highDataChannelPort:%DynamicPortLast%

appcmd set config -section:system.applicationHost/sites /siteDefaults.ftpServer.firewallSupport.externalIp4Address:"%PublicIP%" /commit:apphost

 

appcmd set config -section:system.applicationHost/sites /[name='%FtpSiteName%'].ftpServer.security.ssl.serverCertHash:%SSLCertHash% /commit:apphost
appcmd set config -section:system.applicationHost/sites /[name='%FtpSiteName%'].ftpServer.security.ssl.controlChannelPolicy:"SslRequire" /commit:apphost
appcmd set config -section:system.applicationHost/sites /[name='%FtpSiteName%'].ftpServer.security.ssl.dataChannelPolicy:"SslRequire" /commit:apphost

REM Configure firewall.

netsh advfirewall firewall add rule name="FTP Public Port" dir=in action=allow protocol=TCP localport=%PublicPort%

netsh advfirewall firewall add rule name="FTP Passive Dynamic Ports" dir=in action=allow protocol=TCP localport=%DynamicPortRange%

 

REM Restart the FTP service.

net stop ftpsvc

net start ftpsvc

EXIT /B 0