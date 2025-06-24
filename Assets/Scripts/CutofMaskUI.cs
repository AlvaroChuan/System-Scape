using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class CutofMaskUI : Image
{
    private UnityEngine.Material _customMaterial;

    public override UnityEngine.Material materialForRendering
    {
        get
        {
            if (_customMaterial == null)
            {
                _customMaterial = new UnityEngine.Material(base.materialForRendering);
                _customMaterial.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
            }

            return _customMaterial;
        }
    }
}
