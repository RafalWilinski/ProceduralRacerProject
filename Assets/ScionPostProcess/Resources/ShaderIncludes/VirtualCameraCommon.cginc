#ifndef SCION_VIRTUAL_CAMERA_COMMON
#define SCION_VIRTUAL_CAMERA_COMMON

// ----------------------------------------------------------------------------
// 		Defines
// ----------------------------------------------------------------------------

#define MIN_FNUMBER 1.4
#define MAX_FNUMBER 22
	
#define MIN_ISO 100
#define MAX_ISO 6400

#define MIN_SHUTTER_SPEED (1.0f/4000.0f)
#define MAX_SHUTTER_SPEED (1.0f/30.0f)

#define LIGHT_INTENSITY_MULT 3000.0f 

// ----------------------------------------------------------------------------
// 		Variables (yeah the naming scheme makes little sense, sue me)
// ----------------------------------------------------------------------------

uniform float4 _ScionCameraParams1;
#define ApertureDiameter _ScionCameraParams1.x
#define FocalLength _ScionCameraParams1.y
#define AspectRatio _ScionCameraParams1.z

uniform float4 _VirtualCameraParams1;
#define InvFocalLengthMilliMeter _VirtualCameraParams1.x
#define ManualFNumber _VirtualCameraParams1.y 
#define ManualShutterSpeed _VirtualCameraParams1.z
#define InvExpNegDeltaTTimesTau _VirtualCameraParams1.w //exp(-_DeltaT * _Tau)

uniform float4 _VirtualCameraParams2;
#define ExposureCompensation _VirtualCameraParams2.x 
#define VCFocalLength _VirtualCameraParams2.y
#define VCFocalDistance _VirtualCameraParams2.z
#define PixelsPerMeter _VirtualCameraParams2.w

uniform float4 _VirtualCameraParams3;
#define MinExposure _VirtualCameraParams3.x
#define MaxExposure _VirtualCameraParams3.y

// ----------------------------------------------------------------------------
// 		Structs
// ----------------------------------------------------------------------------

struct CameraOutput
{
	float sceneLuminance;
	float shutterSpeed;
	float ISO;
	float fNumber;
	float exposure;
	float2 CoCScaleAndBias;
	float notUsed;
};

struct PackedCameraOutput
{
	float4 target0 : SV_Target0;
	float4 target1 : SV_Target1;
};

PackedCameraOutput PackCameraOutput(CameraOutput cameraOutput)
{
	PackedCameraOutput packedCO;
	
	packedCO.target0.x = cameraOutput.sceneLuminance;
	packedCO.target0.y = cameraOutput.shutterSpeed;
	packedCO.target0.z = cameraOutput.ISO;
	packedCO.target0.w = cameraOutput.fNumber;
	packedCO.target1.x = cameraOutput.exposure;
	packedCO.target1.y = cameraOutput.CoCScaleAndBias.x;
	packedCO.target1.z = cameraOutput.CoCScaleAndBias.y;
	packedCO.target1.w = cameraOutput.notUsed;
	
	return packedCO;
}
	
// ----------------------------------------------------------------------------
// 		Functions
// ----------------------------------------------------------------------------

float ComputeEV100(float fNumber, float shutterSpeed, float ISOvalue)
{
	return log2(fNumber*fNumber / shutterSpeed * 100.0f / ISOvalue);
}

float ComputeEV100(float avgLuminance)
{
	return log2(avgLuminance * 100.0f / 12.5f);
}

float ConvertEV100ToExposure(float EV100)
{
	float maxLuminance = 1.2f * pow(2.0f, EV100);
	return 1.0f / maxLuminance;
}
	
float SaturationBasedExposure(float fNumber, float shutterSpeed, float ISOvalue)
{
    float l_max = (7800.0f / 65.0f) * fNumber*fNumber / (ISOvalue * shutterSpeed);
    return 1.0f / l_max;
}

float StandardOutputBasedExposure(float fNumber, float shutterSpeed, float ISOvalue)
{
	const float middleGrey = 0.18f;
    float lAvg = (1000.0f / 65.0f) * fNumber*fNumber / (ISOvalue * shutterSpeed);
    return middleGrey / lAvg;
}

float ComputeISO(float fNumber, float shutterSpeed, float ev)
{
    return (fNumber * fNumber * 100.0f) / (shutterSpeed * pow(2.0f, ev));
}
 
float ComputeEV(float fNumber, float shutterSpeed, float ISOvalue)
{
    return log2((fNumber*fNumber * 100.0f) / (shutterSpeed * ISOvalue));
}
 
float ComputeTargetEV(float averageLuminance)
{
    //Light meter calibration constant
    const float K = 12.5f;
    return log2(averageLuminance * 100.0f / K);
}

float ComputeApertureDiameter(float fNumber, float focalLength)
{
	return focalLength / fNumber; 
}

float2 ComputeCoCScaleAndBiasFromAperture(float apertureDiameter, float focalLength, float focalDistance, float pixelsPerMeter)
{
	float apertureTimesFocalLength = apertureDiameter * focalLength;
	float invFocalDistMinusFocalLength = 1.0f / (focalDistance - focalLength);
	
	float CoCScale = apertureTimesFocalLength * focalDistance * invFocalDistMinusFocalLength;
	float CoCBias = -apertureTimesFocalLength * invFocalDistMinusFocalLength;
	return float2(CoCScale, CoCBias) * pixelsPerMeter;
}

float2 ComputeCoCScaleAndBias(float fNumber, float focalLength, float focalDistance, float pixelsPerMeter)
{
	float apertureDiameter = ComputeApertureDiameter(fNumber, focalLength);
	return ComputeCoCScaleAndBiasFromAperture(apertureDiameter, focalLength, focalDistance, pixelsPerMeter);
}
	
#endif // #ifndef SCION_VIRTUAL_CAMERA_COMMON