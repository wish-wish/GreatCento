﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Reference : https://github.com/haolange/Unity-SeparableSubsurface

namespace SSSS
{

    [System.Serializable]
    //[PostProcess(typeof(SeparableSubsurfaceScatteringRenderer), PostProcessEvent.BeforeStack, "SepaSubSurfScatt")]
    public sealed class SeparableSubsurfaceScattering : PostProcessEffectSettings
    {
        [Range(1f, 5f)] public FloatParameter strength = new FloatParameter() { value = 1.5f };
        public ColorParameter color = new ColorParameter() { value = new Color(1f, 0.51f, 0f) };
        public ColorParameter falloffColor = new ColorParameter() { value = new Color(0.83f, 0.22f, 0f) };
    }

    public class SeparableSubsurfaceScatteringRenderer : PostProcessEffectRenderer<SeparableSubsurfaceScattering >
    {

        static class ShaderIDs
        {
            internal static readonly int Strength = Shader.PropertyToID("_Strength");
            internal static readonly int Kernel = Shader.PropertyToID("_Kernel");
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/PostProcessing/SeparableSubsurfaceScattering"));
            sheet.properties.SetFloat(ShaderIDs.Strength, settings.strength);

            var color = settings.color.value;
            var SSSC = Vector3.Normalize(new Vector3(color.r, color.g, color.b));

            var falloffColor = settings.falloffColor.value;
            var SSSFC = Vector3.Normalize(new Vector3(falloffColor.r, falloffColor.g, falloffColor.b));

            var kernel = CalculateKernel(25, SSSC, SSSFC);
            sheet.properties.SetVectorArray(ShaderIDs.Kernel, kernel.ToArray());

            var cmd = context.command;
            cmd.BeginSample("SepSubSurfScatt");
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample("SepSubSurfScatt");
        }

        protected List<Vector4> CalculateKernel(int nSamples, Vector3 strength, Vector3 falloff)
        {
            float RANGE = nSamples > 20 ? 3.0f : 2.0f;
            float EXPONENT = 2.0f;

            var kernel = new List<Vector4>();

            // Calculate the SSS_Offset_UV:
            float step = 2.0f * RANGE / (nSamples - 1);
            for (int i = 0; i < nSamples; i++)
            {
                float o = -RANGE + i * step;
                float sign = o < 0.0f ? -1.0f : 1.0f;
                float w = RANGE * sign * Mathf.Abs(Mathf.Pow(o, EXPONENT)) / Mathf.Pow(RANGE, EXPONENT);
                kernel.Add(new Vector4(0, 0, 0, w));
            }

            // Calculate the SSS_Scale:
            for (int i = 0; i < nSamples; i++)
            {
                float w0 = i > 0 ? Mathf.Abs(kernel[i].w - kernel[i - 1].w) : 0.0f;
                float w1 = i < nSamples - 1 ? Mathf.Abs(kernel[i].w - kernel[i + 1].w) : 0.0f;
                float area = (w0 + w1) / 2.0f;
                Vector3 temp = GetProfile(kernel[i].w, falloff);
                Vector4 tt = new Vector4(area * temp.x, area * temp.y, area * temp.z, kernel[i].w);
                kernel[i] = tt;
            }
            Vector4 t = kernel[nSamples / 2];
            for (int i = nSamples / 2; i > 0; i--)
                kernel[i] = kernel[i - 1];
            kernel[0] = t;
            Vector4 sum = Vector4.zero;

            for (int i = 0; i < nSamples; i++)
            {
                sum.x += kernel[i].x;
                sum.y += kernel[i].y;
                sum.z += kernel[i].z;
            }

            for (int i = 0; i < nSamples; i++)
            {
                Vector4 vecx = kernel[i];
                vecx.x /= sum.x;
                vecx.y /= sum.y;
                vecx.z /= sum.z;
                kernel[i] = vecx;
            }

            Vector4 vec = kernel[0];
            vec.x = (1.0f - strength.x) * 1.0f + strength.x * vec.x;
            vec.y = (1.0f - strength.y) * 1.0f + strength.y * vec.y;
            vec.z = (1.0f - strength.z) * 1.0f + strength.z * vec.z;
            kernel[0] = vec;

            for (int i = 1; i < nSamples; i++)
            {
                var vect = kernel[i];
                vect.x *= strength.x;
                vect.y *= strength.y;
                vect.z *= strength.z;
                kernel[i] = vect;
            }

            return kernel;
        }

        protected Vector3 Gaussian(float variance, float r, Vector3 falloff)
        {
            Vector3 g;

            float rr1 = r / (0.001f + falloff.x);
            g.x = Mathf.Exp((-(rr1 * rr1)) / (2.0f * variance)) / (2.0f * 3.14f * variance);

            float rr2 = r / (0.001f + falloff.y);
            g.y = Mathf.Exp((-(rr2 * rr2)) / (2.0f * variance)) / (2.0f * 3.14f * variance);

            float rr3 = r / (0.001f + falloff.z);
            g.z = Mathf.Exp((-(rr3 * rr3)) / (2.0f * variance)) / (2.0f * 3.14f * variance);

            return g;
        }

        protected Vector3 GetProfile(float r, Vector3 falloff)
        {
            return 0.100f * Gaussian(0.0484f, r, falloff) +
                    0.118f * Gaussian(0.187f, r, falloff) +
                    0.113f * Gaussian(0.567f, r, falloff) +
                    0.358f * Gaussian(1.99f, r, falloff) +
                    0.078f * Gaussian(7.41f, r, falloff);
        }



    }

}


