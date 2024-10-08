﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SZGUIFeleves.Models
{
    public abstract class DrawableObject : IComparable
    {
        public enum ObjectTypes
        {
            Foreground, Background, Decoration
        }

        /// <summary>
        /// Position in screen space
        /// </summary>
        public Vec2d Position { get; set; }

        /// <summary>
        /// In degrees
        /// </summary>
        public double Rotation { get; set; }

        public double OutLineThickness { get; set; }
        public Color OutLineColor { get; set; }
        public Color Color { get; set; }

        /// <summary>
        /// If true, the object is filled with the set Color or set Texture
        /// </summary>
        public bool IsFilled { get; set; }

        /// <summary>
        /// If true, gravity affects on this object.
        /// </summary>
        public bool IsGravity { get; set; }

        /// <summary>
        /// If true, object is allowed to jump, otherwise not.
        /// </summary>
        public bool IsOnGround { get; set; }

        /// <summary>
        /// Current velocity of this object.
        /// </summary>
        public Vec2d Velocity { get; set; }

        /// <summary>
        /// If not null, Color is ignored
        /// </summary>
        [JsonIgnore]
        public BitmapImage Texture { get; set; }
        public string TexturePath { get; set; }
        public double TextureOpacity { get; set; }
        [JsonIgnore]
        public BitmapImage TempTexture { get; set; }
        [JsonIgnore]
        public Vec2d TempPosition { get; set; }

        /// <summary>
        /// This state machine manages animations
        /// </summary>
        [JsonProperty]
        public StateMachine StateMachine { get; set; }

        /// <summary>
        /// Drawing priorty on the screen
        /// <para>Bottom < Default < Custom < Top</para>
        /// </summary>
        public DrawPriority DrawPriority { get; set; }

        /// <summary>
        /// Object type for the lever editor placing rules
        /// <para>(ie can place background behind everything)</para>
        /// </summary>
        public ObjectTypes ObjectType { get; set; }

        /// <summary>
        /// Necessary for lighting and shadow casting
        /// <para>True if polygon (todo) and text</para>
        /// </summary>
        public bool IsTransparent
        {
            get
            {
                if (this is Circle || this is Line || this is Rectangle)   // TODO: Polygon shadow casting
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// If true, object is translated by the camera vector in render
        /// </summary>
        public bool IsAffectedByCamera { get; set; }

        public bool IsPlayer { get; set; }

        public DrawableObject()
        {
            Position = new Vec2d();
            Velocity = new Vec2d(0, 0);
            Color = new Color();
            OutLineThickness = 0.0f;
            OutLineColor = new Color();
            IsFilled = true;
            DrawPriority = DrawPriority.Default;
            IsAffectedByCamera = true;
            IsPlayer = false;
            TextureOpacity = 1.0f;
        }

        public DrawableObject(Vec2d position)
        {
            Position = position;
            Color = new Color();
            OutLineThickness = 0.0f;
            OutLineColor = new Color();
            IsFilled = true;
            DrawPriority = DrawPriority.Default;
            IsAffectedByCamera = true;
            IsPlayer = false;
            TextureOpacity = 1.0f;
        }

        public DrawableObject(Vec2d position, Color color)
        {
            Position = position;
            Color = color;
            OutLineThickness = 0.0f;
            OutLineColor = color;
            IsFilled = true;
            DrawPriority = DrawPriority.Default;
            IsAffectedByCamera = true;
            IsPlayer = false;
            TextureOpacity = 1.0f;
        }

        /// <summary>
        /// Returns middle of the object
        /// </summary>
        /// <returns></returns>
        public abstract Vec2d GetMiddle();

        /// <summary>
        /// Returns true if objects is visible on the screen
        /// </summary>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public abstract bool IsVisible(Camera camera);

        public abstract DrawableObject GetCopy();

        public abstract override bool Equals(object obj);

        // TODO: Does not take rotation into account!!
        // TODO: Polygon, Text intersections
        public abstract bool Intersects(DrawableObject d);
        public abstract bool Intersects(Vec2d v);

        public int CompareTo(object obj)
        {
            if (obj is null || !(obj is DrawableObject))
                return 1;
            else if (this is null || !(this is DrawableObject))
                return -1;

            return DrawPriority.CompareTo((obj as DrawableObject).DrawPriority);
        }

        public void LoadTexture()
        {
            if(Texture is null && !(TexturePath is null) && TexturePath != "")
                Texture = new BitmapImage(new Uri(TexturePath, UriKind.RelativeOrAbsolute));

            if (StateMachine is null)
            {
                if(TexturePath != null && File.Exists(TexturePath))
                    Texture = new BitmapImage(new Uri(TexturePath, UriKind.RelativeOrAbsolute));
            }
            else
                StateMachine.LoadTextures();
        }

        public abstract override int GetHashCode();
    }
}
