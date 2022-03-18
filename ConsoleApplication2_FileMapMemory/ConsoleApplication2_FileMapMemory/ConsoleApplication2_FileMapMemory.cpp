// ConsoleApplication2_FileMapMemory.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Windows.h>

void FileReverse(PCTSTR pszPathName);
void todo(void * pv) {
	char* str = (char*)pv;
	return;
}


int main()
{
	FileReverse(L"C:\\Users\\qwe\\Downloads\\123.txt");
}

void FileReverse(PCTSTR pszPathName)
{
	HANDLE hFile = CreateFile(pszPathName, GENERIC_WRITE | GENERIC_READ, 0, NULL
		, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hFile == INVALID_HANDLE_VALUE)
	{
		printf("File could not be opened.");
		return;
	}

	DWORD dwFileSize = GetFileSize(hFile, NULL);

	HANDLE hFileMap = CreateFileMapping(hFile, NULL, PAGE_READWRITE, 0,
		dwFileSize + sizeof(char), NULL);

	if (hFileMap == NULL) {
		CloseHandle(hFile);
		return;
	}

	LPVOID pvFile = MapViewOfFile(hFileMap, FILE_MAP_READ, 0, 0, 0);
	if (pvFile) {
		todo(pvFile);
		UnmapViewOfFile(pvFile);
	}

	CloseHandle(hFileMap);
	CloseHandle(hFile);
}
