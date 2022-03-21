#include "s33.h"
#include <stdlib.h>


MyStruct * rkrs_open_file(const char* pszPathName)
{
	HANDLE hFile = CreateFile(pszPathName, GENERIC_WRITE | GENERIC_READ, 0, NULL
		, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hFile == INVALID_HANDLE_VALUE)
	{
		return NULL;
	}

	DWORD dwFileSize = GetFileSize(hFile, NULL);

	HANDLE hFileMap = CreateFileMapping(hFile, NULL, PAGE_READWRITE, 0,
		dwFileSize + sizeof(char), NULL);
	if (hFileMap == NULL) {
		CloseHandle(hFile);
		return NULL;
	}

	PVOID pvFile = MapViewOfFile(hFileMap, FILE_MAP_WRITE, 0, 0, 0);
	if (pvFile == NULL) {
		CloseHandle(hFileMap);
		CloseHandle(hFile);
		return NULL;
	}

	MyStruct* mys = (MyStruct*)malloc(sizeof(MyStruct));
	if (mys == NULL) {
		return NULL;
	}

	*mys = { pvFile,hFileMap,hFile };
	return mys;
}


void rkrs_close_file(MyStruct *mys)
{
	if (mys == NULL) {
		return;
	}

	UnmapViewOfFile(mys->pvFile);
	CloseHandle(mys->hFileMap);
	CloseHandle(mys->hFile);

	free(mys->mys2.ppbidd);
	free((void *)mys);
}


void rkrs_parse(MyStruct *mys, MyStruct2 *mys2)
{
	void *pv=mys->pvFile;
	HEADER_H* h = (HEADER_H*)pv;
	RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
	BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);
	BIDD_H** ppbidd = (BIDD_H**)malloc(sizeof(void*) * rkrs->count);
	for (size_t i = 0; i < rkrs->count; i++)
	{
		ppbidd[i] = (BIDD_H*)((char*)pv + bid[i].offset);
	}
	mys->mys2={h,rkrs,bid,ppbidd};
	mys2=&mys->mys2;
}


void* rkrs_read_image_data(MyStruct *mys, int idx)
{
	void *pv=mys->pvFile;
	RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
	if (idx >= rkrs->count)
		return NULL;

	BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);
	BIDD_H* bidd = (BIDD_H*)((char*)pv + bid[idx].offset);

	BIDD_H _bidd = *bidd;
	int* data = bidd->data;
	int* bitmap = (int*)malloc(sizeof(int) * _bidd.width * _bidd.height);
	if (!bitmap)
		return NULL;

	if (_bidd.d4 == 1)
	{
		int x = 0, y = 0, v = 0;
		while (v = *(data++))
		{
			int val = v & 0x00ffffff;
			if (val != 0x00ff00ff)
				val |= 0xff000000;

			int len = v >> 24 & 0xff;
			if (len == 0xff)
				len += *(data++);

			for (int i = 0; i < len; i++)
			{
				bitmap[y * _bidd.width + x] = val;
				if (++x == _bidd.width)
				{
					x = 0;
					++y;
				}
			}
		}

		// Debug.Assert(y == _bidd.height);
	}
	else if (_bidd.d4 == 0)
	{
		for (int y = 0; y < _bidd.height; y++)
		{
			for (int x = 0; x < _bidd.width; x++)
			{
				int val = *(data++);
				if (val != 0x00ff00ff)
					val |= 0xff000000;

				bitmap[y * _bidd.width + x] = val;
			}
		}
	}
	else if (_bidd.d4 == 5)
	{
		int alpha = 0;
		if (_bidd.d5 == 3)
			alpha = 0xff000000;

		for (int y = 0; y < _bidd.height; y++)
		{
			for (int x = 0; x < _bidd.width; x++)
			{
				bitmap[y * _bidd.width + x] = *(data++) | alpha;
			}
		}
	}
	return (void*)bitmap;
}


void parse_data2(void* pv) {
	HEADER_H* h = (HEADER_H*)pv;
	RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
	BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);
	BIDD_H* bidd = (BIDD_H*)((char*)pv + bid->offset);
	return;
}
