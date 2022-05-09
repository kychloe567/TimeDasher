using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Models.DrawableObjects;
using SZGUIFeleves.Models.EngineModels.MovingBackground;

namespace SZGUIFeleves.Models
{
    public static class SceneManager
    {
        private const string ScenePath = "Scenes/";

        public static Scene GetDefaultScene()
        {
            return new Scene("default", new List<DrawableObject>(), 0, new List<DynamicPointLight>(), new MovingBackground());
        }

        public static Scene GetSceneByName(string scene)
        {
            string file = ScenePath + scene + ".json";
            if (!File.Exists(file))
            {
                file = ScenePath + "\\CustomScenes\\" + scene + ".json";
                if (!File.Exists(file))
                    return null;
            }

            SceneJson sj = JsonConvert.DeserializeObject<SceneJson>(File.ReadAllText(file));
            List<DrawableObject> Objects = new List<DrawableObject>();

            double lowestPoint = 0;

            int playerIndex = 0;
            if (sj.Player is null)
            {
                Objects.Add(new Player()
                {
                    IsPlayer = true,
                    Position = new Vec2d(0, 0),
                    Size = new Vec2d(31.5, 63),
                    Color = Color.White,
                    DrawPriority = DrawPriority.Top
                });
            }
            else
            {
                sj.Player.LoadTexture();
                Objects.Add(sj.Player);
            }

            for (int i = 0; i < sj.Circles.Count(); i++)
            {
                sj.Circles[i].LoadTexture();
                Objects.Add(sj.Circles[i]);
                if (sj.Circles[i].Position.y > lowestPoint)
                    lowestPoint = sj.Circles[i].Position.y;
            }
            for(int i = 0; i < sj.Lines.Count(); i++)
            {
                sj.Lines[i].LoadTexture();
                Objects.Add(sj.Lines[i]);
            }
            for(int i = 0; i < sj.Polygons.Count(); i++)
            {
                sj.Polygons[i].LoadTexture();
                Objects.Add(sj.Polygons[i]);
            }
            for (int i = 0; i < sj.Traps.Count(); i++)
            {
                sj.Traps[i].LoadTexture();
                Objects.Add(sj.Traps[i]);
                if (sj.Traps[i].Position.y > lowestPoint)
                    lowestPoint = sj.Traps[i].Position.y;
            }
            for (int i = 0; i < sj.Checkpoints.Count(); i++)
            {
                sj.Checkpoints[i].LoadTexture();
                Objects.Add(sj.Checkpoints[i]);
                if (sj.Checkpoints[i].Position.y > lowestPoint)
                    lowestPoint = sj.Checkpoints[i].Position.y;
            }
            for (int i = 0; i < sj.Rectangles.Count(); i++)
            {
                sj.Rectangles[i].LoadTexture();
                Objects.Add(sj.Rectangles[i]);
                if (sj.Rectangles[i].Position.y > lowestPoint)
                    lowestPoint = sj.Rectangles[i].Position.y;
            }
            for(int i = 0; i < sj.Texts.Count(); i++)
            {
                sj.Texts[i].LoadTexture();
                Objects.Add(sj.Texts[i]);
            }

            foreach(DrawableObject d in Objects)
            {
                if(!(d.StateMachine is null))
                {
                    d.StateMachine.Start();
                }
            }

            sj.MovingBackground.LoadTextures();
            Scene s = new Scene(sj.Title, Objects, playerIndex, sj.PointLights, sj.MovingBackground)
            {
                LowestPoint = lowestPoint
            };

            if (!(s.Objects[s.PlayerIndex].StateMachine is null))
            {
                s.Objects[s.PlayerIndex].StateMachine.SetState("idleright");
            }

            return s;
        }

        public static Scene GetSceneByFullPath(string path)
        {
            if (!File.Exists(path))
                return null;

            SceneJson sj = JsonConvert.DeserializeObject<SceneJson>(File.ReadAllText(path));
            List<DrawableObject> Objects = new List<DrawableObject>();

            double lowestPoint = 0;

            int playerIndex = 0;
            if (sj.Player is null)
            {
                Objects.Add(new Player()
                {
                    IsPlayer = true,
                    Position = new Vec2d(0, 0),
                    Size = new Vec2d(31.5, 63),
                    Color = Color.White,
                    DrawPriority = DrawPriority.Top
                });
            }
            else
            {
                sj.Player.LoadTexture();
                Objects.Add(sj.Player);
            }

            for (int i = 0; i < sj.Circles.Count(); i++)
            {
                sj.Circles[i].LoadTexture();
                Objects.Add(sj.Circles[i]);
                if (sj.Circles[i].Position.y > lowestPoint)
                    lowestPoint = sj.Circles[i].Position.y;
            }
            for (int i = 0; i < sj.Lines.Count(); i++)
            {
                sj.Lines[i].LoadTexture();
                Objects.Add(sj.Lines[i]);
            }
            for (int i = 0; i < sj.Polygons.Count(); i++)
            {
                sj.Polygons[i].LoadTexture();
                Objects.Add(sj.Polygons[i]);
            }
            for (int i = 0; i < sj.Traps.Count(); i++)
            {
                sj.Traps[i].LoadTexture();
                Objects.Add(sj.Traps[i]);
                if (sj.Traps[i].Position.y > lowestPoint)
                    lowestPoint = sj.Traps[i].Position.y;
            }
            for (int i = 0; i < sj.Checkpoints.Count(); i++)
            {
                sj.Checkpoints[i].LoadTexture();
                Objects.Add(sj.Checkpoints[i]);
                if (sj.Checkpoints[i].Position.y > lowestPoint)
                    lowestPoint = sj.Checkpoints[i].Position.y;
            }
            for (int i = 0; i < sj.Rectangles.Count(); i++)
            {
                sj.Rectangles[i].LoadTexture();
                Objects.Add(sj.Rectangles[i]);
                if (sj.Rectangles[i].Position.y > lowestPoint)
                    lowestPoint = sj.Rectangles[i].Position.y;
            }
            for (int i = 0; i < sj.Texts.Count(); i++)
            {
                sj.Texts[i].LoadTexture();
                Objects.Add(sj.Texts[i]);
            }

            sj.MovingBackground.LoadTextures();
            Scene s = new Scene(sj.Title, Objects, playerIndex, sj.PointLights, sj.MovingBackground);

            if (!(s.Objects[s.PlayerIndex].StateMachine is null))
            {
                s.Objects[s.PlayerIndex].StateMachine.SetState("idleright");
            }

            return s;
        }

        public static void SaveScene(Scene scene)
        {
            SceneJson sj = new SceneJson();
            foreach(var obj in scene.Objects)
            {
                if (obj is Player player)
                    sj.Player = player;
                else if (obj is Circle c)
                    sj.Circles.Add(c);
                else if (obj is Line l)
                    sj.Lines.Add(l);
                else if (obj is Polygon p)
                    sj.Polygons.Add(p);
                else if (obj is Trap trap)
                    sj.Traps.Add(trap);
                else if (obj is Checkpoint cp)
                    sj.Checkpoints.Add(cp);
                else if (obj is Rectangle r)
                    sj.Rectangles.Add(r);
                else if (obj is Text t)
                    sj.Texts.Add(t);
            }
            sj.MovingBackground = scene.MovingBackground;
            sj.Title = scene.Title;
            sj.PointLights = scene.PointLights;

            File.WriteAllText(ScenePath + scene.Title + ".json", JsonConvert.SerializeObject(sj));
        }

        public static void SaveScene(Scene scene, string path)
        {
            SceneJson sj = new SceneJson();
            foreach(var obj in scene.Objects)
            {
                if (obj is Player player)
                    sj.Player = player;
                else if (obj is Circle c)
                    sj.Circles.Add(c);
                else if (obj is Line l)
                    sj.Lines.Add(l);
                else if (obj is Polygon p)
                    sj.Polygons.Add(p);
                else if (obj is Trap trap)
                    sj.Traps.Add(trap);
                else if (obj is Checkpoint cp)
                    sj.Checkpoints.Add(cp);
                else if (obj is Rectangle r)
                    sj.Rectangles.Add(r);
                else if (obj is Text t)
                    sj.Texts.Add(t);
            }
            sj.MovingBackground = scene.MovingBackground;
            sj.Title = scene.Title;
            sj.PointLights = scene.PointLights;

            File.WriteAllText(path, JsonConvert.SerializeObject(sj));
        }
    }
}
