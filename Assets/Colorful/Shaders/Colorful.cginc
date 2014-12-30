/* Utils */
half luminance(half3 color)
{
	return dot(color, half3(0.30, 0.59, 0.11));
}

half rot(half value, half low, half hi)
{
	return (value < low) ? value + hi : (value > hi) ? value - hi : value;
}

half rot10(half value)
{
	return rot(value, 0.0, 1.0);
}

half4 pixelate(sampler2D tex, half2 uv, half scale, half ratio)
{
	half ds = 1.0 / scale;
	half2 coord = half2(ds * ceil(uv.x / ds), (ds * ratio) * ceil(uv.y / ds / ratio));
	return half4(tex2D(tex, coord).xyzw);
}

half simpleNoise(half x, half y, half seed, half phase)
{
	half n = x * y * phase * seed;
	return fmod(n, 13) * fmod(n, 123);
}

half invlerp(half from, half to, half value)
{
	return (value - from) / (to - from);
}

/* Color conversion */
half3 HSVtoRGB(float3 c)
{
	half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}

half3 RGBtoHSV(float3 c)
{
	half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
	half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

	half d = q.x - min(q.w, q.y);
	half e = 1.0e-10;
	return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
