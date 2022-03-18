

struct RKRS_H
{
	/// <summary>
	/// BID 偏移
	/// </summary>
	int offset;

	int re1;
	int re2;

	/// <summary>
	/// BID 大小
	/// </summary>
	short size;

	/// <summary>
	/// BID 数量
	/// </summary>
	short count;

	short r3;

	/// <summary>
	/// BID 起始值
	/// </summary>
	unsigned short val;
	//  List<BID_H> _bids;
};


struct BID_H
{
	/// <summary>
	/// 长度
	/// </summary>
	int length;

	/// <summary>
	/// 偏移
	/// </summary>
	int offset;


	//int _bindex;
	//int _bid;
	//  BIDD_H _bidd;
/*
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
	/// <summary>
	/// 图片宽度
	/// </summary>
	short width;

	/// <summary>
	/// 图片高度
	/// </summary>
	short height;

	/// <summary>
	/// 颜色深度
	/// </summary>
	short d3;

	/// <summary>
	/// 压缩模式
	/// </summary>
	short d4;
	unsigned short d5;
	unsigned short d6;
	unsigned short d7;
};


struct HEADER_H
{
	int code;
	char txt[4];
};

void parse_data(void* pv) {
	HEADER_H* h = (HEADER_H*)pv;
	RKRS_H* rkrs = (RKRS_H*)((char*)pv + 0x30);
	BID_H* bid = (BID_H*)((char*)pv + rkrs->offset);
	BIDD_H* bidd = (BIDD_H*)((char*)pv +bid->offset);
	return;
}
