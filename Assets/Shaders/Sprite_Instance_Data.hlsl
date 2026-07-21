#ifndef Sprite_Instance_Data
#define Sprite_Instance_Data

UNITY_INSTANCING_BUFFER_START(Props)
  UNITY_DEFINE_INSTANCED_PROP(float4, _UVFrame)
  UNITY_DEFINE_INSTANCED_PROP(float4, _QuadData)
UNITY_INSTANCING_BUFFER_END(Props)

void Sprite_Instance_Data_UVFrame_float(out float4 Out){
    Out = UNITY_ACCESS_INSTANCED_PROP(Props, _UVFrame);
}

void Sprite_Instance_Data_QuadData_float(out float4 Out){
    Out = UNITY_ACCESS_INSTANCED_PROP(Props, _QuadData);
}

#endif