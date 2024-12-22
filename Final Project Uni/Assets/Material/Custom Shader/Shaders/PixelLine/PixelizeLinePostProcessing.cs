using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using Vector3Parameter = UnityEngine.Rendering.PostProcessing.Vector3Parameter;

[Serializable]
[PostProcess(typeof(PostProcessPixelizeLine), PostProcessEvent.BeforeStack, "Sora/PixelizeLine")]
public sealed class PixelizeLinePostProcessing : PostProcessEffectSettings
{
    public IntParameter _PixelSample = new IntParameter { value = 1 };
    
    [Tooltip("Number of pixels between samples that are tested for an edge. When this value is 1, tested samples are adjacent.")]
    public FloatParameter scale = new FloatParameter { value = 1 };
    public ColorParameter color = new ColorParameter { value = Color.white };
    public ColorParameter normalColor = new ColorParameter { value = Color.white };
    [Tooltip("Difference between depth values, scaled by the current depth, required to draw an edge.")]
    public FloatParameter depthThreshold = new FloatParameter { value = 1.5f };
    [Range(0, 1), Tooltip("The value at which the dot product between the surface normal and the view direction will affect " +
                          "the depth threshold. This ensures that surfaces at right angles to the camera require a larger depth threshold to draw " +
                          "an edge, avoiding edges being drawn along slopes.")]
    public FloatParameter depthNormalThreshold = new FloatParameter { value = 0.5f };
    [Tooltip("Scale the strength of how much the depthNormalThreshold affects the depth threshold.")]
    public FloatParameter depthNormalThresholdScale = new FloatParameter { value = 7 };
    public BoolParameter Highlight = new BoolParameter { value = true };  
}

public sealed class PostProcessPixelizeLine : PostProcessEffectRenderer<PixelizeLinePostProcessing>
{
    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private string ShaderType;
    
    public override void Render(PostProcessRenderContext context)
    {
        ShaderType = "Custom/EnhancedPixelizeShader";

        PropertySheet sheet = context.propertySheets.Get(Shader.Find(ShaderType));

        sheet.properties.SetFloat("_PixelSample", settings._PixelSample);
        sheet.properties.SetFloat("_Scale", settings.scale);
        sheet.properties.SetColor("_Color", settings.color);
        sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
        sheet.properties.SetFloat("_DepthNormalThreshold", settings.depthNormalThreshold);
        sheet.properties.SetFloat("_DepthNormalThresholdScale", settings.depthNormalThresholdScale);
        sheet.properties.SetColor("_Color", settings.color);
        sheet.properties.SetColor("_NormalColor", settings.normalColor);

        //if (settings.Type == 3)
        {
            // Calculate and set the inverse projection matrix
            Matrix4x4 invProjectionMatrix = context.camera.projectionMatrix.inverse;
            sheet.properties.SetMatrix("_InvProjectionMatrix", invProjectionMatrix);
        }

        sheet.properties.SetFloat("_Highlight", settings.Highlight ? 1 : 0);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        
    }


}
