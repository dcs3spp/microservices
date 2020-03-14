#!/bin/bash

# this script is necessary because at the time of development with dotnet 3.1.102
# there is an [issue](https://github.com/dotnet/aspnetcore/issues/19590)
# with macos environments when generating self signed development certificates with 
# ```dotnet dev-certs https````

security login-keychain > login.txt;
loginKeyChain=$(cat ./login.txt);
loginKeyChain=${loginKeyChain:5:${#loginKeyChain}-6};

echo "[ req ]
default_bits       = 4096
default_md         = sha256
default_keyfile    = key.pem
prompt             = no
encrypt_key        = no

distinguished_name = req_distinguished_name
req_extensions     = v3_req
x509_extensions    = v3_req

[ req_distinguished_name ]
commonName         = "localhost"

[ v3_req ]
subjectAltName     = DNS:localhost
basicConstraints   = critical, CA:false
keyUsage           = critical, keyCertSign, digitalSignature, keyEncipherment
extendedKeyUsage   = critical, serverAuth
1.3.6.1.4.1.311.84.1.1 = ASN1:UTF8String:01

subjectAltName= @alt_names

[alt_names]
# Local hosts
DNS.1 = localhost
DNS.2 = 127.0.0.1
DNS.3 = ::1
DNS.4 = docker.for.mac.localhost
DNS.5 = docker.for.windows.localhost
DNS.6 = docker.for.linux.localhost
DNS.7 = identity-api" > https.config;

openssl req -config https.config -new -out https.crt -x509 -days 365;
openssl pkcs12 -export -out https.pfx -inkey key.pem -in https.crt -password pass:password;
certificateSha256Hash=$(openssl x509 -noout -fingerprint -sha256 -inform pem -in https.crt);
certificateSha256Hash=${certificateSha256Hash/SHA256 Fingerprint=/};
certificateSha256Hash=$(echo $certificateSha256Hash | sed 's/://g');
rm key.pem;
echo true > ~/.dotnet/certificate.${certificateSha256Hash}.sentinel;
security import https.pfx -k $loginKeyChain -t cert -f pkcs12 -P password -A;
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain https.crt &&
rm https.crt &&
rm https.pfx &&
rm https.config &&
rm login.txt &&
sudo security set-key-partition-list -D localhost -S unsigned:,teamid:UBF8T346G9 $loginKeyChain;
