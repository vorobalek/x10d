#!/bin/bash
echo "git pull && docker build -t x10d-$1 -f Dockerfile.$1 . && docker stop x10d-$1 && docker rm x10d-$1 && docker run -d -p 5500:80 -p 5501:443 --name=x10d-$1 x10d-$1"