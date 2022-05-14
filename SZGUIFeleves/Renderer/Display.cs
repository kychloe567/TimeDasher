﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SZGUIFeleves.Logic;
using SZGUIFeleves.Models;
using SZGUIFeleves.Models.DrawableObjects;

namespace SZGUIFeleves.Renderer
{
    public class Display : FrameworkElement
    {
        private IGameModel model;
        private Vec2d WindowSize { get; set; }

        public void SetupModel(IGameModel model, int windowWidth, int windowHeight)
        {
            this.model = model;
            model.DrawEvent += WindowSizeChanged;    // Subscribing to the logic's Draw event. Called after logic update
            WindowSize = new Vec2d(windowWidth, windowHeight);
        }

        private void WindowSizeChanged(int WindowSizeWidth, int WindowSizeHeight)
        {
            WindowSize = new Vec2d(WindowSizeWidth, WindowSizeHeight);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (model is null)
                return;

            // Clearing the screen
            dc.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1), new Rect(0, 0, WindowSize.x, WindowSize.y));
            DrawObjects(ref dc, model.MovingBackgrounds, true);

            // Transforming objects by the camera
            dc.PushTransform(new TranslateTransform(-model.Camera.CenteredPosition.x, -model.Camera.CenteredPosition.y));

            // Zoom not working yet
            //dc.PushTransform(new ScaleTransform(model.Camera.Zoom, model.Camera.Zoom));

            DrawObjects(ref dc, model.ObjectsToDisplayWorldSpace, camera:model.Camera);
            dc.Pop();

            // Drawing the UI (or screen fixed objects)
            DrawObjects(ref dc, model.ObjectsToDisplayScreenSpace, isUI:true);

            model.ObjectsToDisplayWorldSpace.Clear();
            model.ObjectsToDisplayScreenSpace.Clear();
        }

        private void DrawObjects(ref DrawingContext dc, List<DrawableObject> objects, bool isBackground = false, bool isUI = false, Camera camera = null)
        {
            bool shadowAdded = false;
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

                if(obj is Shadow s)
                {
                    if (!s.IsVisible(model.Camera) && !isUI)
                        continue;

                    var geometry = new StreamGeometry();
                    geometry.FillRule = FillRule.EvenOdd;
                    using (StreamGeometryContext ctx = geometry.Open())
                    {
                        ctx.BeginFigure(new Point(s.Position.x, s.Position.y), obj.IsFilled, true);

                        foreach (Vec2d point in s.Points)
                            ctx.LineTo(new Point(point.x, point.y), true, true);
                    }
                    //geometry.Freeze();

                    if (!shadowAdded)
                    {
                        StreamGeometry b = new StreamGeometry();
                        b.FillRule = FillRule.EvenOdd;
                        using (StreamGeometryContext ctx = b.Open())
                        {
                            ctx.BeginFigure(new Point(camera.CenteredPosition.x, camera.CenteredPosition.y), obj.IsFilled, true);
                            ctx.LineTo(new Point(camera.CenteredPosition.x + WindowSize.x, camera.CenteredPosition.y), true, true);
                            ctx.LineTo(new Point(camera.CenteredPosition.x + WindowSize.x, camera.CenteredPosition.y + WindowSize.y), true, true);
                            ctx.LineTo(new Point(camera.CenteredPosition.x, camera.CenteredPosition.y + WindowSize.y), true, true);
                        }

                        CombinedGeometry cg = new CombinedGeometry();
                        cg.GeometryCombineMode = GeometryCombineMode.Exclude;
                        cg.Geometry1 = b;
                        cg.Geometry2 = geometry;

                        dc.DrawGeometry(new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 0, 0, 0)), pen, cg);
                        shadowAdded = true;
                    }

                    if (obj.IsFilled)
                        dc.DrawGeometry(brush, pen, geometry);
                    else
                        dc.DrawGeometry(null, pen, geometry);
                }
                else if (obj is Rectangle r)
                {
                    if (!r.IsVisible(model.Camera) && !isBackground && !isUI)
                        continue;

                    Rect rect = new Rect(r.Position.x, r.Position.y, r.Size.x, r.Size.y);
                    if (r.ObjectType != DrawableObject.ObjectTypes.Decoration)
                        rect.Size = new Size(r.Size.x + 1, r.Size.y + 1);

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
                    if (!e.IsVisible(model.Camera) && !isUI)
                        continue;

                    if (obj.IsFilled)
                        dc.DrawEllipse(brush, pen, new Point(e.Position.x, e.Position.y), e.Radius, e.Radius);
                    else
                        dc.DrawEllipse(null, pen, new Point(e.Position.x, e.Position.y), e.Radius, e.Radius);
                }
                else if (obj is Line line)
                {
                    if (!line.IsVisible(model.Camera) && !isUI)
                        continue;

                    dc.DrawLine(pen, new Point(line.Position.x, line.Position.y), new Point(line.Position2.x, line.Position2.y));
                }
                else if (obj is Polygon p)
                {
                    if (!p.IsVisible(model.Camera) && !isUI)
                        continue;

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

                    Geometry geometry = formattedText.BuildGeometry(new Point(t.Position.x, t.Position.y));
                    dc.DrawText(formattedText, new Point(t.Position.x, t.Position.y));

                    dc.DrawGeometry(null, pen, geometry);

                }

                if (obj.Rotation != 0 && !(obj is Text))
                    dc.Pop();
            }
        }
    }
}
