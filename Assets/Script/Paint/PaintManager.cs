using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : Singleton<PaintManager>
{
    public Shader texturePaint;
    public Shader extendIslands;

    CommandBuffer buffer;

    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int positionID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int colorID = Shader.PropertyToID("_PainterColor");
    int textureID = Shader.PropertyToID("_MainTex");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");

    Material paintMaterial;
    Material extendMaterial;

    public override void Awake()
    {
        base.Awake();

        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);

        buffer = new CommandBuffer();
        buffer.name = "CommandBuffer - " + gameObject.name;
    }

    public void initTexture(Paintable paintable)
    {
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        buffer.SetRenderTarget(mask);
        buffer.SetRenderTarget(extend);
        buffer.SetRenderTarget(support);

        paintMaterial.SetFloat(prepareUVID, 1);
        buffer.SetRenderTarget(uvIslands);
        buffer.DrawRenderer(rend, paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    public void paint(Paintable paintable, Vector3 pos, float radius = 1f, float hardness = 0.5f, float strength = 0.5f, Color? color = null)
    {
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        paintMaterial.SetFloat(prepareUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetTexture(textureID, support);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        extendMaterial.SetTexture(uvIslandsID, uvIslands);

        buffer.SetRenderTarget(mask);
        buffer.DrawRenderer(rend, paintMaterial, 0);

        buffer.SetRenderTarget(support);
        buffer.Blit(mask, support);

        buffer.SetRenderTarget(extend);
        buffer.Blit(mask, extend, extendMaterial);

        Graphics.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
