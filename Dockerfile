FROM microsoft/aspnet

SHELL ["powershell", "-command"]

RUN mkdir C:\site

RUN Import-module IISAdministration; \
    New-IISSite -Name "Site" -PhysicalPath C:\site -BindingInformation "*:8000:"

EXPOSE 8000

#VOLUME & COPY Don't mix :)'
#VOLUME c:/site

COPY ./ContosoUniversity c:/site
