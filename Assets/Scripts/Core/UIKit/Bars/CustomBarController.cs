using UnityEngine;

namespace Anomalus.UIKit.Bars
{
    public class CustomBarController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;

        private MaterialPropertyBlock _propertyBlock;

        private Color _color1;
        private Color _color2;
        private float _fillAmount1;
        private float _fillAmount2;

        private static LazyShaderProperty _color1Property = new("_Color");
        private static LazyShaderProperty _color2Property = new("_Color2");
        private static LazyShaderProperty _fillAmount1Property = new("_FillAmount");
        private static LazyShaderProperty _fillAmount2Property = new("_FillAmount2");

        private struct LazyShaderProperty
        {
            public int Id
            {
                get
                {
                    if (id == -1)
                    {
                        id = Shader.PropertyToID(name);
                    }
                    return id;
                }
            }

            private string name;
            private int id;

            public LazyShaderProperty(string name)
            {
                this.name = name;
                id = -1;
            }
        }

        private void Awake()
        {
            _propertyBlock = new();
            _renderer.GetPropertyBlock(_propertyBlock);
            _color1 = _propertyBlock.GetColor(_color1Property.Id);
            _color2 = _propertyBlock.GetColor(_color2Property.Id);
            _fillAmount1 = _propertyBlock.GetFloat(_fillAmount1Property.Id);
            _fillAmount2 = _propertyBlock.GetFloat(_fillAmount2Property.Id);
        }

        public void SetColor1(Color value)
        {
            _color1 = value;
            _propertyBlock.SetColor(_color1Property.Id, value);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetColor2(Color value)
        {
            _color2 = value;
            _propertyBlock.SetColor(_color2Property.Id, value);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetFillAmount1(float value)
        {
            _fillAmount1 = value;
            _propertyBlock.SetFloat(_fillAmount1Property.Id, value);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetFillAmount2(float value)
        {
            _fillAmount2 = value;
            _propertyBlock.SetFloat(_fillAmount2Property.Id, value);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public Color GetColor1() => _color1;
        public Color GetColor2() => _color2;
        public float GetFillAmount1() => _fillAmount1;
        public float GetFillAmount2() => _fillAmount2;
    }
}
