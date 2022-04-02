using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SZGUIFeleves.Models
{
    public class Text : DrawableObject
    {
        public string Content { get; set; }
        public int FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public FontStyle FontStyle { get; set; }
        public FontWeight FontWeight { get; set; }

        #region Constructors
        public Text() : base()
        {
            FontSize = 10;
            FontFamily = new FontFamily("Arial");
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
        }

        public Text(Vec2d position, string content) : base(position)
        {
            Content = content;
            FontSize = 10;
            FontFamily = new FontFamily("Arial");
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
        }

        public Text(Vec2d position, string content, int fontSize) : base(position)
        {
            Content = content;
            FontSize = fontSize;
            FontFamily = new FontFamily("Arial");
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
        }

        public Text(Vec2d position, string content, int fontSize, Color color) : base(position, color)
        {
            Content = content;
            FontSize = fontSize;
            FontFamily = new FontFamily("Arial");
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
        }

        public Text(Vec2d position, string content, int fontSize, FontStyle fontStyle,
                    FontWeight fontWeight, Color color) : base(position, color)
        {
            Content = content;
            FontSize = fontSize;
            FontFamily = new FontFamily("Arial");
            FontStyle = fontStyle;
            FontWeight = fontWeight;
        }

        public Text(Vec2d position, string content, int fontSize, FontFamily fontFamily, FontStyle fontStyle,
                    FontWeight fontWeight, Color color) : base(position, color)
        {
            Content = content;
            FontSize = fontSize;
            FontFamily = fontFamily;
            FontStyle = fontStyle;
            FontWeight = fontWeight;
        }
        #endregion

        public override Vec2d GetMiddle() // TODO: Implementing text middle with FontFamily/Style/Weight and FontSize
        {
            throw new NotImplementedException();
        }

        public override bool IsVisible(Camera camera)
        {
            throw new NotImplementedException();
        }

        public override Text GetCopy()
        {
            Text t = new Text(new Vec2d(Position), Content, FontSize, FontFamily, FontStyle, FontWeight, new Color(Color))
            {
                Rotation = Rotation,
                OutLineThickness = OutLineThickness,
                OutLineColor = new Color(OutLineColor),
                IsFilled = IsFilled,
                DrawPriority = DrawPriority,
                IsAffectedByCamera = IsAffectedByCamera,
                IsPlayer = IsPlayer
            };

            if (Texture != null)
                t.Texture = Texture.Clone();

            if (StateMachine != null)
                t.StateMachine = StateMachine.GetCopy();

            return t;
        }

        public override bool Intersects(DrawableObject d)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
