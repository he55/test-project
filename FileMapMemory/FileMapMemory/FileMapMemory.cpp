// FileMapMemory.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Windows.h>
#include "s33.h"

void OpenFileWithMap(PCTSTR pszPathName);


int main()
{
    OpenFileWithMap(L"C:\\Users\\luckh\\Downloads\\BMP0.BIN");
    std::cout << "Hello World!\n";
}


void OpenFileWithMap(PCTSTR pszPathName)
{
    HANDLE hFile = CreateFile(pszPathName, GENERIC_WRITE | GENERIC_READ, 0, NULL
        , OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile == INVALID_HANDLE_VALUE)
    {
        printf("File could not be opened.");
        return ;
    }

    DWORD dwFileSize = GetFileSize(hFile, NULL);

    HANDLE hFileMap = CreateFileMapping(hFile, NULL, PAGE_READWRITE, 0,
        dwFileSize + sizeof(char), NULL);

    if (hFileMap == NULL) {
        CloseHandle(hFile);
        return ;
    }

    PVOID pvFile = MapViewOfFile(hFileMap, FILE_MAP_WRITE, 0, 0, 0);
    if (pvFile) {
        parse_data(pvFile);
        UnmapViewOfFile(pvFile);
    }

    CloseHandle(hFileMap);
    CloseHandle(hFile);
}
