// ConsoleApplication1.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#ifndef UNICODE
#define UNICODE
#endif

//#define WIN32_LEAN_AND_MEAN

#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <winsock2.h>
#include <Ws2tcpip.h>
#include <stdio.h>

// Link with ws2_32.lib
#pragma comment(lib, "Ws2_32.lib")


int main()
{
    WORD sockVersion = MAKEWORD(2, 2);
    WSADATA wsaData;
    if (WSAStartup(sockVersion, &wsaData) != 0)
    {
        return 0;
    }

    SOCKET soc= socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (soc == INVALID_SOCKET)
    {
        printf("socket error !");
        return 0;
    }


    int n = 1;
    setsockopt(soc, SOL_SOCKET, SO_BROADCAST, (const char*) & n, sizeof(n));
    sockaddr_in si;
    si.sin_family = AF_INET;
    si.sin_port = htons(12345);
    si.sin_addr.S_un.S_addr = INADDR_BROADCAST;
    char buf[] = "cccccooo999";
    sendto(soc, buf, 12, 0,(const sockaddr*) & si, sizeof(si));
}
