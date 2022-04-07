using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public static class SceneManager
    {
        private const string ScenePath = "Scenes/";

        // Get MainMenu
        // Get HubArea

        public static Scene GetDefaultScene()
        {
            return new Scene("default", new List<DrawableObject>(), 0, new List<DynamicPointLight>());
        }

        public static Scene GetScene(string scene)
        {
            if (!File.Exists(ScenePath + scene + ".json"))
                return null;

            SceneJson sj = JsonConvert.DeserializeObject<SceneJson>(File.ReadAllText(ScenePath + scene + ".json"));
            List<DrawableObject> Objects = new List<DrawableObject>();
            int playerIndex = -1;
            for(int i = 0; i < sj.Circles.Count(); i++)
            {
                if (playerIndex == -1 && sj.Circles[i].IsPlayer)
                    playerIndex = i;
                Objects.Add(sj.Circles[i]);
            }
            for(int i = 0; i < sj.Lines.Count(); i++)
            {
                if (playerIndex == -1 && sj.Lines[i].IsPlayer)
                    playerIndex = i;
                Objects.Add(sj.Lines[i]);
            }
            for(int i = 0; i < sj.Polygons.Count(); i++)
            {
                if (playerIndex == -1 && sj.Polygons[i].IsPlayer)
                    playerIndex = i;
                Objects.Add(sj.Polygons[i]);
            }
            for(int i = 0; i < sj.Rectangles.Count(); i++)
            {
                if (playerIndex == -1 && sj.Rectangles[i].IsPlayer)
                    playerIndex = i;
                Objects.Add(sj.Rectangles[i]);
            }
            for (int i = 0; i < sj.Traps.Count(); i++)
            {
                if (playerIndex == -1 && sj.Traps[i].IsPlayer)
                    playerIndex = i;
                Objects.Add(sj.Traps[i]);
            }
            for (int i = 0; i < sj.Texts.Count(); i++)
            {
                if (playerIndex == -1 && sj.Texts[i].IsPlayer)
                    playerIndex = i;
                Objects.Add(sj.Texts[i]);
            }

            Scene s = new Scene(sj.Title, Objects, playerIndex, sj.PointLights);
            return s;
        }

        public static void SaveScene(Scene scene)
        {
            SceneJson sj = new SceneJson();
            foreach(var obj in scene.Objects)
            {
                if (obj is Circle c)
                    sj.Circles.Add(c);
                else if (obj is Line l)
                    sj.Lines.Add(l);
                else if (obj is Polygon p)
                    sj.Polygons.Add(p);
                else if (obj is Rectangle r)
                    sj.Rectangles.Add(r);
                else if (obj is Text t)
                    sj.Texts.Add(t);
            }
            sj.Title = scene.Title;
            sj.PointLights = scene.PointLights;

            File.WriteAllText(ScenePath + scene.Title + ".json", JsonConvert.SerializeObject(sj));
        }
    }
}
