#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"

PRIVATE_PEM=$DIR/private.pem
PUBLIC_PEM=$DIR/public.pem
PFX=$DIR/idsrv3test.pfx
PASSWD=$1

if [ -z "$PASSWD" ]
then
    PASSWD="idsrv3test"
fi

echo "Creating Private Key"
openssl genrsa 2048 > $PRIVATE_PEM

echo "Creating Public Key"
echo """GB
Tyne and Wear
Durham
Acme.org
Dev
dev@acme.org
""" | openssl req -x509 -days 1000 -new -key $PRIVATE_PEM -out $PUBLIC_PEM

echo ""
echo "Creating Certificate"

openssl pkcs12 -export -in $PUBLIC_PEM -inkey $PRIVATE_PEM -out $PFX -password pass:$PASSWD
rm "${PRIVATE_PEM}"
rm "${PUBLIC_PEM}"
