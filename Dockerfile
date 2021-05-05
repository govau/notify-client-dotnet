FROM fsharp:10.2.3-netcore

RUN apt-get update && apt-get --no-install-recommends install -y make
