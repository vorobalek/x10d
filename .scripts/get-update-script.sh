#!/bin/bash
echo "git pull && docker build -t x10d-$1 -f Dockerfile.$1 . && docker stop x10d-$1 && docker rm x10d-$1 && docker run -d -p $2:80 -p $3:443 --name=x10d-$1 x10d-$1"