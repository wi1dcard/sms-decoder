#!/bin/bash

dotnet build -o ../bin
python ser.py | sed -l -n '/CMT/{n;n;p;}' | dotnet ./bin/test.dll