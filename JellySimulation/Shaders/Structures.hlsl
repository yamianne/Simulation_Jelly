struct VSInput
{
    float4 p : POSITION;
    //float3 n : NORMAL;
    //float3 t1 : TANGENT;
    //float3 t2 : BINORMAL;
    //float2 t : TEXCOORD;
    //float4 c : COLOR;
    //float4 mr0 : TEXCOORD1;
    //float4 mr1 : TEXCOORD2;
    //float4 mr2 : TEXCOORD3;
    //float4 mr3 : TEXCOORD4;
};
struct PSInput
{
    float4 p : SV_POSITION;
    float4 vEye : POSITION0;
    float3 n : NORMAL; // normal
    float4 wp : POSITION1;
    float4 sp : TEXCOORD1;
    float2 t : TEXCOORD0; // tex coord	
    float3 t1 : TANGENT; // tangent
    float3 t2 : BINORMAL; // bi-tangent	
    float4 c : COLOR; // solid color (for debug)
    float4 c2 : COLOR1; //vMaterialEmissive
    float4 cDiffuse : COLOR2; //vMaterialDiffuse
};

