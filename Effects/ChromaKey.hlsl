texture s0;
sampler s0sampler = sampler_state {
    Texture = s0;
};

// This shader is mostly based on https://github.com/obsproject/obs-studio/blob/master/plugins/obs-filters/data/chroma_key_filter_v2.effect
// from the obs project

float4 key;
float base_thr;
float alpha_correction;
float spill;

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
    // Chroma distance (ignore luminosity)
    float3 key_tr = rgb2ycbcr(key.rgb);
    return sqrt((pix.y - key_tr.y) * (pix.y - key_tr.y) + (pix.z - key_tr.z) * (pix.z - key_tr.z));
}

float4 pixel_shader(float2 tex : TEXCOORD0) : SV_Target0 {
    float4 rgba = tex2D(s0sampler, tex);
    float3 ycbcr = rgb2ycbcr(rgba.rgb);
    float d = key_dist(ycbcr);
    float base_mask = d - base_thr;
    float full_mask = pow(saturate(base_mask/alpha_correction), 1.5);
    float spill_val = pow(saturate(base_mask/spill), 1.5);
    rgba.a *= full_mask;

    float desat = dot(rgba.rgb, float3(0.2126, 0.7152, 0.0722));
    rgba.rgb = lerp(float3(desat, desat, desat), rgba.rgb, spill_val);

    rgba.rgb *= rgba.a; // Make sure to premultiply
    return rgba;
}

technique Main
{
    pass
    {
        PixelShader = compile ps_2_0 pixel_shader();
    }
}
