texture s0;
sampler s0sampler = sampler_state {
    Texture = s0;
};

float4 key;
float tolabs;
float tolres;

float3 rgb2ycbcr(float3 rgb) {
    // from https://web.archive.org/web/20160304062915/http://www.equasys.de/colorconversion.html
    const float3 offset = float3(0.0625, 0.5, 0.5);
    const float3 y_coef = float3(0.257, 0.5034, 0.098);
    const float3 cb_coef = float3(-0.148, -0.291, -0.439);
    const float3 cr_coef = float3(0.439, -0.368, -0.071);
    float3 res;
    res.x = dot(y_coef, rgb);
    res.y = dot(cb_coef, rgb);
    res.z = dot(cr_coef, rgb);
    return res + offset;
}

float key_dist(float3 pix) {
    float3 key_tr = rgb2ycbcr(key.rgb);
    return sqrt((pix.y - key_tr.y) * (pix.y - key_tr.y) + (pix.z - key_tr.z) * (pix.z - key_tr.z));
}

float4 pixel_shader(float2 tex : TEXCOORD0) : SV_Target0 {
    float4 rgba;
    rgba = tex2D(s0sampler, tex);
    float3 ycbcr = rgb2ycbcr(rgba.rgb);
    float d = key_dist(ycbcr);
    if (d < tolabs) {
        rgba.rgba = float4(0, 0, 0, 0);
    } else if (d < tolres) {
        float mask = (d-tolabs) / (tolres-tolabs);
        rgba.rgb -= key.rgb*(1-mask);
        rgba.a = mask;
    }
    
    return rgba;
}

technique Main
{
    pass
    {
        PixelShader = compile ps_2_0 pixel_shader();
    }
}
