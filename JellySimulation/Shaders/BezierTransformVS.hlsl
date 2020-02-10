#ifndef BezierTransformVS_HLSL
#define BezierTransformVS_HLSL
//#define MESH
//#include"Common.hlsl"
#include"Structures.hlsl"
//#pragma pack_matrix( row_major )

PSInput main(VSInput input)
{
    PSInput output = (PSInput)0;
    output.c = float4(1, 1, 1, 1);
    return output;
}

#endif