using UnityEngine;

namespace MyProject.Shader
{
    [ExecuteAlways] // ExecuteInEditMode 대신 ExecuteAlways 사용
    public class UpdateShaderProperties : MonoBehaviour
    {
        private Transform cachedTransform;
        private readonly string shaderName = "Shader Graphs/ToonRamp";
        private readonly string lightDirProperty = "_LightDir";

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void Update()
        {
            if (cachedTransform.hasChanged)
            {
                UpdateShaderLightDirection();
                cachedTransform.hasChanged = false;
            }
        }

        private void UpdateShaderLightDirection()
        {
            Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
            foreach (var renderer in renderers)
            {
                Material material;
#if UNITY_EDITOR
                material = renderer.sharedMaterial;
#else
                material = renderer.material;
#endif
                if (material != null && material.shader != null &&
                    string.Compare(material.shader.name, shaderName) == 0)
                {
                    material.SetVector(lightDirProperty, cachedTransform.forward);
                }
            }
        }
    }
}