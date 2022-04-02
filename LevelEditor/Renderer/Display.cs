using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using LevelEditor.Logic;
using SZGUIFeleves.Models;

namespace LevelEditor.Renderer
{
    public class Display : FrameworkElement
    {
        private IEditorModel model;
        private Vec2d WindowSize { get; set; }

        public void SetupModel(IEditorModel model, int windowWidth, int windowHeight)
        {
            this.model = model;
            model.DrawEvent += InvalidateVisual;    // Subscribing to the logic's Draw event. Called after logic update
            WindowSize = new Vec2d(windowWidth, windowHeight);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (model is null)
                return;

            // Clearing the screen
            dc.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1), new Rect(0, 0, WindowSize.x, WindowSize.y));

            dc.PushTransform(new TranslateTransform(-model.Camera.CenteredPosition.x, -model.Camera.CenteredPosition.y));
            //dc.PushTransform(new ScaleTransform(model.Camera.Zoom, model.Camera.Zoom, model.Camera.Position.x, model.Camera.Position.y));
            // Transforming objects by the camera

            // Zoom not working yet

            DrawObjects(ref dc, model.ObjectsToDisplayWorldSpace);
            dc.Pop();
            //dc.Pop();

            // Drawing the UI (or screen fixed objects)

            model.ObjectsToDisplayWorldSpace.Clear();
        }

        private void DrawObjects(ref DrawingContext dc, List<DrawableObject> objects)
        {
            foreach (DrawableObject obj in objects)
            {
                // Creating the brush with the set Color or Texture
                Brush brush = new SolidColorBrush();
                if (obj.StateMachine != null)
                {
                    brush = new ImageBrush(obj.StateMachine.CurrentTexture);
                    (brush as ImageBrush).Opacity = obj.TextureOpacity;
                }
                else if (obj.Texture != null)
                {
                    brush = new ImageBrush(obj.Texture);
                    (brush as ImageBrush).Opacity = obj.TextureOpacity;
                }
                else
                {
                    System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb((byte)obj.Color.A, (byte)obj.Color.R, (byte)obj.Color.G, (byte)obj.Color.B);
                    brush = new SolidColorBrush(color);
                }

                // Creating the pen with the set OutlineColor and OutlineThickness/Width
                System.Windows.Media.Color outlineColor = System.Windows.Media.Color.FromArgb
                                                    ((byte)obj.OutLineColor.A, (byte)obj.OutLineColor.R, (byte)obj.OutLineColor.G, (byte)obj.OutLineColor.B);
                Brush outlineBrush = new SolidColorBrush(outlineColor);
                Pen pen = new Pen()
                {
                    Brush = outlineBrush
                };
                if (obj is Line l)
                    pen.Thickness = l.Width;
                else
                    pen.Thickness = obj.OutLineThickness;

                brush.Freeze();
                outlineBrush.Freeze();
                pen.Freeze();


                // TODO: Code refactoring to get Text boundaries to get middle
                if (obj.Rotation != 0 && !(obj is Text))
                {
                    Vec2d middle = obj.GetMiddle();
                    dc.PushTransform(new RotateTransform(obj.Rotation, middle.x, middle.y));
                }

                if (obj is Rectangle r)
                {
                    //if (!r.IsVisible(model.Camera))
                    //    continue;

                    Rect rect = new Rect(r.Position.x, r.Position.y, r.Size.x, r.Size.y);

                    if (r.IsRounded)
                    {
                        if (obj.IsFilled)
                            dc.DrawRoundedRectangle(brush, pen, rect, r.CornerRadius.x, r.CornerRadius.y);
                        else
                            dc.DrawRoundedRectangle(null, pen, rect, r.CornerRadius.x, r.CornerRadius.y);
                    }
                    else
                    {
                        if (obj.IsFilled)
                            dc.DrawRectangle(brush, pen, rect);
                        else
                            dc.DrawRectangle(null, pen, rect);
                    }
                }
                else if (obj is Circle e)
                {
                    //if (!e.IsVisible(model.Camera))
                    //    continue;

                    if (obj.IsFilled)
                        dc.DrawEllipse(brush, pen, new Point(e.Position.x, e.Position.y), e.Radius, e.Radius);
                    else
                        dc.DrawEllipse(null, pen, new Point(e.Position.x, e.Position.y), e.Radius, e.Radius);
                }
                else if (obj is Line line)
                {
                    //if (!line.IsVisible(model.Camera))
                    //    continue;

                    dc.DrawLine(pen, new Point(line.Position.x, line.Position.y), new Point(line.Position2.x, line.Position2.y));
                }
                else if (obj is Polygon p)
                {
                    //if (!p.IsVisible(model.Camera))
                    //    continue;

                    var geometry = new StreamGeometry();
                    geometry.FillRule = FillRule.EvenOdd;
                    using (StreamGeometryContext ctx = geometry.Open())
                    {
                        ctx.BeginFigure(new Point(p.Position.x, p.Position.y), obj.IsFilled, true);

                        foreach (Vec2d point in p.Points)
                            ctx.LineTo(new Point(point.x, point.y), true, true);
                    }
                    geometry.Freeze();

                    if (obj.IsFilled)
                        dc.DrawGeometry(brush, pen, geometry);
                    else
                        dc.DrawGeometry(null, pen, geometry);
                }
                else if (obj is Text t)
                {
                    FormattedText formattedText = new FormattedText(t.Content, System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, new Typeface(t.FontFamily, t.FontStyle, t.FontWeight, FontStretches.Normal), t.FontSize,
                        brush, 10);
                    dc.DrawText(formattedText, new Point(t.Position.x, t.Position.y));
                }

                if (obj.Rotation != 0 && !(obj is Text))
                    dc.Pop();
            }
        }
    }
}
