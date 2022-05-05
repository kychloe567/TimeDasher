using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class StateMachine
    {
        [JsonProperty]
        private Dictionary<string, Animation> States { get; set; }
        [JsonProperty]
        public string CurrentState { get; set; }

        [JsonIgnore]
        public BitmapImage CurrentTexture
        {
            get
            {
                return States[CurrentState].CurrentTexture;
            }
        }

        public StateMachine()
        {
            States = new Dictionary<string, Animation>();
        }

        public StateMachine(Dictionary<string, Animation> states)
        {
            States = states;
            CurrentState = states.First().Value.Title;
        }
        public StateMachine(Animation state)
        {
            States = new Dictionary<string, Animation>();
            States.Add(state.Title, state);
            CurrentState = state.Title;
        }

        public void LoadTextures()
        {
            foreach (KeyValuePair<string, Animation> pair in States)
                pair.Value.LoadTextures();
        }
        public void AddState(Animation an)
        {
            States.Add(an.Title, an);
        }

        public void SetState(string state, int currentTexture = 0, bool fix = false, int stopOn = -1)
        {
            CurrentState = state;
            if(States.ContainsKey(CurrentState))
                States[CurrentState].StartAnimation(currentTexture, fix, stopOn);
        }

        public void Update()
        {
            if (States.ContainsKey(CurrentState))
                States[CurrentState].UpdateAnimation();
        }

        public StateMachine GetCopy()
        {
            Dictionary<string, Animation> states = new Dictionary<string, Animation>();
            foreach(KeyValuePair<string, Animation> pair in States)
                states.Add(pair.Key, pair.Value.GetCopy());
            return new StateMachine(states)
            {
                CurrentState = CurrentState
            };
        }

        public static StateMachine GetPlayerDefault()
        {
            string objectsPath = "Textures\\Character";

            StateMachine sm = new StateMachine();

            Animation idleright = new Animation("idleright");
            foreach (var image in new DirectoryInfo(objectsPath + "\\idleright").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                idleright.AddTexture(bi, image.FullName, 0.2);
            }
            sm.AddState(idleright);

            Animation idleleft = new Animation("idleleft");
            foreach (var image in new DirectoryInfo(objectsPath + "\\idleleft").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                idleleft.AddTexture(bi, image.FullName, 0.2);
            }
            sm.AddState(idleleft);

            Animation runright = new Animation("runright");
            foreach (var image in new DirectoryInfo(objectsPath + "\\runright").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                runright.AddTexture(bi, image.FullName, 0.08);
            }
            sm.AddState(runright);

            Animation runleft = new Animation("runleft");
            foreach (var image in new DirectoryInfo(objectsPath + "\\runleft").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                runleft.AddTexture(bi, image.FullName, 0.08);
            }
            sm.AddState(runleft);
            
            Animation fallingright = new Animation("fallingright");
            BitmapImage fallingrightb = new BitmapImage(new Uri(objectsPath + "\\fallingright.png", UriKind.RelativeOrAbsolute));
            fallingright.AddTexture(fallingrightb, objectsPath + "\\fallingright.png", 1);
            sm.AddState(fallingright);
            
            Animation fallingleft = new Animation("fallingleft");
            BitmapImage fallingleftb = new BitmapImage(new Uri(objectsPath + "\\fallingleft.png", UriKind.RelativeOrAbsolute));
            fallingleft.AddTexture(fallingleftb, objectsPath + "\\fallingleft.png", 1);
            sm.AddState(fallingleft);

            Animation jumpingright = new Animation("jumpingright");
            foreach (var image in new DirectoryInfo(objectsPath + "\\jumpingright").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                jumpingright.AddTexture(bi, image.FullName, 0.15);
            }
            sm.AddState(jumpingright);

            Animation jumpingleft = new Animation("jumpingleft");
            foreach (var image in new DirectoryInfo(objectsPath + "\\jumpingleft").GetFiles("*.png").OrderBy(x => x.Name, new TextureComparer()))
            {
                BitmapImage bi = new BitmapImage(new Uri(image.FullName, UriKind.RelativeOrAbsolute));
                jumpingleft.AddTexture(bi, image.FullName, 0.15);
            }
            sm.AddState(jumpingleft);

            sm.SetState("idleright");
            return sm;
        }
    }
}
