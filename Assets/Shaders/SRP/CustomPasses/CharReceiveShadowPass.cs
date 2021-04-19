using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

class CharReceiveShadowPass : CustomPass
{
    public LayerMask hairLayerMask;
    public Material shadowMaterial;

    RTHandle m_MainLightShadowmapTexture;

    const int k_ShadowmapBufferBits = 32;
    const int s_shadowmap_size = 2048;


    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {

        shadowMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("W02/HDRP_W02_CharToon"));

        //m_MainLightShadowmapTexture = RTHandles.Alloc(
        //            Vector2.one, TextureXR.slices, dimension: TextureXR.dimension,
        //            colorFormat: GraphicsFormat.R16G16B16A16_SFloat,
        //            useDynamicScale: true, enableRandomWrite:true,
        //            name: "_SelfShadowTexture"
        //        );
        m_MainLightShadowmapTexture = RTHandles.Alloc(
         Vector2.one, TextureXR.slices, dimension: TextureXR.dimension,
         colorFormat: GraphicsFormat.R16_UInt, useDynamicScale: true,
         name: "_SelfShadowTexture", depthBufferBits: DepthBits.Depth16
         );

    }

    protected override void Execute(CustomPassContext ctx)
    {
        // Executed every frame for all the camera inside the pass volume.
        // The context contains the command buffer to use to enqueue graphics commands.
        
        CommandBuffer cmd = ctx.cmd;
        Camera shadowCamera = ctx.hdCamera.camera;

        cmd.SetGlobalTexture("_SelfShadowTexture", m_MainLightShadowmapTexture);

        CoreUtils.SetRenderTarget(cmd, m_MainLightShadowmapTexture, ClearFlag.All, Color.black);


        var renstateBlock = new RenderStateBlock(RenderStateMask.Depth)
        {
            depthState = new DepthState(true, CompareFunction.LessEqual),
            stencilState = new StencilState(false),
        };


        var hairResult = new RendererListDesc(new ShaderTagId("DepthOnly"), ctx.cullingResults, shadowCamera)
        {
            rendererConfiguration = PerObjectData.None,
            renderQueueRange = RenderQueueRange.all,        // to change
            sortingCriteria = SortingCriteria.BackToFront,
            excludeObjectMotionVectors = false,
            overrideMaterial = shadowMaterial,
            overrideMaterialPassIndex = (shadowMaterial != null) ? shadowMaterial.FindPass("DepthOnly") : 0,
            stateBlock = renstateBlock,
            layerMask = hairLayerMask,
        };

        CoreUtils.DrawRendererList(ctx.renderContext, cmd, RendererList.Create(hairResult));
    }

    protected override void Cleanup()
    {
        // Cleanup code
        CoreUtils.Destroy(shadowMaterial);
        m_MainLightShadowmapTexture.Release();
    }
}