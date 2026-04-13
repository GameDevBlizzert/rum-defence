float4 MainPS() : SV_Target
{
    return float4(0, 0, 1, 1);
}

technique Basic
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 MainPS();
    }
}