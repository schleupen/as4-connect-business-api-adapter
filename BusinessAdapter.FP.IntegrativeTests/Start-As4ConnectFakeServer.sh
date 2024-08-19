#!/bin/bash

REPOSITORY="as4-connect-business-api-fakeserver-releases"

ls

# Variables
CLIENT_PFX_PATH=$(pwd)"/BusinessAdapter.FP.IntegrativeTests/Zertifikate/client.pfx"
SERVER_PFX_PATH=$(pwd)"/BusinessAdapter.FP.IntegrativeTests/Zertifikate/fakeserver_ca.pfx"
PASSWORD=""

# Temporary files
CLIENT_CERT="client-cert.pem"
CLIENT_KEY="client-key.pem"
SERVER_CERT="server-cert.pem"
SERVER_KEY="server-key.pem"



# Function to import a PFX file
import_pfx() {
    local pfx_path=$1
    local cert_path=$2
    local key_path=$3

    # Extract the certificate and key
    openssl pkcs12 -in "$pfx_path" -out "$cert_path" -clcerts -nokeys -password pass:"$PASSWORD"
    openssl pkcs12 -in "$pfx_path" -out "$key_path" -nocerts -nodes -password pass:"$PASSWORD"

    # Import the certificate into the system
    if [ -f "$cert_path" ]; then
        # Move certificate to system certificate directory (Debian/Ubuntu based systems)
        cp "$cert_path" /usr/local/share/ca-certificates/
        update-ca-certificates
        echo "updated certs"

        # Import the key if needed (for server certificates)
        if [ -f "$key_path" ]; then
            # Secure the private key file
            chmod 600 "$key_path"
            # The key can be stored in a secure location or used as needed by your application
        fi
    else
        echo "Failed to extract certificate from $pfx_path"
    fi
}

# Import client and server certificates
import_pfx "$CLIENT_PFX_PATH" "$CLIENT_CERT" "$CLIENT_KEY"
import_pfx "$SERVER_PFX_PATH" "$SERVER_CERT" "$SERVER_KEY"

# Clean up temporary files
rm -f "$CLIENT_CERT" "$CLIENT_KEY" "$SERVER_CERT" "$SERVER_KEY"
echo "Certificates have been imported successfully."
#
#url="https://github.com/schleupen/$REPOSITORY/releases/latest"
#
#curl -s $url
#
#curl -s https://api.github.com/repos/schleupen/$REPOSITORY/releases/latest \
#| grep "browser_download_url" \
#| cut -d : -f 2,3 \
#| tr -d \" \
#| wget -qi -

#unzip Schleupen.AS4.MSH.BusinessAPI.FakeServer.zip

cd Schleupen.AS4.MSH.BusinessAPI.FakeServer
dotnet run Schleupen.AS4.MSH.BusinessAPI.FakeServer.dll 