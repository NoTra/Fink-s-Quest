using UnityEngine;
using UnityEngine.UI;

public class CutoutMaskUI : Image
{
    public override Material materialForRendering
    {
        get
        {
            var material = new Material(base.materialForRendering);
            material.SetFloat("_StencilComp", (float)UnityEngine.Rendering.CompareFunction.NotEqual);
            return material;
        }
    }
}
