using System.Text;
using UnityEngine;
using TMPro;

namespace Anomalus.UIKit.Tooltip
{
    public sealed class Tooltip : MonoBehaviour
    {
        [SerializeField] private Vector2 _positionOffset;
        [SerializeField] private Fade _fade;
        [SerializeField] private RectTransform _zoneRect;
        [SerializeField] private RectTransform _body;

        [SerializeField] private TMP_Text _descriptionText;

        [System.Serializable]
        public struct Content
        {
            [field: SerializeField]
            public string Text { get; private set; }

            public Content(string text)
            {
                Text = text;
            }
        }

        public struct ContentBuilder
        {
            private StringBuilder _text;
            private StringBuilder Text
            {
                get
                {
                    if (_text == null)
                    {
                        _text = new StringBuilder();
                    }

                    return _text;
                }
            }

            public struct TagBlock : System.IDisposable
            {
                public string Tag { get; }
                public string Extra { get; }
                public string CloseTag { get; }
                private readonly StringBuilder _text;

                public TagBlock(string tag, string extra, string closeTag, bool autoBegin, StringBuilder text)
                {
                    Tag = tag;
                    Extra = extra;
                    _text = text;
                    CloseTag = closeTag ?? tag;

                    if (autoBegin)
                    {
                        Begin();
                    }
                }

                public TagBlock Begin()
                {
                    _text.Append('<');
                    _text.Append(Tag);
                    _text.Append(Extra);
                    _text.Append('>');
                    return this;
                }

                public void Dispose()
                {
                    _text.Append('<');
                    _text.Append('/');
                    _text.Append(CloseTag);
                    _text.Append('>');
                }
            }

            public struct TextSprite
            {
                public string AssetName { get; }
                public string Name { get; }
                public int Index { get; }

                public TextSprite(string name, string asset)
                {
                    AssetName = asset;
                    Name = name;
                    Index = -1;
                }
                public TextSprite(int index, string asset)
                {
                    AssetName = asset;
                    Name = null;
                    Index = index;
                }
                public TextSprite(string name) : this(name, null) { }
                public TextSprite(int index) : this(index, null) { }


                public static implicit operator TextSprite(string name)
                {
                    return new TextSprite(name);
                }
                public static implicit operator TextSprite(int index)
                {
                    return new TextSprite(index);
                }
            }


            //
            // Basic
            //

            public ContentBuilder AppendLine(string line)
            {
                Text.AppendLine(line);
                return this;
            }

            public ContentBuilder AppendLine()
            {
                Text.AppendLine();
                return this;
            }

            public ContentBuilder AppendLine(string styleName, string line)
            {
                Text.Append("<style=\"");
                Text.Append(styleName);
                Text.Append("\">");
                Text.Append(line);
                Text.Append("</style>");
                Text.AppendLine();
                return this;
            }

            public ContentBuilder AppendWithTag(string tag, string line)
            {
                Text.Append("<");
                Text.Append(tag);
                Text.Append(">");
                Text.Append(line);
                Text.Append("</");
                Text.Append(tag);
                Text.Append(">");
                return this;
            }

            public ContentBuilder AppendWithTag(string tag, string extraParams, string line)
            {
                Text.Append("<");
                Text.Append(tag);
                Text.Append(extraParams);
                Text.Append(">");
                Text.Append(line);
                Text.Append("</");
                Text.Append(tag);
                Text.Append(">");
                return this;
            }

            public ContentBuilder Append(string str)
            {
                Text.Append(str);
                return this;
            }

            public TagBlock Block(string tag, string extra, string closeTag = null, bool autoBegin = true)
            {
                return new TagBlock(tag, extra, closeTag, autoBegin, Text);
            }

            public TagBlock StyleBlock(string styleName, bool autoBegin = true)
            {
                return new TagBlock("style", $"=\"{styleName}\"", null, autoBegin, Text);
            }


            //
            // Special
            //

            public ContentBuilder AppendTitle(string title)
            {
                return AppendLine(styleName: "title", title);
            }

            public ContentBuilder AppendColor(Color color, string str)
            {
                return AppendWithTag("color", $"=#{ColorUtility.ToHtmlStringRGB(color)}", str);
            }

            public TagBlock Color(Color color)
            {
                return Block("color", $"=#{ColorUtility.ToHtmlStringRGB(color)}");
            }

            public TagBlock Opacity(int alpha)
            {
                return Block("alpha", $"=#{alpha:X2}", "color");
            }

            public TagBlock RightShift(bool autoBegin = true)
            {
                return StyleBlock("right-shifted", autoBegin);
            }

            public ContentBuilder AppendSprite(string spriteName)
            {
                Text.Append($"<sprite name=\"{spriteName}\">");
                return this;
            }

            public ContentBuilder AppendSprite(int spriteIndex)
            {
                Text.Append($"<sprite index={spriteIndex}>");
                return this;
            }

            public ContentBuilder AppendSprite(string spriteName, string assetName)
            {
                Text.Append($"<sprite=\"{assetName}\" name=\"{spriteName}\">");
                return this;
            }

            public ContentBuilder AppendSprite(int spriteIndex, string assetName)
            {
                Text.Append($"<sprite=\"{assetName}\" index={spriteIndex}>");
                return this;
            }

            public ContentBuilder AppendSprite(TextSprite sprite)
            {
                if (sprite.AssetName != null)
                {
                    if (sprite.Name != null)
                    {
                        return AppendSprite(sprite.Name, sprite.AssetName);
                    }
                    else
                    {
                        return AppendSprite(sprite.Index, sprite.AssetName);
                    }
                }
                else
                {
                    if (sprite.Name != null)
                    {
                        return AppendSprite(sprite.Name);
                    }
                    else
                    {
                        return AppendSprite(sprite.Index);
                    }
                }
            }

            public ContentBuilder AppendVerticalSpace(float amount)
            {
                Text.Append($"<line-height={amount:P0}%>\n</line-height>");
                return this;
            }


            //
            // End
            //

            public Content Build()
            {
                return new Content(Text.ToString());
            }

            public static implicit operator Content(ContentBuilder builder)
            {
                return builder.Build();
            }
        }

        public void Show(Vector2 position, Content content)
        {
            _fade.DoFadeIn();

            RenderContent(content);

            MoveTo(position);
        }

        public void MoveTo(Vector2 position)
        {
            var pos = position + _positionOffset;

            var height = _body.rect.height * _zoneRect.lossyScale.y;

            var offscreen = -height + pos.y - (_zoneRect.position.y + _zoneRect.rect.yMin * _zoneRect.lossyScale.y);

            if (offscreen < 0f)
            {
                pos.y += height - 2f * _positionOffset.y;
            }

            _body.position = pos;
        }

        public void Hide()
        {
            _fade.DoFadeOut();
        }

        private void RenderContent(Content content)
        {
            //titleText.text = content.Title;
            _descriptionText.text = content.Text;
        }
    }
}
