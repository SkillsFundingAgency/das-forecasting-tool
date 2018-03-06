#Requires -RunAsAdministrator

$idppwd = ConvertTo-SecureString -String idsrv3test -Force -AsPlainText
Import-PfxCertificate -FilePath DasIDPCert.pfx -CertStoreLocation cert://LocalMachine/My -Password $idppwd -Exportable