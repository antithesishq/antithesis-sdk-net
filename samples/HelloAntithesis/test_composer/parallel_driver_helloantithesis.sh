#!/usr/bin/env bash
for i in {1..100}
do
    curl --silent --output /dev/null --show-error --fail http://webservice:8080
done