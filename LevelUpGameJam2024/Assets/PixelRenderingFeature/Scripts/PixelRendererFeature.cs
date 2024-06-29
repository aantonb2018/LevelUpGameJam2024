using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Experimental.Rendering.Universal
{
    public class PixelRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class FeatureSettings
        {
            public LayerMask layerMask = 0; //Used to selectively ignore Colliders when casting a ray

            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents; //Enum that controls when the render pass executes
        }

        [System.Serializable]
        public class PixelSettings
        {
            public Material blitMaterial = null;

            [Range(1f, 15f)]
            public float pixelDensity = 1f;

            [Range(0f, 30f)]
            public float colourCount = 1f;

            [Range(0f, 200f)]
            public float outlineStrength = 1f;
        }

        [System.Serializable]
        public class PaletteSettings
        {
            public bool activePalette = false;

            public Texture2D colourPalette = null;
        }

        public FeatureSettings featureSettings = new FeatureSettings();
        public PixelSettings pixelSettings = new PixelSettings();
        public PaletteSettings paletteSettings = new PaletteSettings();
        PixelRendererPass passRenderer;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(passRenderer);
        }

        public override void Create()
        {
            passRenderer = new PixelRendererPass(featureSettings.renderPassEvent, pixelSettings.blitMaterial, pixelSettings.pixelDensity, pixelSettings.colourCount, 
                pixelSettings.outlineStrength, paletteSettings.activePalette, paletteSettings.colourPalette, featureSettings.layerMask);
        }

        public void SetFeatureChanges(float pixelDensity, float colourCount, float outlineStrength, bool activePalette, Texture2D colourPalette)
        {
            pixelSettings.pixelDensity = pixelDensity;
            pixelSettings.colourCount = colourCount;
            pixelSettings.outlineStrength = outlineStrength;

            paletteSettings.activePalette = activePalette;
            paletteSettings.colourPalette = colourPalette;
            SetDirty();
        }

    }
}
