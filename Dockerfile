FROM fsharp:netcore

RUN apt-get update && apt-get --no-install-recommends install -y make
