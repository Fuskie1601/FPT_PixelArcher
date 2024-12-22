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
[PostProcess(typeof(PostProcessPixelize), PostProcessEvent.BeforeStack, "Sora/Pixelize")]
public sealed class PixelizePostProcessing : PostProcessEffectSettings
{
    public IntParameter _PixelSample = new IntParameter { value = 1 };
    
    [Tooltip("Number of pixels between samples that are tested for an edge. When this value is 1, tested samples are adjacent.")]
    public FloatParameter scale = new FloatParameter { value = 1 };
}

public sealed class PostProcessPixelize : PostProcessEffectRenderer<PixelizePostProcessing>
{
    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private string ShaderType;
    
    public override void Render(PostProcessRenderContext context)
    {
        ShaderType = "Hidden/Pixelize";

        PropertySheet sheet = context.propertySheets.Get(Shader.Find(ShaderType));

        sheet.properties.SetFloat("_PixelSample", settings._PixelSample);
        sheet.properties.SetFloat("_Scale", settings.scale);

        //if (settings.Type == 3)
        {
            // Calculate and set the inverse projection matrix
            Matrix4x4 invProjectionMatrix = context.camera.projectionMatrix.inverse;
            sheet.properties.SetMatrix("_InvProjectionMatrix", invProjectionMatrix);
        }

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        
    }


}
