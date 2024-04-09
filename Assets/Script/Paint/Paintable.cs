using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    const int TEXTURE_SIZE = 1024;

    public float extendsIslandOffset = 1;

    int maskTextureID = Shader.PropertyToID("_MaskTexture");

    RenderTexture maskRenderTexture;
    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;
    RenderTexture supportTexture;

    Renderer rend;

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    private void Start()
    {
        maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        maskRenderTexture.filterMode = FilterMode.Bilinear;

        extendIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        uvIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        supportTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        supportTexture.filterMode = FilterMode.Bilinear;

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

        PaintManager.instance.initTexture(this);
    }
    private void OnDisable()
    {
        maskRenderTexture.Release();
        supportTexture.Release();
        extendIslandsRenderTexture.Release();
        uvIslandsRenderTexture.Release();
    }
}
