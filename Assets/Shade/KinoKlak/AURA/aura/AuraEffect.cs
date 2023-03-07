using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Aura
{
    [System.Serializable]
    [PostProcess(typeof(AuraEffectRenderer), PostProcessEvent.BeforeStack, "Aura/Aura Effect")]
    public sealed class AuraEffect : PostProcessEffectSettings
    {
        public TextureParameter buffer = new TextureParameter();
        public FloatParameter threshold = new FloatParameter { value = 0.5f };
        public FloatParameter thickness = new FloatParameter { value = 1 };
        [ColorUsage(false, true)]
        public ColorParameter color = new ColorParameter { value = Color.white };
        public BoolParameter debug = new BoolParameter();
    }

    public sealed class AuraEffectRenderer : PostProcessEffectRenderer<AuraEffect>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Aura/Aura Effect"));

            if ((Texture)settings.buffer == null)
            {
                context.command.BlitFullscreenTriangle(context.source, context.destination);
            }
            else if (settings.debug)
            {
                context.command.Blit(settings.buffer, context.destination);
            }
            else
            {
                sheet.properties.SetTexture("_AuraTex", settings.buffer);
                sheet.properties.SetFloat("_Threshold", settings.threshold);
                sheet.properties.SetFloat("_Thickness", settings.thickness);
                sheet.properties.SetColor("_Color", settings.color);
                context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            }
        }
    }
}
