
struct RKRS_H
{
	int offset; // BID 偏移

	int re1;
	int re2;

	short size; // BID 大小
	short count; // BID 数量

	short r3;

	unsigned short val; // BID 起始值
	//  List<BID_H> _bids;
};


struct BID_H
{
	int length; // 长度
	int offset; // 偏移

/*
	//int _bindex;
	//int _bid;
	//  BIDD_H _bidd;
		 string Id => $"BID_{_bid:X}";
		 string Name => "";
		 string Size => $"{_bidd.width}*{_bidd.height}";
		 string Offset => $"0x{offset:X}";
		 string Length => $"{length - 14}";
		 string D3 => $"0x{_bidd.d3:X}";
		 string D4 => $"0x{_bidd.d4:X}";
		 string D5 => $"0x{_bidd.d5:X}";
		  */
};


struct BIDD_H
{
	short width; // 图片宽度
	short height; // 图片高度
	short d3; // 颜色深度
	short d4; // 压缩模式
	unsigned short d5;
	unsigned short d6;
	unsigned short d7;
	int data[0]; // data image
};


struct HEADER_H
{
	int code;
	char txt[4];
};


#include "s33.h"

#include <stdlib.h>
#include <vector>

void parse_data(void* pv) {
	HEADER_H* h = (HEADER_H*)pv;
	RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
	BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);
	BIDD_H** ppbidd = (BIDD_H**)malloc(sizeof(void*) * rkrs->count);
	for (size_t i = 0; i < rkrs->count; i++)
	{
		ppbidd[i] = (BIDD_H*)((char*)pv + bid[i].offset);
	}

	std::vector<BID_H> vecBid(bid, bid + rkrs->count);
	std::vector<BIDD_H*> vecPbidd(ppbidd, ppbidd + rkrs->count);

	return;
}


void* read_image_data(void* pv, int idx) {
	RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
	if (idx >= rkrs->count)
		return NULL;

	BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);
	BIDD_H* bidd = (BIDD_H*)((char*)pv +bid[idx].offset);

	BIDD_H _bidd = *bidd;
	// int* data = (int*)(bidd + 1);
	int* data =bidd->data;
	int* bitmap = (int*)malloc(sizeof(int) * _bidd.width * _bidd.height);
	if (!bitmap)
		return NULL;

	if (_bidd.d4 == 1)
	{
		int x = 0, y = 0, v =0;
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
