#include <Windows.h>
#include "s33.h"


int main()
{
   MyStruct * p =rkrs_open_file("C:\\Users\\luckh\\Downloads\\BMP0.BIN");
   MyStruct2 mys2;
   rkrs_parse(p,&mys2);
   rkrs_read_image_data(p,0);
   rkrs_close_file(p);
}
