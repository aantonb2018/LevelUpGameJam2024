using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Experimental.Rendering.Universal
{
    public class PixelRendererPass : ScriptableRenderPass
    {
        Material blitMat;
        float pixelDensity;

        List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
        FilteringSettings filteringSettings;

        static int pixelTextureID = Shader.PropertyToID("_PixelTexture");
        static int pixelDepthID = Shader.PropertyToID("_DepthTex");
        static int cameraID = Shader.PropertyToID("_CameraColorTexture");

        public PixelRendererPass(RenderPassEvent renderPassEvent, Material blitMat, float pixelDensity, float count, float power, bool palette, Texture2D texture, int layerMask)
        {
            this.renderPassEvent = renderPassEvent;
            this.blitMat = blitMat;
            this.pixelDensity = pixelDensity;
            blitMat.SetFloat("_PixelDensity", pixelDensity);
            blitMat.SetFloat("_PosterizationCount", count);
            blitMat.SetFloat("_Power", power);
            if (!palette)
            {
                blitMat.DisableKeyword("SAMPLEPALETTE");
            }
            else
            {
                blitMat.EnableKeyword("SAMPLEPALETTE");
            }
            blitMat.SetTexture("_Palette", texture);

            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

            shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            shaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent; //To control the way Unity sorts objects before drawing them, in this case sorting by transparencies
            DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortingCriteria);

            ref CameraData cameraData = ref renderingData.cameraData;
            Camera camera = cameraData.camera;

            int pixelWidth = (int)(camera.pixelWidth / pixelDensity);
            int pixelHeight = (int)(camera.pixelHeight / pixelDensity);
            CommandBuffer cmd = CommandBufferPool.Get("PixelFeature");
            using (new ProfilingScope(cmd, new ProfilingSampler("PixelFeature")))
            {
                
                cmd.GetTemporaryRT(pixelTextureID, pixelWidth, pixelHeight, 0, FilterMode.Point); //Generates a render texure of the size we want for it to look pixel-like. It contains the texture data
                cmd.GetTemporaryRT(pixelDepthID, pixelWidth, pixelHeight, 24, FilterMode.Point, RenderTextureFormat.Depth); //This one contains the depth data in a x24 precision

                cmd.SetRenderTarget(pixelTextureID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                                    pixelDepthID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store); //Creates a render target made from the previous render textures

                cmd.ClearRenderTarget(true, true, Color.clear); //Clear the render texture so it isn't drawing to it each frame

                context.ExecuteCommandBuffer(cmd); //All commands in the buffer are executed without further ado
                cmd.Clear(); //Clear all commands in the buffer so it's now empty
                
                //With what's been done we already have a render target with the objects in the pixel layers in a pixel-like look

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings); //Draw the objects that are filtered to do so, in this case these are the ones in the pixel layer

                //Now we should render the rest of the scene and mix the results
                
                cmd.SetRenderTarget(cameraID, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store); //Generates a render target from the camera shader ID

                cmd.Blit(new RenderTargetIdentifier(pixelTextureID), BuiltinRenderTextureType.CurrentActive, blitMat); //The blit consist in mixing the two different renders, the one of the whole camera and the one that rendered the pixel layer with the effect included

                cmd.ReleaseTemporaryRT(pixelTextureID); //Now the temporal render texture should be cleared
                cmd.ReleaseTemporaryRT(pixelDepthID);
                
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}

